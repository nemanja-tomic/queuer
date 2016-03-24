using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using RabbitMQ.Client;
using Tkn.Queuer.Common;
using Tkn.Queuer.Interface;
using Tkn.Queuer.Models;

namespace Tkn.Queuer.RabbitMQ {
	public abstract class RabbitBase<T> : IQueuer where T : BaseQueueModel {
		ConnectionFactory _connectionFactory;
		List<RabbitConnection> _connections;

		internal List<RabbitConnection> Connections {
			get { return _connections ?? (_connections = new List<RabbitConnection>()); }
			set { _connections = value; }
		}

		protected readonly JsonSerializerSettings JsonSettings;
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
			foreach (var rabbitConnection in Connections) {
				rabbitConnection.Connection.Close();
				if (rabbitConnection.Model.IsOpen) {
					rabbitConnection.Model.Abort();
				}
			}
			_connectionFactory = null;

			GC.SuppressFinalize(this);
		}

		void initializeRabbit() {
			_connectionFactory = new ConnectionFactory {
				HostName = QueueSettings.Hostname,
				UserName = QueueSettings.Username,
				Password = QueueSettings.Password
			};
			if (QueueSettings.Port > 0)
				_connectionFactory.Port = QueueSettings.Port;
			_connectionFactory.AutomaticRecoveryEnabled = true;

			if (!QueueSettings.Groups.Any())
				QueueSettings.Groups.Add("/");

			foreach (var virtualHost in QueueSettings.Groups) {
				_connectionFactory.VirtualHost = virtualHost;
				var connection = _connectionFactory.CreateConnection();
				Connections.Add(new RabbitConnection {
					Model = connection.CreateModel(),
					VirtualHost = virtualHost,
					Connection = connection
				});
			}
		}
	}
}