using System;
using System.Collections.Generic;
using System.Text;

using TestBusinessObjects.Repositories;
using NUnit.Framework;

namespace TestBusinessObjects.Tests
{
    [TestFixture]
    public class CustomerTest
    {
        [Test]
        public void TestLazyOrders()
        {
            CustomerRepository cr = new CustomerRepository ();

            Customer c = cr.GetCustomer (1);
            
            Assert.AreEqual ("frederik", c.Name.ToLower (), "Wrong customer");

            Assert.AreEqual (2, c.Orders.Count, "Number of orders is not ok.");
        }
    }
}
