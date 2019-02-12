using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace Natron.Http.Middleware
{
    public sealed class TraceMiddleware
    {
        private readonly RequestDelegate _next;

        public TraceMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            await _next(context);
        }
    }
}