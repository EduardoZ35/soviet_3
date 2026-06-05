using proyectoNegocioRflex.Utilidades;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace proyectoNegocioRflex.Modelo
{
    public class TipoRechazo
    {
        public DataTable traerTipoRechazo()
        {
            EjecutoresSql ejecutor = new EjecutoresSql();
            string sql = "select idtipoRechazo,mensajeRechazo,updated_at from tipoRechazo";
            return ejecutor.traerDatosDataTable(sql);
        }

        public DataTable traerTipoRechazoPorID(int idtipoRechazo)
        {
            EjecutoresSql ejecutor = new EjecutoresSql();
            string sql = "select idtipoRechazo,mensajeRechazo,updated_at from tipoRechazo where idtipoRechazo=" + idtipoRechazo + " limit 1";
            return ejecutor.traerDatosDataTable(sql);
        }


        public DataTable traerTipoRechazoNube(string updated_at)
        {
            EjecutoresSql ejecutor = new EjecutoresSql();
            string sql = "select idtipoRechazo,mensajeRechazo,updated_at from tipoRechazo where updated_at>='" + updated_at + "'";
            return ejecutor.traerDatosDataTableServidor(sql);
        }


        public bool agregarTipoRechazoLocal(int idtipoRechazo,string mensajeRechazo,string updated_at)
        {
            EjecutoresSql ejecutor = new EjecutoresSql();
            string sql = "insert into tipoRechazo (idtipoRechazo,mensajeRechazo,updated_at) values (" + idtipoRechazo + ",'" + mensajeRechazo + "','" + updated_at +"')";
            return ejecutor.ejecutarConsulta(sql);
        }


        public bool actualizarTipoRechazoLocal(int idtipoRechazo, string mensajeRechazo, string updated_at)
        {
            EjecutoresSql ejecutor = new EjecutoresSql();
            string sql = "update tipoRechazo set mensajeRechazo='" + mensajeRechazo + "', updated_at='" + updated_at + "' where idtipoRechazo=" + idtipoRechazo + "" ;
            return ejecutor.ejecutarConsulta(sql);
        }

        public DataTable seederTraerTipoRechazo()
        {
            EjecutoresSql ejecutor = new EjecutoresSql();
            bool resp;
            string sql = "insert into tipoRechazo (mensajeRechazo) values ('Huella no Reconocida.')";
            resp = ejecutor.ejecutarConsulta(sql);
            sql = "insert into tipoRechazo (mensajeRechazo) values ('PIN de marca incorrecto.')";
            resp = ejecutor.ejecutarConsulta(sql);
            sql = "insert into tipoRechazo (mensajeRechazo) values ('Marca de desayuno ya registrada.')";
            resp = ejecutor.ejecutarConsulta(sql);
            sql = "insert into tipoRechazo (mensajeRechazo) values ('Marca de almuerzo ya registrada.')";
            resp = ejecutor.ejecutarConsulta(sql);
            sql = "insert into tipoRechazo (mensajeRechazo) values ('Marca de once ya registrada.')";
            resp = ejecutor.ejecutarConsulta(sql);
            sql = "insert into tipoRechazo (mensajeRechazo) values ('Marca de cena ya registrada.')";
            resp = ejecutor.ejecutarConsulta(sql);
            sql = "insert into tipoRechazo (mensajeRechazo) values ('Marca inicio jornada registrada.')";
            resp = ejecutor.ejecutarConsulta(sql);
            sql = "insert into tipoRechazo (mensajeRechazo) values ('Marca término jornada ya registrada.')";
            resp = ejecutor.ejecutarConsulta(sql);
            sql = "insert into tipoRechazo (mensajeRechazo) values ('Marca término comida ya registrada.')";            
            resp = ejecutor.ejecutarConsulta(sql);
            sql = "insert into tipoRechazo (mensajeRechazo) values ('Marca de comida fuera de horario.')";
            resp = ejecutor.ejecutarConsulta(sql);
            sql = "insert into tipoRechazo (mensajeRechazo) values ('Marca término de desayuno ya registrada.')";
            resp = ejecutor.ejecutarConsulta(sql);
            sql = "insert into tipoRechazo (mensajeRechazo) values ('Marca término de almuerzo ya registrada.')";
            resp = ejecutor.ejecutarConsulta(sql);
            sql = "insert into tipoRechazo (mensajeRechazo) values ('Marca término de once ya registrada.')";
            resp = ejecutor.ejecutarConsulta(sql);
            sql = "insert into tipoRechazo (mensajeRechazo) values ('Marca término de cena ya registrada.')";
            resp = ejecutor.ejecutarConsulta(sql);

            return traerTipoRechazo();
        }

        



        //INSERT INTO `relojControl`.`tipoRechazo` (`mensajeRechazo`) VALUES('Marca de almuerzo ya registrada');
        //INSERT INTO `relojControl`.`tipoRechazo` (`mensajeRechazo`) VALUES('Marca de once ya registrada');
        //INSERT INTO `relojControl`.`tipoRechazo` (`mensajeRechazo`) VALUES('Marca inicio jornada registrada');
        //INSERT INTO `relojControl`.`tipoRechazo` (`mensajeRechazo`) VALUES('Marca término jornada ya registrada');
        //INSERT INTO `relojControl`.`tipoRechazo` (`mensajeRechazo`) VALUES('Marca término comida ya registrada');

        //Preguntar por datos por defecto
    }
}
