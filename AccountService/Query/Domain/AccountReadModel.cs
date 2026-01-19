using MongoDB.Bson.Serialization.Attributes;

namespace AccountService.Query.Domain;

[BsonIgnoreExtraElements]
public class AccountReadModel
{
    [BsonId]
    public Guid Id { get; set; }

    [BsonElement("email")]
    public string Email { get; set; } = string.Empty;

    [BsonElement("createdAt")]
    public DateTime CreatedAt { get; set; }

    [BsonElement("updatedAt")]
    public DateTime UpdatedAt { get; set; }
}
