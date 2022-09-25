using Polly;

namespace Ct.Domain.Policies
{
    public interface IResilientPolicy
    {
        bool IsCircuitOpen { get; }

        Task<HttpResponseMessage> ExecuteAsync(Func<Task<HttpResponseMessage>> action);
    }
}