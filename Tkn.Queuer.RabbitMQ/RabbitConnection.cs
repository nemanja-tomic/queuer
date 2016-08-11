using RabbitMQ.Client;

namespace Tkn.Queuer.RabbitMQ {
	internal class RabbitConnection {
		public string VirtualHost { get; set; }
		public IConnection Connection { get; set; }
		public IModel Model { get; set; }
	}
}