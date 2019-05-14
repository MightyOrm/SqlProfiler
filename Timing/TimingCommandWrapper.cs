using System;
using System.Collections;
using System.Data;
using System.Data.Common;
using System.Diagnostics;
using System.Dynamic;

namespace SqlProfiler.Timing
{
    /// <summary>
    /// Timing command wrapper. Not supported. Needs updating to use ILogger, etc., at least.
    /// </summary>
    public class TimingCommandWrapper : CommandWrapper
	{
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="wrapped">Command to wrap</param>
		public TimingCommandWrapper(DbCommand wrapped) : base(wrapped)
		{
		}

        /// <summary>
        /// PreExecuteDbDataReader hook
        /// </summary>
        /// <param name="command"></param>
        /// <param name="behavior"></param>
        /// <returns></returns>
		protected override object PreExecuteDbDataReader(DbCommand command, CommandBehavior behavior)
		{
			Console.WriteLine("ExecuteDbDataReader(" + ((behavior != CommandBehavior.Default) ? behavior.ToString() : "") + ")");
			ShowSql(command);
            var stopwatch = new Stopwatch();
            stopwatch.Start();
			return stopwatch;
		}

        /// <summary>
        /// PostExecuteDbDataReader hook
        /// </summary>
        /// <param name="command">The wrapped command</param>
        /// <param name="behavior">The command behaviour</param>
        /// <param name="profilingObject"></param>
		protected override void PostExecuteDbDataReader(DbCommand command, CommandBehavior behavior, object profilingObject)
		{
			ShowTime(command, profilingObject);
		}

        /// <summary>
        /// PreExecuteNonQuery hook
        /// </summary>
        /// <param name="command">The wrapped command</param>
        /// <returns></returns>
		protected override object PreExecuteNonQuery(DbCommand command)
		{
			Console.WriteLine("ExecuteNonQuery()");
			ShowSql(command);
            var stopwatch = new Stopwatch();
            stopwatch.Start();
			return stopwatch;
		}

        /// <summary>
        /// PostExecuteNonQuery hook
        /// </summary>
        /// <param name="command">The wrapped command</param>
        /// <param name="profilingObject"></param>
        protected override void PostExecuteNonQuery(DbCommand command, object profilingObject)
		{
			ShowTime(command, profilingObject);
		}

        /// <summary>
        /// PreExecuteScalar hook
        /// </summary>
        /// <param name="command">The wrapped command</param>
        /// <returns></returns>
		protected override object PreExecuteScalar(DbCommand command)
		{
			Console.WriteLine("ExecuteScalar()");
			ShowSql(command);
            var stopwatch = new Stopwatch();
            stopwatch.Start();
			return stopwatch;
		}

        /// <summary>
        /// PostExecuteScalar hook
        /// </summary>
        /// <param name="command">The wrapped command</param>
        /// <param name="profilingObject"></param>
        protected override void PostExecuteScalar(DbCommand command, object profilingObject)
		{
			ShowTime(command, profilingObject);
		}

        /// <summary>
        /// Log SQL for command
        /// </summary>
        /// <param name="command">The command</param>
		private static void ShowSql(DbCommand command)
		{
			if (command.CommandType == System.Data.CommandType.StoredProcedure)
			{
				Console.Write("(sp) ");
			}
			else if (command.CommandType == System.Data.CommandType.Text)
			{
			}
			else
			{
				throw new NotSupportedException("CommandType=" + command.CommandType + " not supported in SqlProfiler");
			}
			Console.WriteLine(command.CommandText);
			foreach (DbParameter param in command.Parameters)
			{
				// TO DO: Also show the DB specific parameter type for known databases
				Console.WriteLine(" {0} [{1}] = {2} ({3})", param.ParameterName, param.Direction, param.Value, param.DbType);
			}
		}

        /// <summary>
        /// Log time taken for command
        /// </summary>
        /// <param name="command">The command</param>
        /// <param name="profilingObject">The profiling object, which will be a <see cref="Stopwatch"/></param>
        private static void ShowTime(DbCommand command, object profilingObject)
		{
			var stopwatch = (Stopwatch)profilingObject;
            stopwatch.Stop();
			Console.WriteLine("Time: {0}ms", stopwatch.ElapsedMilliseconds);
			Console.WriteLine();
		}
	}
}