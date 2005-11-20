using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using BusinessObjects;

namespace TestBusinessObjects.Repositories
{
    class CustomerRepository
    {
        
        internal static Dictionary<int, string> _customers = new Dictionary<int, string> ();
        internal static Dictionary<int, OrderPersistence> _orders = new Dictionary<int, OrderPersistence> ();

        internal class OrderPersistence
        {
            public int CustomerId;
            public string OrderName;

            public OrderPersistence( int custId, string ordName )
            {
                CustomerId = custId;
                OrderName = ordName;
            }
        }

        public CustomerRepository()
        {
            _customers.Add (1, "Frederik");
            _customers.Add (2, "Barbara");
            _customers.Add (3, "Dimitri");
            _customers.Add (4, "Ronny");
            _customers.Add (5, "Martine");

            _orders.Add (1, new OrderPersistence (1, "Nikon D200"));
            _orders.Add (2, new OrderPersistence (1, "Nikkor 100 - 300 f2.8"));
            _orders.Add (3, new OrderPersistence (2, "Kenwood"));
            _orders.Add (4, new OrderPersistence (2, "Lepelhapjes"));
            _orders.Add (5, new OrderPersistence (2, "Slakom"));
            _orders.Add (6, new OrderPersistence (3, "Computer"));
            _orders.Add (7, new OrderPersistence (4, "Nachtegaal"));
            _orders.Add (8, new OrderPersistence (5, "Pen"));
            _orders.Add (9, new OrderPersistence (6, "Schrift"));
        }

        public Customer GetCustomer( int id )
        {
            Customer c = new Customer (id);
            c.Name = _customers[id];

            c.Orders = new LazyBusinessObjectCollection<Order> (new CustomerOrderLoader(c));

            return c;
        }
    }
}
