using EMS.Dtos.RequestDtos;
using EMS.Dtos.ResponseDtos;

namespace EMS.DL.Abstractions;

public interface IDepartmentRepository
{
    Task AddAsync(DepartmentRequest request, CancellationToken cancellationToken = default);
    Task UpdateAsync(DepartmentRequest request, CancellationToken cancellationToken = default);
    Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);
    Task<DepartmentResponse?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IEnumerable<DepartmentResponse>> GetAllAsync(string? searchKey = null, CancellationToken cancellationToken = default);
    Task<IEnumerable<DropdownResponse>> GetDropdownAsync(CancellationToken cancellationToken = default);
}