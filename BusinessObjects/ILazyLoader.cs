using System;
using System.Collections.Generic;
using System.Text;

namespace BusinessObjects
{
    public interface ILazyLoader<T> where T : BusinessObject
    {
        List<T> GetObjects();
    }
}
