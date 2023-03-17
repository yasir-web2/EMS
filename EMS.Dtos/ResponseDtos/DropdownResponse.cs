namespace EMS.Dtos.ResponseDtos;

public sealed record class DropdownResponse
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
}