namespace PW.Application.Common.Constants
{
    public static class CacheDurations
    {
        public static readonly TimeSpan Instant = TimeSpan.FromSeconds(30);
        public static readonly TimeSpan Short = TimeSpan.FromMinutes(5);
        public static readonly TimeSpan Medium = TimeSpan.FromMinutes(30);
        public static readonly TimeSpan Long = TimeSpan.FromHours(1);
        public static readonly TimeSpan Daily = TimeSpan.FromDays(1);
    }
}
