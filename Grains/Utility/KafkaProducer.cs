using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Confluent.Kafka;
using Confluent.Kafka.Serialization;
using Newtonsoft.Json;

namespace Grains.Utility
{
  public class KafkaProducer : IKafkaProducer, IDisposable
  {
	  private readonly IProducer<String, String> _producer;
	  private readonly JsonSerializerSettings _jsonSerializerSettings;

	  public KafkaProducer()
	  {
		  var config = new Dictionary<String, Object>
		  {
			  { "bootstrap.servers", Environment.GetEnvironmentVariable("KAFKA") }
		  };
		  _producer = new Producer<String,String>(config, new StringSerializer(Encoding.UTF8), new StringSerializer(Encoding.UTF8));
		  _jsonSerializerSettings = new JsonSerializerSettings
		  {
			  TypeNameHandling = TypeNameHandling.All
		  };
	  }

	  public void Dispose()
	  {
		  _producer?.Dispose();
	  }

	  public async Task PushToQueue(String queue, String key, String data)
	  {
		  await _producer.ProduceAsync(queue, new Message<String, String> {Key = key, Value = data, Timestamp = Timestamp.Default});
	  }

	  public async Task PushToQueue(String queue, String key, Object data)
	  {
		  await PushToQueue(queue, key, JsonConvert.SerializeObject(data, _jsonSerializerSettings));
	  }
  }
}