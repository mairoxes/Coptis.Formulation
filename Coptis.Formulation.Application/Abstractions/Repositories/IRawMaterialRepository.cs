using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Coptis.Formulation.Domain.Entities;

namespace Coptis.Formulation.Application.Abstractions.Repositories
{
    public interface IRawMaterialRepository
    {
        Task<RawMaterial?> FindByName(string name, CancellationToken ct);
        Task<RawMaterial?> FindById(Guid id, CancellationToken ct);
        Task Add(RawMaterial rawMaterial, CancellationToken ct);
        Task<List<RawMaterial>> GetAll(CancellationToken ct);
    }
}