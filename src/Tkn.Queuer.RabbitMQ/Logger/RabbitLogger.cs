using System.Text;
using RabbitMQ.Client;
using Tkn.Queuer.Interface;
using Tkn.Queuer.Models;

namespace Tkn.Queuer.RabbitMQ.Logger {
	public class RabbitLogger : IQueueLogger {
		readonly QueueSettingsModel _queueSettings;
		IConnection _connection;
		ConnectionFactory _connectionFactory;
		IModel _model;

		#region Constructors

		public RabbitLogger(QueueSettingsModel settings) {
			_queueSettings = settings;

			initializeRabbit();
		}

		#endregion

		#region Interface implementation

		public void Info(string message) {
			send(LoggerConstants.INFO, message);
		}

		public void Error(string message) {
			send(LoggerConstants.ERROR, message);
		}

		#endregion

		void initializeRabbit() {
			_connectionFactory = new ConnectionFactory {
				HostName = _queueSettings.Hostname,
				UserName = _queueSettings.Username,
				Password = _queueSettings.Password,
				VirtualHost = LoggerConstants.VIRTUAL_HOST
			};
			if (_queueSettings.Port > 0)
				_connectionFactory.Port = _queueSettings.Port;

			_connection = _connectionFactory.CreateConnection();
			_model = _connection.CreateModel();
		}

		void send(string level, string message) {
			var properties = _model.CreateBasicProperties();
			properties.Persistent = true;

			var messageBuffer = Encoding.Default.GetBytes(message);
			_model.BasicPublish(LoggerConstants.EXCHANGE_NAME, $"{_queueSettings.ApplicationName}.{level}", properties, messageBuffer);
		}
	}
}