using PW.Domain.Interfaces;

namespace PW.Domain.Configuration
{
    public class ProfileSettings : ISettings
    {
        public string AvatarFileName { get; set; }
        public string CoverFileName { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Biography { get; set; }
        public string JobTitle { get; set; }
    }
}

