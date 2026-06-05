using proyectoNegocioRflex.Utilidades;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace proyectoNegocioRflex.Modelo
{
    public class TicketCasino
    {

        public DataTable traerTicketCasinoNoRespaldados()
        {
            EjecutoresSql ejecutor = new EjecutoresSql();
            string sql = "select idticketCasino, codigoCobroTicket, persona_rut,reloj_idreloj,marca_codigoMarca,huella_idhuella,marcaPin,tipoComida_idtipoComida,cobrado,fechaHoraCobro,puntoCanje_idpuntoCanje,fechaHoraVencimiento,generadoPorReloj,checksum,notificado,respaldado,created_at,updated_at from ticketCasino where respaldado=0";
            return ejecutor.traerDatosDataTable(sql);
        }
        //codigoCobroTicket + "','" + persona_rut + "'," + reloj_idreloj + "," + marca_idmarca + "," + huella_idhuella + ",0," + tipoComida_idtipoComida + ",0,'" + fechaHoraVencimiento +"',1,'" + checksum + ",0,0,'" + created_at +"','" + updated_at
        public bool agregarTicketCasinoLocal(string codigoCobroTicket, string persona_rut, int reloj_idreloj, int marca_codigoMarca, int huella_idhuella, int marcaPin, int tipoComida_idtipoComida, string fechaHoraVencimiento, string created_at, string updated_at)
        {
            EjecutoresSql ejecutor = new EjecutoresSql();
            Encriptacion enc = new Encriptacion();
            Herramientas h = new Herramientas();
            string fechaSinCaracteres = h.quitarCaracteresEspecialesFechaParaHash(created_at);
            string checksum = enc.Encriptar(marca_codigoMarca.ToString() + reloj_idreloj.ToString() + tipoComida_idtipoComida.ToString() + fechaSinCaracteres);
            string sql;
            if (marcaPin == 0)
            {
                sql = "insert into ticketCasino (codigoCobroTicket, persona_rut,reloj_idreloj,marca_codigoMarca,huella_idhuella,marcaPin,tipoComida_idtipoComida,cobrado,fechaHoraVencimiento,generadoPorReloj,checksum,notificado,respaldado,created_at,updated_at) " +
                "values('" + codigoCobroTicket + "','" + persona_rut + "'," + reloj_idreloj + "," + marca_codigoMarca + "," + huella_idhuella + "," + marcaPin + "," + tipoComida_idtipoComida + ",0,'" + fechaHoraVencimiento + "',1,'" + checksum + "',0,0,'" + created_at + "','" + updated_at + "')";
            }
            else
            {
                sql = "insert into ticketCasino (codigoCobroTicket, persona_rut,reloj_idreloj,marca_codigoMarca,marcaPin,tipoComida_idtipoComida,cobrado,fechaHoraVencimiento,generadoPorReloj,checksum,notificado,respaldado,created_at,updated_at) " +
                "values('" + codigoCobroTicket + "','" + persona_rut + "'," + reloj_idreloj + "," + marca_codigoMarca + "," + marcaPin + "," + tipoComida_idtipoComida + ",0,'" + fechaHoraVencimiento + "',1,'" + checksum + "',0,0,'" + created_at + "','" + updated_at + "')";
            }
            return ejecutor.ejecutarConsulta(sql);
        }

        public bool agregarTicketCasinoNube(string codigoCobroTicket, string persona_rut, int reloj_idreloj, int marca_codigoMarca, int huella_idhuella, int marcaPin, int tipoComida_idtipoComida, string fechaHoraVencimiento, string created_at, string updated_at, string checksum)
        {
            EjecutoresSql ejecutor = new EjecutoresSql();
            string sql;
            if (marcaPin == 0)
            {
                sql = "insert into ticketCasino (codigoCobroTicket, persona_rut,reloj_idreloj,marca_codigoMarca,huella_idhuella,marcaPin,tipoComida_idtipoComida,cobrado,fechaHoraVencimiento,generadoPorReloj,checksum,notificado,respaldado,created_at,updated_at) " +
                "values('" + codigoCobroTicket + "','" + persona_rut + "'," + reloj_idreloj + "," + marca_codigoMarca + "," + huella_idhuella + "," + marcaPin + "," + tipoComida_idtipoComida + ",0,'" + fechaHoraVencimiento + "',1,'" + checksum + "',0,1,'" + created_at + "','" + updated_at + "')";
            }
            else
            {
                sql = "insert into ticketCasino (codigoCobroTicket, persona_rut,reloj_idreloj,marca_codigoMarca,marcaPin,tipoComida_idtipoComida,cobrado,fechaHoraVencimiento,generadoPorReloj,checksum,notificado,respaldado,created_at,updated_at) " +
                "values('" + codigoCobroTicket + "','" + persona_rut + "'," + reloj_idreloj + "," + marca_codigoMarca + "," + marcaPin + "," + tipoComida_idtipoComida + ",0,'" + fechaHoraVencimiento + "',1,'" + checksum + "',0,1,'" + created_at + "','" + updated_at + "')";
            }
            return ejecutor.ejecutarConsultaServidor(sql);
        }


        public DataTable traerIndiceActualTicketCasino()
        {
            EjecutoresSql ejecutor = new EjecutoresSql();
            string sql = "SELECT `AUTO_INCREMENT` FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA = 'relojControl' AND TABLE_NAME = 'ticketCasino';";
            return ejecutor.traerDatosDataTable(sql);
        }


        public DataTable traerTicketCasinoSinRespaldarDatosCompletos()
        {
            EjecutoresSql ejecutor = new EjecutoresSql();
            string sql = "select ticketCasino.idticketCasino, ticketCasino.codigoCobroTicket," +
                            "ticketCasino.checksum, ticketCasino.notificado, persona.rut," +
                            "persona.nombre, persona.segundoNombre, persona.apellidoPaterno , persona.apellidoMaterno, " +
                            "reloj.ubicacion , reloj.nombre, tipoComida.nombre , marca.fecha_marca, marca.hash, " +
                            "empresa.razon_social, empresa.rut_empresa," +
                            "sucursal.calle, sucursal.numero_calle, sucursal.comuna, sucursal.region, sucursal.pais," +
                            "tipoMarca.descripcion ,ticketCasino.fechaHoraVencimiento, persona.correo,sucursal.nombre   " +
                            "from ticketCasino " +
                            "inner join persona on persona.rut = ticketCasino.persona_rut " +
                            "inner join reloj " +
                            "on reloj.idreloj = ticketCasino.reloj_idreloj " +
                            "inner join marca " +
                            "on marca.codigoMarca = ticketCasino.marca_codigoMarca " +
                            "inner join tipoComida " +
                            "on tipoComida.idtipoComida = ticketCasino.tipoComida_idtipoComida " +
                            "inner join empresa " +
                            "on empresa.idempresa = persona.empresa_idempresa " +
                            "inner join sucursal " +
                            "on sucursal.idsucursal = reloj.sucursal_idsucursal " +
                            "inner join tipoMarca " +
                            "on tipoMarca.idtipoMarca = marca.tipoMarca_idtipoMarca " +
                            "where ticketCasino.notificado = 0;";
            return ejecutor.traerDatosDataTable(sql);
        }


        public bool actualizarEstadoRespaldadoTicketCasino(string codigoCobroTicket)
        {
            EjecutoresSql ejecutor = new EjecutoresSql();
            string sql = "update ticketCasino set respaldado=1 where codigoCobroTicket='" + codigoCobroTicket + "'";
            return ejecutor.ejecutarConsulta(sql);
        }

        public bool actualizarEstadoNotificadoTicketCasino(string codigoCobroTicket)
        {
            EjecutoresSql ejecutor = new EjecutoresSql();
            string sql = "update ticketCasino set notificado=1 where codigoCobroTicket='" + codigoCobroTicket + "'";
            return ejecutor.ejecutarConsulta(sql);
        }

        public bool actualizarEstadoNotificadoTicketCasinoNube(string codigoCobroTicket)
        {
            EjecutoresSql ejecutor = new EjecutoresSql();
            string sql = "update ticketCasino set notificado=1 where codigoCobroTicket='" + codigoCobroTicket + "'";
            return ejecutor.ejecutarConsultaServidor(sql);
        }











    }
}
