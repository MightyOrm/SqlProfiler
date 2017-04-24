using System;
using System.Collections;
using System.Data;
using System.Data.Common;
using System.Diagnostics;

namespace SqlProfiler
{
	public class SimpleCommandWrapper : SqlProfilerCommandWrapper
	{
		public SimpleCommandWrapper(DbCommand wrapped) : base(wrapped)
		{
		}

		public override object PreExecuteDbDataReader(DbCommand command, CommandBehavior behavior)
		{
			Console.WriteLine("ExecuteDbDataReader(" + ((behavior != CommandBehavior.Default) ? behavior.ToString() : "") + ")");
			ShowSql(command);
            var stopwatch = new Stopwatch();
            stopwatch.Start();
			return stopwatch;
		}

		public override void PostExecuteDbDataReader(object profiling, CommandBehavior behavior)
		{
			ShowTime(profiling);
		}

		public override object PreExecuteNonQuery(DbCommand command)
		{
			Console.WriteLine("ExecuteNonQuery()");
			ShowSql(command);
            var stopwatch = new Stopwatch();
            stopwatch.Start();
			return stopwatch;
		}
		
		public override void PostExecuteNonQuery(object profiling)
		{
			ShowTime(profiling);
		}

		public override object PreExecuteScalar(DbCommand command)
		{
			Console.WriteLine("ExecuteScalar()");
			ShowSql(command);
            var stopwatch = new Stopwatch();
            stopwatch.Start();
			return stopwatch;
		}
		
		public override void PostExecuteScalar(object profiling)
		{
			ShowTime(profiling);
		}

		private void ShowSql(DbCommand command)
		{
			if (Wrapped.CommandType == System.Data.CommandType.StoredProcedure)
			{
				Console.WriteLine("SP({0})", Wrapped.CommandText);
			}
			else if (Wrapped.CommandType == System.Data.CommandType.Text)
			{
				Console.WriteLine("SQL: \"{0}\"", Wrapped.CommandText);
			}
			else
			{
				throw new NotSupportedException("CommandType=" + Wrapped.CommandType + " not supported in SqlProfiler");
			}
			foreach (DbParameter param in Wrapped.Parameters)
			{
				Console.Write(param.Direction.ToString());
				// TO DO: Also show the DB specific parameter type for known databases
				Console.Write(" {0} = {1} ({2})", param.ParameterName, param.Value, param.DbType);
			}
		}
		
		private void ShowTime(object profiling)
		{
			var stopwatch = (Stopwatch)profiling;
            stopwatch.Stop();
			Console.WriteLine("Time: {0}ms", stopwatch.ElapsedMilliseconds);
		}
	}
}