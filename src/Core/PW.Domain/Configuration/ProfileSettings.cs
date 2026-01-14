using PW.Domain.Interfaces;

namespace PW.Domain.Configuration;

public class ProfileSettings : ISettings
{
   public string AvatarFileName { get; set; } = string.Empty;
   public string CoverFileName { get; set; } = string.Empty;
   public string FirstName { get; set; } = string.Empty;
   public string LastName { get; set; } = string.Empty;
   public string Biography { get; set; } = string.Empty;
   public string JobTitle { get; set; } = string.Empty;
}

