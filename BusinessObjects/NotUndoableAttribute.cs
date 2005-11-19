using System;
using System.Collections.Generic;
using System.Text;

namespace BusinessObjects
{
    /// <summary>
    /// This attribute indicates that a field of an IUndoable type should not be undoable.
    /// </summary>
    [global::System.AttributeUsage (AttributeTargets.Field, Inherited = false, AllowMultiple = true)]
    public sealed class NotUndoableAttribute : Attribute
    {        
    }

}
