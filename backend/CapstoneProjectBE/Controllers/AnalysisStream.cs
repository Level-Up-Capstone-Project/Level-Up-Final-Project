using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http.Features;

namespace CapstoneProjectBE.Controllers;

/// <summary>Streams completed analysis milestones and the final result on the authenticated request.</summary>
internal static class AnalysisStream
{
    private static readonly JsonSerializerOptions Json = new(JsonSerializerDefaults.Web);

    public static async Task WriteAsync(HttpContext context, Func<Func<int, Task>, Task<IActionResult>> analyse)
    {
        var ct = context.RequestAborted;
        context.Response.ContentType = "application/x-ndjson";
        context.Response.Headers.CacheControl = "no-store";
        context.Response.Headers["X-Accel-Buffering"] = "no";
        context.Features.Get<IHttpResponseBodyFeature>()?.DisableBuffering();
        async Task Send(object item)
        {
            await context.Response.WriteAsync(JsonSerializer.Serialize(item, Json) + "\n", ct);
            await context.Response.Body.FlushAsync(ct);
        }
        try
        {
            await Send(new {type = "progress", percent = 0});
            var result = await analyse(percent => Send(new {type = "progress", percent}));
            var status = result is ObjectResult obj ? obj.StatusCode ?? 200 : result is StatusCodeResult code ? code.StatusCode : 500;
            var value = (result as ObjectResult)?.Value;
            if (status >= 400) await Send(new {type = "error", status, details = value});
            else await Send(new {type = "result", data = value});
        }
        catch (OperationCanceledException) when (ct.IsCancellationRequested) { }
        catch (Exception error)
        {
            context.RequestServices.GetRequiredService<ILoggerFactory>().CreateLogger("AnalysisStream").LogError(error, "Document analysis failed");
            if (!ct.IsCancellationRequested)
                await Send(new {type = "error", status = 500, details = new {message = "Document analysis could not finish. Please try again."}});
        }
    }
}
