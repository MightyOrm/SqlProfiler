using System;
using System.Collections;
using System.Data;
using System.Data.Common;

namespace SqlProfiler
{
    /// <summary>
    /// Generic <see cref="DbProviderFactory"/> wrapper
    /// </summary>
	abstract public class FactoryWrapper : DbProviderFactory
	{
        /// <summary>
        /// The wrapped factory
        /// </summary>
		protected DbProviderFactory Wrapped;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="wrapped">The factory to wrap</param>
		public FactoryWrapper(DbProviderFactory wrapped)
		{
			Wrapped = wrapped;
		}

        /// <summary>
        /// Wrap database commands
        /// </summary>
        /// <param name="wrapped">The command to wrap</param>
        /// <returns></returns>
		abstract public CommandWrapper WrapCommand(DbCommand wrapped);

        /// <summary>
        /// Wrapped <see cref="DbProviderFactory.CreateCommand"/> method, with <see cref="PreCreateCommand(DbProviderFactory)"/> and <see cref="PostCreateCommand(DbProviderFactory, object)"/> hooks
        /// </summary>
        /// <returns></returns>
		public override DbCommand CreateCommand()
		{
			var profilingObject = PreCreateCommand(Wrapped);
			var command = Wrapped.CreateCommand();
			var wrapped = WrapCommand(command);
			PostCreateCommand(Wrapped, profilingObject);
			return wrapped;
		}

        /// <summary>
        /// Pre-<see cref="DbProviderFactory.CreateCommand"/> hook
        /// </summary>
        /// <param name="factory">The wrapped factory</param>
        /// <returns></returns>
		public virtual object PreCreateCommand(DbProviderFactory factory) { return null; }

        /// <summary>
        /// Post-<see cref="DbProviderFactory.CreateCommand"/> hook
        /// </summary>
        /// <param name="factory">The wrapped factory</param>
        /// <param name="profilingObject">Whatever was returned from the pre-method hook</param>
        /// <returns></returns>
		public virtual void PostCreateCommand(DbProviderFactory factory, object profilingObject) {}

        /// <summary>
        /// NB Returns non-wrapped connection object. TO DO: Make this return ConnectionWrapper.
        /// </summary>
        /// <param name="connection"></param>
        /// <returns></returns>
		abstract public DbConnection WrapConnection(DbConnection connection);

        /// <summary>
        /// <see cref="DbProviderFactory.CreateConnection"/> hook, with <see cref="PreCreateConnection(DbProviderFactory)"/> and <see cref="PostCreateConnection(DbProviderFactory, object)"/> hooks
        /// </summary>
        /// <returns></returns>
		public override DbConnection CreateConnection()
		{
			var profilingObject = PreCreateConnection(Wrapped);
			var connection = Wrapped.CreateConnection();
			var wrapped = WrapConnection(connection);
			PostCreateConnection(Wrapped, profilingObject);
			return wrapped;
		}

        /// <summary>
        /// Pre-<see cref="DbProviderFactory.CreateConnection"/> hook
        /// </summary>
        /// <param name="factory">The wrapped factory</param>
        /// <returns></returns>
		public virtual object PreCreateConnection(DbProviderFactory factory) { return null; }

        /// <summary>
        /// Pre-<see cref="DbProviderFactory.CreateConnection"/> hook
        /// </summary>
        /// <param name="factory">The wrapped factory</param>
        /// <param name="profilingObject">Whatever was returned from the pre-method hook</param>
        /// <returns></returns>
		public virtual void PostCreateConnection(DbProviderFactory factory, object profilingObject) { }
	}
}