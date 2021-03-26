using FasTnT.Application.Queries.Poll;
using MediatR;
using System;
using System.IO;

namespace FasTnT.Formatter.Xml
{
    public static class QueryRequestParser
    {
        public static IBaseRequest ParseQuery(Stream queryStream)
        {
            return new PollQuery
            {
                QueryName = "SimpleEventQuery",
                Parameters = Array.Empty<QueryParameter>()
            };
        }
    }
}
