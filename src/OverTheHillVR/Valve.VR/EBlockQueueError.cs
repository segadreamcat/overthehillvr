namespace Valve.VR;

public enum EBlockQueueError
{
	None,
	QueueAlreadyExists,
	QueueNotFound,
	BlockNotAvailable,
	InvalidHandle,
	InvalidParam,
	ParamMismatch,
	InternalError,
	AlreadyInitialized,
	OperationIsServerOnly,
	TooManyConnections
}
