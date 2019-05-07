using System;
using System.Data;
using System.Data.Common;
using System.Dynamic;
using System.Linq.Expressions;
using System.Reflection;

namespace SqlProfiler
{
    /// <summary>
    /// Generic <see cref="DbCommand"/> wrapper with dynamic tricks to allow easy access to properties of wrapped command
    /// </summary>
    [System.ComponentModel.DesignerCategory("")]
    public class CommandWrapper : DbCommand, IDynamicMetaObjectProvider
	{
		/// <summary>
		/// The wrapped DbCommand
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
		/// Create the command wrapper
		/// </summary>
		/// <param name="wrapped"></param>
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
		/// <param name="parameter"></param>
		/// <returns></returns>
		public DynamicMetaObject GetMetaObject(Expression expression)
		{
			return new DelegatingMetaObject(expression, this, nameof(Wrapped), BindingFlags.Instance | BindingFlags.Public);
		}

		public override string CommandText { get { return Wrapped.CommandText; } set { Wrapped.CommandText = value; } }
		public override int CommandTimeout { get { return Wrapped.CommandTimeout; } set { Wrapped.CommandTimeout = value; } }
		public override CommandType CommandType { get { return Wrapped.CommandType; } set { Wrapped.CommandType = value; } }
		public override bool DesignTimeVisible {  get { return Wrapped.DesignTimeVisible; } set { Wrapped.DesignTimeVisible = value; }  }
		public override UpdateRowSource UpdatedRowSource { get { return Wrapped.UpdatedRowSource; } set { Wrapped.UpdatedRowSource = value; } }
		protected override DbConnection DbConnection { get { return (DbConnection)_DbConnection.GetValue(Wrapped); } set { _DbConnection.SetValue(Wrapped, (DbConnection)value); } }

		protected override DbParameterCollection DbParameterCollection  { get { return (DbParameterCollection)_DbParameterCollection.GetValue(Wrapped); } }

		protected override DbTransaction DbTransaction { get { return (DbTransaction)_DbTransaction.GetValue(Wrapped); } set { _DbTransaction.SetValue(Wrapped, (DbTransaction)value); } }

		// wrapped call
		public override void Cancel()
		{
			var profiling = PreCancel(Wrapped);
			Wrapped.Cancel();
			PostCancel(profiling);
		}

		// optional user hooks
		protected virtual object PreCancel(DbCommand command) { return null; }
		protected virtual void PostCancel(object profiling) {}

		// wrapped call
		public override int ExecuteNonQuery()
		{
			var profiling = PreExecuteNonQuery(Wrapped);
			var result = Wrapped.ExecuteNonQuery();
			PostExecuteNonQuery(profiling);
			return result;
		}

		// optional user hooks
		protected virtual object PreExecuteNonQuery(DbCommand command) { return null; }
		protected virtual void PostExecuteNonQuery(object profiling) {}

		// wrapped call
		public override object ExecuteScalar()
		{
			var profiling = PreExecuteScalar(Wrapped);
			var result = Wrapped.ExecuteScalar();
			PostExecuteScalar(profiling);
			return result;
		}

		// optional user hooks
		protected virtual object PreExecuteScalar(DbCommand command) { return null; }
		protected virtual void PostExecuteScalar(object profiling) {}

		// wrapped call
		public override void Prepare()
		{
			var profiling = PrePrepare(Wrapped);
			Wrapped.Prepare();
			PostPrepare(profiling);
		}
		
		// optional user hooks
		protected virtual object PrePrepare(DbCommand command) { return null; }
		protected virtual void PostPrepare(object profiling) {}

		// wrapped call
		protected override DbParameter CreateDbParameter()
		{
			var profiling = PreCreateDbParameter(Wrapped);
            DbParameter param;
            try
            {
                param = (DbParameter)_CreateDbParameter.Invoke(Wrapped, null);
            }
            catch (TargetInvocationException ex)
            {
                throw ex.InnerException;
            }
            PostCreateDbParameter(profiling);
			return param;
		}

		// optional user hooks
		protected virtual object PreCreateDbParameter(DbCommand command) { return null; }
		protected virtual void PostCreateDbParameter(object profiling) {}

		// wrapped call
		protected override DbDataReader ExecuteDbDataReader(CommandBehavior behavior)
		{
			var profiling = PreExecuteDbDataReader(Wrapped, behavior);
            DbDataReader reader;
            try
            {
                reader = (DbDataReader)_ExecuteDbDataReader.Invoke(Wrapped, new object[] { behavior });
            }
            catch (TargetInvocationException ex)
            {
                throw ex.InnerException;
            }
            PostExecuteDbDataReader(profiling, behavior);
			return reader;
		}

		// optional user hooks
		protected virtual object PreExecuteDbDataReader(DbCommand command, CommandBehavior behavior) { return null; }
		protected virtual void PostExecuteDbDataReader(object profiling, CommandBehavior behavior) {}
	}
}
