using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;

namespace DbCore.DbDriver
{
    internal class SqlClientDriver : IDriver
    {
        #region IDriver Members

        public IDbConnection CreateConnection()
        {
            return new SqlConnection ();
        }

        public DbDataAdapter CreateDataAdapter()
        {
            return new SqlDataAdapter ();
        }

        #endregion
    }
}
