using System;
using System.Collections;
using System.Data;
using System.Data.Common;

namespace SqlProfiler
{
	abstract public class FactoryWrapper : DbProviderFactory
	{
		protected DbProviderFactory Wrapped;

		public FactoryWrapper(DbProviderFactory wrapped)
		{
			Wrapped = wrapped;
		}

		abstract public CommandWrapper WrapCommand(DbCommand command);

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

		// TO DO: Make this return ConnectionWrapper
		abstract public DbConnection WrapConnection(DbConnection connection);

		public override DbConnection CreateConnection()
		{
			var profiling = PreCreateConnection();
			var connection = Wrapped.CreateConnection();
			var wrapped = WrapConnection(connection);
			PostCreateConnection(profiling);
			return wrapped;
		}

		public virtual object PreCreateConnection() { return null; }
		public virtual void PostCreateConnection(object profiling) { }
	}
}