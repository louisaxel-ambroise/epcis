using FasTnT.Domain.Model.Events;
using FasTnT.Domain.Model.Masterdata;

namespace FasTnT.Domain.Model.Queries;

public class QueryData
{
    public List<Event> EventList { get; init; }
    public List<MasterData> VocabularyList { get; init; }

    public static QueryData Empty => new() { EventList = new() };

    public static implicit operator QueryData(List<Event> events) => new() { EventList = events };
    public static implicit operator QueryData(List<MasterData> vocabulary) => new() { VocabularyList = vocabulary };
}