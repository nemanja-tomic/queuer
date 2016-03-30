using System.Text;
using RabbitMQ.Client;
using Tkn.Queuer.Interface;
using Tkn.Queuer.Models;
using Tkn.Queuer.Models.Enums;

namespace Tkn.Queuer.RabbitMQ.Logger {
	public class RabbitLogger : IQueueLogger {
		readonly QueueSettingsModel _queueSettings;
		IConnection _connection;
		ConnectionFactory _connectionFactory;
		IModel _model;
		readonly LogLevel _logLevel;

		#region Constructors

		public RabbitLogger(QueueSettingsModel settings) {
			_queueSettings = settings;

			initializeRabbit();
		}

		public RabbitLogger(LoggerQueueSettingsModel settings) : this(settings as QueueSettingsModel) {
			_logLevel = settings.LogLevel;
		}

		#endregion

		#region Interface implementation

		public void Debug(string message) {
			if (validLogLevel(LogLevel.Debug))
				send(LogLevel.Debug, message);
		}

		public void Info(string message) {
			if (validLogLevel(LogLevel.Info))
				send(LogLevel.Info, message);
		}

		public void Warn(string message) {
			if (validLogLevel(LogLevel.Warn))
				send(LogLevel.Warn, message);
		}

		public void Error(string message) {
			if (validLogLevel(LogLevel.Error))
				send(LogLevel.Error, message);
		}

		public void Fatal(string message) {
			if (validLogLevel(LogLevel.Fatal))
				send(LogLevel.Fatal, message);
		}

		#endregion

		bool validLogLevel(LogLevel level) {
			if (_logLevel == LogLevel.Off)
				return false;
			return (_logLevel == LogLevel.All) || (level <= _logLevel);
		}

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

		void send(LogLevel level, string message) {
			var properties = _model.CreateBasicProperties();
			properties.Persistent = true;

			var messageBuffer = Encoding.Default.GetBytes(message);
			_model.BasicPublish(
				LoggerConstants.EXCHANGE_NAME,
				$"{_queueSettings.ApplicationName}.{level.ToString().ToLower()}",
				properties,
				messageBuffer);
		}
	}
}