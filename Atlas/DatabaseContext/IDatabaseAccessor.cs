// // -----------------------------------------------------------------------
// // <copyright file="IDatabaseAccessor.cs">
// //     Copyright 2020 Clint Irving
// //     All rights reserved.
// // </copyright>
// // <author>Clint Irving</author>
// // -----------------------------------------------------------------------

using System.Data.Common;
using System.Data.Entity;
using System.Threading;
using System.Threading.Tasks;

namespace Atlas.DatabaseContext
{
    public interface IDatabaseAccessor
    {
        string DatabaseName { get; }

        DbConnection Connection { get; }

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
        int ExecuteSqlCommand(string sql, params object[] parameters);

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
        /// <param name="transactionalBehavior"> Controls the creation of a transaction for this command. </param>
        /// <param name="sql"> The command string. </param>
        /// <param name="parameters"> The parameters to apply to the command string. </param>
        /// <returns> The result returned by the database after executing the command. </returns>
        int ExecuteSqlCommand(
            TransactionalBehavior transactionalBehavior,
            string sql,
            params object[] parameters);

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
        Task<int> ExecuteSqlCommandAsync(string sql, params object[] parameters);

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
        /// <param name="transactionalBehavior"> Controls the creation of a transaction for this command. </param>
        /// <param name="sql"> The command string. </param>
        /// <param name="parameters"> The parameters to apply to the command string. </param>
        /// <returns>
        ///     A task that represents the asynchronous operation.
        ///     The task result contains the result returned by the database after executing the command.
        /// </returns>
        Task<int> ExecuteSqlCommandAsync(
            TransactionalBehavior transactionalBehavior,
            string sql,
            params object[] parameters);

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
        Task<int> ExecuteSqlCommandAsync(
            string sql,
            CancellationToken cancellationToken,
            params object[] parameters);

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
        /// <param name="transactionalBehavior"> Controls the creation of a transaction for this command. </param>
        /// <param name="sql"> The command string. </param>
        /// <param name="cancellationToken">
        ///     A <see cref="T:System.Threading.CancellationToken" /> to observe while waiting for the task to complete.
        /// </param>
        /// <param name="parameters"> The parameters to apply to the command string. </param>
        /// <returns>
        ///     A task that represents the asynchronous operation.
        ///     The task result contains the result returned by the database after executing the command.
        /// </returns>
        Task<int> ExecuteSqlCommandAsync(
            TransactionalBehavior transactionalBehavior,
            string sql,
            CancellationToken cancellationToken,
            params object[] parameters);
    }
}