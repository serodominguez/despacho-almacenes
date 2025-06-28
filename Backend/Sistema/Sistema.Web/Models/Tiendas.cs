using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sistema.Web.Models
{
    public class Tiendas
    {
        public int PK_TIENDA { get; set; }
        public string PK_TIPO_TDA { get; set; }
        public string NOMBRE { get; set; }

        public List<Ordenes> ordenes { get; set; }
    }
}
