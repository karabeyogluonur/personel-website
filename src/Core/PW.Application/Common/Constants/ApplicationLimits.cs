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

        public static class Technology
        {
            public const int NameMaxLength = 100;
            public const int DescriptionMaxLength = 500;
            public const int UrlMaxLength = 250;
            public const int MaxIconSizeBytes = 2 * 1024 * 1024; // 2MB
            public static readonly string[] AllowedIconExtensions = { ".jpg", ".jpeg", ".png", ".svg" };
        }

        public static class GeneralSettings
        {
            public const int SiteTitleMaxLength = 256;
            public const int MaxFileSizeBytes = 2 * 1024 * 1024;
            public static readonly string[] AllowedLogoExtensions = { ".jpg", ".jpeg", ".png" };
            public static readonly string[] AllowedFaviconExtensions = { ".ico", ".png" };
        }

        public static class ProfileSettings
        {
            public const int FirstNameMaxLength = 50;
            public const int LastNameMaxLength = 50;
            public const int JobTitleMaxLength = 100;
            public const int BiographyMaxLength = 1000;
            public const int MaxImageSizeBytes = 2 * 1024 * 1024;
            public static readonly string[] AllowedImageExtensions = { ".jpg", ".jpeg", ".png" };
        }

        public static class Category
        {
            public const int NameMaxLength = 100;
            public const int DescriptionMaxLength = 500;
            public const int MaxCoverImageSizeBytes = 5 * 1024 * 1024; // 5MB
            public static readonly string[] AllowedCoverImageExtensions = { ".jpg", ".jpeg", ".png", ".svg" };
        }

    }
}
