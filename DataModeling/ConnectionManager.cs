using System;
using System.Data;
using System.Data.Common;

namespace DataModeling
{
    public class ConnectionManager : IDisposable
    {
        private readonly DbConnection connection;
        private readonly bool needsClosed;

        public ConnectionManager(DbConnection connection)
        {
            if (connection == null)
            {
                throw new ArgumentNullException("connection");
            }
            this.connection = connection;
            if (connection.State == ConnectionState.Closed)
            {
                connection.Open();
                needsClosed = true;
            }
        }

        ~ConnectionManager()
        {
            Dispose(false);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (needsClosed)
                {
                    connection.Close();
                }
            }
        }
    }
}
