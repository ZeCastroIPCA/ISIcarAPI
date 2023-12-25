using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

public class Car
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string? Id { get; set; }
    public string Make { get; set; }
    public string Model { get; set; }
    public int Year { get; set; }
}
