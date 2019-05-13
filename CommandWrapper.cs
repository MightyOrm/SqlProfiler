using System;
using System.Data;
using System.Data.Common;
using System.Dynamic;
using System.Linq.Expressions;
using System.Reflection;

namespace SqlProfiler
{
    /// <summary>
    /// Generic <see cref="DbCommand"/> wrapper with dynamic tricks to allow easy access to driver-specific properties of wrapped command
    /// </summary>
    [System.ComponentModel.DesignerCategory("")]
    public class CommandWrapper : DbCommand, IDynamicMetaObjectProvider
	{
		/// <summary>
		/// The wrapped command
		/// </summary>
		/// <returns></returns>
		/// <remarks>This is public for simple debugging by the user; the call to <see cref="DelegatingMetaObject"/> needs to add <see cref="BindingFlags.Public"/> to match.</remarks>
		public DbCommand Wrapped { get; }

		private readonly PropertyInfo _DbConnection;
		private readonly PropertyInfo _DbParameterCollection;
		private readonly PropertyInfo _DbTransaction;

		private readonly MethodInfo _CreateDbParameter;
		private readonly MethodInfo _ExecuteDbDataReader;

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="wrapped">The command to wrap</param>
		public CommandWrapper(DbCommand wrapped)
		{
			Wrapped = wrapped;

			// Get all the protected elements which we need to pass back to the wrapped object
			var type = wrapped.GetType();
			_DbConnection = type.GetNonPublicProperty("DbConnection");
			_DbParameterCollection = type.GetNonPublicProperty("DbParameterCollection");
			_DbTransaction = type.GetNonPublicProperty("DbTransaction");
			_CreateDbParameter = type.GetNonPublicMethod("CreateDbParameter", Type.EmptyTypes);
			_ExecuteDbDataReader = type.GetNonPublicMethod("ExecuteDbDataReader", new Type[] { typeof(CommandBehavior) });
		}

        /// <summary>
        /// Supports dynamically getting and setting properties on the wrapped DbCommand
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        public DynamicMetaObject GetMetaObject(Expression expression)
		{
			return new DelegatingMetaObject(expression, this, nameof(Wrapped), BindingFlags.Instance | BindingFlags.Public);
		}

        /// <summary>
        /// Wrapped <see cref="DbCommand.CommandText"/> property
        /// </summary>
		public override string CommandText
        {
            get { return Wrapped.CommandText; }
            set { Wrapped.CommandText = value; }
        }

        /// <summary>
        /// Wrapped <see cref="DbCommand.CommandTimeout"/> property
        /// </summary>
		public override int CommandTimeout
        {
            get { return Wrapped.CommandTimeout; }
            set { Wrapped.CommandTimeout = value; }
        }

        /// <summary>
        /// Wrapped <see cref="DbCommand.CommandType"/> property
        /// </summary>
		public override CommandType CommandType
        {
            get { return Wrapped.CommandType; }
            set { Wrapped.CommandType = value; }
        }

        /// <summary>
        /// Wrapped <see cref="DbCommand.DesignTimeVisible"/> property
        /// </summary>
		public override bool DesignTimeVisible
        {
            get { return Wrapped.DesignTimeVisible; }
            set { Wrapped.DesignTimeVisible = value; }
        }

        /// <summary>
        /// Wrapped <see cref="DbCommand.UpdatedRowSource"/> property
        /// </summary>
		public override UpdateRowSource UpdatedRowSource
        {
            get { return Wrapped.UpdatedRowSource; }
            set { Wrapped.UpdatedRowSource = value; }
        }

        /// <summary>
        /// Wrapped non-public <see cref="DbCommand.DbConnection"/> property
        /// </summary>
		protected override DbConnection DbConnection
        {
            get { return (DbConnection)_DbConnection.GetValue(Wrapped); }
            set { _DbConnection.SetValue(Wrapped, (DbConnection)value); }
        }

        /// <summary>
        /// Wrapped non-public <see cref="DbCommand.DbParameterCollection "/> property
        /// </summary>
		protected override DbParameterCollection DbParameterCollection
        {
            get { return (DbParameterCollection)_DbParameterCollection.GetValue(Wrapped); }
        }

