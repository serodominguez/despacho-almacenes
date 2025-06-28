using Sistema.Web.Interfaces;
using Sistema.Web.Models;
using Microsoft.Extensions.Configuration;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Transactions;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.IO;

namespace Sistema.Web.Services
{
    public class SalidasService : ISalidasService
    {
        public readonly string _connectionString;


        string seccion;
        string tipo;

        public SalidasService(IConfiguration _configuration)
        {
            _connectionString = _configuration.GetConnectionString("OracleDBConnection");
        }

        public void CrearSalida(Salidas salida)
        {

            int i;
            int index;
            int sequence;
            int count = 1;
            int number = 1;
            string bulto = "CAJ";
            seccion = salida.PK_SECCION.ToString();
            string fecha = DateTime.Now.ToString("dd/MM/yyyy");
            using (OracleConnection con = new OracleConnection(_connectionString))
            {
                con.Open();

                using (OracleCommand cmd = new OracleCommand())
                {
                    using (OracleTransaction transaction = con.BeginTransaction())
                    {
                        cmd.Connection = con;
                        cmd.Transaction = transaction;
                        cmd.CommandText = "SELECT COUNT(*) FROM PED_ENT WHERE PK_ORDEN_PEDIDO = '"+ salida.PK_PEDIDO  +"' AND PK_ENTIDAD = '"+ salida.PK_ENTIDAD +"' AND ESTADO = 'D'";
                        if (Convert.ToInt32(cmd.ExecuteScalar()) == 0)
                        {
                            try
                            {
                                ObtenerNumero(out int numero);
                                ObtenerSecuencia(out int secuencia);
                                cmd.CommandText = "INSERT INTO DOCUMENTOS (pk_documento, pk_seccion, pk_tipo_doc, pk_tipo_mov, nro_doc, pk_semana, fecha_doc, pk_entidad, estado, pk_emp_des, pk_emp_rev, pk_tipo_tra, pk_emp_tra, nombre_tra, placa, pk_usuario, fecha, obs, pk_concepto, es_ppack ) VALUES ('" + secuencia + "','" + salida.PK_SECCION + "','" + salida.PK_TIPO_DOC + "','" + salida.PK_TIPO_MOV + "','" + numero + "','" + salida.PK_SEMANA + "', TO_DATE('" + fecha + "', 'dd/mm/rrrr'),'" + salida.PK_ENTIDAD + "','" + salida.ESTADO + "','" + salida.PK_EMP_DES + "','" + salida.PK_EMP_REV + "','" + salida.PK_TIPO_TRA + "','" + salida.PK_EMP_TRA + "','" + salida.NOMBRE_TRA + "','" + salida.PLACA + "','" + salida.PK_USUARIO + "', TO_DATE('" + fecha + "', 'dd/mm/rrrr'),'" + salida.OBS + "','" + salida.PK_CONCEPTO + "','" + salida.ESS_PPACK + "')";
                                cmd.ExecuteNonQuery();

                                foreach (Detalle_Salidas filas in salida.detalles)
                                {
                                    cmd.CommandText = "SELECT fastr_verifica_dui('" + filas.PK_ARTICULO + "') FROM DUAL";
                                    var dui = cmd.ExecuteScalar().ToString();
                                    if (dui == "S")
                                    {
                                        string cadena = filas.PK_PPREPACK;
                                        cadena = cadena.TrimStart(new Char[] { '0' });
                                        i = Convert.ToInt32(filas.CPREPACKS);

                                        while (i >= 1)
                                        {
                                            index = count++;
                                            cmd.CommandText = "INSERT INTO DETALLES_DOC (pk_documento, pk_secuencia, pk_canal, pk_articulo, pk_calidad, pk_plan, pk_entidad, pk_marca, pk_categoria, pk_subcategoria, pventa, pcosto, cb, tam1, tam2, tam3, tam4, tam5, tam6, tam7, tam8, tam9, pk_ppack) VALUES ('" + secuencia + "','" + index + "','" + filas.PK_CANAL + "','" + filas.PK_ARTICULO + "','" + filas.PK_CALIDAD + "','" + filas.PK_PLAN + "','" + salida.PK_ENTIDAD + "','" + filas.PK_MARCA + "','" + filas.PK_CATEGORIA + "','" + filas.PK_SUBCATEGORIA + "','" + filas.PVENTA.Replace(",", ".") + "','" + filas.PCOSTO.Replace(",", ".") + "','" + filas.CB + "','" + filas.TAM1 + "','" + filas.TAM2 + "','" + filas.TAM3 + "','" + filas.TAM4 + "','" + filas.TAM5 + "','" + filas.TAM6 + "','" + filas.TAM7 + "','" + filas.TAM8 + "','" + filas.TAM9 + "', '" + filas.PK_PPREPACK + "')";
                                            cmd.ExecuteNonQuery();
                                            i--;

                                            cmd.CommandText = "UPDATE STOCK SET tam1 = tam1 - '" + filas.TAM1 + "', tam2 = tam2 - '" + filas.TAM2 + "', tam3 = tam3 - '" + filas.TAM3 + "', tam4 = tam4 - '" + filas.TAM4 + "', tam5 = tam5 - '" + filas.TAM5 + "', tam6 = tam6 - '" + filas.TAM6 + "', tam7 = tam7 - '" + filas.TAM7 + "', tam8 = tam8 - '" + filas.TAM8 + "', tam9 = tam9 - '" + filas.TAM9 + "' WHERE pk_seccion = '" + salida.PK_SECCION + "' AND pk_canal = '" + filas.PK_CANAL + "' AND pk_articulo = '" + filas.PK_ARTICULO + "' AND pk_calidad = '" + filas.PK_CALIDAD + "' AND pk_plan = '" + filas.PK_PLAN + "' AND pk_ppack = '" + cadena + "'";
                                            if (cmd.ExecuteNonQuery() == 0)
                                            {
                                                throw new Exception();
                                            }
                                        }

                                        sequence = number++;
                                        cmd.CommandText = "UPDATE STOCK_PPACK SET cant = cant - '" + filas.CPREPACKS + "' WHERE pk_seccion = '" + salida.PK_SECCION + "'  AND pk_canal = '" + filas.PK_CANAL + "' AND pk_articulo = '" + filas.PK_ARTICULO + "' AND pk_plan = '" + 1 + "' AND pk_ppack ='" + cadena + "'";
                                        cmd.ExecuteNonQuery();

                                        cmd.CommandText = "INSERT INTO DETALLES_DPPACK (pk_documento, pk_secuencia, pk_canal, pk_articulo, pk_ppack, pk_plan, pk_entidad, cb, cant) VALUES ('" + secuencia + "', '" + sequence + "', '" + filas.PK_CANAL + "', '" + filas.PK_ARTICULO + "', '" + cadena + "', '" + filas.PK_PLAN + "', '" + salida.PK_ENTIDAD + "', '" + filas.CB + "', '" + filas.CPREPACKS + "')";
                                        cmd.ExecuteNonQuery();
                                    }

                                }

                                foreach (Ordenes cola in salida.ordenes)
                                {

                                    cmd.CommandText = "INSERT INTO ORD_DOC (pk_orden_ped, pk_documento) VALUES ('" + cola.PK_ORDEN_PED + "', '" + secuencia + "')";
                                    cmd.ExecuteNonQuery();

                                    cmd.CommandText = "UPDATE PED_ENT SET estado = 'D' WHERE  pk_orden_pedido = '" + cola.PK_ORDEN_PED + "' AND pk_entidad = '" + salida.PK_ENTIDAD + "' AND estado  in ('AS','OP')";
                                    cmd.ExecuteNonQuery();
                                }

                                cmd.CommandText = "UPDATE NUMERACIONES n SET n.actual=actual + 1 WHERE  n.activo = 'S' AND n.pk_tipo_doc = '3' AND n.pk_entidad = '" + salida.PK_SECCION + "'";
                                cmd.ExecuteNonQuery();

                                cmd.CommandText = "INSERT INTO BULTOS (pk_documento, pk_tipo_bul, pk_bulto, cantidad) VALUES ('" + secuencia + "', '" + bulto + "', '" + salida.TOTAL_PREPACKS + "', '" + salida.TOTAL_PARES + "' )";
                                cmd.ExecuteNonQuery();
                                transaction.Commit();
                                con.Close();
                            }
                            catch (Exception e)
                            {
                                transaction.Rollback();
                                string ruta = ("C:\\Users\\sdominguez_adm\\Documents\\Error" + salida.PK_ENTIDAD + ".txt");
                                using (StreamWriter sw = new StreamWriter(ruta))
                                {
                                    sw.WriteLine(e.ToString());
                                }
                                throw e;
                            }
                        }
                    }
                }
            }
        }

