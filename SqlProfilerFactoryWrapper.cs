using System;
using System.Collections;
using System.Data;
using System.Data.Common;

namespace SqlProfiler
{
	abstract public class SqlProfilerFactoryWrapper : DbProviderFactory
	{
		protected DbProviderFactory Wrapped;

		public SqlProfilerFactoryWrapper(DbProviderFactory wrapped)
		{
			Wrapped = wrapped;
		}

		// This is what you need to override to make a SqlProfilerFactoryWrapper
		abstract public DbCommand WrapCommand(DbCommand command);

		public override DbCommand CreateCommand()
		{
			var profiling = PreCreateCommand();
			var command = Wrapped.CreateCommand();
			var wrapped = WrapCommand(command);
			PostCreateCommand(profiling);
			return wrapped;
		}

		public virtual object PreCreateCommand() { return null; }
		public virtual void PostCreateCommand(object profiling) {}
	}
}