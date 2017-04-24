using System;
using System.Collections;
using System.Data;
using System.Data.Common;

namespace SqlProfiler
{
	public class SimpleFactoryWrapper : SqlProfilerFactoryWrapper
	{
		public SimpleFactoryWrapper(DbProviderFactory wrapped) : base(wrapped) {}

		override public DbCommand WrapCommand(DbCommand command)
		{
			return new SimpleCommandWrapper(command);
		}
	}
}