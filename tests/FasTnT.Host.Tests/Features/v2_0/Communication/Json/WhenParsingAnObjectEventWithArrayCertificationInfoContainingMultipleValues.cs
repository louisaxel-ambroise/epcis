using FasTnT.Domain.Model.Events;
using FasTnT.Host.Features.v2_0.Communication.Json.Parsers;

namespace FasTnT.Host.Tests.Features.v2_0.Communication.Json;

[TestClass]
public class WhenParsingAnObjectEventWithArrayCertificationInfoContainingMultipleValues : JsonParsingTestCase
{
    public static readonly string ResourceName = "FasTnT.Host.Tests.Features.v2_0.Communication.Resources.Events.ObjectEvent_StringCertificationInfoArrayInvalid.json";

    public Event Event { get; set; }
    public Exception Catched { get; set; }

    [TestInitialize]
    public void When()
    {
        try
        {
            var parser = JsonEventParser.Create(ParseResource(ResourceName).RootElement, TestNamespaces.Default);
            Event = parser.Parse();
        }
        catch (Exception ex)
        {
            Catched = ex is AggregateException aggEx ? aggEx.InnerException : ex;
        }
    }

    [TestMethod]
    public void ItShouldThrowAnException()
    {
        Assert.IsNotNull(Catched);
    }

    [TestMethod]
    public void ItShouldNotReturnAnEvent()
    {
        Assert.IsNull(Event);
    }
}
