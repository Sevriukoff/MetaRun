using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using RiskOfOptions.OptionConfigs;
using Sevriukoff.MetaRun.Mod.Base.Interfaces;

namespace Sevriukoff.MetaRun.Mod.Base;

public class BaseTrackerOptions : IBindableOptionGroup, IOptionsFor<BaseEventTracker>
{
    public BaseEventTracker ConfiguredObject { get; set; }
    public BindableOption<bool> IsActive { get; init; }
    public BindableOption<float> MaxEventSummation { get; init; }
    public BindableOption<float> LingerMs { get; init; }
    
    private readonly Type _trackerType;
    private bool IsRunTrackerType => _trackerType.Name.Contains("Run");
    protected string Section => string.Join(" ",
       (IsRunTrackerType ? new[] {"(R)"} : new[] {"(C)"})
       .Concat(Regex.Split(_trackerType.Name, @"(?=[A-Z])")
           .Skip(2)
           .TakeWhile(x => x != "Tracker")
       ));

    public BaseTrackerOptions(Type type)
    {
        _trackerType = type;

        IsActive = new BindableOption<bool>(Section, "Включить трекер")
        {
            Description = "Вкл/Выкл трекер", DefaultValue = true
        };
        
        MaxEventSummation = new BindableOption<float>(Section, "Кол-во событий для суммирования")
        {
            Description = "Сколько событий могут быть объедены в одно за время хранения сообщений в буфере перед отправкой на сервер. Позволяет снизить нагрузку при большом количестве собыйтий в секунду. Особенно полезно для трекера передвижения"
        };
        
        LingerMs = new BindableOption<float>(Section, "Время хранения сообщений в мс")
        {
            Description = "Сколько милисекунд будут храниться события во внутреннем буфере трекера перед отправкой на сервер. Отдельно это параметр практически не имеет значения. Рекомендуется использовать вместе с параметром \\\"Кол-во событий для суммирования\\\""
        };
    }

    public virtual IEnumerable<(OptionBase, BaseOptionConfig)> GetOption()
    {
        yield return (IsActive, null);
        yield return (MaxEventSummation, new StepSliderConfig{ max = 255, increment = 5 });
        yield return (LingerMs, new StepSliderConfig{ max = 30000, increment = 100 });
    }

    public event Action AnyOptionChanged;
}