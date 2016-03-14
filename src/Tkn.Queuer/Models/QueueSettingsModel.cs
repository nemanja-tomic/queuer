namespace Tkn.Queuer.Models {
	public class QueueSettingsModel {
		public string Hostname { get; set; }
		public string VirtualHost { get; set; }
		public string Username { get; set; }
		public string Password { get; set; }
		public int Port { get; set; }
		public string ApplicationName { get; set; }
	}
}