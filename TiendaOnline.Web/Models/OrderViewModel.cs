using System;
using System.Collections.Generic;

namespace TiendaOnline.Web.Models
{
    public class OrderViewModel
    {
        public int Id { get; set; }                // Identificador del pedido
        public DateTime Date { get; set; }         // Fecha del pedido
        public string Status { get; set; }         // Estado del pedido (e.g., Pendiente, Completado)
        public List<OrderItemViewModel> Items { get; set; } // Detalles del pedido
    }
}
