using System;
using System.ComponentModel;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace BusinessObjects
{
    /// <summary>
    /// Base class from which every custom business object must inherit.
    /// </summary>
    [Serializable]
    public abstract class BusinessObject : IUndoable, IEditableObject
    {
        #region IsNew, IsDirty, IsDeleted functionality

        private bool _isNew     = true;
        private bool _isDirty   = false;
        private bool _isDeleted = false;

        /// <summary>
        /// Marks the object as dirty
        /// </summary>
        protected void MarkDirty()
        {
            _isDirty = true;
        }

        /// <summary>
        /// Marks the object as 'clean'.
        /// </summary>
        protected void MarkClean()
        {
            _isDirty = false;
        }

        /// <summary>
        /// Marks the object as 'new'.
        /// </summary>
        protected void MarkNew()
        {
            _isNew = true;            
            _isDeleted = false;
            MarkDirty ();
        }

        internal void MarkDeleted()
        {
            _isDeleted = true;
            MarkDirty ();
        }

        protected void MarkOld()
        {
            _isNew = false;
            MarkClean ();
        }

        /// <summary>
        /// Gets a boolean value that indicates whether this is a new object or not.
        /// </summary>
        public bool IsNew
        {
            get
            {
                return _isNew;
            }
        }

        /// <summary>
        /// Gets a boolean value that indicates whether this object must be deleted.
        /// </summary>
        public bool IsDeleted
        {
            get
            {
                return _isDeleted;
            }
        }

        /// <summary>
        /// Gets a boolean value that indicates whether this object has changed.
        /// </summary>
        public virtual bool IsDirty
        {
            get
            {
                return _isDirty;
            }
        }

        #endregion

        #region IUndoable Members

        /// <summary>
        /// This stack keeps track of the different 'snapshots' of the object.
        /// </summary>
        [NotUndoable]
        private Stack _stateStack = new Stack ();

        /// <summary>
        /// Gets the editlevel of the object.
        /// </summary>
        protected int EditLevel
        {
            get
            {
                return _stateStack.Count;
            }
        }

        /// <summary>
        /// Save the current state of the object so that you can revert to it if necessary.
        /// </summary>
        public void CreateSnapshot()
        {
            Type                        currentType;
            Hashtable                   state = new Hashtable();
            FieldInfo[]                 fields;
            string                      fieldName;

            currentType = this.GetType ();

            do
            {
                // Get the member-fields of this type
                fields = currentType.GetFields (BindingFlags.Public |
                                                BindingFlags.NonPublic |
                                                BindingFlags.Instance);

                foreach( FieldInfo field in fields )
                {
                    // If this field is declared in the current-type, and, it is 
                    // a field that is undoable, then keep track of its state.
                    if( field.DeclaringType == currentType &&
                        this.IsUndoableField (field) )
                    {
                        object fieldValue = field.GetValue (this);

                        // If the field is an IUndoable type, cascade the call.
                        IUndoable uf = fieldValue as IUndoable;

                        if( uf != null )
                        {
                            uf.CreateSnapshot ();
                        }
                        else
                        {
                            fieldName = currentType.Name + "." + field.Name;
                            state.Add (fieldName, fieldValue);
                        }
                    }                    
                }

                currentType = currentType.BaseType;

            } while( currentType != typeof (BusinessObject) );

            // Now we have saved the complete state of the object in the
            // hashtable.  We need to stack it into the state-stack.
            MemoryStream buffer = new MemoryStream ();
            BinaryFormatter fmt = new BinaryFormatter ();
            fmt.Serialize (buffer, state);
            _stateStack.Push (buffer.ToArray ()); // Stack it as a byte-array
        }

        /// <summary>
        /// Commit the changes made to the object since the last time 'CreateSnapshot' was called.
        /// </summary>
        public void CommitSnapshot()
        {
            // We can only commit the changes if a snapshot was created.
            if( EditLevel > 0 )
            {
                // Remove the last state from the stack.
                _stateStack.Pop ();

                // Check if the object contains other IUndoable types, so that
                // we can call CommitSnapshot on these fields as well.
                Type currentType;
                FieldInfo[] fields;

                currentType = this.GetType ();

                do
                {
                    fields = currentType.GetFields (BindingFlags.Instance |
                                                    BindingFlags.Public |
                                                    BindingFlags.NonPublic);

                    foreach( FieldInfo field in fields )
                    {
                        if( field.DeclaringType == currentType &&
                            this.IsUndoableField (field) )
                        {
                            object fieldValue = field.GetValue (this);

                            IUndoable uf = fieldValue as IUndoable;

                            if( uf != null )
                            {
                                uf.CommitSnapshot ();
                            }
                        }
                    }

                    currentType = currentType.BaseType;

                } while( currentType != typeof (BusinessObject) );
            }
        }

        /// <summary>
        /// Undo the changes that were made since the last time 'CreateSnapshot' was called.
        /// </summary>
        public void RevertToPreviousState()
        {
            // We can only undo if we've a state to revert to.
            if( EditLevel > 0 )
            {
                // Get the latest state of the object that was pushed on the state-stack
                MemoryStream buffer = new MemoryStream ((byte[])_stateStack.Pop ());
                buffer.Position = 0;
                BinaryFormatter fmt = new BinaryFormatter ();
                Hashtable state = (Hashtable)fmt.Deserialize (buffer);

                Type currentType = this.GetType ();
                FieldInfo[] fields;
                string fieldName;

                do
                {
                    fields = currentType.GetFields (BindingFlags.Public |
                                                    BindingFlags.NonPublic |
                                                    BindingFlags.Instance);

                    foreach( FieldInfo field in fields )
                    {
                        if( field.DeclaringType == currentType &&
                            this.IsUndoableField (field) )
                        {
                            object fieldValue = field.GetValue (this);

                            IUndoable uf = fieldValue as IUndoable;

                            if( uf != null )
                            {
                                uf.RevertToPreviousState ();
                            }
                            else
                            {
                                fieldName = currentType.Name + "." + field.Name;
                                field.SetValue (this, state[fieldName]);
                            }
                        }
                    }

                    currentType = currentType.BaseType;

                } while( currentType != typeof (object) );
            }
        }

        /// <summary>
        /// Checks whether the given field is undoable or not.
        /// Fields that are marked with the NotUndoableAttribute are not
        /// undoable.
        /// </summary>
        /// <param name="field"></param>
        /// <returns></returns>
        private bool IsUndoableField( FieldInfo field )
        {
            return !Attribute.IsDefined (field, typeof (NotUndoableAttribute));
        }

        #endregion

        #region Editlevel tracking for BusinessObjects in a BusinessObjectCollection

        private int _editLevelAdded;

        internal int EditLevelAdded
        {
            get
            {
                return _editLevelAdded;
            }
            set
            {
                _editLevelAdded = value;
            }
        }

        #endregion

        #region IEditableObject Members

        [NotUndoable]
        private bool _bindingEdit = false;

        public void BeginEdit()
        {
            if( _bindingEdit == false )
            {
                this.CreateSnapshot ();
                _bindingEdit = true;
            }
        }

        public void CancelEdit()
        {
            if( _bindingEdit )
            {
                this.RevertToPreviousState ();
            }
        }

        public void EndEdit()
        {
            if( _bindingEdit )
            {
                this.CommitSnapshot ();
            }
        }

        #endregion
    }
}
