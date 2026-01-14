using System.ComponentModel;

namespace PW.Application.Common.Extensions;

public static class StringExtensions
{
   public static T? ToType<T>(this string value)
   {
      object? result = value.ToType(typeof(T));

      return result != null ? (T)result : default;
   }

   public static object? ToType(this string value, Type destinationType)
   {
      if (string.IsNullOrEmpty(value)) return null;

      TypeConverter converter = TypeDescriptor.GetConverter(destinationType);

      if (converter.CanConvertFrom(typeof(string)))
      {
         try
         {
            return converter.ConvertFrom(value);
         }
         catch
         {
            return null;
         }
      }

      return null;
   }
}
