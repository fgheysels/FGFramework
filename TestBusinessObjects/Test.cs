using System;
using System.Collections.Generic;
using System.Text;
using BusinessObjects;
namespace TestBusinessObjects
{
    class Test : LazyBusinessObjectCollection<Order>
    {
        public Test( ILazyLoader<Order> o ) : base(o)
        {
        }
    }
}
