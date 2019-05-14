using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace SqlProfiler
{
    static internal class ObjectExtensions
    {
#if NETSTANDARD1_4
        public static MethodInfo GetMethod(this Type type, string name, BindingFlags bindingFlags, object binder, CallingConventions callingConvention, Type[] types, object[] modifiers)
        {
            if (binder != null || modifiers != null)
            {
                throw new NotSupportedException($"{nameof(binder)} and {nameof(modifiers)} not supported in {nameof(SqlProfiler)} {nameof(Type)}.{nameof(GetMethod)} extension method");
            }
            MethodInfo[] methods = type.GetMethods(bindingFlags);
            List<MethodInfo> method = methods.Where(m => {
                if (m.Name != name) return false;
                if ((m.CallingConvention & callingConvention) != callingConvention) return false;
                var p = m.GetParameters();
                if (p.Length != types.Length) return false;
                for (int i = 0; i < types.Length; i++)
                {
                    if (types[i] != p[i].ParameterType) return false;
                }
                return true;
            }).ToList();
            if (method.Count == 0) return null;
            if (method.Count > 1) throw new AmbiguousMatchException();
            return method[0];
        }
#endif

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
			// (This variant of GetMethod doesn't exist in .Net Core 1.1 or .Net Standard 1.4; but we've now implemented it partially above.)
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
