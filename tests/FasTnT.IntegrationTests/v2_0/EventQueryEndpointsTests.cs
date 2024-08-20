using FasTnT.Application.Database;
using FasTnT.Domain.Enumerations;
using FasTnT.Domain.Model;
using FasTnT.Domain.Model.Events;
using FasTnT.IntegrationTests.v2_0.Interfaces;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace FasTnT.IntegrationTests.v2_0;

[TestClass]
public class EventQueryEndpointsTests
{
    internal static WebApplicationFactory<Program> TestHost { get; set; }
    internal static HttpClient Client { get; set; }

    [ClassInitialize]
    public static void AssemblyInit(TestContext _)
    {
        TestHost = new FasTnTApplicationFactory(nameof(EventQueryEndpointsTests));
        Client = TestHost.CreateDefaultClient();
        Client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", "YWRtaW46UEBzc3cwcmQ=");

        using (var scope = TestHost.Server.Services.CreateScope())
        {
            var dbContext = scope.ServiceProvider.GetService<EpcisContext>();

            dbContext.Add(new Request
            {
                RecordTime = DateTime.Now,
                DocumentTime = DateTime.Now,
                SchemaVersion = "2.0",
                UserId = "431257CC4ADAF410486CDD3D6DC22F08",
                Events =
                [
                    new Event
                    {
                        Action = EventAction.Add,
                        Type = EventType.ObjectEvent,
                        Disposition = "disp",
                        BusinessLocation = "loc1",
                        BusinessStep = "step",
                        ReadPoint = "readpoint",
                        Epcs = [
                            new Epc { Type = EpcType.List, Id = "test:epc:1" },
                            new Epc { Type = EpcType.List, Id = "test:epc:2" },
                        ]
                    }
                ]
            });

            dbContext.Add(new Request
            {
                RecordTime = DateTime.Now,
                DocumentTime = DateTime.Now,
                SchemaVersion = "2.0",
                UserId = "ANOTHERUSER",
                Events =
                [
                    new Event
                    {
                        Disposition = "seconddisp",
                        BusinessLocation = "secondloc1",
                        BusinessStep = "secondstep",
                        ReadPoint = "secondreadpoint",
                        Epcs = [
                            new Epc { Type = EpcType.List, Id = "second:epc:1" },
                            new Epc { Type = EpcType.List, Id = "second:epc:2" },
                        ]
                    }
                ]
            });
            dbContext.SaveChanges();
        }
    }

    [TestMethod]
    public void GetEpcEventShouldReturnACollectionResult()
    {
        var response = Client.GetAsync("/epcs/test:epc:1/events").Result;

        Assert.IsNotNull(response);
        Assert.IsTrue(response.IsSuccessStatusCode);

        var collection = response.Content.ReadFromJsonAsync<QueryResult>().Result;
        Assert.IsNotNull(collection);
        Assert.AreEqual(1, collection.EpcisBody.QueryResults.ResultBody.EventList.Count);
    }

    [TestMethod]
    public void GetEventTypeEventsShouldReturnACollectionResult()
    {
        var response = Client.GetAsync("/eventtypes/ObjectEvent/events").Result;

        Assert.IsNotNull(response);
        Assert.IsTrue(response.IsSuccessStatusCode);

        var collection = response.Content.ReadFromJsonAsync<QueryResult>().Result;
        Assert.IsNotNull(collection);
        Assert.AreEqual(1, collection.EpcisBody.QueryResults.ResultBody.EventList.Count);
    }

    [TestMethod]
    public void GetDispositionEventsShouldReturnACollectionResult()
    {
        var response = Client.GetAsync("/dispositions/disp/events").Result;

        Assert.IsNotNull(response);
        Assert.IsTrue(response.IsSuccessStatusCode);

        var collection = response.Content.ReadFromJsonAsync<QueryResult>().Result;
        Assert.IsNotNull(collection);
        Assert.AreEqual(1, collection.EpcisBody.QueryResults.ResultBody.EventList.Count);
    }

    [TestMethod]
    public void GetReadPointEventsShouldReturnACollectionResult()
    {
        var response = Client.GetAsync("/readPoints/readpoint/events").Result;

        Assert.IsNotNull(response);
        Assert.IsTrue(response.IsSuccessStatusCode);

        var collection = response.Content.ReadFromJsonAsync<QueryResult>().Result;
        Assert.IsNotNull(collection);
        Assert.AreEqual(1, collection.EpcisBody.QueryResults.ResultBody.EventList.Count);
    }

