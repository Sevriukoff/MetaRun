using System;

namespace Sevriukoff.MetaRun.Mod.Base.Interfaces;

public interface IBindableOptionGroup : IOptionGroup
{
    event Action AnyOptionChanged;
}