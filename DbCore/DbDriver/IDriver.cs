using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.Common;

namespace DbCore.DbDriver
{
    /// <summary>
    /// Interface contract for database-drivers.
    /// </summary>
    public interface IDriver
    {
        IDbConnection CreateConnection();

        DbDataAdapter CreateDataAdapter();
    }
}
