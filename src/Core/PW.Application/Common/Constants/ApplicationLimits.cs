namespace PW.Application.Common.Constants
{
    public static class ApplicationLimits
    {
        public static class Common
        {
            public const int NameMaxLength = 50;
            public const int DescriptionMaxLength = 500;
            public const int UrlMaxLength = 255;
            public const int TitleMaxLength = 100;
        }

        public static class Language
        {
            public const int CodeMaxLength = 2;
            public const int MaxFlagSizeBytes = 2 * 1024 * 1024; // 2MB
            public static readonly string[] AllowedFlagExtensions = { ".jpg", ".jpeg", ".png", ".svg" };
        }

        public static class User
        {
            public const int FirstNameMaxLength = 50;
            public const int LastNameMaxLength = 50;
            public const int EmailMaxLength = 150;
            public const int PasswordMinLength = 6;
        }

    }
}
