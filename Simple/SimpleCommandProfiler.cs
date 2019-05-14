using System;
using System.Collections;
using System.Data;
using System.Data.Common;
using System.Diagnostics;
using System.Dynamic;

namespace SqlProfiler.Simple
{
    /// <summary>
    /// Simple <see cref="DbCommand"/> profiler.
    /// </summary>
    public class SimpleCommandProfiler : CommandWrapper
    {
        private readonly Action<DbCommandMethod, DbCommand, CommandBehavior?> _intercept;

        /// <summary>
        /// Construct simple profiling command wrapper.
        /// </summary>
        /// <param name="wrapped">Command to wrap</param>
        /// <param name="intercept">Profiling action</param>
        public SimpleCommandProfiler(DbCommand wrapped, Action<DbCommandMethod, DbCommand, CommandBehavior?> intercept) : base(wrapped)
        {
            _intercept = intercept;
        }

        /// <summary>
        /// PreExecuteDbDataReader hook
        /// </summary>
        /// <param name="command"></param>
        /// <param name="behavior"></param>
        /// <returns></returns>
		protected override object PreExecuteDbDataReader(DbCommand command, CommandBehavior behavior)
        {
            _intercept(DbCommandMethod.ExecuteDbDataReader, command, behavior);
            return null;
        }

        /// <summary>
        /// PreExecuteNonQuery hook
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
		protected override object PreExecuteNonQuery(DbCommand command)
        {
            _intercept(DbCommandMethod.ExecuteNonQuery, command, null);
            return null;
        }

        /// <summary>
        /// PreExecuteScalar hook
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
		protected override object PreExecuteScalar(DbCommand command)
        {
            _intercept(DbCommandMethod.ExecuteScalar, command, null);
            return null;
        }
    }
}