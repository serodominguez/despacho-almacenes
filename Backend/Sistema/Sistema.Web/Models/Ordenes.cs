using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sistema.Web.Models
{
    public class Ordenes
    {
        public int PK_ORDEN_PED { get; set; }
        public int NRO_ORDEN { get; set; }
        public int PK_SEMANA { get; set; }
        public string OBS { get; set; }
        public string ESTADO { get; set; }
    }
}
