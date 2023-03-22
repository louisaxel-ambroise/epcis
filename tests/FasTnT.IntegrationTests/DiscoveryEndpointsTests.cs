using FasTnT.Application.Database;
using FasTnT.Domain.Model;
using FasTnT.Domain.Model.Events;
using FasTnT.IntegrationTests.Interfaces;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace FasTnT.IntegrationTests;

[TestClass]
public class DiscoveryEndpointsTests
{
    internal static WebApplicationFactory<Program> TestHost { get; set; }
    internal static HttpClient Client { get; set; }

    [ClassInitialize]
    public static void AssemblyInit(TestContext _)
    {
        TestHost = new FasTnTApplicationFactory();
        Client = TestHost.CreateDefaultClient();
        Client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", "YWRtaW46UEBzc3cwcmQ=");

        using (var scope = TestHost.Server.Services.CreateScope())
        {
            var dbContext = scope.ServiceProvider.GetService<EpcisContext>();
            
            dbContext.Add(new Request
            {
                CaptureTime = DateTime.Now,
                DocumentTime = DateTime.Now,
                SchemaVersion = "2.0",
                UserId = "431257CC4ADAF410486CDD3D6DC22F08",
                Events = new List<Event>
                {
                    new Event
                    {
                        Disposition = "disp",
                        BusinessLocation = "loc1",
                        BusinessStep = "step",
                        ReadPoint = "readpoint",
                        Epcs = new List<Epc> {
                            new Epc { Type = Domain.Enumerations.EpcType.List, Id = "test:epc:1" },
                            new Epc { Type = Domain.Enumerations.EpcType.List, Id = "test:epc:2" },
                        }
                    },
                    new Event
                    {
                        BusinessLocation = "loc2",
                        BusinessStep = "step",
                        Epcs = new List<Epc> {
                            new Epc { Type = Domain.Enumerations.EpcType.List, Id = "test:epc:3" }
                        }
                    }
                }
            });

            dbContext.Add(new Request
            {
                CaptureTime = DateTime.Now,
                DocumentTime = DateTime.Now,
                SchemaVersion = "2.0",
                UserId = "ANOTHERUSER",
                Events = new List<Event>
                {
                    new Event
                    {
                        Disposition = "seconddisp",
                        BusinessLocation = "secondloc1",
                        BusinessStep = "secondstep",
                        ReadPoint = "secondreadpoint",
                        Epcs = new List<Epc> {
                            new Epc { Type = Domain.Enumerations.EpcType.List, Id = "second:epc:1" },
                            new Epc { Type = Domain.Enumerations.EpcType.List, Id = "second:epc:2" },
                        }
                    }
                }
            });
            dbContext.SaveChanges();
        }
    }

    [TestMethod]
    public void GetHealthEndpointShouldReturnACollectionResult()
    {
        var response = Client.GetAsync("/health").Result;

        Assert.IsNotNull(response);
        Assert.IsTrue(response.IsSuccessStatusCode);
    }

    [TestMethod]
    public void GetEpcListShouldReturnACollectionResult()
    {
        var response = Client.GetAsync("/v2_0/epcs").Result;

        Assert.IsNotNull(response);
        Assert.IsTrue(response.IsSuccessStatusCode);

        var collection = response.Content.ReadFromJsonAsync<CollectionResult>().Result;
        Assert.IsNotNull(collection);
        Assert.AreEqual(3, collection.Members.Length);
    }

    [TestMethod]
    public void GetEventTypesShouldReturnACollectionResult()
    {
        var response = Client.GetAsync("/v2_0/eventtypes").Result;

        Assert.IsNotNull(response);
        Assert.IsTrue(response.IsSuccessStatusCode);

        var collection = response.Content.ReadFromJsonAsync<CollectionResult>().Result;
        Assert.IsNotNull(collection);
        Assert.AreEqual(5, collection.Members.Length);
    }

    [TestMethod]
    public void GetDispositionsShouldReturnACollectionResult()
    {
        var response = Client.GetAsync("/v2_0/dispositions").Result;

        Assert.IsNotNull(response);
        Assert.IsTrue(response.IsSuccessStatusCode);

        var collection = response.Content.ReadFromJsonAsync<CollectionResult>().Result;
        Assert.IsNotNull(collection);
        Assert.AreEqual(1, collection.Members.Length);
    }

