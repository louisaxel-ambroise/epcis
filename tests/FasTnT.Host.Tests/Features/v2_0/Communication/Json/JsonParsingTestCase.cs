﻿using FasTnT.Host.Communication.Json.Parsers;
using System.Reflection;
using System.Text.Json;

namespace FasTnT.Host.Tests.Features.v2_0.Communication.Json;

public abstract class JsonParsingTestCase
{
    protected static JsonDocument ParseResource(string resourceName)
    {
        var manifest = Assembly.GetExecutingAssembly().GetManifestResourceStream(resourceName);
        using var resourceStream = JsonDocumentParser.Instance.ParseAsync(manifest, default);

        return resourceStream.Result;
    }
}
