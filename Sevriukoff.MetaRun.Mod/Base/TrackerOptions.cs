using System;
using Sevriukoff.MetaRun.Domain.Base;

namespace Sevriukoff.MetaRun.Mod.Base;

public class TrackerOptions
{
    public bool IsActive { get; set; }
    public byte MaxEventSummation { get; set; }
    public int LingerMs { get; set; }
    public bool TrackOnlyHost { get; set; }
    public Action<EventMetaData> OnEventTracked;

    public static TrackerOptions CreateDefault() => new()
        {IsActive = true, MaxEventSummation = 0, LingerMs = 0, TrackOnlyHost = false};
}