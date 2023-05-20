using System.IO.Compression;
using System.Net.Http.Headers;
using System.Net.Mime;
using System.Text;
using OpenHabUrlSegment;
using Yarp.ReverseProxy.Configuration;
using Yarp.ReverseProxy.Transforms;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddReverseProxy()
    .AddTransforms(context =>
    {
        context.AddResponseTransform(async responseContext =>
        {
            if (responseContext.ProxyResponse != null)
            {
                if (responseContext.ProxyResponse.Content.Headers.ContentEncoding.Contains("br"))
                {
                    var readAsStreamAsync = await responseContext.ProxyResponse.Content.ReadAsStreamAsync();
                    var brotliDecompressionStream = new BrotliStream(readAsStreamAsync, CompressionMode.Decompress);
                    using var reader = new StreamReader(brotliDecompressionStream);
                    var body = await reader.ReadToEndAsync();

                    if (!string.IsNullOrEmpty(body))
                    {
                        responseContext.SuppressResponseBody = true;

                        var uncompressedBytes = Encoding.UTF8.GetBytes(body
                                                                           .Replace("""src="/js/""", """src="/openhab/js/""")
                                                                           .Replace("""href="/css/""", """href="/openhab/css/""")
                                                                           .Replace("""href="/res/""", """href="/openhab/res/""")
                                                                           .Replace("""window.location="/auth?response_type=code&client_id="+encodeURIComponent(window.location.origin)+"&redirect_uri="+encodeURIComponent(window.location.origin)+""", """window.location="/openhab/auth?response_type=code&client_id="+encodeURIComponent(window.location.origin)+"&redirect_uri="+encodeURIComponent(window.location.origin)+"/openhab"+""")
                                                                           .Replace("""action="/auth""", """action="/openhab/auth""")
                                                                           .Replace("""+"images/""", """+"openhab/images/""")
                                                                           .Replace("""+"js/""", """+"openhab/js/""")
                                                                           .Replace("""path:"/analyzer/""", """path:"/openhab/analyzer/""")
                                                                           .Replace("""path:"/res/""", """path:"/openhab/res/""")
                                                                           .Replace("""("/rest/""", """("/openhab/rest/"""));
                        var compressedBytes = new byte[uncompressedBytes.Length];
                        BrotliEncoder.TryCompress(uncompressedBytes, compressedBytes, out var bytesWritten);
                        responseContext.HttpContext.Response.ContentLength = bytesWritten;
                        await responseContext.HttpContext.Response.Body.WriteAsync(compressedBytes);
                    }
                }
                else if (responseContext.ProxyResponse.Content.Headers.ContentType?.MediaType == MediaTypeNames.Text.Html)
                {
                    var stream = await responseContext.ProxyResponse.Content.ReadAsStreamAsync();
                    using var reader = new StreamReader(stream);
                    var body = await reader.ReadToEndAsync();
                    if (!string.IsNullOrEmpty(body))
                    {
                        responseContext.SuppressResponseBody = true;

                        var bytes = Encoding.UTF8.GetBytes(body
                                                               .Replace("""action="/auth""", """action="/openhab/auth""")
                                                               .Replace("""type="button" href="/">""", """type="button" href="openhab/">""")
                                                               .Replace("""href="res/""", """href="openhab/res/""")
                                                               .Replace("""href="/manifest.json""", """href="openhab/manifest.json"""));
                        responseContext.HttpContext.Response.ContentLength = bytes.Length;
                        await responseContext.HttpContext.Response.Body.WriteAsync(bytes);
                    }
                }
            }
        });
    });
builder.Services.AddSingleton<IProxyConfigProvider, OpenHabProxyConfigProvider>();
builder.Services.Configure<OpenHabProxyOptions>(builder.Configuration.GetSection(OpenHabProxyOptions.SectionName));

var app = builder.Build();
app.MapReverseProxy();

app.Run();