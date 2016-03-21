using System;
using System.Text;
using Newtonsoft.Json;
using RabbitMQ.Client.Events;
using Tkn.Queuer.Common;
using Tkn.Queuer.Interface;
using Tkn.Queuer.Models;

namespace Tkn.Queuer.RabbitMQ {
	public class RabbitConsumer<T> : RabbitBase<T>, IQueueConsumer<T> where T : BaseQueueModel, new() {
		public RabbitConsumer(QueueSettingsModel settings) : base(settings) {
			Model.BasicQos(0, 1, false);
		}

		public void Subscribe(string queue, Action<T> handler) {
			var consumer = new EventingBasicConsumer(Model);
			consumer.Received += (model, args) => {
				try {
					var message = Encoding.Default.GetString(args.Body);

					handler(convertFromJson(message));
					Model.BasicAck(args.DeliveryTag, false);
				} catch (QueueHandlerException ex) {
					Model.BasicReject(args.DeliveryTag, ex.Requeue);
				}
			};
			Model.BasicConsume(queue, false, consumer);
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