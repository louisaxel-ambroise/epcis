using FasTnT.Application.Domain.Model.Events;
using FasTnT.Application.Domain.Model.Masterdata;

namespace FasTnT.Application.Domain.Model.Queries;

public class QueryData
{
    public List<Event> EventList { get; set; }
    public List<MasterData> VocabularyList { get; set; }

    public static QueryData Empty => new() { EventList = new() };

    public static implicit operator QueryData(List<Event> events) => new() { EventList = events };
    public static implicit operator QueryData(List<MasterData> vocabulary) => new() { VocabularyList = vocabulary };
}