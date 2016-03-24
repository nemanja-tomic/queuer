using Tkn.Queuer.Models;

namespace Tkn.Queuer.Interface {
	public interface IQueuePublisher<in T> : IQueuer where T : BaseQueueModel {
		void Send(string exchange, string routingKey, T model);
		void Send(string exchange, string routingKey, T model, string virtualHost);
	}
}