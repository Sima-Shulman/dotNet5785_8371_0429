

using System.Collections;
using System.Reflection;
using System.Text;

namespace Helpers;

internal static class Tools
{
    public static string ToStringProperty<T>(this T t)
    {
        if (t == null)
            return "";

        var result = new StringBuilder();
        var properties = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);

        foreach (var property in properties)
        {
            var value = property.GetValue(t);
            if (value is IEnumerable enumerable && value is not string)
            {
                result.Append($"{property.Name}: [");
                foreach (var item in enumerable)
                {
                    result.Append($"{item}, ");
                }
                result.Append("], ");
            }
            else
            {
                result.Append($"{property.Name}: {value}, ");
            }
        }
        return result.ToString().TrimEnd(',', ' ');
    }

}
