using System;

namespace Tkn.Queuer.Exceptions {
	public class QueuerException : Exception {
		public QueuerException() { }

		public QueuerException(string message) : base(message) { }

		public QueuerException(string message, Exception innerException) : base(message, innerException) { }
	}
}