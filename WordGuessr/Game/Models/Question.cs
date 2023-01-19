using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace WordGuessr.Game.Models;

public class Question
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string? Id { get; init; }
    public required string QuestionBody { get; init; }
    public required string Answer { get; init; }
}