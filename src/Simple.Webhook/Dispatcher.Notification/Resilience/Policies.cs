using Polly;
using Polly.CircuitBreaker;
using Polly.Extensions.Http;
using Polly.Retry;
using Polly.Timeout;
using Polly.Wrap;

namespace Dispatcher.Notification.Resilience
{
    public static class Policies
    {
        public static AsyncCircuitBreakerPolicy CreatePolicy()
        {
            return Policy
                .Handle<Exception>()
                .CircuitBreakerAsync(3, TimeSpan.FromSeconds(10),
                    onBreak: (a, b, c) =>
                    {
                        ShowCircuitState("Open (onBreak)", ConsoleColor.Red);
                    },
                    onReset: (a) =>
                    {
                        ShowCircuitState("Closed (onReset)", ConsoleColor.Green);
                    },
                    onHalfOpen: () =>
                    {
                        ShowCircuitState("Half Open (onHalfOpen)", ConsoleColor.Yellow);
                    });
        }

        private static void ShowCircuitState(
            string descStatus, ConsoleColor backgroundColor)
        {
            var previousBackgroundColor = Console.BackgroundColor;
            var previousForegroundColor = Console.ForegroundColor;

            Console.BackgroundColor = backgroundColor;
            Console.ForegroundColor = ConsoleColor.Black;

            Console.Out.WriteLine($" ***** Estado do Circuito: {descStatus} **** ");

            Console.BackgroundColor = previousBackgroundColor;
            Console.ForegroundColor = previousForegroundColor;
        }
        private const int MAX_RETRY = 3;
        private const int RAISED_POWDER = 2;
        public static AsyncPolicyWrap<HttpResponseMessage> DefaultPolicy(int timeout = 10) => RetryPolicy.WrapAsync(Timeout(timeout));

        public static AsyncTimeoutPolicy<HttpResponseMessage> Timeout(int seconds = 10)
            => Policy
                .TimeoutAsync<HttpResponseMessage>(seconds);

        public static AsyncRetryPolicy<HttpResponseMessage> RetryPolicy
            => HttpPolicyExtensions
                .HandleTransientHttpError()
                .WaitAndRetryAsync(MAX_RETRY, attempt => TimeSpan.FromSeconds(Math.Pow(RAISED_POWDER, attempt)), (_, timeSpan, retryCount, context) =>
                {
                    context.Remove("Attempt");
                    context.Add("Attempt", retryCount);
                });
    }
}
