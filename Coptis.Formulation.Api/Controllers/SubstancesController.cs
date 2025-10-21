
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
    public class SubstancesController : ControllerBase
    {
        private readonly ISubstanceService _substanceService;

        public SubstancesController(ISubstanceService substanceService)
        {
            _substanceService = substanceService;
        }

        [HttpGet("by-weight")]
        public async Task<ActionResult<IReadOnlyList<SubstanceUsageItem>>> GetByWeight(CancellationToken ct)
        {
            var substances = await _substanceService.GetMostUsedByWeight(ct);
            return Ok(substances);
        }

        [HttpGet("by-count")]
        public async Task<ActionResult<IReadOnlyList<SubstanceUsageItem>>> GetByCount(CancellationToken ct)
        {
            var substances = await _substanceService.GetMostUsedByFormulaCount(ct);
            return Ok(substances);
        }
    }
}