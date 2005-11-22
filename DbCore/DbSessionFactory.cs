using System;
using System.Collections.Generic;
using System.Text;
using System.Data;

using DbCore.DbDriver;

namespace DbCore
{
    public class DbSessionFactory
    {
        private IDriver _driver;
        private string _connectionString;

        internal DbSessionFactory( IDriver driver, string connectionString )
        {
            _driver = driver;
            _connectionString = connectionString;
        }

        public DbSession CreateDbSession()
        {
            IDbConnection conn = _driver.CreateConnection ();

            conn.ConnectionString = _connectionString;

            return new DbSession (conn, _driver);
        }
    }
}
