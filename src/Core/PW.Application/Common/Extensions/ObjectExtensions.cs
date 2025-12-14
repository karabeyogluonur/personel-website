using System.ComponentModel;

namespace PW.Application.Common.Extensions
{
    public static class ObjectExtensions
    {
        public static string ToInvariantString(this object value)
        {
            if (value == null) return string.Empty;

            var converter = TypeDescriptor.GetConverter(value.GetType());
            return converter.ConvertToInvariantString(value);
        }
    }
}
