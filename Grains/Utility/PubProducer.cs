using System;
using System.Threading.Tasks;
using Newtonsoft.Json;
using StackExchange.Redis;

namespace Grains.Utility
{
  public class PubProducer : IPubProducer, IDisposable
  {
	  private readonly ISubscriber _producer;
	  private readonly JsonSerializerSettings _jsonSerializerSettings;

	  public PubProducer()
	  {
		  var redis = ConnectionMultiplexer.Connect(Environment.GetEnvironmentVariable("REDIS"));
		  _producer = redis.GetSubscriber();
		  _jsonSerializerSettings = new JsonSerializerSettings
		  {
			  TypeNameHandling = TypeNameHandling.All
		  };
	  }

	  public void Dispose()
	  {
	  }

	  public async Task PushToQueue(String queue, String key, String data)
	  {
		  await _producer.PublishAsync(queue, data);
	  }

	  public async Task PushToQueue(String queue, String key, Object data)
	  {
		  await PushToQueue(queue, key, JsonConvert.SerializeObject(data, _jsonSerializerSettings));
	  }
  }
}