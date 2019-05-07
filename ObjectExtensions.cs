using System;
using System.Reflection;

namespace SqlProfiler
{
	static internal class ObjectExtensions
	{
		static public PropertyInfo GetNonPublicProperty(this Type type, string name)
		{
			if (type == null) throw new ArgumentNullException(nameof(type));
			// Including System.Reflection.TypeExtensions where needed to avoid explicit call to .GetTypeInfo() here
			var pi = type.GetProperty(name, BindingFlags.Instance | BindingFlags.NonPublic);
			if (pi == null) throw new InvalidOperationException(type + " must have non-public property " + name);
			return pi;
		}

		static public MethodInfo GetNonPublicMethod(this Type type, string name, Type[] types)
		{
			if (type == null) throw new ArgumentNullException(nameof(type));
			// Including System.Reflection.TypeExtensions where needed to avoid explicit call to .GetTypeInfo() here
			// (This variant of GetMethod doesn't exist in .Net Core 1.1 or .Net Standard 1.4; which means we simply can't do DbCommand wrapping in those.)
			var mi = type.GetMethod(name, BindingFlags.Instance | BindingFlags.NonPublic, null, CallingConventions.HasThis, types, null);
			if (mi == null) throw new InvalidOperationException(type + " must have non-public method " + name);
			return mi;
		}

#if NET40
		internal static void SetValue(this PropertyInfo prop, object obj, object value)
		{
			prop.SetValue(obj, value, null);
		}

		internal static object GetValue(this PropertyInfo prop, object obj)
		{
			return prop.GetValue(obj, null);
		}
#endif
	}
}
