namespace OpenHabUrlSegment;

public class OpenHabProxyOptions
{
    public const string SectionName = nameof(OpenHabProxyOptions);

    public string OpenHabHost { get; set; } = string.Empty;

    public uint OpenHabPort { get; set; }

    public string UrlPathSegment { get; set; } = string.Empty;
}