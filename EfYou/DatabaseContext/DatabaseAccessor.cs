// // -----------------------------------------------------------------------
// // <copyright file="DatabaseAccessor.cs">
// //     Copyright 2020 Clint Irving
// //     All rights reserved.
// // </copyright>
// // <author>Clint Irving</author>
// // -----------------------------------------------------------------------

using System.Data.Common;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace EfYou.DatabaseContext
{
    /// <summary>
    ///     Class to be used for direct access to the underlying DB.
    /// </summary>
    public class DatabaseAccessor : IDatabaseAccessor
    {
        private readonly DatabaseFacade _database;

        public DatabaseAccessor(DatabaseFacade database)
        {
            _database = database;
        }

        public string DatabaseName => _database.GetDbConnection().Database;

        /// <summary>
        ///     Executes the given DDL/DML command against the database.
        ///     As with any API that accepts SQL it is important to parameterize any user input to protect against a SQL injection
        ///     attack. You can include parameter place holders in the SQL query string and then supply parameter values as
        ///     additional arguments. Any parameter values you supply will automatically be converted to a DbParameter.
        ///     context.Database.ExecuteSqlCommand("UPDATE dbo.Posts SET Rating = 5 WHERE Author = @p0", userSuppliedAuthor);
        ///     Alternatively, you can also construct a DbParameter and supply it to SqlQuery. This allows you to use named
        ///     parameters in the SQL query string.
        ///     context.Database.ExecuteSqlCommand("UPDATE dbo.Posts SET Rating = 5 WHERE Author = @author", new
        ///     SqlParameter("@author", userSuppliedAuthor));
        /// </summary>
        /// <remarks>
        ///     If there isn't an existing local or ambient transaction a new transaction will be used
        ///     to execute the command.
        /// </remarks>
        /// <param name="sql"> The command string. </param>
        /// <param name="parameters"> The parameters to apply to the command string. </param>
        /// <returns> The result returned by the database after executing the command. </returns>
        public virtual int ExecuteSqlCommand(string sql, params object[] parameters)
        {
            return _database.ExecuteSqlRaw(sql, parameters);
        }

        /// <summary>
        ///     Executes the given DDL/DML command against the database.
        ///     As with any API that accepts SQL it is important to parameterize any user input to protect against a SQL injection
        ///     attack. You can include parameter place holders in the SQL query string and then supply parameter values as
        ///     additional arguments. Any parameter values you supply will automatically be converted to a DbParameter.
        ///     context.Database.ExecuteSqlCommand("UPDATE dbo.Posts SET Rating = 5 WHERE Author = @p0", userSuppliedAuthor);
        ///     Alternatively, you can also construct a DbParameter and supply it to SqlQuery. This allows you to use named
        ///     parameters in the SQL query string.
        ///     context.Database.ExecuteSqlCommand("UPDATE dbo.Posts SET Rating = 5 WHERE Author = @author", new
        ///     SqlParameter("@author", userSuppliedAuthor));
        /// </summary>
        /// <param name="transaction"> Controls if a transaction will wrap the command. </param>
        /// <param name="sql"> The command string. </param>
        /// <param name="parameters"> The parameters to apply to the command string. </param>
        /// <returns> The result returned by the database after executing the command. </returns>
        public virtual int ExecuteSqlCommand(
            bool transaction,
            string sql,
            params object[] parameters)
        {
            if(transaction)
            {
                _database.BeginTransaction();

                var result = _database.ExecuteSqlRaw(sql, parameters);

                _database.CommitTransaction();

                return result;
            }

            return ExecuteSqlCommand(sql, parameters);
        }

        /// <summary>
        ///     Asynchronously executes the given DDL/DML command against the database.
        ///     As with any API that accepts SQL it is important to parameterize any user input to protect against a SQL injection
        ///     attack. You can include parameter place holders in the SQL query string and then supply parameter values as
        ///     additional arguments. Any parameter values you supply will automatically be converted to a DbParameter.
        ///     context.Database.ExecuteSqlCommandAsync("UPDATE dbo.Posts SET Rating = 5 WHERE Author = @p0", userSuppliedAuthor);
        ///     Alternatively, you can also construct a DbParameter and supply it to SqlQuery. This allows you to use named
        ///     parameters in the SQL query string.
        ///     context.Database.ExecuteSqlCommandAsync("UPDATE dbo.Posts SET Rating = 5 WHERE Author = @author", new
        ///     SqlParameter("@author", userSuppliedAuthor));
        /// </summary>
        /// <remarks>
        ///     Multiple active operations on the same context instance are not supported.  Use 'await' to ensure
        ///     that any asynchronous operations have completed before calling another method on this context.
        ///     If there isn't an existing local transaction a new transaction will be used
        ///     to execute the command.
        /// </remarks>
        /// <param name="sql"> The command string. </param>
        /// <param name="parameters"> The parameters to apply to the command string. </param>
        /// <returns>
        ///     A task that represents the asynchronous operation.
        ///     The task result contains the result returned by the database after executing the command.
        /// </returns>
        public virtual Task<int> ExecuteSqlCommandAsync(string sql, params object[] parameters)
        {
            return _database.ExecuteSqlRawAsync(sql, parameters);
        }

        /// <summary>
        ///     Asynchronously executes the given DDL/DML command against the database.
        ///     As with any API that accepts SQL it is important to parameterize any user input to protect against a SQL injection
        ///     attack. You can include parameter place holders in the SQL query string and then supply parameter values as
        ///     additional arguments. Any parameter values you supply will automatically be converted to a DbParameter.
        ///     context.Database.ExecuteSqlCommandAsync("UPDATE dbo.Posts SET Rating = 5 WHERE Author = @p0", userSuppliedAuthor);
        ///     Alternatively, you can also construct a DbParameter and supply it to SqlQuery. This allows you to use named
        ///     parameters in the SQL query string.
        ///     context.Database.ExecuteSqlCommandAsync("UPDATE dbo.Posts SET Rating = 5 WHERE Author = @author", new
        ///     SqlParameter("@author", userSuppliedAuthor));
        /// </summary>
        /// <remarks>
        ///     Multiple active operations on the same context instance are not supported.  Use 'await' to ensure
        ///     that any asynchronous operations have completed before calling another method on this context.
        /// </remarks>
        /// <param name="transaction"> Controls if a transaction will wrap the command. </param>
        /// <param name="sql"> The command string. </param>
        /// <param name="parameters"> The parameters to apply to the command string. </param>
        /// <returns>
        ///     A task that represents the asynchronous operation.
        ///     The task result contains the result returned by the database after executing the command.
        /// </returns>
        public virtual Task<int> ExecuteSqlCommandAsync(
            bool transaction,
            string sql,
            params object[] parameters)
        {
            if (transaction)
            {
                _database.BeginTransaction();

                var result = _database.ExecuteSqlRawAsync(sql, parameters);

                _database.CommitTransaction();

                return result;
            }

            return ExecuteSqlCommandAsync(sql, parameters);
        }

        /// <summary>
        ///     Asynchronously executes the given DDL/DML command against the database.
        ///     As with any API that accepts SQL it is important to parameterize any user input to protect against a SQL injection
        ///     attack. You can include parameter place holders in the SQL query string and then supply parameter values as
        ///     additional arguments. Any parameter values you supply will automatically be converted to a DbParameter.
        ///     context.Database.ExecuteSqlCommandAsync("UPDATE dbo.Posts SET Rating = 5 WHERE Author = @p0", userSuppliedAuthor);
        ///     Alternatively, you can also construct a DbParameter and supply it to SqlQuery. This allows you to use named
        ///     parameters in the SQL query string.
        ///     context.Database.ExecuteSqlCommandAsync("UPDATE dbo.Posts SET Rating = 5 WHERE Author = @author", new
        ///     SqlParameter("@author", userSuppliedAuthor));
        /// </summary>
        /// <remarks>
        ///     Multiple active operations on the same context instance are not supported.  Use 'await' to ensure
        ///     that any asynchronous operations have completed before calling another method on this context.
        ///     If there isn't an existing local transaction a new transaction will be used
        ///     to execute the command.
        /// </remarks>
        /// <param name="sql"> The command string. </param>
        /// <param name="cancellationToken">
        ///     A <see cref="T:System.Threading.CancellationToken" /> to observe while waiting for the task to complete.
        /// </param>
        /// <param name="parameters"> The parameters to apply to the command string. </param>
        /// <returns>
        ///     A task that represents the asynchronous operation.
        ///     The task result contains the result returned by the database after executing the command.
        /// </returns>
        public virtual Task<int> ExecuteSqlCommandAsync(
            string sql,
            CancellationToken cancellationToken,
            params object[] parameters)
        {
            return _database.ExecuteSqlRawAsync(sql, cancellationToken, parameters);
        }

        /// <summary>
        ///     Asynchronously executes the given DDL/DML command against the database.
        ///     As with any API that accepts SQL it is important to parameterize any user input to protect against a SQL injection
        ///     attack. You can include parameter place holders in the SQL query string and then supply parameter values as
        ///     additional arguments. Any parameter values you supply will automatically be converted to a DbParameter.
        ///     context.Database.ExecuteSqlCommandAsync("UPDATE dbo.Posts SET Rating = 5 WHERE Author = @p0", userSuppliedAuthor);
        ///     Alternatively, you can also construct a DbParameter and supply it to SqlQuery. This allows you to use named
        ///     parameters in the SQL query string.
        ///     context.Database.ExecuteSqlCommandAsync("UPDATE dbo.Posts SET Rating = 5 WHERE Author = @author", new
        ///     SqlParameter("@author", userSuppliedAuthor));
        /// </summary>
        /// <remarks>
        ///     Multiple active operations on the same context instance are not supported.  Use 'await' to ensure
        ///     that any asynchronous operations have completed before calling another method on this context.
        /// </remarks>
        /// <param name="transaction"> Controls if a transaction will wrap the command. </param>
        /// <param name="sql"> The command string. </param>
        /// <param name="cancellationToken">
        ///     A <see cref="T:System.Threading.CancellationToken" /> to observe while waiting for the task to complete.
        /// </param>
        /// <param name="parameters"> The parameters to apply to the command string. </param>
        /// <returns>
        ///     A task that represents the asynchronous operation.
        ///     The task result contains the result returned by the database after executing the command.
        /// </returns>
        public virtual Task<int> ExecuteSqlCommandAsync(
            bool transaction,
            string sql,
            CancellationToken cancellationToken,
            params object[] parameters)
        {
            if (transaction)
            {
                _database.BeginTransaction();

                var result = _database.ExecuteSqlRawAsync(sql, cancellationToken, parameters);

                _database.CommitTransaction();

                return result;
            }

            return ExecuteSqlCommandAsync(sql, cancellationToken, parameters);
        }

        public DbConnection Connection => _database.GetDbConnection();
    }
}