    [TestMethod]
    public void GetBizStepEventsShouldReturnACollectionResult()
    {
        var response = Client.GetAsync("/bizSteps/step/events").Result;

        Assert.IsNotNull(response);
        Assert.IsTrue(response.IsSuccessStatusCode);

        var collection = response.Content.ReadFromJsonAsync<QueryResult>().Result;
        Assert.IsNotNull(collection);
        Assert.AreEqual(1, collection.EpcisBody.QueryResults.ResultBody.EventList.Count);
    }

    [TestMethod]
    public void GetBizLocationEventsShouldReturnACollectionResult()
    {
        var response = Client.GetAsync("/bizLocations/loc1/events").Result;

        Assert.IsNotNull(response);
        Assert.IsTrue(response.IsSuccessStatusCode);

        var collection = response.Content.ReadFromJsonAsync<QueryResult>().Result;
        Assert.IsNotNull(collection);
        Assert.AreEqual(1, collection.EpcisBody.QueryResults.ResultBody.EventList.Count);
    }

    [TestMethod]
    public void GetEpcEventsForUnknownValueShouldReturnACollectionResult()
    {
        var response = Client.GetAsync("/epcs/unknown:epc:1/events").Result;

        Assert.IsNotNull(response);
        Assert.IsTrue(response.IsSuccessStatusCode);

        var collection = response.Content.ReadFromJsonAsync<QueryResult>().Result;
        Assert.IsNotNull(collection);
        Assert.AreEqual(0, collection.EpcisBody.QueryResults.ResultBody.EventList.Count);
    }

    [TestMethod]
    public void GetEventTypeEventsForUnknownValueShouldReturnACollectionResult()
    {
        var response = Client.GetAsync("/eventtypes/AggregationEvent/events").Result;

        Assert.IsNotNull(response);
        Assert.IsTrue(response.IsSuccessStatusCode);

        var collection = response.Content.ReadFromJsonAsync<QueryResult>().Result;
        Assert.IsNotNull(collection);
        Assert.AreEqual(0, collection.EpcisBody.QueryResults.ResultBody.EventList.Count);
    }

    [TestMethod]
    public void GetDispositionEventsForUnknownValueShouldReturnACollectionResult()
    {
        var response = Client.GetAsync("/dispositions/unknown/events").Result;

        Assert.IsNotNull(response);
        Assert.IsTrue(response.IsSuccessStatusCode);

        var collection = response.Content.ReadFromJsonAsync<QueryResult>().Result;
        Assert.IsNotNull(collection);
        Assert.AreEqual(0, collection.EpcisBody.QueryResults.ResultBody.EventList.Count);
    }

    [TestMethod]
    public void GetReadPointEventsForUnknownValueShouldReturnACollectionResult()
    {
        var response = Client.GetAsync("/readPoints/unknown/events").Result;

        Assert.IsNotNull(response);
        Assert.IsTrue(response.IsSuccessStatusCode);

        var collection = response.Content.ReadFromJsonAsync<QueryResult>().Result;
        Assert.IsNotNull(collection);
        Assert.AreEqual(0, collection.EpcisBody.QueryResults.ResultBody.EventList.Count);
    }

    [TestMethod]
    public void GetBizStepEventsForUnknownValueShouldReturnACollectionResult()
    {
        var response = Client.GetAsync("/bizSteps/unknown/events").Result;

        Assert.IsNotNull(response);
        Assert.IsTrue(response.IsSuccessStatusCode);

        var collection = response.Content.ReadFromJsonAsync<QueryResult>().Result;
        Assert.IsNotNull(collection);
        Assert.AreEqual(0, collection.EpcisBody.QueryResults.ResultBody.EventList.Count);
    }

    [TestMethod]
    public void GetBizLocationEventsForUnknownValueShouldReturnACollectionResult()
    {
        var response = Client.GetAsync("/bizLocations/unknown/events").Result;

        Assert.IsNotNull(response);
        Assert.IsTrue(response.IsSuccessStatusCode);

        var collection = response.Content.ReadFromJsonAsync<QueryResult>().Result;
        Assert.IsNotNull(collection);
        Assert.AreEqual(0, collection.EpcisBody.QueryResults.ResultBody.EventList.Count);
    }

    [TestMethod]
    public void GetAllEventsShouldReturnACollectionResult()
    {
        var response = Client.GetAsync("/events").Result;

        Assert.IsNotNull(response);
        Assert.IsTrue(response.IsSuccessStatusCode);

        var collection = response.Content.ReadFromJsonAsync<QueryResult>().Result;
        Assert.IsNotNull(collection);
        Assert.AreEqual(1, collection.EpcisBody.QueryResults.ResultBody.EventList.Count);
    }
}
