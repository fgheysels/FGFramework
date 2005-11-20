using System;
using System.Collections.Generic;
using System.Text;
using BusinessObjects;

namespace TestBusinessObjects
{
    [Serializable]
    public class Order : BusinessObject
    {
        private int _id;
        private Customer _customer;
        private string _name;

        public int Id
        {
            get
            {
                return _id;
            }
        }

        public Customer OwningCustomer
        {
            get
            {
                return _customer;
            }
            set
            {
                _customer = value;
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
    }
}
