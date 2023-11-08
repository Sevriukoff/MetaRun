using System.Collections.Generic;
using RiskOfOptions.OptionConfigs;

namespace Sevriukoff.MetaRun.Mod.Base.Interfaces;

public interface IOptionGroup
{
    IEnumerable<(OptionBase, BaseOptionConfig)> GetOption();
}