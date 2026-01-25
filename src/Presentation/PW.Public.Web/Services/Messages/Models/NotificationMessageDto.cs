namespace PW.Public.Web.Services.Messages.Models;

public class NotificationMessageDto
{
   public string Type { get; set; }
   public string Message { get; set; } = string.Empty;
   public string Title { get; set; } = string.Empty;
}
