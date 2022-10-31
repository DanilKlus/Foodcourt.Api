namespace Foodcourt.Data.Api.Entities.Orders;

public enum OrderStatus
{
    /// <summary>
    /// Заказ создан
    /// </summary>
    Created,
    /// <summary>
    /// Заказ в очереди на готовку
    /// </summary>
    InQueue,
    /// <summary>
    /// Заказ готовят
    /// </summary>
    InWork,
    /// <summary>
    /// Заказ готов к выдаче
    /// </summary>
    Ready,
    /// <summary>
    /// Заказ выдан
    /// </summary>
    Issued,
    /// <summary>
    /// Заказ отменен
    /// </summary>
    Cancelled
}