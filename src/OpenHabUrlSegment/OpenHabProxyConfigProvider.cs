using Microsoft.Extensions.Options;
using Microsoft.Extensions.Primitives;
using Yarp.ReverseProxy.Configuration;

namespace OpenHabUrlSegment;

internal class OpenHabProxyConfigProvider : IProxyConfigProvider
{
    private readonly IReadOnlyList<RouteConfig> _routes;

    private readonly IReadOnlyList<ClusterConfig> _clusters;

    public OpenHabProxyConfigProvider(IOptions<OpenHabProxyOptions> options)
    {
        _routes = new[] { new RouteConfig { RouteId = "route1", ClusterId = "cluster1", Match = new RouteMatch { Path = "/openhab/{**catch-all}" } } };

        _clusters = new[]
        {
            new ClusterConfig
            {
                ClusterId = "cluster1",
                Destinations = new Dictionary<string, DestinationConfig> { { "destination1", new DestinationConfig { Address = "http://127.0.0.1:8080/" } } }
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