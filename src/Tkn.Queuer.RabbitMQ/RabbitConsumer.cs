using System;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Tkn.Queuer.Exceptions;
using Tkn.Queuer.Interface;
using Tkn.Queuer.Models;

namespace Tkn.Queuer.RabbitMQ {
	public class RabbitConsumer<T> : RabbitBase<T>, IQueueConsumer<T> where T : BaseQueueModel, new() {
		public RabbitConsumer(QueueSettingsModel settings) : base(settings) {
		}

		public void Subscribe(string queue, Action<T> handler) {
			if (!Connections.Any())
				throw new QueuerException("There are no active RabbitMQ connections, please check QueueSettings.");
			if (Connections.Count > 1)
				throw new QueuerException("There are more than one active RabbitMQ connections. Please specify target virtual host in QueueSettings.");

			var rabbitModel = Connections.First().Model;
			consumeQueue(queue, handler, rabbitModel);
		}

		public void Subscribe(string queue, Action<T> handler, string virtualHost) {
			var connection = Connections.FirstOrDefault(x => x.VirtualHost == virtualHost);
			if (connection == null)
				throw new QueuerException($"There are no active connections for virtual host [{virtualHost}], please check QueueSettings.");

			var rabbitModel = connection.Model;
			consumeQueue(queue, handler, rabbitModel);
		}

		void consumeQueue(string queue, Action<T> handler, IModel rabbitModel) {
			rabbitModel.BasicQos(0, 1, false);
			var consumer = new EventingBasicConsumer(rabbitModel);
			consumer.Received += (model, args) => {
				try {
					var message = Encoding.Default.GetString(args.Body);

					handler(convertFromJson(message));
					rabbitModel.BasicAck(args.DeliveryTag, false);
				} catch (QueueHandlerException ex) {
					rabbitModel.BasicReject(args.DeliveryTag, ex.Requeue);
				}
			};
			rabbitModel.BasicConsume(queue, false, consumer);
		}

		T convertFromJson(string json) {
			var returnObject = new T();

			try {
				returnObject = JsonConvert.DeserializeObject<T>(json, JsonSettings);

				returnObject.IsValid = true;
			} catch {
				// ignored since return object will have isValid set to "false" by default
			}

			return returnObject;
		}
	}
}