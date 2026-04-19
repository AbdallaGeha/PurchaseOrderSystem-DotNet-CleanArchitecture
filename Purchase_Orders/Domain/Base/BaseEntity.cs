namespace Purchase_Orders.Domain.Base
{
    public abstract class BaseEntity
    {
        public Guid Id { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime LastModifiedDate { get; set; }
    }
}
