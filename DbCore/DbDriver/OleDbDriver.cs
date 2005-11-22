using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.OleDb;
using System.Data.Common;

namespace DbCore.DbDriver
{
    internal class OleDbDriver : IDriver
    {
        #region IDriver Members

        public IDbConnection CreateConnection()
        {
            return new OleDbConnection ();
        }

        public DbDataAdapter CreateDataAdapter()
        {
            return new OleDbDataAdapter ();
        }

        #endregion
    }
}
