using EMS.Dtos.RequestDtos;
using EMS.Dtos.ResponseDtos;

namespace EMS.DL.Abstractions;

public interface IEmployeeRepository
{
    Task AddAsync(EmployeeRequest request, CancellationToken cancellationToken = default);
    Task UpdateAsync(EmployeeRequest request, CancellationToken cancellationToken = default);
    Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);
    Task<EmployeeResponse?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IEnumerable<EmployeeResponse>> GetAllAsync(string? searchKey = null, CancellationToken cancellationToken = default);
    Task<IEnumerable<EmployeeResponse>> GetAllByDepartmentAsync(Guid departmentId, CancellationToken cancellationToken = default);
}