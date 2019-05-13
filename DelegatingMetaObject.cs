using System;
using System.Dynamic;
using System.Linq.Expressions;
using System.Reflection;

namespace SqlProfiler
{
    /// <summary>
    /// Delegating meta-object: methods and properties which can't be handled by the outer object are handled by the inner object, which
    /// can be an <see cref="IDynamicMetaObjectProvider"/> (including, but not limited to <see cref="DynamicObject"/>) or just a plain object.
    /// </summary>
    internal class DelegatingMetaObject : DynamicMetaObject
	{
		private readonly DynamicMetaObject _innerMetaObject;

		/// <summary>
		/// Create delegating meta-object: methods and properties which can't be handled by the outer object are handled by the inner object, which
		/// can be an <see cref="IDynamicMetaObjectProvider"/> (including, but not limited to <see cref="DynamicObject"/>) or just a plain object.
		/// </summary>
		/// <remarks>
		/// This one is using Expression.Property or Expression.Field and it notices when you change the object to a different instance.
		/// However... this is even harder than I thought - this one still doesn't work if you change the type of the object at the binding site
		/// (e.g. sometimes a wrapping SqlProfiler and sometimes an unwrapped SqlCommand...). Still, this is good enough for us.
		/// </remarks>
		public DelegatingMetaObject(Expression expression, object outerObject, string innnerMemberName, BindingFlags bindingAttr = BindingFlags.Instance)
			: base(expression, BindingRestrictions.Empty, outerObject)
		{
			var outerType = outerObject.GetType();
			PropertyInfo innerProperty = outerType.GetProperty(innnerMemberName, bindingAttr);
			FieldInfo innerField = (innerProperty != null) ? null : outerType.GetField(innnerMemberName, bindingAttr);
			if (innerProperty == null && innerField == null)
			{
				throw new InvalidOperationException(string.Format("There is no {0} Property or Field named '{1}' in {2}", bindingAttr, innnerMemberName, outerType));
			}
			var innerObject = innerProperty != null ? innerProperty.GetValue(outerObject) : innerField.GetValue(outerObject);
			Expression self = Expression.Convert(Expression, LimitType);
			Expression innerExpression = innerProperty != null ? Expression.Property(self, innerProperty) : Expression.Field(self, innerField);
			var innerDynamicProvider = innerObject as IDynamicMetaObjectProvider;
			if (innerDynamicProvider != null)
			{
				_innerMetaObject = innerDynamicProvider.GetMetaObject(innerExpression);
			}
			else
			{
				// support non-dynamic inner object
				_innerMetaObject = new DynamicMetaObject(innerExpression, BindingRestrictions.Empty, innerObject);
			}
		}

        /// <summary>
        /// Dynamic invoke member
        /// </summary>
        /// <param name="binder">Binder</param>
        /// <param name="args">Arguments</param>
        /// <returns></returns>
		public override DynamicMetaObject BindInvokeMember(InvokeMemberBinder binder, DynamicMetaObject[] args)
		{
			DynamicMetaObject retval = _innerMetaObject.BindInvokeMember(binder, args);

			// call any parent object non-dynamic methods before trying wrapped object
			retval = binder.FallbackInvokeMember(this, args, retval);

			return retval;
		}

        /// <summary>
        /// Dynamic set member
        /// </summary>
        /// <param name="binder">Binder</param>
        /// <param name="value">Value</param>
        /// <returns></returns>
		public override DynamicMetaObject BindSetMember(SetMemberBinder binder, DynamicMetaObject value)
		{
			DynamicMetaObject retval = _innerMetaObject.BindSetMember(binder, value);

			// set any parent object non-dynamic member before trying wrapped object
			retval = binder.FallbackSetMember(this, value, retval);

			return retval;
		}

        /// <summary>
        /// Dynamic get member
        /// </summary>
        /// <param name="binder">Binder</param>
        /// <returns></returns>
		public override DynamicMetaObject BindGetMember(GetMemberBinder binder)
		{
			DynamicMetaObject retval = _innerMetaObject.BindGetMember(binder);

			// get from any parent object non-dynamic member before trying wrapped object
			retval = binder.FallbackGetMember(this, retval);

			return retval;
		}
	}
}
