using EMS.Dtos.RequestDtos;
using System.Numerics;

namespace EMS.Entities;

public sealed class Department : EntityBase
{
    #region Constructors

    public Department()
    {

    }

    public Department(DepartmentRequest request)
    {
        Id = Guid.NewGuid();
        CreatedDate = DateTime.Now;
        Name = request.Name;
    }

    #endregion

    #region Properties

    public string Name { get; set; } = string.Empty;

    #endregion

    #region Navigations

    public ICollection<Employee> Employees { get; set; } = new List<Employee>();

    #endregion

    #region Operators

    public static explicit operator Department(DepartmentRequest request) => new(request);

    #endregion
}
