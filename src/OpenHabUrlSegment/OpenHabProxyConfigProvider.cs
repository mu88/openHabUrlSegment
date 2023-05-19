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
                .WithTransformRequestHeader("Host", $"{openHab}", false),
            new RouteConfig { RouteId = "route2", ClusterId = "cluster2", Match = new RouteMatch { Path = "js/{**catch-all}" } }
                .WithTransformPathRemovePrefix("/js")
                .WithTransformRequestHeader("Host", $"{openHab}", false),
            new RouteConfig { RouteId = "route3", ClusterId = "cluster3", Match = new RouteMatch { Path = "css/{**catch-all}" } }
                .WithTransformPathRemovePrefix("/css")
                .WithTransformRequestHeader("Host", $"{openHab}", false),
            new RouteConfig { RouteId = "route4", ClusterId = "cluster4", Match = new RouteMatch { Path = "res/{**catch-all}" } }
                .WithTransformPathRemovePrefix("/res")
                .WithTransformRequestHeader("Host", $"{openHab}", false),
            new RouteConfig { RouteId = "route5", ClusterId = "cluster5", Match = new RouteMatch { Path = "rest/{**catch-all}" } }
                .WithTransformPathRemovePrefix("/rest")
                .WithTransformRequestHeader("Host", $"{openHab}", false),
            new RouteConfig { RouteId = "route6", ClusterId = "cluster6", Match = new RouteMatch { Path = "fonts/{**catch-all}" } }
                .WithTransformPathRemovePrefix("/fonts")
                .WithTransformRequestHeader("Host", $"{openHab}", false),
            new RouteConfig { RouteId = "route7", ClusterId = "cluster7", Match = new RouteMatch { Path = "images/{**catch-all}" } }
                .WithTransformPathRemovePrefix("/images")
                .WithTransformRequestHeader("Host", $"{openHab}", false)
        };

        _clusters = new[]
        {
            new ClusterConfig
            {
                ClusterId = "cluster1",
                Destinations = new Dictionary<string, DestinationConfig> { { "destination1", new DestinationConfig { Address = $"http://{openHab}/" } } }
            },
            new ClusterConfig
            {
                ClusterId = "cluster2",
                Destinations = new Dictionary<string, DestinationConfig> { { "destination2", new DestinationConfig { Address = $"http://{openHab}/js" } } }
            },
            new ClusterConfig
            {
                ClusterId = "cluster3",
                Destinations = new Dictionary<string, DestinationConfig> { { "destination3", new DestinationConfig { Address = $"http://{openHab}/css" } } }
            },
            new ClusterConfig
            {
                ClusterId = "cluster4",
                Destinations = new Dictionary<string, DestinationConfig> { { "destination4", new DestinationConfig { Address = $"http://{openHab}/res" } } }
            },
            new ClusterConfig
            {
                ClusterId = "cluster5",
                Destinations = new Dictionary<string, DestinationConfig> { { "destination5", new DestinationConfig { Address = $"http://{openHab}/rest" } } }
            },
            new ClusterConfig
            {
                ClusterId = "cluster6",
                Destinations = new Dictionary<string, DestinationConfig> { { "destination6", new DestinationConfig { Address = $"http://{openHab}/fonts" } } }
            },
            new ClusterConfig
            {
                ClusterId = "cluster7",
                Destinations = new Dictionary<string, DestinationConfig> { { "destination7", new DestinationConfig { Address = $"http://{openHab}/images" } } }
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