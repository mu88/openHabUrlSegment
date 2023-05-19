using OpenHabUrlSegment;
using Yarp.ReverseProxy.Configuration;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddReverseProxy();
builder.Services.AddSingleton<IProxyConfigProvider, OpenHabProxyConfigProvider>();

var app = builder.Build();
app.MapReverseProxy();

app.Run();