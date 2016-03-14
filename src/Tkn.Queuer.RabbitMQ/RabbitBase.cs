using System;
using Newtonsoft.Json;
using RabbitMQ.Client;
using Tkn.Queuer.Common;
using Tkn.Queuer.Interface;
using Tkn.Queuer.Models;

namespace Tkn.Queuer.RabbitMQ {
	public abstract class RabbitBase<T> : IQueuer where T : BaseQueueModel {
		IConnection _connection;
		ConnectionFactory _connectionFactory;

		protected readonly JsonSerializerSettings JsonSettings;
		protected IModel Model;
		protected QueueSettingsModel QueueSettings;

		protected RabbitBase(QueueSettingsModel settings) {
			QueueSettings = settings;

			JsonSettings = new JsonSerializerSettings {
				ContractResolver = new GenericPropertyContractResolver(typeof(T)),
				Formatting = Formatting.Indented
			};

			initializeRabbit();
		}

		public void Dispose() {
			_connection?.Close();

			if ((Model != null) && Model.IsOpen)
				Model.Abort();

			_connectionFactory = null;

			GC.SuppressFinalize(this);
		}

		void initializeRabbit() {
			_connectionFactory = new ConnectionFactory {
				HostName = QueueSettings.Hostname,
				UserName = QueueSettings.Username,
				Password = QueueSettings.Password
			};
			if (!string.IsNullOrEmpty(QueueSettings.VirtualHost))
				_connectionFactory.VirtualHost = QueueSettings.VirtualHost;
			if (QueueSettings.Port > 0)
				_connectionFactory.Port = QueueSettings.Port;

			_connection = _connectionFactory.CreateConnection();
			Model = _connection.CreateModel();
		}
	}
}