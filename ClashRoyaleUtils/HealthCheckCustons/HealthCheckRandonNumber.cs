using Microsoft.Extensions.Diagnostics.HealthChecks;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace ClashRoyaleUtils.HealthCheckCustons
{
    public class HealthCheckRandonNumber : IHealthCheck
    {
        public Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default(CancellationToken))
        {
            var rnd = new Random();
            var number = rnd.Next(100);

            if (number % 2 == 0)
                return Task.FromResult(HealthCheckResult.Healthy(description: $"O valor atual é par: {number}"));

            return Task.FromResult(HealthCheckResult.Unhealthy(description: $"O valor atual é ímpar: {number}"));
        }
    }
}
