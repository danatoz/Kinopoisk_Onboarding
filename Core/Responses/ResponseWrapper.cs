using Core.Entities.Enums;

namespace Core.Responses;

public class ResponseWrapper<T>
{
    public OperationStatus OperationStatus { get; set; }

    public T ResponseData { get; set; }

    public ResponseWrapper()
    {
    }

    public ResponseWrapper(OperationStatus operationStatus)
    {
        OperationStatus = operationStatus;
    }
}