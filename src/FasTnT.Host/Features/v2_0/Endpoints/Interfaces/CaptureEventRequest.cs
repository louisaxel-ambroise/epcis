﻿using FasTnT.Domain.Model;
using FasTnT.Host.Features.v2_0.Communication.Json.Parsers;

namespace FasTnT.Host.Features.v2_0.Endpoints.Interfaces;

public record CaptureEventRequest(Request Request)
{
    public static async ValueTask<CaptureEventRequest> BindAsync(HttpContext context)
    {
        var headerContext = context.Request.Headers.TryGetValue("GS1-Extensions", out var extensions) ? extensions.ToArray() : Array.Empty<string>();
        var epcisContext = Namespaces.ParseHeader(headerContext);

        var request = await JsonCaptureRequestParser.ParseEventAsync(context.Request.Body, epcisContext, context.RequestAborted);

        return request;
    }

    public static implicit operator CaptureEventRequest(Request request) => new(request);
}
