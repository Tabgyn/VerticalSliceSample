namespace VerticalSliceSample.Api.Database.Entities;

public interface IEntity
{
    int Id { get; }
    Guid ReferenceId { get; }
}
