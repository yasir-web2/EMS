namespace EMS.Dtos.ResponseDtos;

public sealed record class DepartmentResponse : ResponseBase
{
    public string Name { get; set; } = string.Empty;
}