    [TestMethod]
    public void GetReadPointsShouldReturnACollectionResult()
    {
        var response = Client.GetAsync("/v2_0/readPoints").Result;

        Assert.IsNotNull(response);
        Assert.IsTrue(response.IsSuccessStatusCode);

        var collection = response.Content.ReadFromJsonAsync<CollectionResult>().Result;
        Assert.IsNotNull(collection);
        Assert.AreEqual(1, collection.Members.Length);
    }

    [TestMethod]
    public void GetBizStepsShouldReturnACollectionResult()
    {
        var response = Client.GetAsync("/v2_0/bizSteps").Result;

        Assert.IsNotNull(response);
        Assert.IsTrue(response.IsSuccessStatusCode);

        var collection = response.Content.ReadFromJsonAsync<CollectionResult>().Result;
        Assert.IsNotNull(collection);
        Assert.AreEqual(1, collection.Members.Length);
    }

    [TestMethod]
    public void GetBizLocationsShouldReturnACollectionResult()
    {
        var response = Client.GetAsync("/v2_0/bizLocations").Result;

        Assert.IsNotNull(response);
        Assert.IsTrue(response.IsSuccessStatusCode);

        var collection = response.Content.ReadFromJsonAsync<CollectionResult>().Result;
        Assert.IsNotNull(collection);
        Assert.AreEqual(2, collection.Members.Length);
    }

    [TestMethod]
    public void GetEpcDetailShouldReturnACollectionResult()
    {
        var response = Client.GetAsync("/v2_0/epcs/test:epc:2").Result;

        Assert.IsNotNull(response);
        Assert.IsTrue(response.IsSuccessStatusCode);

        var collection = response.Content.ReadFromJsonAsync<CollectionResult>().Result;
        Assert.IsNotNull(collection);
        Assert.AreEqual(1, collection.Members.Length);
        Assert.AreEqual("events", collection.Members[0]);
    }

    [TestMethod]
    public void GetEventTypeDetailShouldReturnACollectionResult()
    {
        var response = Client.GetAsync("/v2_0/eventtypes/ObjectEvent").Result;

        Assert.IsNotNull(response);
        Assert.IsTrue(response.IsSuccessStatusCode);

        var collection = response.Content.ReadFromJsonAsync<CollectionResult>().Result;
        Assert.IsNotNull(collection);
        Assert.AreEqual(1, collection.Members.Length);
        Assert.AreEqual("events", collection.Members[0]);
    }

    [TestMethod]
    public void GetDispositionDetailShouldReturnACollectionResult()
    {
        var response = Client.GetAsync("/v2_0/dispositions/disp").Result;

        Assert.IsNotNull(response);
        Assert.IsTrue(response.IsSuccessStatusCode);

        var collection = response.Content.ReadFromJsonAsync<CollectionResult>().Result;
        Assert.IsNotNull(collection);
        Assert.AreEqual(1, collection.Members.Length);
        Assert.AreEqual("events", collection.Members[0]);
    }

    [TestMethod]
    public void GetReadPointDetailShouldReturnACollectionResult()
    {
        var response = Client.GetAsync("/v2_0/readPoints/readpoint").Result;

        Assert.IsNotNull(response);
        Assert.IsTrue(response.IsSuccessStatusCode);

        var collection = response.Content.ReadFromJsonAsync<CollectionResult>().Result;
        Assert.IsNotNull(collection);
        Assert.AreEqual(1, collection.Members.Length);
        Assert.AreEqual("events", collection.Members[0]);
    }

    [TestMethod]
    public void GetBizStepDetailShouldReturnACollectionResult()
    {
        var response = Client.GetAsync("/v2_0/bizSteps/step").Result;

        Assert.IsNotNull(response);
        Assert.IsTrue(response.IsSuccessStatusCode);

        var collection = response.Content.ReadFromJsonAsync<CollectionResult>().Result;
        Assert.IsNotNull(collection);
        Assert.AreEqual(1, collection.Members.Length);
        Assert.AreEqual("events", collection.Members[0]);
    }

    [TestMethod]
    public void GetBizLocationDetailsShouldReturnACollectionResult()
    {
        var response = Client.GetAsync("/v2_0/bizLocations/loc1").Result;

        Assert.IsNotNull(response);
        Assert.IsTrue(response.IsSuccessStatusCode);

        var collection = response.Content.ReadFromJsonAsync<CollectionResult>().Result;
        Assert.IsNotNull(collection);
        Assert.AreEqual(1, collection.Members.Length);
        Assert.AreEqual("events", collection.Members[0]);
    }
}
