using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace PartyScreenEnhancements
{
    public static class ReflectionHelper
    {
        internal static object Call(this object o, string methodName, params object[] args)
        {
            MethodInfo method = o.GetType().GetMethod(methodName, BindingFlags.Instance | BindingFlags.NonPublic);
            if (method != null)
            {
                try
                {
                    return method.Invoke(o, args);
                }
                catch (Exception)
                {
                }
            }
            return null;
        }

        internal static object GetField(this object o, string fieldName)
        {
            FieldInfo field = o.GetType().GetField(fieldName, BindingFlags.Instance | BindingFlags.NonPublic);
            if (field != null)
            {
                try
                {
                    return field.GetValue(o);
                }
                catch (Exception)
                {
                }
            }
            return null;
        }
    }
}
