using System;
using System.Collections;
using System.Data;
using System.Data.Common;

namespace SqlProfiler.Timing
{
    /// <summary>
    /// Timing factory wrapper. Not supported. Needs updating to use ILogger, etc., at least.
    /// </summary>
	public class TimingFactoryWrapper : FactoryWrapper
	{
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="wrapped">Factory to wrap</param>
		public TimingFactoryWrapper(DbProviderFactory wrapped) : base(wrapped) {}

        /// <summary>
        /// Wrap command in <see cref="TimingCommandWrapper"/>.
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
		override public CommandWrapper WrapCommand(DbCommand command)
		{
			return new TimingCommandWrapper(command);
		}

        /// <summary>
        /// Return connection as is.
        /// </summary>
        /// <param name="connection"></param>
        /// <returns></returns>
		override public DbConnection WrapConnection(DbConnection connection)
		{
			return connection;
		}
	}
}