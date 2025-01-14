using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TiendaOnline.Core.Enums;
using TiendaOnline.Core.Entities;


namespace TiendaOnline.Core.Entities
{
    public class Order
    {
        public int OrderId { get; set; }
        public DateTime Date { get; set; }
        public OrderStatus Status { get; set; } // Cambiado de string a OrderStatus
        public required string UserId { get; set; }
        public required ApplicationUser User { get; set; } // Relación con usuarios
        public required ICollection<OrderProduct> OrderProducts { get; set; } // Relación con productos
    }
}


