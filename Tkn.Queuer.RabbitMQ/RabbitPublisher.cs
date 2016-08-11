using System.Linq;
using System.Text;
using Newtonsoft.Json;
using RabbitMQ.Client;
using Tkn.Queuer.Exceptions;
using Tkn.Queuer.Interface;
using Tkn.Queuer.Models;

namespace Tkn.Queuer.RabbitMQ {
	public class RabbitPublisher<T> : RabbitBase<T>, IQueuePublisher<T> where T : BaseQueueModel {
		public RabbitPublisher(QueueSettingsModel settings) : base(settings) { }

		public void Send(string exchange, string routingKey, T model) {
			if (!Connections.Any())
				throw new QueuerException("Couldn't find active RabbitMQ connections, check QueueSettings.");
			if (Connections.Count > 1)
				throw new QueuerException("There are more than one active RabbitMQ connections, please add target virtual host to QueueSettings.");

			var rabbitModel = Connections.First().Model;

			sendToExchange(exchange, routingKey, model, rabbitModel);
		}

		public void Send(string exchange, string routingKey, T model, string virtualHost) {
			var connection = Connections.FirstOrDefault(x => x.VirtualHost == virtualHost);
			if (connection == null)
				throw new QueuerException($"There are no active connections for virtual host [{virtualHost}], please check QueueSettings.");

			sendToExchange(exchange, routingKey, model, connection.Model);
		}

		void sendToExchange(string exchange, string routingKey, T model, IModel rabbitModel) {
			var properties = rabbitModel.CreateBasicProperties();
			properties.Persistent = true;

			var json = convertToJson(model);
			var messageBuffer = Encoding.Default.GetBytes(json);
			rabbitModel.BasicPublish(exchange, routingKey, properties, messageBuffer);
		}

		string convertToJson(T model) {
			return JsonConvert.SerializeObject(model, JsonSettings);
		}
	}
}