        public async Task<IEnumerable<Salidas>> LeerSalidas(string id)
        {
            try
            {
                var semana = ObtenerSemana(id);
                string pk_semana = Cache.InicioCache.PK_SEMANA;
                List<Salidas> lista = new List<Salidas>();
                using (OracleConnection con = new OracleConnection(_connectionString))
                {
                    await Task.Run(() => {
                        using (OracleCommand cmd = new OracleCommand())
                        {
                            con.Open();
                            cmd.BindByName = true;
                            cmd.CommandText = @"SELECT a.pk_documento, a.nro_doc, a.tipo_salida, a.almacen, a.pk_semana, a.fecha_doc, a.tienda_almacen, b.prepack, b.pares, a.estado, d.login
                                            FROM (SELECT m.pk_usuario, m.pk_seccion almacen, tm.descr tipo_salida, m.pk_semana, m.pk_documento, m.nro_doc, TO_CHAR(m.fecha_doc,'DD/MM/RR') fecha_doc, m.estado, t.nombre tienda_almacen
                                            FROM DOCUMENTOS m, TIENDAS t, TIPOS_MOV tm
                                            WHERE m.pk_tipo_doc = '3'
                                            AND m.pk_tipo_mov = '0'
                                            AND m.pk_tipo_doc = tm.pk_tipo_doc
                                            AND m.pk_tipo_mov = tm.pk_tipo_mov
                                            AND m.estado <> 'A'
                                            AND m.es_ppack = 'S'
                                            AND m.pk_entidad = t.pk_tienda) a,
                                            (SELECT b.pk_documento, b.pk_bulto as prepack, b.cantidad as pares FROM bultos b, documentos d WHERE d.pk_documento =  b.pk_documento) b,
                                            (SELECT u.pk_usuario, u.login FROM usuarios u, documentos d WHERE d.pk_usuario = u.pk_usuario GROUP BY u.pk_usuario, u.login) d
                                                WHERE  a.pk_documento = b.pk_documento(+) 
                                             AND a.pk_usuario = d.pk_usuario(+) AND
                                             pk_semana = '" + pk_semana + "' AND almacen = '" + id + "' ORDER BY nro_doc DESC";
                            cmd.Connection = con;
                            OracleDataReader rdr = cmd.ExecuteReader();
                            while (rdr.Read())
                            {
                                Salidas salidas = new Salidas();
                                salidas.PK_DOCUMENTO = Convert.ToInt32(rdr["PK_DOCUMENTO"]);
                                salidas.NRO_DOC = rdr["NRO_DOC"].ToString();
                                salidas.TIPO_SALIDA = rdr["TIPO_SALIDA"].ToString();
                                salidas.PK_ENTIDAD = rdr["ALMACEN"].ToString();
                                salidas.PK_SEMANA = rdr["PK_SEMANA"].ToString();
                                salidas.FECHA_DOC = rdr["FECHA_DOC"].ToString();
                                salidas.NOMBRE = rdr["TIENDA_ALMACEN"].ToString();
                                salidas.TOTAL_PREPACKS = Convert.ToInt32(rdr["PREPACK"]);
                                salidas.TOTAL_PARES = Convert.ToInt32(rdr["PARES"]);
                                salidas.ESTADO = rdr["ESTADO"].ToString();
                                salidas.LOGIN = rdr["LOGIN"].ToString();
                                lista.Add(salidas);
                            }
                        }
                        con.Close();
                    }); 
                }
                return lista;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<IEnumerable<Tiendas>> LeerTiendas(string ciudad, string entidad)
        {
            try
            {
                switch (entidad)
                {
                    case "510":
                        tipo = "BAT";
                        break;
                    case "511":
                        tipo = "BAT";
                        break;
                    case "520":
                        tipo = "MAN";
                        break;
                    case "521":
                        tipo = "MAN";
                        break;
                    case "530":
                        tipo = "PP";
                        break;
                    case "531":
                        tipo = "PP";
                        break;
                    case "509":
                        tipo = "BG";
                        break;
                    case "631":
                        tipo = "CAT";
                        break;
                }

                List<Tiendas> lista = new List<Tiendas>();
                using (OracleConnection con = new OracleConnection(_connectionString))
                {
                    await Task.Run(() => {
                        using (OracleCommand cmd = new OracleCommand())
                        {
                            con.Open();
                            cmd.BindByName = true;

                            switch (ciudad)
                            {
                                case "La Paz":
                                    cmd.CommandText = @"SELECT pk_tienda, pk_tipo_tda, nombre FROM (SELECT * FROM TIENDAS WHERE pk_tipo_tda ='" + tipo + "') WHERE pk_tienda >= 21000 AND pk_tienda <= 21999 ORDER BY pk_tienda";
                                    cmd.Connection = con;
                                    OracleDataReader rdrlp = cmd.ExecuteReader();
                                    while (rdrlp.Read())
                                    {
                                        Tiendas tiendas = new Tiendas();
                                        tiendas.PK_TIENDA = Convert.ToInt32(rdrlp["PK_TIENDA"]);
                                        tiendas.PK_TIPO_TDA = rdrlp["PK_TIPO_TDA"].ToString();
                                        tiendas.NOMBRE = rdrlp["NOMBRE"].ToString();
                                        lista.Add(tiendas);
                                    }
                                    break;

                                case "Cochabamba":
                                    cmd.CommandText = @"SELECT pk_tienda, pk_tipo_tda, nombre FROM (SELECT * FROM TIENDAS WHERE pk_tipo_tda ='" + tipo + "') WHERE pk_tienda >= 22000 AND pk_tienda <= 22999 ORDER BY pk_tienda";
                                    cmd.Connection = con;
                                    OracleDataReader rdrcb = cmd.ExecuteReader();
                                    while (rdrcb.Read())
                                    {
                                        Tiendas tiendas = new Tiendas();
                                        tiendas.PK_TIENDA = Convert.ToInt32(rdrcb["PK_TIENDA"]);
                                        tiendas.PK_TIPO_TDA = rdrcb["PK_TIPO_TDA"].ToString();
                                        tiendas.NOMBRE = rdrcb["NOMBRE"].ToString();
                                        lista.Add(tiendas);
                                    }
                                    break;

                                case "Santa Cruz":
                                    cmd.CommandText = @"SELECT pk_tienda, pk_tipo_tda, nombre FROM (SELECT * FROM TIENDAS WHERE pk_tipo_tda ='" + tipo + "') WHERE pk_tienda >= 23000 AND pk_tienda <= 23999 ORDER BY pk_tienda";
                                    cmd.Connection = con;
                                    OracleDataReader rdrsc = cmd.ExecuteReader();
                                    while (rdrsc.Read())
                                    {
                                        Tiendas tiendas = new Tiendas();
                                        tiendas.PK_TIENDA = Convert.ToInt32(rdrsc["PK_TIENDA"]);
                                        tiendas.PK_TIPO_TDA = rdrsc["PK_TIPO_TDA"].ToString();
                                        tiendas.NOMBRE = rdrsc["NOMBRE"].ToString();
                                        lista.Add(tiendas);
                                    }
                                    break;

                                case "Oruro":
                                    cmd.CommandText = @"SELECT pk_tienda, pk_tipo_tda, nombre FROM (SELECT * FROM TIENDAS WHERE pk_tipo_tda ='" + tipo + "') WHERE pk_tienda >= 24000 AND pk_tienda <= 24999 ORDER BY pk_tienda";
                                    cmd.Connection = con;
                                    OracleDataReader rdror = cmd.ExecuteReader();
                                    while (rdror.Read())
                                    {
                                        Tiendas tiendas = new Tiendas();
                                        tiendas.PK_TIENDA = Convert.ToInt32(rdror["PK_TIENDA"]);
                                        tiendas.PK_TIPO_TDA = rdror["PK_TIPO_TDA"].ToString();
                                        tiendas.NOMBRE = rdror["NOMBRE"].ToString();
                                        lista.Add(tiendas);
                                    }
                                    break;

                                case "Potosi":
                                    cmd.CommandText = @"SELECT pk_tienda, pk_tipo_tda, nombre FROM (SELECT * FROM TIENDAS WHERE pk_tipo_tda ='" + tipo + "') WHERE pk_tienda >= 25000 AND pk_tienda <= 25999 ORDER BY pk_tienda";
                                    cmd.Connection = con;
                                    OracleDataReader rdrpt = cmd.ExecuteReader();
                                    while (rdrpt.Read())
                                    {
                                        Tiendas tiendas = new Tiendas();
                                        tiendas.PK_TIENDA = Convert.ToInt32(rdrpt["PK_TIENDA"]);
                                        tiendas.PK_TIPO_TDA = rdrpt["PK_TIPO_TDA"].ToString();
                                        tiendas.NOMBRE = rdrpt["NOMBRE"].ToString();
                                        lista.Add(tiendas);
                                    }
                                    break;

                                case "Sucre":
                                    cmd.CommandText = @"SELECT pk_tienda, pk_tipo_tda, nombre FROM (SELECT * FROM TIENDAS WHERE pk_tipo_tda ='" + tipo + "') WHERE pk_tienda >= 26000 AND pk_tienda <= 26999 ORDER BY pk_tienda";
                                    cmd.Connection = con;
                                    OracleDataReader rdrch = cmd.ExecuteReader();
                                    while (rdrch.Read())
                                    {
                                        Tiendas tiendas = new Tiendas();
                                        tiendas.PK_TIENDA = Convert.ToInt32(rdrch["PK_TIENDA"]);
                                        tiendas.PK_TIPO_TDA = rdrch["PK_TIPO_TDA"].ToString();
                                        tiendas.NOMBRE = rdrch["NOMBRE"].ToString();
                                        lista.Add(tiendas);
                                    }
                                    break;

                                case "Tarija":
                                    cmd.CommandText = @"SELECT pk_tienda, pk_tipo_tda, nombre FROM (SELECT * FROM TIENDAS WHERE pk_tipo_tda ='" + tipo + "') WHERE pk_tienda >= 27000 AND pk_tienda <= 27999 ORDER BY pk_tienda";
                                    cmd.Connection = con;
                                    OracleDataReader rdrtj = cmd.ExecuteReader();
                                    while (rdrtj.Read())
                                    {
                                        Tiendas tiendas = new Tiendas();
                                        tiendas.PK_TIENDA = Convert.ToInt32(rdrtj["PK_TIENDA"]);
                                        tiendas.PK_TIPO_TDA = rdrtj["PK_TIPO_TDA"].ToString();
                                        tiendas.NOMBRE = rdrtj["NOMBRE"].ToString();
                                        lista.Add(tiendas);
                                    }
                                    break;

                                case "Beni":
                                    cmd.CommandText = @"SELECT pk_tienda, pk_tipo_tda, nombre FROM (SELECT * FROM TIENDAS WHERE pk_tipo_tda ='" + tipo + "') WHERE pk_tienda >= 28000 AND pk_tienda <= 28999 ORDER BY pk_tienda";
                                    cmd.Connection = con;
                                    OracleDataReader rdrbn = cmd.ExecuteReader();
                                    while (rdrbn.Read())
                                    {
                                        Tiendas tiendas = new Tiendas();
                                        tiendas.PK_TIENDA = Convert.ToInt32(rdrbn["PK_TIENDA"]);
                                        tiendas.PK_TIPO_TDA = rdrbn["PK_TIPO_TDA"].ToString();
                                        tiendas.NOMBRE = rdrbn["NOMBRE"].ToString();
                                        lista.Add(tiendas);
                                    }
                                    break;

                                case "Pando":
                                    cmd.CommandText = @"SELECT pk_tienda, pk_tipo_tda, nombre FROM (SELECT * FROM TIENDAS WHERE pk_tipo_tda ='" + tipo + "') WHERE pk_tienda >= 29000 AND pk_tienda <= 29999 ORDER BY pk_tienda";
                                    cmd.Connection = con;
                                    OracleDataReader rdrpn = cmd.ExecuteReader();
                                    while (rdrpn.Read())
                                    {
                                        Tiendas tiendas = new Tiendas();
                                        tiendas.PK_TIENDA = Convert.ToInt32(rdrpn["PK_TIENDA"]);
                                        tiendas.PK_TIPO_TDA = rdrpn["PK_TIPO_TDA"].ToString();
                                        tiendas.NOMBRE = rdrpn["NOMBRE"].ToString();
                                        lista.Add(tiendas);
                                    }
                                    break;
                            }
                        }
                        con.Close();

                    });   
                }
                return lista;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<IEnumerable<Empleados>> LeerDespachador()
        {
            try
            {
                List<Empleados> lista = new List<Empleados>();
                using (OracleConnection con = new OracleConnection(_connectionString))
                {
                    await Task.Run(() => {
                        using (OracleCommand cmd = new OracleCommand())
                        {
                            con.Open();
                            cmd.BindByName = true;
                            cmd.CommandText = @"SELECT e.nro_doc, (e.nombres||' '||e.paterno||' '||e.materno) as empleado
                                                FROM TEM_EMP te, empleados e WHERE te.pk_tipo_emp = 'D'
                                                AND te.pk_empleado = e.pk_empleado AND e.activo = 'S' ORDER BY empleado";
                            cmd.Connection = con;
                            OracleDataReader rdr = cmd.ExecuteReader();
                            while (rdr.Read())
                            {
                                Empleados empleados = new Empleados();
                                empleados.NRO_DOC = rdr["NRO_DOC"].ToString();
                                empleados.EMPLEADO = rdr["EMPLEADO"].ToString();
                                lista.Add(empleados);
                            }
                        }
                        con.Close();
                    });
                }
                return lista;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<IEnumerable<Empleados>> LeerRevisor() 
        {
            try
            {
                List<Empleados> lista = new List<Empleados>();
                using (OracleConnection con = new OracleConnection(_connectionString))
                {
                    await Task.Run(() =>
                    {
                        using (OracleCommand cmd = new OracleCommand())
                        {
                            con.Open();
                            cmd.BindByName = true;
                            cmd.CommandText = @"SELECT e.nro_doc, (e.nombres||' '||e.paterno||' '||e.materno) as empleado
                                                FROM TEM_EMP te, empleados e WHERE te.pk_tipo_emp = 'R'
                                                AND te.pk_empleado = e.pk_empleado AND e.activo = 'S' ORDER BY empleado";
                            cmd.Connection = con;
                            OracleDataReader rdr = cmd.ExecuteReader();
                            while (rdr.Read())
                            {
                                Empleados empleados = new Empleados();
                                empleados.NRO_DOC = rdr["NRO_DOC"].ToString();
                                empleados.EMPLEADO = rdr["EMPLEADO"].ToString();
                                lista.Add(empleados);
                            }
                        }
                        con.Close();
                    });


                }
                return lista;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<IEnumerable<Empleados>> LeerTransportador()
        {
            try
            {
                List<Empleados> lista = new List<Empleados>();
                using (OracleConnection con = new OracleConnection(_connectionString))
                {
                    await Task.Run(() => {
                        using (OracleCommand cmd = new OracleCommand())
                        {
                            con.Open();
                            cmd.BindByName = true;
                            cmd.CommandText = @"SELECT e.nro_doc, (e.nombres||' '||e.paterno||' '||e.materno) as empleado
                                                FROM TEM_EMP te, empleados e WHERE te.pk_tipo_emp = 'T'
                                                AND te.pk_empleado = e.pk_empleado AND e.activo = 'S' ORDER BY empleado";
                            cmd.Connection = con;
                            OracleDataReader rdr = cmd.ExecuteReader();
                            while (rdr.Read())
                            {
                                Empleados empleados = new Empleados();
                                if (rdr["NRO_DOC"] != DBNull.Value)
                                    empleados.NRO_DOC = rdr["NRO_DOC"].ToString();
                                empleados.EMPLEADO = rdr["EMPLEADO"].ToString();
                                lista.Add(empleados);
                            }
                        }
                        con.Close();
                    });
                }
                return lista;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<IEnumerable<Vehiculos>> LeerVehiculos(string id)
        {
            try
            {
                List<Vehiculos> lista = new List<Vehiculos>();
                using (OracleConnection con = new OracleConnection(_connectionString))
                {
                    await Task.Run(() => {
                        using (OracleCommand cmd = new OracleCommand())
                        {
                            con.Open();
                            cmd.BindByName = true;
                            cmd.CommandText = @"SELECT pk_empleado, placa FROM VEHICULOS WHERE pk_empleado = '" + id + "' AND activo = 'S'";
                            cmd.Connection = con;
                            OracleDataReader rdr = cmd.ExecuteReader();
                            while (rdr.Read())
                            {
                                Vehiculos vehiculo = new Vehiculos();
                                vehiculo.PK_EMPLEADO = rdr["PK_EMPLEADO"].ToString();
                                vehiculo.PLACA = rdr["PLACA"].ToString();
                                lista.Add(vehiculo);
                            }
                        }
                        con.Close();
                    });
                }
                return lista;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<IEnumerable<Ordenes>> LeerOrdenes(int id, int pk)
        {
            try
            {
                List<Ordenes> lista = new List<Ordenes>();

                if (pk == 510)
                {
                    using (OracleConnection con = new OracleConnection(_connectionString))
                    {
                        await Task.Run(() =>
                        {
                            using (OracleCommand cmd = new OracleCommand())
                            {
                                con.Open();
                                cmd.BindByName = true;
                                cmd.CommandText = @"SELECT op.pk_orden_ped, TO_NUMBER(op.nro_orden) nro_orden, op.pk_semana, op.obs FROM ordenes_ped op, 
                                                    (select distinct p.pk_orden_ped,p.pk_entidad, pe.estado, pe.pk_entidad tienda from pedidos p, ped_ent pe where p.pk_pedido = pe.pk_pedido) p WHERE  p.tienda LIKE '" + id + "' AND op.tipo = 'IMP' AND p.estado in ('AS','OP') AND op.pk_orden_ped = p.pk_orden_ped ORDER BY op.nro_orden";
                                cmd.Connection = con;
                                OracleDataReader rdr = cmd.ExecuteReader();
                                while (rdr.Read())
                                {
                                    Ordenes ordenes = new Ordenes();
                                    ordenes.PK_ORDEN_PED = Convert.ToInt32(rdr["PK_ORDEN_PED"]);
                                    ordenes.NRO_ORDEN = Convert.ToInt32(rdr["NRO_ORDEN"]);
                                    ordenes.PK_SEMANA = Convert.ToInt32(rdr["PK_SEMANA"]);
                                    ordenes.OBS = rdr["OBS"].ToString();
                                    lista.Add(ordenes);
                                }
                            }
                            con.Close();
                        });
                    }
                }
                else
                {
                    using (OracleConnection con = new OracleConnection(_connectionString))
                    {
                        await Task.Run(() =>
                        {
                            using (OracleCommand cmd = new OracleCommand())
                            {
                                con.Open();
                                cmd.BindByName = true;
                                cmd.CommandText = @"SELECT op.pk_orden_ped, TO_NUMBER(op.nro_orden) nro_orden, op.pk_semana, op.obs FROM ordenes_ped op, 
                                                    (select distinct p.pk_orden_ped,p.pk_entidad, pe.estado, pe.pk_entidad tienda from pedidos p, ped_ent pe where p.pk_pedido = pe.pk_pedido) p WHERE  p.tienda LIKE '" + id + "' AND p.estado in ('AS','OP') AND op.pk_orden_ped = p.pk_orden_ped ORDER BY op.nro_orden";
                                cmd.Connection = con;
                                OracleDataReader rdr = cmd.ExecuteReader();
                                while (rdr.Read())
                                {
                                    Ordenes ordenes = new Ordenes();
                                    ordenes.PK_ORDEN_PED = Convert.ToInt32(rdr["PK_ORDEN_PED"]);
                                    ordenes.NRO_ORDEN = Convert.ToInt32(rdr["NRO_ORDEN"]);
                                    ordenes.PK_SEMANA = Convert.ToInt32(rdr["PK_SEMANA"]);
                                    ordenes.OBS = rdr["OBS"].ToString();
                                    lista.Add(ordenes);
                                }
                            }
                            con.Close();
                        });
                    }
                }
                return lista;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<IEnumerable<Pedidos>> LeerPedidos(Tiendas tienda)
        {
            try
            {
                List<Pedidos> lista = new List<Pedidos>();
                using (OracleConnection con = new OracleConnection(_connectionString))
                {
                    await Task.Run(() =>{
                        using (OracleCommand cmd = new OracleCommand())
                        {
                            con.Open();
                            cmd.BindByName = true;
                            foreach (Ordenes filas in tienda.ordenes)
                            {
                                cmd.CommandText = @"SELECT p.pk_orden_ped, d.pk_articulo,d.pk_ppack, d.tam1,d.tam2,d.tam3,d.tam4,d.tam5,d.tam6,d.tam7,d.tam8,d.tam9, d.cant_pack, (d.tam1+d.tam2+d.tam3+d.tam4+d.tam5+d.tam6+d.tam7+d.tam8+d.tam9) AS total_pares
                                                        FROM PEDIDOS p INNER JOIN DETALLES_PED d ON p.pk_pedido = d.pk_pedido WHERE p.pk_orden_ped = '" + filas.PK_ORDEN_PED + "' AND d.pk_entidad = '" + tienda.PK_TIENDA + "'";
                                cmd.Connection = con;
                                OracleDataReader rdr = cmd.ExecuteReader();
                                while (rdr.Read())
                                {
                                    Pedidos pedidos = new Pedidos();
                                    pedidos.PK_PEDIDO = rdr["PK_ORDEN_PED"].ToString();
                                    pedidos.PK_ARTICULO = rdr["PK_ARTICULO"].ToString();
                                    pedidos.PK_PPACK = rdr["PK_PPACK"].ToString();
                                    pedidos.TAM1 = Convert.ToInt32(rdr["TAM1"]);
                                    pedidos.TAM2 = Convert.ToInt32(rdr["TAM2"]);
                                    pedidos.TAM3 = Convert.ToInt32(rdr["TAM3"]);
                                    pedidos.TAM4 = Convert.ToInt32(rdr["TAM4"]);
                                    pedidos.TAM5 = Convert.ToInt32(rdr["TAM5"]);
                                    pedidos.TAM6 = Convert.ToInt32(rdr["TAM6"]);
                                    pedidos.TAM7 = Convert.ToInt32(rdr["TAM7"]);
                                    pedidos.TAM8 = Convert.ToInt32(rdr["TAM8"]);
                                    pedidos.TAM9 = Convert.ToInt32(rdr["TAM9"]);
                                    pedidos.CANT_PACK = Convert.ToInt32(rdr["CANT_PACK"]);
                                    pedidos.TOTAL_PARES = Convert.ToInt32(rdr["TOTAL_PARES"]);
                                    lista.Add(pedidos);
                                }
                            }
                        }
                        con.Close();
                    });
                }
                return lista;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<IEnumerable<Usuarios>> LeerUsuarios(string entidad)
        {

            List<Usuarios> lista = new List<Usuarios>();
            using (OracleConnection con = new OracleConnection(_connectionString))
            {
                await Task.Run(() =>
                {
                    using (OracleCommand cmd = new OracleCommand())
                    {
                        con.Open();
                        cmd.BindByName = true;
                        cmd.CommandText = @"SELECT u.login FROM usuarios u, usr_ent e WHERE e.pk_usuario = u.pk_usuario AND u.activo = 'S' AND e.PK_ENTIDAD = '" + entidad + "'";
                        cmd.Connection = con;
                        OracleDataReader rdr = cmd.ExecuteReader();
                        while (rdr.Read())
                        {
                            Usuarios usuario = new Usuarios();
                            usuario.LOGIN = rdr["LOGIN"].ToString();
                            lista.Add(usuario);
                        }
                    }
                    con.Close();

                });
            }
            return lista;
        }

        public IEnumerable<Pedidos> ValidarArticulos(Salidas salida)
        {
            List<Pedidos> lista = new List<Pedidos>();
            using (OracleConnection con = new OracleConnection(_connectionString))
            {
                using (OracleCommand cmd = new OracleCommand())
                {
                    con.Open();
                    cmd.BindByName = true;
                    cmd.Connection = con;
                    foreach (Ordenes filas in salida.ordenes)
                    {
                        foreach (Detalle_Salidas detalle in salida.detalles)
                        {
                            cmd.CommandText = "SELECT fastr_verifica_dui('" + detalle.PK_ARTICULO + "') FROM DUAL";
                            var dui = cmd.ExecuteScalar().ToString();
                            if (dui == "S")
                            {
                                cmd.CommandText = @"SELECT p.pk_orden_ped, d.pk_articulo, d.pk_ppack, d.tam1,d.tam2,d.tam3,d.tam4,d.tam5,d.tam6,d.tam7,d.tam8,d.tam9, d.cant_pack, (d.tam1+d.tam2+d.tam3+d.tam4+d.tam5+d.tam6+d.tam7+d.tam8+d.tam9) AS total_pares
                                                        FROM PEDIDOS p INNER JOIN DETALLES_PED d ON p.pk_pedido = d.pk_pedido WHERE p.pk_orden_ped = '" + filas.PK_ORDEN_PED + "' AND d.pk_entidad = '" + salida.PK_ENTIDAD + "' AND d.pk_articulo = '" + detalle.PK_ARTICULO + "' AND d.pk_ppack = '" + detalle.PK_PPREPACK + "'";
                                OracleDataReader rdr = cmd.ExecuteReader();
                                if (rdr.HasRows)
                                {
                                    while (rdr.Read())
                                    {
                                        int c = Convert.ToInt32(rdr["CANT_PACK"]);

                                        if (detalle.CPREPACKS != c)
                                        {
                                            Pedidos pedidos = new Pedidos();
                                            pedidos.PK_ARTICULO = rdr["PK_ARTICULO"].ToString() + rdr["PK_PPACK"].ToString() + "/" + rdr["CANT_PACK"].ToString();
                                            lista.Add(pedidos);
                                        }
                                    }
                                }
                            }
                            else
                            {
                                Pedidos pedidos = new Pedidos();
                                pedidos.PK_ARTICULO = detalle.PK_ARTICULO + detalle.PK_PPREPACK + "SIN DUI";
                                lista.Add(pedidos);
                            }
                        }
                    }
                }
                con.Close();
            }
            return lista;
        }

        public IEnumerable<Articulos> ValidarSalidas(Salidas salida)
        {
            List<Articulos> lista = new List<Articulos>();
            using (OracleConnection con = new OracleConnection(_connectionString))
            {
                using (OracleCommand cmd = new OracleCommand())
                {
                    con.Open();
                    cmd.BindByName = true;
                    foreach (Ordenes filas in salida.ordenes)
                    {
                        foreach (Detalle_Salidas detalle in salida.detalles)
                        {
                            cmd.CommandText = @"SELECT  d.pk_articulo, (d.tam1+d.tam2+d.tam3+d.tam4+d.tam5+d.tam6+d.tam7+d.tam8+d.tam9) AS total_pares
                                                        FROM PEDIDOS p INNER JOIN DETALLES_PED d ON p.pk_pedido = d.pk_pedido WHERE p.pk_orden_ped = '" + filas.PK_ORDEN_PED + "' AND d.pk_entidad = '" + salida.PK_ENTIDAD + "' AND d.pk_articulo = '" + detalle.PK_ARTICULO + "' AND d.pk_ppack = '" + detalle.PK_PPREPACK + "'";
                            cmd.Connection = con;

                            OracleDataReader rdr = cmd.ExecuteReader();

                            if (rdr.HasRows)
                            {
                                while (rdr.Read())
                                {
                                    Articulos articulos = new Articulos();
                                    if (detalle.CPARES > Convert.ToInt32(rdr["TOTAL_PARES"]))
                                    {
                                        articulos.PK_ARTICULO = rdr["PK_ARTICULO"].ToString();
                                        articulos.CPARES = Convert.ToInt32(rdr["TOTAL_PARES"]);
                                        lista.Add(articulos);
                                    }
                                }
                            }
    
                        }
                    }
                }
                con.Close();
            }
            return lista;
        }

        public async Task<Articulos> BuscarArticulo(string codigo, string prepack, int semana, int seccion, int pedido, int tienda)
        {
            try
            {
                string cadena = prepack;
                cadena = cadena.TrimStart(new Char[] { '0' });
                //var semana = GenerarSemana();
                //string pk_semana = Cache.InicioCache.PK_SEMANA;
                Articulos articulos = new Articulos();
                using (OracleConnection con = new OracleConnection(_connectionString))
                {
                    await Task.Run(() => {
                        using (OracleCommand cmd = new OracleCommand())
                        {
                            con.Open();
                            cmd.BindByName = true;
                            cmd.Connection = con;
                            cmd.CommandText = @"SELECT p.pk_orden_ped, d.pk_articulo,d.pk_ppack, d.tam1,d.tam2,d.tam3,d.tam4,d.tam5,d.tam6,d.tam7,d.tam8,d.tam9, d.cant_pack, (d.tam1+d.tam2+d.tam3+d.tam4+d.tam5+d.tam6+d.tam7+d.tam8+d.tam9) AS total_pares FROM PEDIDOS p INNER JOIN DETALLES_PED d ON p.pk_pedido = d.pk_pedido WHERE p.pk_orden_ped = '"+ pedido +"' AND d.pk_entidad = '"+ tienda +"' AND d.pk_articulo = '"+ codigo +"' AND d.pk_ppack = '"+ cadena +"'";
                            if (Convert.ToInt32(cmd.ExecuteScalar()) > 0)
                            {
                                cmd.CommandText = @"SELECT pk_articulo, pk_marca, pk_categoria, pk_subcategoria, pk_entidad, cb from ARTICULOS WHERE pk_articulo ='" + codigo + "'";
                                OracleDataReader rdr1 = cmd.ExecuteReader();
                                while (rdr1.Read())
                                {
                                    articulos.PK_ARTICULO = rdr1["PK_ARTICULO"].ToString();
                                    articulos.PK_MARCA = rdr1["PK_MARCA"].ToString();
                                    articulos.PK_CATEGORIA = rdr1["PK_CATEGORIA"].ToString();
                                    articulos.PK_SUBCATEGORIA = rdr1["PK_SUBCATEGORIA"].ToString();
                                    articulos.PK_ENTIDAD = rdr1["PK_ENTIDAD"].ToString();
                                    articulos.CB = rdr1["CB"].ToString();
                                }

                                if (articulos.PK_ARTICULO != null)
                                {
                                    cmd.CommandText = @"SELECT prepack_code, pairs_qty, size_sort10, size_sort20, size_sort30, size_sort40, size_sort50, size_sort60, size_sort70, size_sort80, size_sort90 FROM COINS3.val_prepack WHERE prepack_code='" + cadena + "'";
                                    OracleDataReader rdr2 = cmd.ExecuteReader();
                                    while (rdr2.Read())
                                    {
                                        articulos.CPARES = Convert.ToInt32(rdr2["PAIRS_QTY"]);
                                        articulos.PK_PPREPACK = rdr2["PREPACK_CODE"].ToString();
                                        articulos.TAM1 = rdr2["SIZE_SORT10"].ToString();
                                        articulos.TAM2 = rdr2["SIZE_SORT20"].ToString();
                                        articulos.TAM3 = rdr2["SIZE_SORT30"].ToString();
                                        articulos.TAM4 = rdr2["SIZE_SORT40"].ToString();
                                        articulos.TAM5 = rdr2["SIZE_SORT50"].ToString();
                                        articulos.TAM6 = rdr2["SIZE_SORT60"].ToString();
                                        articulos.TAM7 = rdr2["SIZE_SORT70"].ToString();
                                        articulos.TAM8 = rdr2["SIZE_SORT80"].ToString();
                                        articulos.TAM9 = rdr2["SIZE_SORT90"].ToString();
                                    }

                                    cmd.CommandText = @"SELECT precio, costo FROM ( select fanum_precios('" + semana + "' , '" + codigo + "', 1, 'P') as precio from dual) a, (select fanum_precios('" + semana + "', '" + codigo + "', 1, 'C') as costo from dual) b";
                                    OracleDataReader rdr3 = cmd.ExecuteReader();
                                    while (rdr3.Read())
                                    {
                                        articulos.PVENTA = rdr3["PRECIO"].ToString();
                                        articulos.PCOSTO = rdr3["COSTO"].ToString();

                                    }

                                    cmd.CommandText = @"SELECT pk_canal, pk_plan FROM (SELECT s.pk_canal, s.pk_articulo, s.pk_calidad, s.pventa, s.pcosto, s.pk_plan, s.pk_marca, s.pk_categoria, s.pk_subcategoria, s.tam1 + s.tam2 + s.tam3 + s.tam4 + s.tam5 + s.tam6 + s.tam7 + s.tam8 + s.tam9 cantidad, s.tam1,s.tam2,s.tam3,s.tam4,s.tam5,s.tam6,s.tam7,s.tam8,s.tam9 
                                                    FROM stock s WHERE s.pk_seccion = '" + seccion + "' AND s.pk_articulo = '" + codigo + "' AND s.pk_calidad = 1 AND s.pk_ppack = '" + cadena + "' AND(s.tam1 <> 0 OR s.tam2 <> 0 OR s.tam3 <> 0 OR s.tam4 <> 0 OR s.tam5 <> 0 OR s.tam6 <> 0 OR s.tam7 <> 0 OR s.tam8 <> 0 OR s.tam9 <> 0) ORDER BY s.pk_plan) WHERE rownum = 1";
                                    OracleDataReader rdr4 = cmd.ExecuteReader();
                                    while (rdr4.Read())
                                    {
                                        articulos.PK_CANAL = rdr4["PK_CANAL"].ToString();
                                        articulos.PK_PLAN = rdr4["PK_PLAN"].ToString();
                                    }
                                }
                            }
                        }
                        con.Close();
                    });
                    return articulos;
                }
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public async Task<Usuarios> InicioSesion(string usuario, string entidad) 
        {
            try
            {
                Usuarios usuarios = new Usuarios();
                using (OracleConnection con = new OracleConnection(_connectionString))
                {
                    await Task.Run(() =>{
                        using (OracleCommand cmd = new OracleCommand())
                        {
                            con.Open();
                            cmd.BindByName = true;
                            cmd.CommandText = @"SELECT u.pk_usuario, u.login, e.pk_entidad FROM usuarios u, usr_ent e 
                                                WHERE e.pk_usuario = u.pk_usuario AND u.login = '" + usuario + "' AND u.activo = 'S' AND e.PK_ENTIDAD = '" + entidad + "'";
                            cmd.Connection = con;
                            OracleDataReader rdr = cmd.ExecuteReader();
                            while (rdr.Read())
                            {
                                usuarios.PK_USUARIO = Convert.ToInt32(rdr["PK_USUARIO"]);
                                usuarios.LOGIN = rdr["LOGIN"].ToString();
                                usuarios.PK_ENTIDAD = rdr["PK_ENTIDAD"].ToString();
                            }
                        }
                        con.Close();

                    }); 
                }
                return usuarios;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<Semanas> ObtenerSemana(string pk)
        {
            try
            {
                //string hoy= DateTime.Now.ToString("dd/MM/yyyy");
                Semanas semana = new Semanas();
                using (OracleConnection con = new OracleConnection(_connectionString))
                {
                    await Task.Run(() =>{
                        using (OracleCommand cmd = new OracleCommand())
                        {
                            con.Open();
                            cmd.BindByName = true;
                            cmd.CommandText = @"SELECT PK_SEMANA FROM SECCIONES WHERE PK_SECCION = '"+ pk +"'";
                            cmd.Connection = con;
                            OracleDataReader rdr = cmd.ExecuteReader();
                            while (rdr.Read())
                            {
                                semana.PK_SEMANA = rdr["PK_SEMANA"].ToString();
                            }
                        }
                        con.Close();
                    });
                }
                return semana;
            }
            catch (Exception)
            {
                throw;
            }
        }
        public async Task<Semanas> GenerarSemana()
        {
            try
            {
                string hoy = DateTime.Now.ToString("dd/MM/yyyy");
                Semanas semana = new Semanas();
                using (OracleConnection con = new OracleConnection(_connectionString))
                {
                    await Task.Run(() => {
                        using (OracleCommand cmd = new OracleCommand())
                        {
                            con.Open();
                            cmd.BindByName = true;
                            cmd.CommandText = @"SELECT pk_semana, inicio, fin FROM SEMANAS WHERE TO_DATE('" + hoy + "', 'dd/mm/rrrr') BETWEEN TO_DATE (inicio, 'dd/mm/rrrr') AND TO_DATE (fin, 'dd/mm/rrrr')";
                            cmd.Connection = con;
                            OracleDataReader rdr = cmd.ExecuteReader();
                            while (rdr.Read())
                            {
                                semana.PK_SEMANA = rdr["PK_SEMANA"].ToString();
                            }
                        }
                        con.Close();
                    });
                }
                return semana;
            }
            catch (Exception)
            {
                throw;
            }
        }

        private void ObtenerSecuencia(out int secuencia)
        {
            try
            {
                secuencia = 0;
                using (OracleConnection con = new OracleConnection(_connectionString))
                {
                    using (OracleCommand cmd = new OracleCommand())
                    {
                        con.Open();
                        cmd.BindByName = true;
                        cmd.CommandText = @"SELECT seq_doc.nextval FROM dual";
                        cmd.Connection = con;
                        OracleDataReader rdr = cmd.ExecuteReader();
                        while (rdr.Read())
                        {
                            secuencia = Convert.ToInt32(rdr[rdr.GetOrdinal("NEXTVAL")]);
                        }
                    }
                    con.Close();
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        private void ObtenerNumero(out int numero)
        {
            try
            {
                numero = 0;
                using (OracleConnection con = new OracleConnection(_connectionString))
                {
                    using (OracleCommand cmd = new OracleCommand())
                    {
                        con.Open();
                        cmd.BindByName = true;
                        cmd.CommandText = @"SELECT actual FROM numeraciones WHERE activo = 'S' AND pk_tipo_doc = '3' AND pk_entidad = '" + seccion + "'";
                        cmd.Connection = con;
                        OracleDataReader rdr = cmd.ExecuteReader();
                        while (rdr.Read())
                        {
                            numero = Convert.ToInt32(rdr[rdr.GetOrdinal("actual")]);
                        }
                    }
                    con.Close();
                }
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
