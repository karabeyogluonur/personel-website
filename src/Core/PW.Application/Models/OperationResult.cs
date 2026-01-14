using PW.Application.Common.Enums;

namespace PW.Application.Models;

public class OperationResult
{
   public bool Succeeded { get; protected set; }
   public List<OperationError> Errors { get; protected set; } = new List<OperationError>();
   public bool IsFailure => !Succeeded;

   public static OperationResult Success()
   {
      return new OperationResult { Succeeded = true };
   }

   public static OperationResult Failure(string message, OperationErrorType type = OperationErrorType.BusinessRule)
   {
      return new OperationResult
      {
         Succeeded = false,
         Errors = new List<OperationError> { new OperationError(message, type) }
      };
   }

   public static OperationResult Failure(IEnumerable<OperationError> errors)
   {
      return new OperationResult
      {
         Succeeded = false,
         Errors = errors.ToList()
      };
   }
}

public class OperationResult<T> : OperationResult
{
   public T? Data { get; private set; }

   public static OperationResult<T> Success(T data)
   {
      return new OperationResult<T>
      {
         Succeeded = true,
         Data = data
      };
   }

   public new static OperationResult<T> Failure(string message, OperationErrorType type = OperationErrorType.BusinessRule)
   {
      return new OperationResult<T>
      {
         Succeeded = false,
         Errors = new List<OperationError> { new OperationError(message, type) }
      };
   }

   public new static OperationResult<T> Failure(IEnumerable<OperationError> errors)
   {
      return new OperationResult<T>
      {
         Succeeded = false,
         Errors = errors.ToList()
      };
   }
}
