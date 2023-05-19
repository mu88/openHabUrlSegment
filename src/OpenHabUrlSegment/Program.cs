using OpenHabUrlSegment;
using Yarp.ReverseProxy.Configuration;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddReverseProxy();
builder.Services.AddSingleton<IProxyConfigProvider, OpenHabProxyConfigProvider>();
builder.Services.Configure<OpenHabProxyOptions>(builder.Configuration.GetSection(OpenHabProxyOptions.SectionName));

var app = builder.Build();
app.MapReverseProxy();

app.Run();