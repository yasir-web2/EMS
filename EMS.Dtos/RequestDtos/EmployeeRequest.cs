namespace EMS.Dtos.RequestDtos;

public sealed record class EmployeeRequest : RequestBase
{
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public DateTime DateOfBirth { get; set; }
    public Guid DepartmentId { get; set; }
    public string? DepartmentName { get; set; }
}