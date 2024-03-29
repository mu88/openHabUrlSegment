﻿using Microsoft.Extensions.Primitives;
using Yarp.ReverseProxy.Configuration;
using Yarp.ReverseProxy.Transforms;

namespace OpenHabUrlSegment;

internal class OpenHabProxyConfigProvider : IProxyConfigProvider
{
    private readonly IReadOnlyList<RouteConfig> _routes;

    private readonly IReadOnlyList<ClusterConfig> _clusters;

    public OpenHabProxyConfigProvider()
    {
        _routes = new[]
        {
            new RouteConfig { RouteId = "route1", ClusterId = "cluster1", Match = new RouteMatch { Path = "openhab/{**catch-all}" } }
                .WithTransformPathRemovePrefix("/openhab")
                .WithTransformRequestHeader("Host", "localhost:9000")
        };

        _clusters = new[]
        {
            new ClusterConfig
            {
                ClusterId = "cluster1",
                Destinations = new Dictionary<string, DestinationConfig> { { "destination1", new DestinationConfig { Address = "http://localhost:9000/" } } }
            }
        };
    }

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

        public IReadOnlyList<RouteConfig> Routes { get; }

        public IReadOnlyList<ClusterConfig> Clusters { get; }

        public IChangeToken ChangeToken { get; }
    }
}