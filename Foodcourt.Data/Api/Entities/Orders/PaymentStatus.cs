namespace Foodcourt.Data.Api.Entities.Orders;

public enum PaymentStatus
{
    /// <summary>
    /// Оплата не начата
    /// </summary>
    Created,
    /// <summary>
    /// Оплата в процессе
    /// </summary>
    InProgress,
    /// <summary>
    /// Заказ оплачен
    /// </summary>
    Paid,
    /// <summary>
    /// Ошибка оплаты
    /// </summary>
    Failed,
    /// <summary>
    /// Платеж отменен
    /// </summary>
    Cancelled,
}