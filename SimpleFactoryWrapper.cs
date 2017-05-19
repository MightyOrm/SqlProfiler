using System;
using System.Collections;
using System.Data;
using System.Data.Common;

namespace SqlProfiler
{
	public class SimpleFactoryWrapper : FactoryWrapper
	{
		public SimpleFactoryWrapper(DbProviderFactory wrapped) : base(wrapped) {}

		override public CommandWrapper WrapCommand(DbCommand command)
		{
			return new SimpleCommandWrapper(command);
		}

		override public DbConnection WrapConnection(DbConnection connection)
		{
			return connection;
		}
	}
}