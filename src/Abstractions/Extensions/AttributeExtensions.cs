using System.ComponentModel;
using System.Reflection;

namespace Honamic.PayMaster.Extensions;

public static class AttributeExtensions
{
    public static string? GetEnumDescription(this Enum value)
    {
        FieldInfo fi = value.GetType().GetField(value.ToString());

        DescriptionAttribute[] attributes = fi.GetCustomAttributes(typeof(DescriptionAttribute), false) as DescriptionAttribute[];

        if (attributes != null && attributes.Any())
        {
            return attributes.First().Description;
        }

        return null;
    }

    public static string SeparateCamelCase(this string stringValue)
    {
        if (stringValue is null)
            return stringValue;


        for (int i = 1; i < stringValue.Length; i++)
        {
            if (char.IsUpper(stringValue[i]))
            {
                stringValue = stringValue.Insert(i, " ");
                i++;
            }
        }

        return stringValue;
    }
}
