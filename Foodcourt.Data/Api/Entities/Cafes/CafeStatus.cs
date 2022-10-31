namespace Foodcourt.Data.Api.Entities.Cafes;

public enum CafeStatus
{
    /// <summary>
    /// Кафе создано
    /// </summary>
    Created,
    /// <summary>
    /// Запрос на регестрацию кафе утвержден
    /// </summary>
    Approved,
    /// <summary>
    /// Запрос на регестрацию кафе отклонен
    /// </summary>
    Rejected,
}