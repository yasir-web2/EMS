using System.ComponentModel.DataAnnotations;

namespace EMS.Dtos.RequestDtos;

public sealed record class DepartmentRequest : RequestBase
{
    [Required]
    public string Name { get; set; } = string.Empty;
}