using Microsoft.Extensions.Options;
using Microsoft.Extensions.Primitives;
using Yarp.ReverseProxy.Configuration;
using Yarp.ReverseProxy.Transforms;

namespace OpenHabUrlSegment;

internal class OpenHabProxyConfigProvider : IProxyConfigProvider
{
    private readonly IReadOnlyList<RouteConfig> _routes;

    private readonly IReadOnlyList<ClusterConfig> _clusters;

    public OpenHabProxyConfigProvider(IOptions<OpenHabProxyOptions> options)
    {
        var config = options.Value;
        var openHab = $"{config.OpenHabHost}:{config.OpenHabPort}";
        _routes = new[]
        {
            new RouteConfig { RouteId = "route1", ClusterId = "cluster1", Match = new RouteMatch { Path = $"{config.UrlPathSegment}/{{**catch-all}}" } }
                .WithTransformPathRemovePrefix("/openhab")
                .WithTransformRequestHeader("Host", $"{openHab}", false)
        };

        _clusters = new[]
        {
            new ClusterConfig
            {
                ClusterId = "cluster1",
                Destinations = new Dictionary<string, DestinationConfig> { { "destination1", new DestinationConfig { Address = $"http://{openHab}/" } } }
            }
        };
    }

    /// <inheritdoc />
    public IProxyConfig GetConfig() => new OpenHabProxyConfig(_routes, _clusters);

    private class OpenHabProxyConfig : IProxyConfig
    {
        private readonly CancellationTokenSource _cts = new();

        public OpenHabProxyConfig(IReadOnlyList<RouteConfig> routes, IReadOnlyList<ClusterConfig> clusters)
        {
            Routes = routes;
            Clusters = clusters;
            ChangeToken = new CancellationChangeToken(_cts.Token);
        }

        /// <inheritdoc />
        public IReadOnlyList<RouteConfig> Routes { get; }

        /// <inheritdoc />
        public IReadOnlyList<ClusterConfig> Clusters { get; }

        /// <inheritdoc />
        public IChangeToken ChangeToken { get; }
    }
}