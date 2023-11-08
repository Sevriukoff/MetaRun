using BepInEx.Configuration;

namespace Sevriukoff.MetaRun.Mod.Base.Interfaces;

public interface IBindableOption<T>
{
    void Bind(ConfigEntry<T> bind);
}