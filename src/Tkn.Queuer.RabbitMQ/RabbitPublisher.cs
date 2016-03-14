using System.Text;
using Newtonsoft.Json;
using Tkn.Queuer.Interface;
using Tkn.Queuer.Models;

namespace Tkn.Queuer.RabbitMQ {
	public class RabbitPublisher<T> : RabbitBase<T>, IQueuePublisher<T> where T : BaseQueueModel {
		public RabbitPublisher(QueueSettingsModel settings) : base(settings) { }

		public void Send(string exchange, string routingKey, T model) {
			var properties = Model.CreateBasicProperties();
			properties.Persistent = true;

			var json = convertToJson(model);
			var messageBuffer = Encoding.Default.GetBytes(json);
			Model.BasicPublish(exchange, routingKey, properties, messageBuffer);
		}

		string convertToJson(T model) {
			return JsonConvert.SerializeObject(model, JsonSettings);
		}
	}
}