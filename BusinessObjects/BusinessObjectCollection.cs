using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using System.Collections.ObjectModel;

namespace BusinessObjects
{
    [Serializable]    
    //public class BusinessObjectCollection<T> : CollectionBase, IUndoable, IBindingList where T : BusinessObject 
    public class BusinessObjectCollection<T> : Collection<T>, IUndoable /*, IBindingList*/ where T : BusinessObject    
    {
        public bool IsDirty
        {
            get
            {
                foreach( T businessObject in Items )
                {
                    if( businessObject.IsDirty )
                    {
                        return true;
                    }
                }
                return false;
            }
        }        

        public T[] ToArray()
        {            
            List<T> list = new List<T> ();

            foreach( T item in Items )
            {
                list.Add (item);
            }
            
            return list.ToArray ();
        }

        protected override void InsertItem( int index, T item )
        {
            item.EditLevelAdded = _editLevel;

            base.InsertItem (index, item);
        }
                
        private Collection<T> _deletedItems = new Collection<T>();

        protected override void RemoveItem( int index )
        {
            // Since we do not have direct access to the item that's being removed here,
            // we'll have to get it first.
            T businessItem = Items[index];

            if( businessItem != null )
            {
                DeleteBusinessObject (businessItem);
            }

            base.RemoveItem(index);
        }        

        private void DeleteBusinessObject( T item )
        {
            item.MarkDeleted ();
            _deletedItems.Add (item);
        }

        private void UndeleteBusinessObject( T item )
        {
            int saveLevel = item.EditLevelAdded;
            
            Items.Add (item);            
            
            if( item.EditLevelAdded != saveLevel )
            {
                item.EditLevelAdded = saveLevel;
            }

            // Check to see if the EditLevelAdded stayed the same
            //System.Diagnostics.Debug.Assert (saveLevel == item.EditLevelAdded,
              //                               "EditLevelAdded should not have changed.");

            // Remove the item from the deleted items collection, since we've undeleted it
            _deletedItems.Remove (item);
        }

        /// <summary>
        /// Gets an array that contains the BusinessObjects that are marked for deletion.
        /// </summary>
        /// <returns></returns>
        public T[] GetDeletedBusinessObjects()
        {
            List<T> items = new List<T> ();

            foreach( T item in _deletedItems )
            {
                items.Add (item);
            }

            return items.ToArray ();
        }

        public void ClearDeleted()
        {
            _deletedItems.Clear ();
        }

        #region IUndoable Members

        private int _editLevel = 0;

        public void CreateSnapshot()
        {
            _editLevel++;

            foreach( T item in Items )
            {
                item.CreateSnapshot ();
            }

            foreach( T item in _deletedItems )
            {
                item.CreateSnapshot ();
            }
        }

        public void CommitSnapshot()
        {
            _editLevel--;

            if( _editLevel < 0 )
            {
                _editLevel = 0;
            }

            foreach( T item in Items )
            {
                item.CommitSnapshot ();
                if( item.EditLevelAdded > _editLevel )
                {
                    item.EditLevelAdded = _editLevel;
                }
            }

            foreach( T item in _deletedItems )
            {
                item.CommitSnapshot ();
                if( item.EditLevelAdded > _editLevel )
                {
                    item.EditLevelAdded = _editLevel;
                }
            }
        }

        public void RevertToPreviousState()
        {
            _editLevel--;

            if( _editLevel < 0 )
            {
                _editLevel = 0;
            }

            for( int i = Items.Count - 1; i >= 0; i-- )
            {
                T item = Items[i];

                item.RevertToPreviousState ();

                if( item.EditLevelAdded > _editLevel )
                {
                    base.Items.Remove (item);
                }
            }

            for( int i = _deletedItems.Count - 1; i >= 0; i-- )
            {
                T item = _deletedItems[i];

                item.RevertToPreviousState ();

                if( item.EditLevelAdded > _editLevel )
                {
                    _deletedItems.Remove (item);
                }

                if( item.IsDeleted == false )
                {
                    this.UndeleteBusinessObject (item);
                }
            }
            
        }

        #endregion

        #region IBindingList Members

       

       //#endregion

       // #region IBindingList Members

       // public void AddIndex( PropertyDescriptor property )
       // {
       //     throw new Exception ("The method or operation is not implemented.");
       // }

       // public object AddNew()
       // {
       //     throw new Exception ("The method or operation is not implemented.");
       // }

       // public bool AllowEdit
       // {
       //     get
       //     {
       //         throw new Exception ("The method or operation is not implemented.");
       //     }
       // }

       // public bool AllowNew
       // {
       //     get
       //     {
       //         throw new Exception ("The method or operation is not implemented.");
       //     }
       // }

       // public bool AllowRemove
       // {
       //     get
       //     {
       //         throw new Exception ("The method or operation is not implemented.");
       //     }
       // }

       // public void ApplySort( PropertyDescriptor property, ListSortDirection direction )
       // {
       //     throw new Exception ("The method or operation is not implemented.");
       // }

       // public int Find( PropertyDescriptor property, object key )
       // {
       //     throw new Exception ("The method or operation is not implemented.");
       // }

       // public bool IsSorted
       // {
       //     get
       //     {
       //         throw new Exception ("The method or operation is not implemented.");
       //     }
       // }

       // public event ListChangedEventHandler ListChanged;

       // public void RemoveIndex( PropertyDescriptor property )
       // {
       //     throw new Exception ("The method or operation is not implemented.");
       // }

       // public void RemoveSort()
       // {
       //     throw new Exception ("The method or operation is not implemented.");
       // }

       // public ListSortDirection SortDirection
       // {
       //     get
       //     {
       //         throw new Exception ("The method or operation is not implemented.");
       //     }
       // }

       // public PropertyDescriptor SortProperty
       // {
       //     get
       //     {
       //         throw new Exception ("The method or operation is not implemented.");
       //     }
       // }

       // public bool SupportsChangeNotification
       // {
       //     get
       //     {
       //         throw new Exception ("The method or operation is not implemented.");
       //     }
       // }

       // public bool SupportsSearching
       // {
       //     get
       //     {
       //         throw new Exception ("The method or operation is not implemented.");
       //     }
       // }

       // public bool SupportsSorting
       // {
       //     get
       //     {
       //         throw new Exception ("The method or operation is not implemented.");
       //     }
       // }

        #endregion
    }
}
