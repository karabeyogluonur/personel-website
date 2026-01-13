using PW.Application.Common.Enums;

namespace PW.Application.Models;

public record OperationError(string Message, OperationErrorType Type = OperationErrorType.BusinessRule, string? PropertyName = null);
