namespace EMS.Dtos.ResponseDtos;

public abstract record class ResponseBase
{
    public Guid Id { get; set; }
    public DateTime CreatedDate { get; set; }
    public DateTime? UpdatedDate { get; set; }
}