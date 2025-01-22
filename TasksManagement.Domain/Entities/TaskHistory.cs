using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.Diagnostics.CodeAnalysis;

namespace TasksManagement.Domain.Entities;

[ExcludeFromCodeCoverage]
public class TaskHistory
{
    [BsonId, BsonGuidRepresentation(GuidRepresentation.Standard)]
    public Guid Id { get; set; }

    [BsonElement, BsonGuidRepresentation(GuidRepresentation.Standard)]
    public Guid TaskId { get; set; }

    [BsonElement]
    public string Changes { get; set; }

    [BsonElement]
    public DateTime ChangedAt { get; set; }

    [BsonElement, BsonGuidRepresentation(GuidRepresentation.Standard)]
    public Guid ChangedBy { get; set; }

    [BsonConstructor]
    public TaskHistory(string changes, Guid taskId, Guid changedBy)
    {
        Id = Guid.NewGuid();
        Changes = changes;
        TaskId = taskId;
        ChangedBy = changedBy;
        ChangedAt = DateTime.UtcNow;
    }
}