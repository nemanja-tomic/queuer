namespace Tkn.Queuer.Interface {
	public interface IQueueLogger {
		void Info(string message);
		void Error(string message);
	}
}