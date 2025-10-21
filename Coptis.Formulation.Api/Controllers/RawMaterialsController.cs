
using Coptis.Formulation.Application.Abstractions.Services;
using Coptis.Formulation.Application.Models;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Coptis.Formulation.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RawMaterialsController : ControllerBase
    {
        private readonly IRawMaterialService _rawMaterialService;

        public RawMaterialsController(IRawMaterialService rawMaterialService)
        {
            _rawMaterialService = rawMaterialService;
        }

        [HttpGet]
        public async Task<ActionResult<IReadOnlyList<RawMaterialListItem>>> GetAll(CancellationToken ct)
        {
            var materials = await _rawMaterialService.GetAll(ct);
            return Ok(materials);
        }

        [HttpPut("{id}/price")]
        public async Task<ActionResult> UpdatePrice(string id, [FromBody] UpdatePriceRequest request, CancellationToken ct)
        {
            var result = await _rawMaterialService.UpdatePrice(id, request.PriceAmount, ct);

            if (!result.Success)
                return BadRequest(new { message = result.ErrorMessage });

            return Ok(new { message = "Price updated successfully", affectedFormulasCount = result.AffectedFormulasCount });
        }
    }

    public record UpdatePriceRequest(decimal PriceAmount);
}