using proyectoNegocioRflex.Utilidades;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace proyectoNegocioRflex.Modelo
{
    public class Marca
    {

        public DataTable traerMarcasPorTrabajadorYFecha(string persona_rut, string fechaInicio, string fechaTermino)
        {
            EjecutoresSql ejecutor = new EjecutoresSql();
            string sql = "select marca.idmarca,tipoMarca_idtipoMarca,marca.persona_rut,marca.empresa_idempresa," +
            "marca.sucursal_idsucursal, marca.imagenHuella_numeroDedo,marca.fecha_marca, marca.ingresador, marca.actualizador," +
            "marca.reloj_idreloj,marca.hash, marca.marcaConPin,tipoMarca.descripcion, reloj.nombre,marca.nombreEquipoEdicion,marca.incidencia " +
            "from marca,tipoMarca,reloj where " +
            "marca.persona_rut = '" + persona_rut +
            "' and marca.fecha_marca between '" + fechaInicio + "' and '" + fechaTermino + "'" +
            "  and marca.tipoMarca_idtipoMarca = tipoMarca.idtipoMarca and " +
            "marca.reloj_idreloj = reloj.idreloj";
            return ejecutor.traerDatosDataTable(sql);
        }

        public bool agregarMarca(int tipoMarca_idtipoMarca, string persona_rut, int empresa_idempresa, int sucursal_idsucursal,
            int imagenHuella_numeroDedo, string ingresador, string actualizador, int reloj_idreloj, int marcaConPin, int respaldado, string[] arregloFechaDeterminada)
        {


            //  *0 La hora
            //  *1 zona Horaria
            //  *2 si fue obtenida desde el servidor 1 para servidor 0 para local.

            string fechaSistema = arregloFechaDeterminada[0];
            string zonaHoraria = arregloFechaDeterminada[1];
            int horaServidor = int.Parse(arregloFechaDeterminada[2]);
            EjecutoresSql ejecutor = new EjecutoresSql();
            string sql;
            string checksum;

            int codigoMarca = int.Parse(traerUltimaMarcaEnRelojLocal() + reloj_idreloj.ToString());
            if (marcaConPin == 1)
            {
                checksum = generarHashMarca(fechaSistema, persona_rut, "pin", tipoMarca_idtipoMarca.ToString());
                sql = "insert into marca(tipoMarca_idtipoMarca, persona_rut, empresa_idempresa," +
                "sucursal_idsucursal,fecha_marca, ingresador,actualizador,reloj_idreloj,hash, marcaConPin,respaldado,nombreEquipoEdicion,zonaHoraria,esHoraServidor,codigoMarca) " +
                "values(" + tipoMarca_idtipoMarca + ",'" + persona_rut + "'," + empresa_idempresa +
                "," + sucursal_idsucursal + ",'" + fechaSistema + "','" + ingresador + "','" + actualizador +
                "'," + reloj_idreloj + ",'" + checksum + "'," + marcaConPin + "," + respaldado + ",'" + Environment.MachineName + "','" + zonaHoraria + "'," + horaServidor + "," + codigoMarca + ")";
            }
            else
            {
                checksum = generarHashMarca(fechaSistema, persona_rut, imagenHuella_numeroDedo.ToString(), tipoMarca_idtipoMarca.ToString());
                sql = "insert into marca(tipoMarca_idtipoMarca, persona_rut, empresa_idempresa," +
                "sucursal_idsucursal, imagenHuella_numeroDedo,fecha_marca, ingresador,actualizador,reloj_idreloj,hash, marcaConPin,respaldado,nombreEquipoEdicion,zonaHoraria,esHoraServidor,codigoMarca) " +
                "values(" + tipoMarca_idtipoMarca + ",'" + persona_rut + "'," + empresa_idempresa +
                "," + sucursal_idsucursal + "," + imagenHuella_numeroDedo + ",'" + fechaSistema + "','" + ingresador + "','" + actualizador +
                "'," + reloj_idreloj + ",'" + checksum + "'," + marcaConPin + "," + respaldado + ",'" + Environment.MachineName + "','" + zonaHoraria + "'," + horaServidor + "," + codigoMarca + ")";
            }
            return ejecutor.ejecutarConsulta(sql);
        }

        private string generarHashMarca(string fechaSistema, string persona_rut, string numeroDedo, string tipoMarca_idtipoMarca)
        {
            Encriptacion e = new Encriptacion();
            Herramientas h = new Herramientas();
            string fechaSistemaSinCaracteresEspeciales = h.quitarCaracteresEspecialesFechaParaHash(fechaSistema);
            string rutDesencriptado = e.Desencriptar(persona_rut);
            return e.Encriptar(fechaSistemaSinCaracteresEspeciales + rutDesencriptado + numeroDedo + tipoMarca_idtipoMarca);
        }

        public DataTable traerUltimaMarca(string persona_rut)
        {
            EjecutoresSql ejecutor = new EjecutoresSql();
            string sql = "select marca.idmarca,tipoMarca_idtipoMarca,marca.persona_rut,marca.empresa_idempresa," +
            "marca.sucursal_idsucursal, marca.imagenHuella_numeroDedo,marca.fecha_marca, marca.ingresador, marca.actualizador," +
            "marca.reloj_idreloj,marca.hash, marca.marcaConPin,tipoMarca.descripcion, reloj.nombre, marca.zonaHoraria,marca.esHoraServidor,marca.codigoMarca " +
            "from marca,tipoMarca,reloj where " +
            "marca.persona_rut = '" + persona_rut + "' and " +
            "marca.tipoMarca_idtipoMarca = tipoMarca.idtipoMarca and " +
            "marca.reloj_idreloj = reloj.idreloj  and marca.nombreEquipoEdicion='" + Environment.MachineName + "' " +
            "order by marca.idmarca desc limit 1;";
            return ejecutor.traerDatosDataTable(sql);
        }

        public bool actualizarEstadoNotificacion(int idmarca)
        {
            EjecutoresSql ejecutor = new EjecutoresSql();
            string sql = "update marca set notificada =1 where idmarca=" + idmarca + "";
            return ejecutor.ejecutarConsulta(sql);
        }

        public bool actualizarEstadoNotificacionNube(string hash)
        {
            EjecutoresSql ejecutor = new EjecutoresSql();
            string sql = "update marca set notificada =1 where hash='" + hash + "'";
            return ejecutor.ejecutarConsultaServidor(sql);
        }

        public DataTable traerMarcasAsistenciaSinRespaldar()
        {
            EjecutoresSql ejecutor = new EjecutoresSql();
            string sql = "select idmarca ,tipoMarca_idtipoMarca,persona_rut, " +
                         "empresa_idempresa,sucursal_idsucursal,  imagenHuella_numeroDedo, " +
                         "fecha_marca,ingresador, actualizador, " +
                         "reloj_idreloj, hash, marcaConPin, " +
                         "respaldado, notificada, nombreEquipoEdicion,zonaHoraria,esHoraServidor,incidencia,codigoMarca " +
                         "from marca where respaldado=0 and nombreEquipoEdicion= '" + Environment.MachineName + "' and tipoMarca_idtipoMarca <> " + ConfiguracionesConstantes.MARCA_CASINO + ";";
            return ejecutor.traerDatosDataTable(sql);
        }

        public DataTable traerMarcasCasinoSinRespaldar()
        {
            EjecutoresSql ejecutor = new EjecutoresSql();
            string sql = "select idmarca ,tipoMarca_idtipoMarca,persona_rut, " +
                         "empresa_idempresa,sucursal_idsucursal,  imagenHuella_numeroDedo, " +
                         "fecha_marca,ingresador, actualizador, " +
                         "reloj_idreloj, hash, marcaConPin, " +
                         "respaldado, notificada, nombreEquipoEdicion,zonaHoraria,esHoraServidor,incidencia,codigoMarca " +
                         "from marca where respaldado=0 and nombreEquipoEdicion= '" + Environment.MachineName + "' and tipoMarca_idtipoMarca = " + ConfiguracionesConstantes.MARCA_CASINO + ";";
            return ejecutor.traerDatosDataTable(sql);
        }

        public bool actualizarEstadoRespaldoMarca(int idmarca)
        {
            EjecutoresSql ejecutor = new EjecutoresSql();
            string sql = "update marca set respaldado =1 where idmarca=" + idmarca + "";
            return ejecutor.ejecutarConsulta(sql);
        }


        public bool agregarMarcaNube(int tipoMarca_idtipoMarca, string persona_rut, int empresa_idempresa, int sucursal_idsucursal,
         string imagenHuella_numeroDedo, string ingresador, string actualizador, int reloj_idreloj, int marcaConPin, string checksum, DateTime fechaSistema, int notificada, string zonaHoraria, int esHoraServidor, string incidencia, int codigoMarca)
        {
            Encriptacion e = new Encriptacion();
            Herramientas h = new Herramientas();
            EjecutoresSql ejecutor = new EjecutoresSql();
            string sql;
            string fecha = h.formatearFecha(fechaSistema, true);
            string ingresadorDecodificado = e.Desencriptar(ingresador);
            string rutTrabajadorDecodificado = e.Desencriptar(persona_rut);
            string rutActualizadorDecodificado = e.Desencriptar(actualizador);
            if (marcaConPin == 1)
            {
                if (codigoMarca == 0) //Hacemos esta comprobación por si que al momento de generar la marca y no se ha actualizado el valor de la columna codigoMarca no se caiga por tratar de mandar un valor null
                {
                    sql = "insert into marca(tipoMarca_idtipoMarca, persona_rut, empresa_idempresa," +
                    "sucursal_idsucursal,fecha_marca, ingresador,actualizador,reloj_idreloj,hash, marcaConPin,respaldado,nombreEquipoEdicion,notificada,zonaHoraria,esHoraServidor,incidencia) " +
                    "values(" + tipoMarca_idtipoMarca + ",'" + rutTrabajadorDecodificado + "'," + empresa_idempresa +
                    "," + sucursal_idsucursal + ",'" + fecha + "','" + ingresadorDecodificado + "','" + rutActualizadorDecodificado +
                    "'," + reloj_idreloj + ",'" + checksum + "'," + marcaConPin + ",1,'" + Environment.MachineName + "'," + notificada + ",'" + zonaHoraria + "'," + esHoraServidor + ",'" + incidencia + "')";
                }
                else
                {
                    sql = "insert into marca(tipoMarca_idtipoMarca, persona_rut, empresa_idempresa," +
                    "sucursal_idsucursal,fecha_marca, ingresador,actualizador,reloj_idreloj,hash, marcaConPin,respaldado,nombreEquipoEdicion,notificada,zonaHoraria,esHoraServidor,incidencia,codigoMarca) " +
                    "values(" + tipoMarca_idtipoMarca + ",'" + rutTrabajadorDecodificado + "'," + empresa_idempresa +
                    "," + sucursal_idsucursal + ",'" + fecha + "','" + ingresadorDecodificado + "','" + rutActualizadorDecodificado +
                    "'," + reloj_idreloj + ",'" + checksum + "'," + marcaConPin + ",1,'" + Environment.MachineName + "'," + notificada + ",'" + zonaHoraria + "'," + esHoraServidor + ",'" + incidencia + "'," + codigoMarca + ")";
                }

            }
            else
            {
                if (codigoMarca == 0)
                {
                    sql = "insert into marca(tipoMarca_idtipoMarca, persona_rut, empresa_idempresa," +
                    "sucursal_idsucursal, imagenHuella_numeroDedo,fecha_marca, ingresador,actualizador,reloj_idreloj,hash, marcaConPin,respaldado,nombreEquipoEdicion,notificada,zonaHoraria,esHoraServidor,incidencia) " +
                    "values(" + tipoMarca_idtipoMarca + ",'" + rutTrabajadorDecodificado + "'," + empresa_idempresa +
                    "," + sucursal_idsucursal + "," + int.Parse(imagenHuella_numeroDedo) + ",'" + fecha + "','" + ingresadorDecodificado + "','" + rutActualizadorDecodificado +
                    "'," + reloj_idreloj + ",'" + checksum + "'," + marcaConPin + ",1,'" + Environment.MachineName + "'," + notificada + ",'" + zonaHoraria + "'," + esHoraServidor + ",'" + incidencia + "')";
                }
                else
                {
                    sql = "insert into marca(tipoMarca_idtipoMarca, persona_rut, empresa_idempresa," +
                    "sucursal_idsucursal, imagenHuella_numeroDedo,fecha_marca, ingresador,actualizador,reloj_idreloj,hash, marcaConPin,respaldado,nombreEquipoEdicion,notificada,zonaHoraria,esHoraServidor,incidencia,codigoMarca) " +
                    "values(" + tipoMarca_idtipoMarca + ",'" + rutTrabajadorDecodificado + "'," + empresa_idempresa +
                    "," + sucursal_idsucursal + "," + int.Parse(imagenHuella_numeroDedo) + ",'" + fecha + "','" + ingresadorDecodificado + "','" + rutActualizadorDecodificado +
                    "'," + reloj_idreloj + ",'" + checksum + "'," + marcaConPin + ",1,'" + Environment.MachineName + "'," + notificada + ",'" + zonaHoraria + "'," + esHoraServidor + ",'" + incidencia + "'," + codigoMarca + ")";
                }
            }
            return ejecutor.ejecutarConsultaServidor(sql);
        }

        //Cada reloj se hará cargo de sus propias marcas sin notificar.
        public DataTable traerMarcasSinNotificar()
        {
            EjecutoresSql ejecutor = new EjecutoresSql();
            string sql = "select idmarca ,tipoMarca_idtipoMarca,persona_rut, " +
                         "empresa_idempresa,sucursal_idsucursal,  imagenHuella_numeroDedo, " +
                         "fecha_marca,ingresador, actualizador, " +
                         "reloj_idreloj, hash, marcaConPin, " +
                         "respaldado,  notificada, macRelojRegistro,incidencia " +
                         "from marca where notificada=0 and nombreEquipoEdicion= '" + Environment.MachineName + "'";
            return ejecutor.traerDatosDataTable(sql);
        }

        public DataTable traerDatosCompletosMarcasSinNotificar()
        {
            EjecutoresSql ejecutor = new EjecutoresSql();
            string sql = "select marca.idmarca,persona.rut, persona.nombre ,persona.segundoNombre,persona.apellidoPaterno,persona.apellidoMaterno,  persona.correo,empresa.rut_empresa, " +
                        "empresa.razon_social,marca.tipoMarca_idtipoMarca,tipoMarca.descripcion,  sucursal.nombre,sucursal.calle,sucursal.numero_calle, " +
                        "sucursal.comuna,sucursal.region,sucursal.pais,reloj.ubicacion,reloj.nombre,marca.hash,marca.fecha_marca,sucursal.ciudad,marca.incidencia, persona.empresa_idempresa " +
                        "from marca, persona,empresa,sucursal,reloj,tipoMarca " +
                        "where " +
                        "marca.notificada = 0 and  " +
                        "marca.nombreEquipoEdicion ='" + Environment.MachineName + "' and " +
                        "marca.persona_rut = persona.rut and " +
                        "marca.empresa_idempresa = empresa.idempresa and " +
                        "marca.sucursal_idsucursal = sucursal.idsucursal and " +
                        "marca.reloj_idreloj = reloj.idreloj and " +
                        "marca.tipoMarca_idtipoMarca = tipoMarca.idtipoMarca";
            return ejecutor.traerDatosDataTable(sql);
        }


        public DataTable traerDatosCompletosMarcasSinNotificarExcluyeCasino()
        {
            EjecutoresSql ejecutor = new EjecutoresSql();
            string sql = "select marca.idmarca,persona.rut, persona.nombre ,persona.segundoNombre,persona.apellidoPaterno,persona.apellidoMaterno,  persona.correo,empresa.rut_empresa, " +
                        "empresa.razon_social,marca.tipoMarca_idtipoMarca,tipoMarca.descripcion,  sucursal.nombre,sucursal.calle,sucursal.numero_calle, " +
                        "sucursal.comuna,sucursal.region,sucursal.pais,reloj.ubicacion,reloj.nombre,marca.hash,marca.fecha_marca,sucursal.ciudad,marca.incidencia, persona.empresa_idempresa " +
                        "from marca, persona,empresa,sucursal,reloj,tipoMarca " +
                        "where " +
                        "marca.notificada = 0 and  " +
                        "marca.nombreEquipoEdicion ='" + Environment.MachineName + "' and " +
                        "marca.persona_rut = persona.rut and " +
                        "marca.empresa_idempresa = empresa.idempresa and " +
                        "marca.sucursal_idsucursal = sucursal.idsucursal and " +
                        "marca.reloj_idreloj = reloj.idreloj and " +
                        "marca.tipoMarca_idtipoMarca = tipoMarca.idtipoMarca and " +
                        "marca.tipoMarca_idtipoMarca <> 5";
            return ejecutor.traerDatosDataTable(sql);
        }


        public DataTable traerDatosCompletosMarcasSinNotificarSoloCasino()
        {
            EjecutoresSql ejecutor = new EjecutoresSql();
            string sql = "select marca.idmarca,persona.rut, persona.nombre ,persona.segundoNombre,persona.apellidoPaterno,persona.apellidoMaterno,  persona.correo,empresa.rut_empresa, " +
                        "empresa.razon_social,marca.tipoMarca_idtipoMarca,tipoMarca.descripcion,  sucursal.nombre,sucursal.calle,sucursal.numero_calle, " +
                        "sucursal.comuna,sucursal.region,sucursal.pais,reloj.ubicacion,reloj.nombre,marca.hash,marca.fecha_marca,sucursal.ciudad,marca.incidencia, persona.empresa_idempresa " +
                        "from marca, persona,empresa,sucursal,reloj,tipoMarca " +
                        "where " +
                        "marca.notificada = 0 and  " +
                        "marca.nombreEquipoEdicion ='" + Environment.MachineName + "' and " +
                        "marca.persona_rut = persona.rut and " +
                        "marca.empresa_idempresa = empresa.idempresa and " +
                        "marca.sucursal_idsucursal = sucursal.idsucursal and " +
                        "marca.reloj_idreloj = reloj.idreloj and " +
                        "marca.tipoMarca_idtipoMarca = tipoMarca.idtipoMarca and " +
                        "marca.tipoMarca_idtipoMarca = 5";
            return ejecutor.traerDatosDataTable(sql);
        }



        public DataTable traerFechaUltimaMarcaLocal(string persona_rut)
        {
            EjecutoresSql ejecutor = new EjecutoresSql();
            string sql = "select idmarca,fecha_marca from marca where persona_rut = '" + persona_rut + "' order by idmarca desc limit 1;";
            return ejecutor.traerDatosDataTable(sql);
        }

        public bool actualizarIncidenciaMarca(string incidencia, int idmarca)
        {
            EjecutoresSql ejecutor = new EjecutoresSql();
            string sql = "update marca set incidencia ='" + incidencia + "' where idmarca=" + idmarca + "";
            return ejecutor.ejecutarConsulta(sql);
        }


        /**
         *Función que se utilizará para buscar los datos de las marcas para ser impresas (menos casino) 
         */
        public DataTable traerDatosCompletosMarcasSinNotificarPorIDMarca(int idmarca)
        {
            EjecutoresSql ejecutor = new EjecutoresSql();
            string sql = "select marca.idmarca,persona.rut, persona.nombre ,persona.segundoNombre,persona.apellidoPaterno,persona.apellidoMaterno,  persona.correo,empresa.rut_empresa, " +
                        "empresa.razon_social,marca.tipoMarca_idtipoMarca,tipoMarca.descripcion,  sucursal.nombre,sucursal.calle,sucursal.numero_calle, " +
                        "sucursal.comuna,sucursal.region,sucursal.pais,reloj.ubicacion,reloj.nombre,marca.hash,marca.fecha_marca,sucursal.ciudad,marca.incidencia " +
                        "from marca, persona,empresa,sucursal,reloj,tipoMarca " +
                        "where " +
                        "marca.idmarca =" + idmarca + " and  " +
                        "marca.persona_rut = persona.rut and " +
                        "marca.empresa_idempresa = empresa.idempresa and " +
                        "marca.sucursal_idsucursal = sucursal.idsucursal and " +
                        "marca.reloj_idreloj = reloj.idreloj and " +
                        "marca.tipoMarca_idtipoMarca = tipoMarca.idtipoMarca";
            return ejecutor.traerDatosDataTable(sql);
        }


        private string traerUltimaMarcaEnRelojLocal()
        {
            EjecutoresSql ejecutor = new EjecutoresSql();
            DataTable dt;
            string id = "";
            string sql = "SELECT `AUTO_INCREMENT` FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA = 'relojControl' AND TABLE_NAME = 'marca';";

            dt = ejecutor.traerDatosDataTable(sql);
            if (dt.Rows != null)
            {
                if (dt.Rows.Count > 0)
                {
                    id = dt.Rows[0][0].ToString();
                }
            }
            return id;
        }

        public DataTable traerMarcaPorCodigoMarca(int codigoMarca)
        {
            EjecutoresSql ejecutor = new EjecutoresSql();
            string sql = "select idmarca ,tipoMarca_idtipoMarca,persona_rut, " +
                         "empresa_idempresa,sucursal_idsucursal,  imagenHuella_numeroDedo, " +
                         "fecha_marca,ingresador, actualizador, " +
                         "reloj_idreloj, hash, marcaConPin, " +
                         "respaldado, notificada, nombreEquipoEdicion,zonaHoraria,esHoraServidor,incidencia,codigoMarca " +
                         "from marca where respaldado=0 and codigoMarca=" + codigoMarca + ";";
            return ejecutor.traerDatosDataTable(sql);
        }



    }
}
