namespace Foodcourt.Data.Api.Entities.Orders;

public enum OrderStatus
{
    //TODO: add summary
    Created,
    InQueue,
    InWork,
    Ready,
    Issued,
    Cancelled
}