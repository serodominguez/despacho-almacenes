using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sistema.Web.Models
{
    public class Salidas
    {
        public int PK_DOCUMENTO { get; set; }
        public string PK_SECCION { get; set; }
        public string PK_TIPO_DOC { get; set; }
        public string PK_TIPO_MOV { get; set; }
        public string NRO_DOC { get; set; }
        public string PK_SEMANA { get; set; }
        public string FECHA_DOC { get; set; }
        public string PK_ENTIDAD { get; set; }
        public string ESTADO { get; set; }
        public string PK_EMP_DES { get; set; }
        public string PK_EMP_REV { get; set; }
        public string PK_EMP_EMB { get; set; }
        public string PK_TIPO_TRA { get; set; }
        public string PK_EMP_TRA { get; set; }
        public string NOMBRE_TRA { get; set; }
        public string PLACA { get; set; }
        public string PK_USUARIO { get; set; }
        public string FECHA { get; set; }
        public string OBS { get; set; }
        public string PK_CONCEPTO { get; set; }
        public string ESS_PPACK { get; set; }
        public string PK_PEDIDO { get; set; }

        public List<Detalle_Salidas> detalles { get; set; }
        public List<Ordenes> ordenes { get; set; }


        public int TOTAL_PREPACKS { get; set; }
        public int TOTAL_PARES { get; set; }
        public int TOTAL_PVENTA { get; set; }


        public string NOMBRE { get; set; }
        public string TIPO_SALIDA { get; set; }
        public string LOGIN { get; set; }
    }
}
