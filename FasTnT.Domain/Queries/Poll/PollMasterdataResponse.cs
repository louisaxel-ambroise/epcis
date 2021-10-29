using FasTnT.Domain.Model;

namespace FasTnT.Domain.Queries.Poll
{
    public class PollMasterdataResponse : PollResponse
    {
        public PollMasterdataResponse(string queryName, List<MasterData> vocabularyList)
        {
            QueryName = queryName;
            VocabularyList = vocabularyList;
        }
    }
}