        /// <summary>
        /// Wrapped non-public <see cref="DbCommand.DbTransaction"/> property
        /// </summary>
		protected override DbTransaction DbTransaction { get { return (DbTransaction)_DbTransaction.GetValue(Wrapped); } set { _DbTransaction.SetValue(Wrapped, (DbTransaction)value); } }

        /// <summary>
        /// Wrapped <see cref="DbCommand.Cancel"/> method, with <see cref="PreCancel(DbCommand)"/> and <see cref="PostCancel(DbCommand, object)"/> hooks
        /// </summary>
		public override void Cancel()
		{
			var profilingObject = PreCancel(Wrapped);
			Wrapped.Cancel();
			PostCancel(Wrapped, profilingObject);
		}

        /// <summary>
        /// Pre-<see cref="DbCommand.Cancel"/> hook
        /// </summary>
        /// <param name="command">The wrapped command</param>
        /// <returns></returns>
		protected virtual object PreCancel(DbCommand command) { return null; }

        /// <summary>
        /// Post-<see cref="DbCommand.Cancel"/> hook
        /// </summary>
        /// <param name="command">The wrapped command</param>
        /// <param name="profilingObject">Whatever was returned from <see cref="PreCancel(DbCommand)"/></param>
        /// <returns></returns>
		protected virtual void PostCancel(DbCommand command, object profilingObject) {}

        /// <summary>
        /// Wrapped <see cref="DbCommand.ExecuteNonQuery"/> method, with <see cref="PreExecuteNonQuery(DbCommand)"/> and <see cref="PostExecuteNonQuery(DbCommand, object)"/> hooks
        /// </summary>
        /// <returns></returns>
		public override int ExecuteNonQuery()
		{
			var profilingObject = PreExecuteNonQuery(Wrapped);
			var result = Wrapped.ExecuteNonQuery();
			PostExecuteNonQuery(Wrapped, profilingObject);
			return result;
		}

        /// <summary>
        /// Pre-<see cref="DbCommand.ExecuteNonQuery"/> hook
        /// </summary>
        /// <param name="command">The wrapped command</param>
        /// <returns></returns>
		protected virtual object PreExecuteNonQuery(DbCommand command) { return null; }

        /// <summary>
        /// Post-<see cref="DbCommand.ExecuteNonQuery"/> hook
        /// </summary>
        /// <param name="command">The wrapped command</param>
        /// <param name="profilingObject">Whatever was returned from the pre-method hook</param>
        /// <returns></returns>
		protected virtual void PostExecuteNonQuery(DbCommand command, object profilingObject) {}

        /// <summary>
        /// Wrapped <see cref="DbCommand.ExecuteScalar"/> method, with <see cref="PreExecuteScalar(DbCommand)"/> and <see cref="PostExecuteScalar(DbCommand, object)"/> hooks
        /// </summary>
        /// <returns></returns>
		public override object ExecuteScalar()
		{
			var profilingObject = PreExecuteScalar(Wrapped);
			var result = Wrapped.ExecuteScalar();
			PostExecuteScalar(Wrapped, profilingObject);
			return result;
		}

        /// <summary>
        /// Pre-<see cref="DbCommand.ExecuteScalar"/> hook
        /// </summary>
        /// <param name="command">The wrapped command</param>
        /// <returns></returns>
		protected virtual object PreExecuteScalar(DbCommand command) { return null; }

        /// <summary>
        /// Post-<see cref="DbCommand.ExecuteScalar"/> hook
        /// </summary>
        /// <param name="command">The wrapped command</param>
        /// <param name="profilingObject">Whatever was returned from the pre-method hook</param>
        /// <returns></returns>
		protected virtual void PostExecuteScalar(DbCommand command, object profilingObject) {}

