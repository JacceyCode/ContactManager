using Microsoft.AspNetCore.Mvc.Filters;

namespace ContactManager.Filters.ActionFilters
{
    public class ResponseHeaderActionFilter : IActionFilter, IOrderedFilter
    {
        private readonly ILogger<ResponseHeaderActionFilter> _logger;
        private readonly string Key;
        private readonly string Value;
        public int Order { get; set; }

        public ResponseHeaderActionFilter(ILogger<ResponseHeaderActionFilter> logger, string key, string value, int order)
        {
            _logger = logger;
            Key = key;
            Value = value;
            Order = order;
        }

        public void OnActionExecuted(ActionExecutedContext context)
        {
            _logger.LogInformation("{FilterName} {MethodName} method", nameof(ResponseHeaderActionFilter), nameof(OnActionExecuted));

        }

        public void OnActionExecuting(ActionExecutingContext context)
        {
            _logger.LogInformation("{FilterName} {MethodName} method", nameof(ResponseHeaderActionFilter), nameof(OnActionExecuting));

            context.HttpContext.Response.Headers[Key] = Value;
        }
    }
}
