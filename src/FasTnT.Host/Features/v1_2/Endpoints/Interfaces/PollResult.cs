using FasTnT.Application.Domain.Model.Events;
using FasTnT.Application.Domain.Model.Masterdata;

namespace FasTnT.Host.Features.v1_2.Endpoints.Interfaces;

public record PollResult
{
    public PollResult(string queryName, List<Event> eventList)
    {
        QueryName = queryName;
        EventList = eventList;
    }

    public PollResult(string queryName, List<MasterData> vocabularyList)
    {
        QueryName = queryName;
        VocabularyList = vocabularyList;
    }

    public string QueryName { get; init; }
    public List<Event> EventList { get; init; }
    public List<MasterData> VocabularyList { get; init; }
}
