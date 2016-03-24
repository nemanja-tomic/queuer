using System;
using Tkn.Queuer.Models;

namespace Tkn.Queuer.Interface {
	public interface IQueueConsumer<out T> : IQueuer where T : BaseQueueModel {
		void Subscribe(string queue, Action<T> handler);
		void Subscribe(string queue, Action<T> handler, string virtualHost);
	}
}