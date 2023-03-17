using EMS.Dtos.ResponseDtos;
using EMS.Entities;

namespace EMS.DL.Extensions;

public static class EntitiesExtensions
{
    #region Employee

    public static IEnumerable<EmployeeResponse> ToResponse(this List<Employee> employees)
    {
        if (employees is null || !employees.Any())
            return Enumerable.Empty<EmployeeResponse>();

        return employees.Select(employee => new EmployeeResponse
        {
            CreatedDate = employee.CreatedDate,
            DateOfBirth = employee.DateOfBirth,
            DepartmentId = employee.DepartmentId,
            DepartmentName = employee.Department?.Name,
            Email = employee.Email,
            Id = employee.Id,
            Name = employee.Name,
            UpdatedDate = employee.UpdatedDate
        });
    }

    public static EmployeeResponse? ToResponse(this Employee? employee)
    {
        if (employee is null)
            return null;

        return new EmployeeResponse
        {
            CreatedDate = employee.CreatedDate,
            DateOfBirth = employee.DateOfBirth,
            DepartmentId = employee.DepartmentId,
            DepartmentName = employee.Department?.Name,
            Email = employee.Email,
            Id = employee.Id,
            Name = employee.Name,
            UpdatedDate = employee.UpdatedDate
        };
    }

    #endregion

    #region Department

    public static IEnumerable<DepartmentResponse> ToResponse(this List<Department> departments)
    {
        if (departments is null || !departments.Any())
            return Enumerable.Empty<DepartmentResponse>();

        return departments.Select(department => new DepartmentResponse
        {
            CreatedDate = department.CreatedDate,
            Id = department.Id,
            Name = department.Name,
            UpdatedDate = department.UpdatedDate
        });
    }

    public static DepartmentResponse? ToResponse(this Department? department)
    {
        if (department is null)
            return null;

        return new DepartmentResponse
        {
            CreatedDate = department.CreatedDate,
            Id = department.Id,
            Name = department.Name,
            UpdatedDate = department.UpdatedDate
        };
    }

    #endregion
}