        /// <summary>
        /// Wrapped <see cref="DbCommand.Prepare"/> method, with <see cref="PrePrepare(DbCommand)"/> and <see cref="PostPrepare(DbCommand, object)"/> hooks
        /// </summary>
		public override void Prepare()
		{
			var profilingObject = PrePrepare(Wrapped);
			Wrapped.Prepare();
			PostPrepare(Wrapped, profilingObject);
		}

        /// <summary>
        /// Pre-<see cref="DbCommand.Prepare"/> hook
        /// </summary>
        /// <param name="command">The wrapped command</param>
        /// <returns></returns>
        protected virtual object PrePrepare(DbCommand command) { return null; }

        /// <summary>
        /// Post-<see cref="DbCommand.Prepare"/> hook
        /// </summary>
        /// <param name="command">The wrapped command</param>
        /// <param name="profilingObject">Whatever was returned from the pre-method hook</param>
        /// <returns></returns>
        protected virtual void PostPrepare(DbCommand command, object profilingObject) {}

        /// <summary>
        /// Wrapped non-public <see cref="DbCommand.CreateDbParameter"/> method, with <see cref="PreCreateDbParameter(DbCommand)"/> and <see cref="PostCreateDbParameter(DbCommand, object)"/> hooks
        /// </summary>
        /// <returns></returns>
		protected override DbParameter CreateDbParameter()
		{
			var profilingObject = PreCreateDbParameter(Wrapped);
            DbParameter param;
            try
            {
                param = (DbParameter)_CreateDbParameter.Invoke(Wrapped, null);
            }
            catch (TargetInvocationException ex)
            {
                throw ex.InnerException;
            }
            PostCreateDbParameter(Wrapped, profilingObject);
			return param;
		}

        /// <summary>
        /// Pre-<see cref="DbCommand.CreateDbParameter"/> hook
        /// </summary>
        /// <param name="command">The wrapped command</param>
        /// <returns></returns>
		protected virtual object PreCreateDbParameter(DbCommand command) { return null; }

        /// <summary>
        /// Post-<see cref="DbCommand.CreateDbParameter"/> hook
        /// </summary>
        /// <param name="command">The wrapped command</param>
        /// <param name="profilingObject">Whatever was returned from the pre-method hook</param>
        /// <returns></returns>
		protected virtual void PostCreateDbParameter(DbCommand command, object profilingObject) {}

        /// <summary>
        /// Wrapped non-public <see cref="DbCommand.ExecuteDbDataReader(CommandBehavior)"/> method, with <see cref="PreExecuteDbDataReader(DbCommand, CommandBehavior)"/> and <see cref="PostExecuteDbDataReader(DbCommand, CommandBehavior, object)"/> hooks
        /// </summary>
        /// <param name="behavior">The command behavior</param>
        /// <returns></returns>
		protected override DbDataReader ExecuteDbDataReader(CommandBehavior behavior)
		{
			var profilingObject = PreExecuteDbDataReader(Wrapped, behavior);
            DbDataReader reader;
            try
            {
                reader = (DbDataReader)_ExecuteDbDataReader.Invoke(Wrapped, new object[] { behavior });
            }
            catch (TargetInvocationException ex)
            {
                throw ex.InnerException;
            }
            PostExecuteDbDataReader(Wrapped, behavior, profilingObject);
			return reader;
		}

        /// <summary>
        /// Pre-<see cref="DbCommand.ExecuteDbDataReader(CommandBehavior)"/> hook
        /// </summary>
        /// <param name="command">The wrapped command</param>
        /// <param name="behavior">The command behavior</param>
        /// <returns></returns>
		protected virtual object PreExecuteDbDataReader(DbCommand command, CommandBehavior behavior) { return null; }

        /// <summary>
        /// Post-<see cref="DbCommand.ExecuteDbDataReader(CommandBehavior)"/> hook
        /// </summary>
        /// <param name="command">The wrapped command</param>
        /// <param name="behavior">The command behavior</param>
        /// <param name="profilingObject">Whatever was returned from the pre-method hook</param>
        /// <returns></returns>
		protected virtual void PostExecuteDbDataReader(DbCommand command, CommandBehavior behavior, object profilingObject) {}
	}
}
