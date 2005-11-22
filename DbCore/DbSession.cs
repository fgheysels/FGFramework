using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.Common;
using System.Reflection;

using DbCore.DbDriver;

namespace DbCore
{
    public class DbSession
    {

        private IDbConnection _connection;
        private IDbTransaction _transaction;
        private IDriver _driver;
        private bool _explicitOpenedTransaction;

        internal DbSession( IDbConnection conn, IDriver driver )
        {
            _connection = conn;
            _driver = driver;
        }

        public void BeginTransaction()
        {
            if( _transaction == null )
            {
                if( _connection.State == ConnectionState.Closed )
                {
                    _connection.Open ();
                    _explicitOpenedTransaction = false;
                }

                _transaction = _connection.BeginTransaction ();
            }
            else
            {
                throw new InvalidOperationException ("There is already a transaction active in this session.");
            }
        }

        public void CommitTransaction()
        {
            if( _transaction != null )
            {
                _transaction.Commit ();

                if( _explicitOpenedTransaction == false )
                {
                    _connection.Close ();
                }

                _transaction = null;
            }
        }

        public void RollbackTransaction()
        {
            if( _transaction != null )
            {
                _transaction.Rollback ();

                if( _explicitOpenedTransaction == false )
                {
                    _connection.Close ();
                }

                _transaction = null;
            }
        }

        public void OpenSession()
        {
            if( _connection.State != ConnectionState.Open )
            {
                _connection.Open ();
                _explicitOpenedTransaction = true;
            }
        }

        public void CloseSession()
        {
            _connection.Close ();
        }

        public IDataReader ExecuteDataReader( IDbCommand command )
        {
            command.Connection = _connection;
            command.Transaction = _transaction;
            return command.ExecuteReader ();
        }

        public int ExecuteCommand( IDbCommand command )
        {
            command.Connection = _connection;
            command.Transaction = _transaction;
            return command.ExecuteNonQuery ();
        }

        public object ExecuteScalar( IDbCommand command )
        {
            command.Connection = _connection;
            command.Transaction = _transaction;
            return command.ExecuteScalar ();
        }

        public void FillDataTable( IDbCommand command, DataTable dt )
        {
            command.Connection = _connection;
            command.Transaction = _transaction;

            DbDataAdapter da = _driver.CreateDataAdapter ();

            // Get the select-command property via reflection.
            PropertyInfo pInfo = da.GetType ().GetProperty ("SelectCommand");

            if( pInfo != null )
            {
                pInfo.SetValue (da, command, null);
            }
            else
            {
                throw new InvalidOperationException ("Data Adapter has no SelectCommand property.");
            }

            da.Fill (dt);
        }
    }
}
