using System;
using System.Threading.Tasks;
using Confluent.Kafka;
using Newtonsoft.Json;
using Sevriukoff.MetaRun.Domain;
using Sevriukoff.MetaRun.Domain.Base;

namespace Sevriukoff.MetaRun.Mod;

public class KafkaProducer : IProducer
{
    private readonly ProducerConfig _config;
    
    public KafkaProducer(string server)
    {
        _config = new ProducerConfig
        {
            BootstrapServers = server
        };
    }

    public async Task ProduceAsync(EventMetaData gameEvent)
    {
        using (var producer = new ProducerBuilder<Null, string>(_config).Build())
        {
            var message = new Message<Null, string>()
            {
                Value = JsonConvert.SerializeObject(gameEvent)
            };
            
            producer.Produce("topic_52", message);
        }
    }
}