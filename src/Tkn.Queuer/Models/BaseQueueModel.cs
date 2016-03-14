namespace Tkn.Queuer.Models {
	public class BaseQueueModel {
		public string Tid { get; set; }
		public bool IsValid { get; set; } = false;
	}
}