using System;
using System.Collections.Generic;
using System.Text;
using BusinessObjects;
using TestBusinessObjects.Repositories;

namespace TestBusinessObjects
{
    [Serializable]
    public class Customer : BusinessObject
    {
        private int _customerId;
        private string _name;
        private LazyBusinessObjectCollection<Order> _orders;

        public int CustomerId
        {
            get
            {
                return _customerId;
            }
        }

        public string Name
        {
            get
            {
                return _name;
            }
            set
            {
                _name = value;
            }
        }



        internal LazyBusinessObjectCollection<Order> Orders
        {
            get
            {
                return _orders;
            }
            set
            {
                _orders = value;
            }
        }

        internal Customer( int id  )
        {
            _customerId = id;
            _orders = new LazyBusinessObjectCollection<Order> (new CustomerOrderLoader (this));
        }

        public void AddOrder( Order o )
        {
            o.OwningCustomer = this;
            _orders.Add (o);
        }

        public void RemoveOrder( Order o )
        {
            o.OwningCustomer = null;
            _orders.Remove (o);
        }

        public Order[] GetOrders()
        {
            List<Order> orders = new List<Order> ();
            for( int i = 0; i < _orders.Count; i++ )
            {
                orders.Add (_orders[i]);
            }
            return orders.ToArray ();
        }
    }
}
