using Coptis.Formulation.Api.Filters;
using Coptis.Formulation.Application.Abstractions.Services;
using Coptis.Formulation.Application.Contracts.Import.Dtos;
using Coptis.Formulation.Application.Implementations.Services;
using Coptis.Formulation.Application.Models;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Coptis.Formulation.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class FormulasController : ControllerBase
    {
        private readonly IFormulaImportService _importService;
        private readonly IFormulaService _formulaService;

        public FormulasController(IFormulaImportService importService, IFormulaService formulaService)
        { _importService = importService; _formulaService = formulaService; }

        [HttpPost("import")]
        [TypeFilter(typeof(ValidateFormulaAttribute))]
        public async Task<ActionResult<ImportResult>> Import([FromBody] FormulaDto dto, CancellationToken ct)
        {
            var result = await _importService.Import(dto, ct);
            if (result.Status == ImportStatus.Failed) return BadRequest(result);
            return Ok(result);
        }

        [HttpGet] 
        public async Task<ActionResult<IReadOnlyList<FormulaListItem>>> GetAll([FromQuery] string? q, CancellationToken ct)
      => Ok(await _formulaService.GetAll(q, ct));
    }
}
