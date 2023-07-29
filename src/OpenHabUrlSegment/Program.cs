using Microsoft.AspNetCore.Rewrite;
using OpenHabUrlSegment;
using Yarp.ReverseProxy.Configuration;
using Yarp.ReverseProxy.Transforms;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddReverseProxy();
builder.Services.AddSingleton<IProxyConfigProvider, OpenHabProxyConfigProvider>();
builder.Services.Configure<OpenHabProxyOptions>(builder.Configuration.GetSection(OpenHabProxyOptions.SectionName));

var app = builder.Build();

app.UseRewriter(new RewriteOptions().AddRewrite("^(?!openhab/)(.*)", "openhab/$1", true));

// app.UseRewriter(new RewriteOptions().Add(context =>
// {
//     var request = context.HttpContext.Request;
//     if (
//         // request.Headers.Referer.Any(refererHeader => refererHeader != null && refererHeader.Contains("/openhab")) && 
//         !request.Path.StartsWithSegments(new PathString("/openhab")))
//     {
//         context.Result = RuleResult.SkipRemainingRules;
//         request.Path = $"/openhab/{request.Path.Value?.TrimStart('/')}";
//     }
// }));

// app.UseRewriter(new RewriteOptions().AddRedirect("^(?!openhab/)(.*)", "openhab/$1"));

app.MapReverseProxy();

app.Run();