using System.Threading;
using System.Threading.Tasks;
using Coptis.Formulation.Application.Contracts.Import.Dtos;
using Coptis.Formulation.Application.Models;

namespace Coptis.Formulation.Application.Abstractions.Services
{
    public interface IFormulaImportService
    {
        Task<ImportResult> Import(FormulaDto dto, CancellationToken ct);
    }
}
