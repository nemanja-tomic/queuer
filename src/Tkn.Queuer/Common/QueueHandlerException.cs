using System;

namespace Tkn.Queuer.Common
{
    public class QueueHandlerException : Exception
    {
	    public bool Requeue { get; set; }

	    public QueueHandlerException(bool requeue) {
		    Requeue = requeue;
	    }

	    public QueueHandlerException(bool requeue, string message) : base(message) {
		    Requeue = requeue;
	    }

	    public QueueHandlerException(bool requeue, string message, Exception innerException) : base(message, innerException) {
		    Requeue = requeue;
	    }
    }
}
