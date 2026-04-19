namespace Purchase_Orders.Application.IUOW
{
    public interface IUnitOfWork
    {
        Task SaveChangesAsync();
    }
}