using System.Linq;
using System.Reflection;

namespace GameProject.CoreEngine
{
    public static class CoreUtils
    {
        private class PropCopier<T>
        {
            public static readonly PropertyInfo[] Properties;
            
            static PropCopier()
            {
                Properties = typeof(T).GetProperties();
            }
        }
        
        public static void CopyPropertiesFrom<T>(this T destination, T source)
        {
            foreach (var property in PropCopier<T>.Properties
                .Where(p => p.CanRead && p.CanWrite))
                property.SetValue(destination, property.GetValue(source));
        }
    }
}