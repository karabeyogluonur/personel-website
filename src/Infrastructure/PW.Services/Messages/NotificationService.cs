using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using PW.Application.Common.Enums;
using PW.Application.Common.Models.Dtos;
using PW.Application.Interfaces.Messages;

public class NotificationService : INotificationService
{
    private readonly ITempDataDictionaryFactory _tempDataDictionaryFactory;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private const string ToastKey = "OKNotification_Message";

    public NotificationService(ITempDataDictionaryFactory tempDataDictionaryFactory, IHttpContextAccessor httpContextAccessor)
    {
        _tempDataDictionaryFactory = tempDataDictionaryFactory;
        _httpContextAccessor = httpContextAccessor;
    }

    private async Task PrepareTempDataAsync(NotificationType notificationType, string message, string title = "")
    {
        if (_httpContextAccessor.HttpContext == null)
            throw new InvalidOperationException("HttpContext is not available.");

        var tempData = _tempDataDictionaryFactory.GetTempData(_httpContextAccessor.HttpContext);

        var existingNotificationMessages = tempData.ContainsKey(ToastKey)
            ? JsonConvert.DeserializeObject<List<NotificationMessageDto>>((string)tempData[ToastKey])!
            : new List<NotificationMessageDto>();

        existingNotificationMessages.Add(new NotificationMessageDto
        {
            Type = notificationType.ToString(),
            Message = message,
            Title = title
        });

        tempData[ToastKey] = JsonConvert.SerializeObject(existingNotificationMessages);

        await Task.CompletedTask;
    }

    public async Task NotificationAsync(NotificationType type, string message, string title = "")
    {
        await PrepareTempDataAsync(type, message, title);
    }

    public async Task SuccessNotificationAsync(string message, string title = "")
    {
        await PrepareTempDataAsync(NotificationType.Success, message, title);
    }

    public async Task InfoNotificationAsync(string message, string title = "")
    {
        await PrepareTempDataAsync(NotificationType.Info, message, title);
    }

    public async Task WarningNotificationAsync(string message, string title = "")
    {
        await PrepareTempDataAsync(NotificationType.Warning, message, title);
    }

    public async Task ErrorNotificationAsync(string message, string title = "")
    {
        await PrepareTempDataAsync(NotificationType.Error, message, title);
    }
}
