using System.Reflection;

namespace Server.Utility;

public static class ReflectionHelper
{
    public static IEnumerable<KeyValuePair<Type, T>> GetAllTypes<T>() where T : Attribute
    {
        return from type in GetAllTypes() 
            from customAttribute in type.GetCustomAttributes(typeof(T), false)
            select new KeyValuePair<Type, T>(type, (T) customAttribute);
    }

    private static IEnumerable<Type> GetAllTypes()
    {
        return AppDomain.CurrentDomain.GetAssemblies().SelectMany(assembly => assembly.GetTypes());
    }
    
    public static T? GetAttribute<T>(FieldInfo field) where T : Attribute
    {
        foreach (var customAttribute in field.GetCustomAttributes())
        {
            if (customAttribute is T attribute)
                return attribute;
        }
        return null;
    }
    
    public static T Clone<T>(T source) where T : new()
    {
        var target = new T();
        var properties = typeof(T).GetProperties();

        foreach (var prop in properties)
        {
            if (prop.CanWrite)
            {
                var value = prop.GetValue(source);

                if (value is ICloneable cloneable)
                {
                    prop.SetValue(target, cloneable.Clone());
                    continue;
                }
                
                prop.SetValue(target, value);
            }
        }
        return target!;
    }
}