using Microsoft.AspNetCore.Mvc.Filters;

namespace WebApiAutores.Filters
{
    public class FiltroDeException: ExceptionFilterAttribute
    {
        private readonly ILogger<FiltroDeException> logger;

        public FiltroDeException(ILogger<FiltroDeException> logger)
        {
            this.logger = logger;
        }

        public override void OnException(ExceptionContext context)
        {

            logger.LogError(context.Exception, context.Exception.Message);

            base.OnException(context);
        }

    }
}
