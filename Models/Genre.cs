using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace BookBoostApi.Models;

public enum Genre
{
    [BsonRepresentation(BsonType.String)]
    Romance,

    [BsonRepresentation(BsonType.String)]
    Fantasy,

    [BsonRepresentation(BsonType.String)]
    MysteryThriller,

    [BsonRepresentation(BsonType.String)]
    ScienceFiction,

    [BsonRepresentation(BsonType.String)]
    YoungAdult,

    [BsonRepresentation(BsonType.String)]
    NonFiction
}

public enum Topic
{
    [BsonRepresentation(BsonType.String)]
    AdviceAndHowTo,

    [BsonRepresentation(BsonType.String)]
    AmericanHistoricalRomance,

    [BsonRepresentation(BsonType.String)]
    Bestsellers,

    [BsonRepresentation(BsonType.String)]
    BiographiesAndMemoirs,

    [BsonRepresentation(BsonType.String)]
    Business,

    [BsonRepresentation(BsonType.String)]
    Childrens,

    [BsonRepresentation(BsonType.String)]
    ChristianFiction,

    [BsonRepresentation(BsonType.String)]
    ChristianNonfiction,

    [BsonRepresentation(BsonType.String)]
    Classics,

    [BsonRepresentation(BsonType.String)]
    ContemporaryRomance,

    [BsonRepresentation(BsonType.String)]
    Cooking,

    [BsonRepresentation(BsonType.String)]
    CozyMysteries,

    [BsonRepresentation(BsonType.String)]
    CrimeFiction,

    [BsonRepresentation(BsonType.String)]
    DarkRomance,

    [BsonRepresentation(BsonType.String)]
    EroticRomance,

    [BsonRepresentation(BsonType.String)]
    Fantasy,

    [BsonRepresentation(BsonType.String)]
    GeneralNonfiction,

    [BsonRepresentation(BsonType.String)]
    HistoricalFiction,

    [BsonRepresentation(BsonType.String)]
    HistoricalMysteries,

    [BsonRepresentation(BsonType.String)]
    HistoricalRomance,

    [BsonRepresentation(BsonType.String)]
    History,

    [BsonRepresentation(BsonType.String)]
    Horror,

    [BsonRepresentation(BsonType.String)]
    Humor,

    [BsonRepresentation(BsonType.String)]
    LiteraryFiction,

    [BsonRepresentation(BsonType.String)]
    MiddleGrade,

    [BsonRepresentation(BsonType.String)]
    Mysteries,

    [BsonRepresentation(BsonType.String)]
    MysteryThriller,

    [BsonRepresentation(BsonType.String)]
    NewAdultRomance,

    [BsonRepresentation(BsonType.String)]
    NonFiction,

    [BsonRepresentation(BsonType.String)]
    ParanormalRomance,

    [BsonRepresentation(BsonType.String)]
    Parenting,

    [BsonRepresentation(BsonType.String)]
    PoliticsAndCurrentEvents,

    [BsonRepresentation(BsonType.String)]
    PsychologicalThrillers,

    [BsonRepresentation(BsonType.String)]
    ReligionAndSpirituality,

    [BsonRepresentation(BsonType.String)]
    Romance,

    [BsonRepresentation(BsonType.String)]
    RomanticSuspense,

    [BsonRepresentation(BsonType.String)]
    RomCom,

    [BsonRepresentation(BsonType.String)]
    Science,

    [BsonRepresentation(BsonType.String)]
    ScienceFiction,

    [BsonRepresentation(BsonType.String)]
    SupernaturalSuspense,

    [BsonRepresentation(BsonType.String)]
    TeenAndYoungAdult,

    [BsonRepresentation(BsonType.String)]
    Thrillers,

    [BsonRepresentation(BsonType.String)]
    TimeTravelRomance,

    [BsonRepresentation(BsonType.String)]
    TrueCrime,

    [BsonRepresentation(BsonType.String)]
    WomensFiction,

    [BsonRepresentation(BsonType.String)]
    YoungAdult
}
