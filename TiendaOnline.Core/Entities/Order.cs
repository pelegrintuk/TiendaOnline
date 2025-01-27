using TiendaOnline.Core.Enums;


namespace TiendaOnline.Core.Entities
{
    public class Order
    {
        public int OrderId { get; set; }
        public DateTime Date { get; set; }
        public OrderStatus Status { get; set; }
        public string UserId { get; set; }
        public ApplicationUser User { get; set; }
        public ICollection<OrderProduct> OrderProducts { get; set; }
    }
}


