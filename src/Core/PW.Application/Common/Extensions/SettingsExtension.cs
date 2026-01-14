using System.Linq.Expressions;
using PW.Domain.Interfaces;

namespace PW.Application.Common.Extensions;

public static class SettingExtensions
{
   public static string GetSettingsKeyPrefix(this Type type)
   {
      string className = type.Name;
      if (className.EndsWith("Settings"))
         return className.Substring(0, className.Length - "Settings".Length);

      return className;
   }
   public static string BuildSettingKey(this Type type, string propertyName)
   {
      return $"{type.GetSettingsKeyPrefix()}.{propertyName}";
   }

   public static string GetSettingKey<TSettings, TProp>(this Expression<Func<TSettings, TProp>> keySelector)
       where TSettings : ISettings
   {
      if (keySelector.Body is not MemberExpression member)
         throw new ArgumentException($"Expression '{keySelector}' refers to a method, not a property.");

      return typeof(TSettings).BuildSettingKey(member.Member.Name);
   }
}
