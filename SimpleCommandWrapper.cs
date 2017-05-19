using System;
using System.Collections;
using System.Data;
using System.Data.Common;
using System.Diagnostics;
using System.Dynamic;

namespace SqlProfiler
{
	public class SimpleCommandWrapper : CommandWrapper
	{
		public SimpleCommandWrapper(DbCommand wrapped) : base(wrapped)
		{
		}

		protected override object PreExecuteDbDataReader(DbCommand command, CommandBehavior behavior)
		{
			Console.WriteLine("ExecuteDbDataReader(" + ((behavior != CommandBehavior.Default) ? behavior.ToString() : "") + ")");
			ShowSql(command);
            var stopwatch = new Stopwatch();
            stopwatch.Start();
			return stopwatch;
		}

		protected override void PostExecuteDbDataReader(object profiling, CommandBehavior behavior)
		{
			ShowTime(profiling);
		}

		protected override object PreExecuteNonQuery(DbCommand command)
		{
			Console.WriteLine("ExecuteNonQuery()");
			ShowSql(command);
            var stopwatch = new Stopwatch();
            stopwatch.Start();
			return stopwatch;
		}
		
		protected override void PostExecuteNonQuery(object profiling)
		{
			ShowTime(profiling);
		}

		protected override object PreExecuteScalar(DbCommand command)
		{
			Console.WriteLine("ExecuteScalar()");
			ShowSql(command);
            var stopwatch = new Stopwatch();
            stopwatch.Start();
			return stopwatch;
		}
		
		protected override void PostExecuteScalar(object profiling)
		{
			ShowTime(profiling);
		}

		private void ShowSql(DbCommand command)
		{
			if (Wrapped.CommandType == System.Data.CommandType.StoredProcedure)
			{
				Console.Write("(sp) ");
			}
			else if (Wrapped.CommandType == System.Data.CommandType.Text)
			{
			}
			else
			{
				throw new NotSupportedException("CommandType=" + Wrapped.CommandType + " not supported in SqlProfiler");
			}
			Console.WriteLine(Wrapped.CommandText);
			foreach (DbParameter param in Wrapped.Parameters)
			{
				// TO DO: Also show the DB specific parameter type for known databases
				Console.WriteLine(" {0} [{1}] = {2} ({3})", param.ParameterName, param.Direction, param.Value, param.DbType);
			}
		}
		
		private void ShowTime(object profiling)
		{
			var stopwatch = (Stopwatch)profiling;
            stopwatch.Stop();
			Console.WriteLine("Time: {0}ms", stopwatch.ElapsedMilliseconds);
			Console.WriteLine();
		}
	}
}