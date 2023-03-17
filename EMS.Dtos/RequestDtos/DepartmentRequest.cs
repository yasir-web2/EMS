namespace EMS.Dtos.RequestDtos;

public sealed record class DepartmentRequest : RequestBase
{
    public string Name { get; set; } = string.Empty;
}