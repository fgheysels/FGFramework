using System;
using System.Collections.Generic;
using System.Text;

namespace BusinessObjects
{
    /// <summary>
    /// LazyBusinessObjectCollection provides the functionality of a collection, but,
    /// the collection is only populated once it's first accessed.
    /// It is a virtual proxy.
    /// </summary>
    /// <typeparam name="T">The type of the specific BusinessObjects that will be contained
    /// in this collection.</typeparam>
    [Serializable]
    public class LazyBusinessObjectCollection<T> : IUndoable where T : BusinessObject
    {
        /// <summary>
        /// The ILazyLoader that will get the objects that will be contained in
        /// this collection, once the lazycollection is accessed for the first time.
        /// </summary>
        private ILazyLoader<T> _loader;

        /// <summary>
        /// The BusinessObjectCollection where this LazyCollection is a wrapper (proxy) for.        
        /// </summary>
        /// <remarks>Do not access this field directly, use the CollectionObj property instead.</remarks>
        private BusinessObjectCollection<T> _collection = null;

        /// <summary>
        /// The property accessor that must be used when you 
        /// </summary>
        private BusinessObjectCollection<T> CollectionObj
        {
            get
            {
                if( _collection == null )
                {
                    // Initialize the collection...
                    _collection = new BusinessObjectCollection<T> ();

                    // Get the objects and add them to the collection.
                    foreach( T item in _loader.GetObjects () )
                    {
                        _collection.Add (item);
                    }

                }
                return _collection;
            }
        }

        #region Constructor

        /// <summary>
        /// Creates an instance of a LazyBusinessObjectCollection.
        /// </summary>
        /// <param name="loader">An ILazyLoader that will be used to populate the lazy collection.</param>
        public LazyBusinessObjectCollection( ILazyLoader<T> loader )
        {
            // Make sure that the loader argument is not null, since we need 
            // a valid loader to be able to populate the collection.
            if( loader == null )
            {
                throw new InvalidOperationException ("The ILazyLoader<T> argument cannot be null.");
            }
            _loader = loader;
        }

        #endregion

        #region Collection interface 

        public bool IsDirty
        {
            get
            {
                if( _collection != null )
                {
                    return CollectionObj.IsDirty;
                }
                else
                {
                    return false;
                }
            }
        }

        public int Count
        {
            get
            {
                return CollectionObj.Count;
            }
        }

        public T this[int index]
        {
            get
            {
                return CollectionObj[index];
            }
        }

        public void Add( T item )
        {
            CollectionObj.Add (item);
        }

        public void Insert( int index, T item )
        {
            CollectionObj.Insert (index, item);
        }

        public void Remove( T item )
        {
            CollectionObj.Remove (item);
        }

        public T[] ToArray()
        {
            List<T> list = new List<T> ();

            for( int i = 0; i < CollectionObj.Count; i++ )
            {
                list.Add (CollectionObj[i]);
            }

            return list.ToArray ();
        }

        public void RemoveAt( int index )
        {
            CollectionObj.RemoveAt (index);
        }

        public T[] GetDeletedBusinessObjects()
        {
            return CollectionObj.GetDeletedBusinessObjects ();
        }

        public void ClearDeleted()
        {
            CollectionObj.ClearDeleted ();
        }

        #endregion

        #region IUndoable Members

        public void CreateSnapshot()
        {
            // Delegate the call to the collection itself; if the collection is not 
            // loaded yet, it will be loaded now.
            // If the collection is not loaded yet, we do not want to create the snapshot.
            if( _collection != null )
            {
                CollectionObj.CreateSnapshot ();
            }
        }

        public void CommitSnapshot()
        {
            if( _collection != null )
            {
                CollectionObj.CommitSnapshot ();
            }
        }

        public void RevertToPreviousState()
        {
            if( _collection != null )
            {
                CollectionObj.RevertToPreviousState ();
            }
        }

        #endregion
    }
}
