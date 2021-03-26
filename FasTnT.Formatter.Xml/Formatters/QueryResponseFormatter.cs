using FasTnT.Application.Commands;
using FasTnT.Application.Queries.Poll;

namespace FasTnT.Formatter.Xml
{
    public static class XmlResponseFormatter
    {
        public static string Format(object response)
        {
            if(response is PollResponse)
            {
                return "OK query";
            }
            if(response is CaptureEpcisRequestResponse)
            {
                return "OK Capture";
            }

            return "Unknown error";
        }
    }
}
