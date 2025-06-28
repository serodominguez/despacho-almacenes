using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sistema.Web.Models
{
    public class Pedidos
    {
        public string PK_PEDIDO { get; set; }
        public string PK_ARTICULO { get; set; }
        public string PK_PPACK { get; set; }
        public int TAM1 { get; set; }
        public int TAM2 { get; set; }
        public int TAM3 { get; set; }
        public int TAM4 { get; set; }
        public int TAM5 { get; set; }
        public int TAM6 { get; set; }
        public int TAM7 { get; set; }
        public int TAM8 { get; set; }
        public int TAM9 { get; set; }
        public int CANT_PACK { get; set; }
        public int TOTAL_PARES { get; set; }
    }
}
