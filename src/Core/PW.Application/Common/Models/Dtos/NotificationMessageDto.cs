using PW.Application.Common.Enums;

namespace PW.Application.Common.Models.Dtos
{
    public class NotificationMessageDto
    {
        public string Type { get; set; }
        public string Message { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
    }
}
