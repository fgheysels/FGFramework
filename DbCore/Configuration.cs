using System;
using System.Configuration;
using System.Collections.Generic;
using System.Text;

using DbCore.DbDriver;

namespace DbCore
{
    public class Configuration
    {
        public Configuration()
        {
        }

        /// <summary>
        /// Creates a DbSessionFactory that will be able to open DbSession to
        /// the database that is specified in the 
        /// </summary>
        /// <returns></returns>
        public DbSessionFactory CreateSessionFactory()
        {

            if( ConfigurationManager.ConnectionStrings["default"] == null )
            {
                throw new ConfigurationErrorsException ("default connectionstring is not specified in the config-file.");
            }

            string connString = ConfigurationManager.ConnectionStrings["default"].ConnectionString;

            return CreateSessionFactory (connString);
        }

        public DbSessionFactory CreateSessionFactory( string connectionString )
        {
            string dbType = ConfigurationManager.AppSettings["dbtype"];

            IDriver driver = null;

            switch( dbType.ToLower() )
            {
                case "oledb":
                    driver = new OleDbDriver ();
                    break;
                case "sqlclient":
                    driver = new SqlClientDriver ();
                    break;  
            }

            if( driver == null )
            {
                throw new ConfigurationErrorsException ("No valid driver found for dbtype = " + dbType);
            }

            return new DbSessionFactory (driver, connectionString);
        }
    }
}
