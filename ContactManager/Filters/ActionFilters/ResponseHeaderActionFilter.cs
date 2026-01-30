using Microsoft.AspNetCore.Mvc.Filters;

namespace ContactManager.Filters.ActionFilters
{
    public class ResponseHeaderFilterFactoryAttribute : Attribute, IFilterFactory
    {
        private readonly string _key;
        private readonly string _value;
        private readonly int _order;
        public bool IsReusable => false;
        public ResponseHeaderFilterFactoryAttribute(string key, string value, int order)
        {
            _key = key;
            _value = value;
            _order = order;
        }

        public IFilterMetadata CreateInstance(IServiceProvider serviceProvider)
        {
            //var filter = new ResponseHeaderActionFilter(_key, _value, _order);
            var filter = serviceProvider.GetRequiredService<ResponseHeaderActionFilter>();

            filter.Key = _key;
            filter.Value = _value;
            filter.Order = _order;

            return filter;
        }
    }

    public class ResponseHeaderActionFilter : IAsyncActionFilter, IOrderedFilter
    {
       
        //private readonly string _key;
        //private readonly string _value;
        public string Key { get; set; }
        public string Value { get; set; }
        public int Order { get; set; }

        private readonly ILogger<ResponseHeaderActionFilter> _logger;

        public ResponseHeaderActionFilter(ILogger<ResponseHeaderActionFilter> logger)
        {
            _logger = logger;
        }

        //public ResponseHeaderActionFilter(string key, string value, int order)
        //{
        //    _key = key;
        //    _value = value;
        //    Order = order;
        //}

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            // Before logic
            _logger.LogInformation("{FilterName} {MethodName} method - before", nameof(ResponseHeaderActionFilter), nameof(OnActionExecutionAsync));

            // Next call in the pipeline
            await next();

            // After logic
            _logger.LogInformation("{FilterName} {MethodName} method - after", nameof(ResponseHeaderActionFilter), nameof(OnActionExecutionAsync));

            //context.HttpContext.Response.Headers[_key] = _value;
            context.HttpContext.Response.Headers[Key] = Value;
        }
    }
}
