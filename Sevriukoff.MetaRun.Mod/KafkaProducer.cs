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
    private readonly IProducer<Guid, string> _producer;

    private readonly JsonSerializerSettings _jsonSetting;
    
    public KafkaProducer(string server)
    {
        _config = new ProducerConfig
        {
            BootstrapServers = server,
            Acks = Acks.None,
            BatchSize = 655360,
            LingerMs = 100
        };

        _producer = new ProducerBuilder<Guid, string>(_config)
            .SetKeySerializer(new GuidSerializer())
            .Build();
        
        _jsonSetting = new JsonSerializerSettings
        {
            TypeNameHandling = TypeNameHandling.Auto
        };
    }

    public async Task ProduceAsync(EventMetaData gameEvent)
    {
        var message = new Message<Guid, string>
        {
            Key = gameEvent.RunId,
            Value = JsonConvert.SerializeObject(gameEvent, _jsonSetting)
        };

        await _producer.ProduceAsync("topic_52", message);
    }

    public void Produce(EventMetaData gameEvent)
    {
        var message = new Message<Guid, string>
        {
            Key = gameEvent.RunId,
            Value = JsonConvert.SerializeObject(gameEvent, _jsonSetting)
        };

        _producer.Produce("topic_52", message);
    }
}

public class GuidSerializer : ISerializer<Guid>
{
    public byte[] Serialize(Guid data, SerializationContext context)
    {
        return data.ToByteArray();
    }
}