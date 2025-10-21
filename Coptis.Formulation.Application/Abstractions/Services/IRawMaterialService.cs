using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Coptis.Formulation.Application.Models;

namespace Coptis.Formulation.Application.Abstractions.Services
{
    public interface IRawMaterialService
    {
        Task<IReadOnlyList<RawMaterialListItem>> GetAll(CancellationToken ct);
        Task<UpdatePriceResult> UpdatePrice(string id, decimal newPrice, CancellationToken ct);
    }
}