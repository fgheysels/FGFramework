using System;
using System.Collections.Generic;
using System.Text;

namespace BusinessObjects
{
    /// <summary>
    /// Interface contract for types that must support undo-capabilities.
    /// </summary>
    public interface IUndoable
    {
        void CreateSnapshot();
        void CommitSnapshot();
        void RevertToPreviousState();
    }
}
