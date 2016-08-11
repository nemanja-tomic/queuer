using System.Collections.Generic;

namespace Tkn.Queuer.Models {
	public class QueueSettingsModel {
		public string Hostname { get; set; }
		public List<string> Groups { get; set; }
		public string Username { get; set; }
		public string Password { get; set; }
		public int Port { get; set; }
		public string ApplicationName { get; set; }

		public QueueSettingsModel() {
			Groups = new List<string>();
		}
	}
}