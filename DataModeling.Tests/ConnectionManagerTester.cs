using System;
using System.Data;
using System.Data.Common;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;

namespace DataModeling.Tests
{
    [TestClass]
    public class ConnectionManagerTester
    {
        [TestMethod]
        public void ShouldLeaveOpenConnectionOpen()
        {
            DbConnection connection = Substitute.For<DbConnection>();
            connection.State.Returns(ConnectionState.Open);
            ConnectionManager manager = new ConnectionManager(connection);
            manager.Dispose();
            connection.Received(0).Close();
        }

        [TestMethod]
        public void ShouldOpenAndCloseClosedConnection()
        {
            DbConnection connection = Substitute.For<DbConnection>();
            connection.State.Returns(ConnectionState.Closed);
            ConnectionManager manager = new ConnectionManager(connection);
            manager.Dispose();
            connection.Received(1).Open();
            connection.Received(1).Close();
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ShouldThrowExceptionIfConnectionNull()
        {
            DbConnection connection = null;
            new ConnectionManager(connection);
        }
    }
}
