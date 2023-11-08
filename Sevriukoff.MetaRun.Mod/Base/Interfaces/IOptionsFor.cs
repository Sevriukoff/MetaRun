namespace Sevriukoff.MetaRun.Mod.Base.Interfaces;

public interface IOptionsFor<T>
{
    public T ConfiguredObject { get; set; }
}