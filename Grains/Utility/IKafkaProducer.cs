﻿using System;
using System.Threading.Tasks;

namespace Grains.Utility
{
	public interface IKafkaProducer
	{
		Task PushToQueue(String queue, String key, String data);
		Task PushToQueue(String queue, String key, Object data);
	}
}