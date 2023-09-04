using System.Threading.Tasks;
using Sevriukoff.MetaRun.Domain;
using Sevriukoff.MetaRun.Domain.Base;

namespace Sevriukoff.MetaRun.Mod;

public interface IProducer
{ 
    Task ProduceAsync(EventMetaData gameEvent);
}