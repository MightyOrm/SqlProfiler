using System;
using System.Data;
using System.Data.Common;
using System.Dynamic;
using System.Linq.Expressions;
using System.Reflection;

namespace SqlProfiler
{
	/// <summary>
	/// Supports dynamically getting and setting named properties on the wrapped DbCommand.
	/// </summary>
	internal class DynamicCommandWrapper : DynamicObject
	{
		private DbCommand Wrapped;
		private TypeInfo TypeInfo = typeof(DbCommand).GetTypeInfo();

		public DynamicCommandWrapper(DbCommand wrapped)
		{
			Wrapped = wrapped;
		}

		public override bool TryGetMember(GetMemberBinder binder, out object result)
		{
			var property = TypeInfo.GetProperty(binder.Name);
			
			if (property == null)
			{
				result = null;
				return false;
			}

			result = property.GetValue(Wrapped);
			return true;
		}

		public override bool TrySetMember(SetMemberBinder binder, object value)
		{
			var property = TypeInfo.GetProperty(binder.Name);
			
			if (property == null)
			{
				return false;
			}

			property.SetValue(Wrapped, value);
			return true;
		}
	}

	public class SqlProfilerCommandWrapper : DbCommand, IDynamicMetaObjectProvider
	{
		/// <summary>
		/// The wrapped DbCommand
		/// </summary>
		/// <returns></returns>
		public DbCommand Wrapped { get; }

		// Supports dynamically getting and setting named properties on the wrapped DbCommand.
		private DynamicCommandWrapper DynamicWrapper;
		
		public SqlProfilerCommandWrapper(DbCommand wrapped)
		{
			Wrapped = wrapped;
			DynamicWrapper = new DynamicCommandWrapper(Wrapped);
		}

		/// <summary>
		/// Supports dynamically getting and setting named properties on the wrapped DbCommand.
		/// </summary>
		/// <param name="parameter"></param>
		/// <returns></returns>
		public DynamicMetaObject GetMetaObject(Expression parameter)
		{
			return ((IDynamicMetaObjectProvider)DynamicWrapper).GetMetaObject(parameter);
		}

		public override string CommandText { get { return Wrapped.CommandText; } set { Wrapped.CommandText = value; } }
		public override int CommandTimeout { get { return Wrapped.CommandTimeout; } set { Wrapped.CommandTimeout = value; } }
		public override CommandType CommandType { get { return Wrapped.CommandType; } set { Wrapped.CommandType = value; } }
		public override bool DesignTimeVisible {  get { return Wrapped.DesignTimeVisible; } set { Wrapped.DesignTimeVisible = value; }  }
		public override UpdateRowSource UpdatedRowSource { get { return Wrapped.UpdatedRowSource; } set { Wrapped.UpdatedRowSource = value; } }
		protected override DbConnection DbConnection { get { return ((dynamic)Wrapped).DbConnection; } set { ((dynamic)Wrapped).DbConnection = value; } }

		protected override DbParameterCollection DbParameterCollection  { get { return ((dynamic)Wrapped).DbParameterCollection; } }

		protected override DbTransaction DbTransaction { get { return ((dynamic)Wrapped).DbTransaction; } set { ((dynamic)Wrapped).DbTransaction = value; } }

		// wrapped call
		public override void Cancel()
		{
			var profiling = PreCancel(Wrapped);
			Wrapped.Cancel();
			PostCancel(profiling);
		}

		// optional user hooks
		public virtual object PreCancel(DbCommand command) { return null; }
		public virtual void PostCancel(object profiling) {}

		// wrapped call
		public override int ExecuteNonQuery()
		{
			var profiling = PreExecuteNonQuery(Wrapped);
			var result = Wrapped.ExecuteNonQuery();
			PostExecuteNonQuery(profiling);
			return result;
		}

		// optional user hooks
		public virtual object PreExecuteNonQuery(DbCommand command) { return null; }
		public virtual void PostExecuteNonQuery(object profiling) {}

		// wrapped call
		public override object ExecuteScalar()
		{
			var profiling = PreExecuteScalar(Wrapped);
			var result = Wrapped.ExecuteScalar();
			PostExecuteScalar(profiling);
			return result;
		}

		// optional user hooks
		public virtual object PreExecuteScalar(DbCommand command) { return null; }
		public virtual void PostExecuteScalar(object profiling) {}

		// wrapped call
		public override void Prepare()
		{
			var profiling = PrePrepare(Wrapped);
			Wrapped.Prepare();
			PostPrepare(profiling);
		}
		
		// optional user hooks
		public virtual object PrePrepare(DbCommand command) { return null; }
		public virtual void PostPrepare(object profiling) {}

		// wrapped call
		protected override DbParameter CreateDbParameter()
		{
			var profiling = PreCreateDbParameter(Wrapped);
			var reader = ((dynamic)Wrapped).CreateDbParameter();
			PostCreateDbParameter(profiling);
			return reader;
		}

		// optional user hooks
		public virtual object PreCreateDbParameter(DbCommand command) { return null; }
		public virtual void PostCreateDbParameter(object profiling) {}

		// wrapped call
		protected override DbDataReader ExecuteDbDataReader(CommandBehavior behavior)
		{
			var profiling = PreExecuteDbDataReader(Wrapped, behavior);
			var reader = ((dynamic)Wrapped).ExecuteDbDataReader();
			PostExecuteDbDataReader(profiling, behavior);
			return reader;
		}

		// optional user hooks
		public virtual object PreExecuteDbDataReader(DbCommand command, CommandBehavior behavior) { return null; }
		public virtual void PostExecuteDbDataReader(object profiling, CommandBehavior behavior) {}
	}
}
