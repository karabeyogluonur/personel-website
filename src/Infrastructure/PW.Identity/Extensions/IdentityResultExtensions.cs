using Microsoft.AspNetCore.Identity;
using PW.Application.Common.Models;

namespace PW.Identity.Extensions
{
    public static class IdentityResultExtensions
    {
        public static OperationResult<T> ToOperationResult<T>(this IdentityResult result, T? data = default)
        {
            return result.Succeeded
                ? OperationResult<T>.Success(data!)
                : OperationResult<T>.Failure(result.Errors.Select(e => e.Description).ToArray());
        }

        public static OperationResult ToOperationResult(this IdentityResult result)
        {
            return result.Succeeded
                ? OperationResult.Success()
                : OperationResult.Failure(result.Errors.Select(e => e.Description).ToArray());
        }
    }
}
