using EMS.Dtos.RequestDtos;
using System.ComponentModel.DataAnnotations.Schema;

namespace EMS.Entities;

public sealed class Employee : EntityBase
{
    #region Constructors

    public Employee()
    {
        
    }

    public Employee(EmployeeRequest request)
    {
        Id = Guid.NewGuid();
        CreatedDate = DateTime.Now;
        Name = request.Name;
        Email = request.Email;
        DateOfBirth = request.DateOfBirth;
        DepartmentId = request.DepartmentId;
    }

    #endregion

    #region Properties

    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public DateTime DateOfBirth { get; set; }

    [ForeignKey("Department")]
    public Guid DepartmentId { get; set; }

    #endregion

    #region Navigations

    public Department? Department { get; set; }

    #endregion

    #region Operators

    public static explicit operator Employee(EmployeeRequest request) => new(request);

    #endregion
}