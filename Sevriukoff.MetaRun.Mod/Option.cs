using Sevriukoff.MetaRun.Mod.Base;

namespace Sevriukoff.MetaRun.Mod;

public class Option<T>(string section, string key, T defaultValue) : OptionBase(section, key, typeof(T), defaultValue)
{
    public virtual T Value { get; set; }
    public override object BoxedValue
    {
        get => Value;
        set => Value = (T) value;
    }
}