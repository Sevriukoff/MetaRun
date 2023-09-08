using System;
using Sevriukoff.MetaRun.Domain.Base;

namespace Sevriukoff.MetaRun.Mod.Base;

public class TrackerOptions
{
    public bool IsActive { get; set; }
    public string Priority { get; set; }
    public bool TrackOnlyHost { get; set; }
    public Action<EventMetaData> OnEventTracked;
}