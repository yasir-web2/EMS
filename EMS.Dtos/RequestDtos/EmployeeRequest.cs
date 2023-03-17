using System.ComponentModel.DataAnnotations;

namespace EMS.Dtos.RequestDtos;

public sealed record class EmployeeRequest : RequestBase
{
    [Required]
    public string Name { get; set; } = string.Empty;
    [Required]
    public string Email { get; set; } = string.Empty;
    [Required]
    public DateTime DateOfBirth { get; set; }
    [Required]
    public Guid DepartmentId { get; set; }
    public string? DepartmentName { get; set; }
}