using System;
using System.Dynamic;
using System.Linq.Expressions;
using System.Reflection;

namespace SqlProfiler
{
	// TO DO: internal
	public class DelegatingMetaObject : DynamicMetaObject
	{
		private readonly DynamicMetaObject _innerMetaObject;

		/// <summary>
		/// Create delegating meta-object: methods and properties which can't be handled by the outer object are handled by the inner object, which
		/// can be an <see cref="IDynamicMetaObjectProvider"/> (including, but not limited to <see cref="MSDynamicObject"/>) or just a plain object.
		/// </summary>
		public DelegatingMetaObject(Expression expression, object outerObject, string wrappedPropertyName, BindingFlags binding = BindingFlags.Instance)
			: base(expression, BindingRestrictions.Empty, outerObject)
		{
			PropertyInfo innerProperty = outerObject.GetType().GetProperty(wrappedPropertyName, binding);
			var innerObject = innerProperty.GetValue(outerObject);
			Expression self = Expression.Convert(Expression, LimitType);
			Expression innerExpression = Expression.Property(self, innerProperty);
			var innerDynamicProvider = innerObject as IDynamicMetaObjectProvider;
			if (innerDynamicProvider != null)
			{
				_innerMetaObject = innerDynamicProvider.GetMetaObject(innerExpression);
			}
			else
			{
				_innerMetaObject = new DynamicMetaObject(innerExpression, BindingRestrictions.Empty, innerObject);
			}
		}

		public override DynamicMetaObject BindInvokeMember(InvokeMemberBinder binder, DynamicMetaObject[] args)
		{
			DynamicMetaObject retval = _innerMetaObject.BindInvokeMember(binder, args);

			// call any parent object non-dynamic methods before trying wrapped object
			retval = binder.FallbackInvokeMember(this, args, retval);

			return retval;
		}

		public override DynamicMetaObject BindSetMember(SetMemberBinder binder, DynamicMetaObject value)
		{
			DynamicMetaObject retval = _innerMetaObject.BindSetMember(binder, value);

			// set any parent object non-dynamic member before trying wrapped object
			retval = binder.FallbackSetMember(this, value, retval);

			return retval;
		}

		public override DynamicMetaObject BindGetMember(GetMemberBinder binder)
		{
			DynamicMetaObject retval = _innerMetaObject.BindGetMember(binder);

			// get from any parent object non-dynamic member before trying wrapped object
			retval = binder.FallbackGetMember(this, retval);

			return retval;
		}
	}
}
