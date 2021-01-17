using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using System;
using System.Data.Common;

namespace IdentityShell.Hosting
{
    /// <summary>
    /// Factory to create/reuse a SqLite database connection in memory
    /// https://www.meziantou.net/testing-ef-core-in-memory-using-sqlite.htm
    /// </summary>
    /// <remarks>
    /// As is understand the single connection is reused by EF core anyway.
    /// If this isn't viable and more connectionn are required the option ?cache=shared might be a viable solutions
    /// or give the in memory DB a name: https://sqlite.org/inmemorydb.html
    /// </remarks>
    public sealed class InMemoryDbContextOptionsBuilder : IDisposable
    {
        public InMemoryDbContextOptionsBuilder(string name)
        {
            this.name = name;
        }

        private DbConnection connection;
        private readonly string name;

        public DbContextOptions CreateOptions(DbContextOptionsBuilder opts, Action<SqliteDbContextOptionsBuilder> sqliteOptionsAction = null)
        {
            if (this.connection is null)
                this.InitializeDbConnection();

            return opts.UseSqlite(this.connection, sqliteOptionsAction).Options;
        }

        private void InitializeDbConnection()
        {
            this.connection = new SqliteConnection($"Data Source={name};Mode=Memory;Cache=Shared");
            //this.connection = new SqliteConnection(@$"Data Source=d:\tmp\{name}.db");
            this.connection.Open();
        }

        #region IDisposable

        public void Dispose()
        {
            this.connection?.Dispose();
            this.connection = null;
        }

        #endregion IDisposable
    }
}