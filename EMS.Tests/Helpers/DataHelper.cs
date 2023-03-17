using EMS.Entities;

namespace EMS.Tests.Helpers;

public static class DataHelper
{
    public static List<Department> GetDepartments()
    {
        return new List<Department>
            {
                new Department
                {
                    CreatedDate = DateTime.Now,
                    Id = Guid.Parse("bed64240-98e5-4b63-9469-fc961aa214c9"),
                    Name = "HR"
                },
                new Department
                {
                    CreatedDate = DateTime.Now,
                    Id = Guid.Parse("3736a738-3405-4eb3-b9ba-33ddbf1d3b76"),
                    Name = "IT"
                }
            };
    }

    public static List<Employee> GetEmployees()
    {
        return new List<Employee>
            {
                new Employee
                {
                    CreatedDate = DateTime.Now,
                    DateOfBirth = DateTime.Now.AddYears(-23),
                    DepartmentId = Guid.Parse("bed64240-98e5-4b63-9469-fc961aa214c9"),
                    Email = "jhon@gmail.com",
                    Id = Guid.Parse("3045f66f-45af-4a1b-92ad-24855e2e2826"),
                    Name = "Jhon"
                },
                new Employee
                {
                    CreatedDate = DateTime.Now,
                    DateOfBirth = DateTime.Now.AddYears(-25),
                    DepartmentId = Guid.Parse("3736a738-3405-4eb3-b9ba-33ddbf1d3b76"),
                    Email = "william@gmail.com",
                    Id = Guid.Parse("187f1962-8e71-4eca-b7a3-a81a354f249b"),
                    Name = "William"
                }
            };
    }

    public static List<Employee> GetEmployees(string searchKey)
    {
        return GetEmployees().Where(x => x.Name.Contains(searchKey) || x.Email.Contains(searchKey)).ToList();
    }
}