using Sistema.Web.Interfaces;
using Sistema.Web.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Security.Claims;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.IdentityModel.Tokens.Jwt;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Authorization;

namespace Sistema.Web.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class SalidasController : ControllerBase
    {
        ISalidasService sistemaService;

        private readonly IConfiguration config;

        public SalidasController(ISalidasService _sistemaService, IConfiguration _config)
        {
            sistemaService = _sistemaService;
            config = _config;
        }

        [Authorize(Roles = "administrador")]
        [HttpPost]
        public IActionResult CrearSalida(Salidas salida)
        {
            sistemaService.CrearSalida(salida);
            return Ok();
        }

        [Authorize(Roles = "administrador")]
        [HttpPost]
        public async Task<ActionResult> LeerPedidos(Tiendas tienda)
        {
            IEnumerable<Pedidos> pedidos = await Task.Run(() => sistemaService.LeerPedidos(tienda));
            return Ok(pedidos);
        }

        [Authorize(Roles = "administrador")]
        [HttpGet("{id}")]
        public async Task<ActionResult> Listar(string id)
        {
            IEnumerable<Salidas> salidas = await Task.Run(() => sistemaService.LeerSalidas(id));
            return Ok(salidas);
        }

        [Authorize(Roles = "administrador")]
        [HttpGet("{parametro}")]
        public async Task<ActionResult> CargarTienda(string parametro)
        {
            string cadena = parametro;
            char delimitador = ',';
            string[] valores = cadena.Split(delimitador);
            string ciudad = valores[0].ToString();
            string entidad = valores[1].ToString();

            IEnumerable<Tiendas> tiendas = await Task.Run (() => sistemaService.LeerTiendas(ciudad, entidad));
            return Ok(tiendas);
        }

        [Authorize(Roles = "administrador")]
        [HttpGet]
        public async Task<ActionResult> CargarDespachador()
        {
            IEnumerable<Empleados> empleados = await Task.Run(() => sistemaService.LeerDespachador());
            return Ok(empleados);
        }

        [Authorize(Roles = "administrador")]
        [HttpGet]
        public async Task<ActionResult> CargarRevisor()
        {
            IEnumerable<Empleados> empleados = await Task.Run(() => sistemaService.LeerRevisor());
            return Ok(empleados);
        }

        [Authorize(Roles = "administrador")]
        [HttpGet]
        public async Task<ActionResult> CargarTransportador()
        {
            IEnumerable<Empleados> empleados = await Task.Run(() => sistemaService.LeerTransportador());
            return Ok(empleados);
        }

        [Authorize(Roles = "administrador")]
        [HttpGet("{id}")]
        public async Task<ActionResult> CargarVehiculo(string id)
        {
            IEnumerable<Vehiculos> vehiculo = await Task.Run(() => sistemaService.LeerVehiculos(id));
            return Ok(vehiculo);
        }

        //[Authorize(Roles = "administrador")]
        [HttpGet("{id}/{pk}")]
        public async Task<ActionResult> CargarOrdenes(int id, int pk)
        {
            IEnumerable<Ordenes> pedido = await Task.Run(() => sistemaService.LeerOrdenes(id, pk));
            return Ok(pedido);
        }

        [Authorize(Roles = "administrador")]
        [HttpGet("{pk}")]
        public async Task<ActionResult> ObtenerSemana(string pk)
        {
            var semana = await Task.Run(() => sistemaService.ObtenerSemana(pk));
            return Ok(new Semanas
            {
                PK_SEMANA = semana.PK_SEMANA
            });
        }

        [Authorize(Roles = "administrador")]
        [HttpGet("{barra}/{semana}/{seccion}/{pedido}/{tienda}")]
        public async Task<ActionResult> BuscarArticulo(string barra, int semana, int seccion, int pedido, int tienda)
        {
            if (barra == "null")
            {
                return NotFound();
            }
            else
            {
                if (barra.Length > 8)
                {
                    string[] arreglo = barra.Split(',');
                    string codigo = arreglo[0];
                    string prepack = arreglo[1];

                    if (Regex.IsMatch(codigo, @"^[0-9]+$") && Regex.IsMatch(prepack, @"^[0-9]+$"))
                    {
                        var articulo = await Task.Run(() => sistemaService.BuscarArticulo(codigo, prepack, semana, seccion, pedido, tienda));

                        if (articulo.PK_ARTICULO != null && articulo.PK_PPREPACK != null)
                        {
                            if (articulo.PK_PLAN != null)
                            {
                                return Ok(new Articulos
                                {
                                    PK_ARTICULO = articulo.PK_ARTICULO,
                                    PK_PPREPACK = articulo.PK_PPREPACK,
                                    PK_MARCA = articulo.PK_MARCA,
                                    PK_CATEGORIA = articulo.PK_CATEGORIA,
                                    PK_SUBCATEGORIA = articulo.PK_SUBCATEGORIA,
                                    PK_ENTIDAD = articulo.PK_ENTIDAD,
                                    PK_PLAN = articulo.PK_PLAN,
                                    PK_CANAL = articulo.PK_CANAL,
                                    CB = articulo.CB,
                                    PVENTA = articulo.PVENTA,
                                    PCOSTO = articulo.PCOSTO,
                                    CPARES = articulo.CPARES,
                                    TAM1 = articulo.TAM1,
                                    TAM2 = articulo.TAM2,
                                    TAM3 = articulo.TAM3,
                                    TAM4 = articulo.TAM4,
                                    TAM5 = articulo.TAM5,
                                    TAM6 = articulo.TAM6,
                                    TAM7 = articulo.TAM7,
                                    TAM8 = articulo.TAM8,
                                    TAM9 = articulo.TAM9

                                });
                            }
                            else
                            {
                                return BadRequest();
                            }
                        }
                    }
                }
            }
            return NotFound();
        }

        [Authorize(Roles = "administrador")]
        [HttpPost]
        public async Task<ActionResult> ValidarArticulos(Salidas salida)
        {
            IEnumerable<Pedidos> pedidos = await Task.Run(() => sistemaService.ValidarArticulos(salida));
            return Ok(pedidos);
        }

        [Authorize(Roles = "administrador")]
        [HttpPost]
        public async Task<ActionResult> ValidarSalidas(Salidas salida)
        {
            IEnumerable<Articulos> articulos = await Task.Run(() => sistemaService.ValidarSalidas(salida));
            return Ok(articulos);
        }

        [HttpGet("{entidad}")]
        public async Task<ActionResult> CargarUsuario(string entidad)
        {
            IEnumerable<Usuarios> usuario = await Task.Run(() => sistemaService.LeerUsuarios(entidad));
            return Ok(usuario);
        }

        [HttpPost]
        public async Task<ActionResult> Inicio(Usuarios personal)
        {
            var semana = await Task.Run(() => sistemaService.ObtenerSemana(personal.PK_ENTIDAD));
            string rol = "administrador";
            string usuario = personal.LOGIN;
            string entidad = personal.PK_ENTIDAD;
            Cache.InicioCache.PK_ENTIDAD = personal.PK_ENTIDAD;
            Cache.InicioCache.PK_SEMANA = semana.PK_SEMANA;
            var usuarios = await Task.Run(() => sistemaService.InicioSesion(usuario, entidad));
            if (usuarios.PK_USUARIO == 0 || usuarios.LOGIN == null)
            {
                return NotFound();
            }

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, usuarios.PK_USUARIO.ToString()),
                new Claim(ClaimTypes.Name, usuarios.LOGIN.ToString()),
                new Claim(ClaimTypes.Role, rol),
                new Claim("pk_usuario", usuarios.PK_USUARIO.ToString()),
                new Claim("rol", rol),
                new Claim("usuario", usuarios.LOGIN),
                new Claim("entidad", usuarios.PK_ENTIDAD)
            };
            return Ok(
                new {token = GenerarToken(claims)}    
            );
        }

        private string GenerarToken(List<Claim> claims)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config["Jwt:Key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
              config["Jwt:Issuer"],
              config["Jwt:Issuer"],
              expires: DateTime.Now.AddMinutes(240),
              signingCredentials: creds,
              claims: claims);
            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
