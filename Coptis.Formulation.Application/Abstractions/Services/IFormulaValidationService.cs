using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Coptis.Formulation.Application.Contracts.Import.Dtos;
using Coptis.Formulation.Application.Models;

namespace Coptis.Formulation.Application.Abstractions.Services
{
    public interface IFormulaValidationService
    {
        Task<(bool isValid, List<ImportIssue> issues)> Validate(FormulaDto dto, CancellationToken ct);
    }
}
