using System.Linq;
using System.Threading.Tasks;
using Coptis.Formulation.Application.Abstractions.Services;
using Coptis.Formulation.Application.Contracts.Import.Dtos;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;

namespace Coptis.Formulation.Api.Filters
{
    public sealed class ValidateFormulaAttribute : System.Attribute, IAsyncActionFilter
    {
        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var dto = context.ActionArguments.Values.OfType<FormulaDto>().FirstOrDefault();
            if (dto is null)
            {
                await next();
                return;
            }

            var validator = context.HttpContext.RequestServices.GetRequiredService<IFormulaValidationService>();
            var (isValid, issues) = await validator.Validate(dto, context.HttpContext.RequestAborted);

            if (!isValid)
            {
                context.Result = new BadRequestObjectResult(new { status = "validation_failed", issues });
                return;
            }

            await next();
        }
    }
}
