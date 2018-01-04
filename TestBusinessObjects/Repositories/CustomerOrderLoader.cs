using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using BusinessObjects;

namespace TestBusinessObjects.Repositories
{
    class CustomerOrderLoader : ILazyLoader<Order>
    {        
        private Customer _c;

        internal CustomerOrderLoader( Customer c )
        {     
            _c = c;
        }

        #region ILazyLoader<Order> Members

        public List<Order> GetObjects()
        {
            List<Order> orderList = new List<Order> ();

                IEnumerator os = CustomerRepository._orders.Values.GetEnumerator ();

                while( os.MoveNext () )
                {
                    if( ( (CustomerRepository.OrderPersistence)os.Current ).CustomerId == _c.CustomerId )
                    {
                        Order o = new Order ();

                        o.Name = ( (CustomerRepository.OrderPersistence)os.Current ).OrderName;
                        o.OwningCustomer = _c;

                        orderList.Add (o);
                    }
                }

            return orderList;
        }

        #endregion
    }
}
