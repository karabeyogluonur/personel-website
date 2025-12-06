namespace PW.Application.Common.Models
{
    public class OperationResult<T> : OperationResult
    {
        public T? Data { get; private set; }

        public static OperationResult<T> Success(T data)
            => new OperationResult<T> { Succeeded = true, Data = data };

        public static OperationResult<T> Failure(params string[] errors)
            => new OperationResult<T> { Succeeded = false, Errors = errors.ToList() };
    }

    public class OperationResult
    {
        public bool Succeeded { get; protected set; }
        public IEnumerable<string> Errors { get; set; } = Enumerable.Empty<string>();

        public static OperationResult Success()
            => new OperationResult { Succeeded = true };

        public static OperationResult Failure(params string[] errors)
            => new OperationResult { Succeeded = false, Errors = errors.ToList() };
    }
}
