namespace EMS.Dtos.RequestDtos;

public abstract record class RequestBase
{
    public Guid? Id { get; set; }
}