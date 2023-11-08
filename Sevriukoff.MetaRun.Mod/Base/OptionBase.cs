using System;

namespace Sevriukoff.MetaRun.Mod.Base;

public abstract class OptionBase(string section, string key, Type type, object defaultValue)
{
    public string Section { get; } = section;
    public string Key { get; } = key;
    public Type Type { get; } = type;
    public object DefaultValue { get; init; } = defaultValue;
    public abstract  object BoxedValue { get; set; }
    public string Description { get; init; }
}