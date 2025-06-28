using Sistema.Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sistema.Web.Interfaces
{
    public interface ISalidasService
    {
        Task<IEnumerable<Salidas>> LeerSalidas(string id);          
        Task<IEnumerable<Tiendas>> LeerTiendas(string ciudad, string entidad);
        Task<IEnumerable<Empleados>> LeerDespachador();
        Task<IEnumerable<Empleados>> LeerRevisor();
        Task<IEnumerable<Empleados>> LeerTransportador();
        Task<IEnumerable<Vehiculos>> LeerVehiculos(string id);
        Task<IEnumerable<Ordenes>> LeerOrdenes(int id, int pk);
        Task<IEnumerable<Pedidos>> LeerPedidos(Tiendas tienda);
        IEnumerable<Pedidos> ValidarArticulos(Salidas salida);
        IEnumerable<Articulos> ValidarSalidas(Salidas salida);
        Task<IEnumerable<Usuarios>> LeerUsuarios(string entidad);
        Task<Articulos> BuscarArticulo(string codigo, string prepack, int semana, int seccion, int pedido, int tienda);
        Task<Semanas> ObtenerSemana(string pk);
        Task<Usuarios> InicioSesion(string usuario, string entidad);
        void CrearSalida(Salidas sal);
        
    }
}
