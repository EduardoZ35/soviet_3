using proyectoNegocioRflex.Utilidades;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace proyectoNegocioRflex.Modelo
{
    public class InhabilitacionMarca
    {
        public bool agregarInhabilitacionMarca(string rutEncriptado, int tim_tipoInhabilitacionMarca, DateTime fecha, int habilitado, string comentario, int respaldado, int turnoDia)
        {
            Herramientas h = new Herramientas();
            EjecutoresSql ejecutor = new EjecutoresSql();
            string fechaFormateada = h.formatearFechaSinHora(fecha);
            string fechaCreatedAt = h.formatearFecha(fecha, true);
            string sql = "insert into inhabilitacionMarca(" +
                "persona_rut, tim_tipoInhabilitacionMarca, fecha, habilitado, comentario,bloqueoMarca,respaldado,created_at,nombreEquipoEdicion,turnoDia)values('" + rutEncriptado + "'," +
                tim_tipoInhabilitacionMarca + ",'" + fechaFormateada + "'," + habilitado + ",'" + comentario + "',0," + respaldado + ",'" + fechaCreatedAt + "','" + Environment.MachineName + "'," + turnoDia + ")";
            return ejecutor.ejecutarConsulta(sql);
        }

        public string preguntarInhabilitacionMarca(string rutEncriptado, int tim_tipoInhabilitacionMarca, DateTime fecha)
        {
            Herramientas h = new Herramientas();
            EjecutoresSql ejecutor = new EjecutoresSql();
            //   string fechaFormateada = h.formatearFecha(fecha, false);
            string fechaFormateada = h.formatearFechaSinHora(fecha);
            string comentario;
            string sql = "select idinhabilitacionMarca,comentario,bloqueoMarca from inhabilitacionMarca where persona_rut='" + rutEncriptado + "' and  " +
            "tim_tipoInhabilitacionMarca=" + tim_tipoInhabilitacionMarca + " and fecha='" + fechaFormateada + "' and habilitado=1 limit 1";
            DataTable dt = ejecutor.traerDatosDataTable(sql);
            comentario = dt != null && dt.Rows.Count > 0 ? dt.Rows[0][1].ToString() : "Permitido";
            return comentario;

        }


        //1280 x 800
        //-------------------------------------------------------------------------------------------------------



        //public bool agregarInhabilitacionMarcaServidor(string rutEncriptado, int tim_tipoInhabilitacionMarca, DateTime fecha, int habilitado, string comentario, string ip)
        //{
        //    Herramientas h = new Herramientas();
        //    EjecutoresSql ejecutor = new EjecutoresSql();
        //    string fechaFormateada = h.formatearFecha(fecha, false);
        //    string sql = "insert into inhabilitacionMarca(" +
        //        "persona_rut, tim_tipoInhabilitacionMarca, fecha, habilitado, comentario,bloqueoMarca,respaldado)values('" + rutEncriptado + "'," +
        //        tim_tipoInhabilitacionMarca + ",'" + fechaFormateada + "'," + habilitado + ",'" + comentario + "',0 ,1)";
        //    bool resp = ejecutor.ejecutarConsultaServidor(sql);
        //    return resp;
        //}

        public string preguntarInhabilitacionMarcaServidor(string rutSinEncriptar, int tim_tipoInhabilitacionMarca, DateTime fecha, int turnoDia)
        {
            EjecutoresSql ejecutor = new EjecutoresSql();
            string dia = fecha.Day < 10 ? "0" + fecha.Day.ToString() : fecha.Day.ToString();
            string mes = fecha.Month < 10 ? "0" + fecha.Month.ToString() : fecha.Month.ToString();
            string fechaFormateada = fecha.Year + "/" + mes + "/" + dia;
            string sql = "select idinhabilitacionMarca,comentario,bloqueoMarca from inhabilitacionMarca where persona_rut='" + rutSinEncriptar + "'and " +
            "tim_tipoInhabilitacionMarca=" + tim_tipoInhabilitacionMarca + " and fecha='" + fechaFormateada + "' and habilitado=1 and turnoDia=" + turnoDia + " limit 1";
            DataTable dt = ejecutor.traerDatosDataTableServidor(sql);
            return dt != null && dt.Rows.Count > 0 ? dt.Rows[0][1].ToString() : "Permitido";
        }

        public DataTable preguntarBloqueoMarca(string rutNormal, int turnoDia)
        {
            EjecutoresSql ejecutor = new EjecutoresSql();
            string sql = "select idinhabilitacionMarca,comentario from inhabilitacionMarca where " +
                " persona_rut='" + rutNormal + "' and fecha= curdate() and habilitado=1 and bloqueoMarca=1 and turnoDia=" + turnoDia + " limit 1";
            return ejecutor.traerDatosDataTableServidor(sql);
        }

        public DataTable preguntarBloqueoMarcaLocal(string rutNormal, int turnoDia)
        {
            EjecutoresSql ejecutor = new EjecutoresSql();
            string sql = "select idinhabilitacionMarca,comentario from inhabilitacionMarca where " +
                " persona_rut='" + rutNormal + "' and fecha= curdate() and habilitado=1 and bloqueoMarca=1 and turnoDia=" + turnoDia + " limit 1";
            return ejecutor.traerDatosDataTable(sql);
        }
        public string preguntarInhabilitacionMarcaLocal(string rutEncriptado, int tim_tipoInhabilitacionMarca, DateTime fecha, int turnoDia)
        {
            EjecutoresSql ejecutor = new EjecutoresSql();
            string dia = fecha.Day < 10 ? "0" + fecha.Day.ToString() : fecha.Day.ToString();
            string mes = fecha.Month < 10 ? "0" + fecha.Month.ToString() : fecha.Month.ToString();
            string fechaFormateada = fecha.Year + "/" + mes + "/" + dia;
            string sql = "select idinhabilitacionMarca,comentario,bloqueoMarca from inhabilitacionMarca where persona_rut='" + rutEncriptado + "'and " +
            "tim_tipoInhabilitacionMarca=" + tim_tipoInhabilitacionMarca + " and fecha='" + fechaFormateada + "' and habilitado=1 and turnoDia=" + turnoDia + " limit 1;";
            DataTable dt = ejecutor.traerDatosDataTable(sql);
            return dt != null && dt.Rows.Count > 0 ? dt.Rows[0][1].ToString() : "Permitido";
        }

        //Usado para sincronizador.
        #region Usados por Sincronizador

        //Traer las inhabilitaciones sin sincronizar del dia
        public DataTable traerInhabilitacionesSinSincronizar()
        {
            EjecutoresSql ejecutor = new EjecutoresSql();
            string sql = "select idinhabilitacionMarca, persona_rut, tim_tipoInhabilitacionMarca, fecha, habilitado, comentario, bloqueoMarca,  respaldado,nombreEquipoEdicion,turnoDia " +
                "from inhabilitacionMarca where respaldado =0;";
            return ejecutor.traerDatosDataTable(sql);
        }

        //Tambien usado por el programa del reloj.
        public bool agregarInhabilitacionMarcaServidor(string rut, int tim_tipoInhabilitacionMarca, DateTime fecha, int habilitado, string comentario, int turnoDia)
        {
            Herramientas h = new Herramientas();
            EjecutoresSql ejecutor = new EjecutoresSql();
            string fechaFormateada = h.formatearFechaSinHora(fecha);
            string sql = "insert into inhabilitacionMarca(" +
                "persona_rut, tim_tipoInhabilitacionMarca, fecha, habilitado, comentario,bloqueoMarca,respaldado,nombreEquipoEdicion,turnoDia)values('" + rut + "'," +
                tim_tipoInhabilitacionMarca + ",'" + fechaFormateada + "'," + habilitado + ",'" + comentario + "',0,1,'" + Environment.MachineName + "'," + turnoDia + ");";
            return ejecutor.ejecutarConsultaServidor(sql);
        }

        /**
         * Función que actualiza el estado de la inhabilitación local indicando que ya está respaldada en el servidor.
         */
        public bool actualizarEstadoInhabilitacionMarcaLocal(int idinhabilitacionMarca)
        {
            EjecutoresSql ejecutor = new EjecutoresSql();
            string sql = "update inhabilitacionMarca set respaldado=1 where idinhabilitacionMarca=" + idinhabilitacionMarca + "";
            return ejecutor.ejecutarConsulta(sql);
        }


        //Las proximas 4 funciones van de la mano con la mejora de que el sincronizador traiga los datos de las inhabilitaciones...
        //1) primero traeremos las inhabilitaciones que tengan un mayor updated_at que la fecha consultada ( no hacemos la distinción por nombre de equipo porque puede que 
        //el registro de inhabilitación se haya editado y se tenga que actualizar en local.).


        //2)Consultamos si el registro Si ya existe( preguntando por la fecha, turnoDia, tipo de inhabilitacion de marca y el rut de la persona ) 
        //y si detecta algun cambio en la información (que sería el estado habilitado o bloquea) 
        //3)actualizamos el registro local
        //4)de lo contrario agregamos.

        //Si no existe preguntando por la fecha, turnoDia, tipo de inhabilitacion de marca y el rut de la persona  agregamos el registro.


        /**
         * 1) Función que trae los datos de inhabilitaciones desde la nube para ser agregados o actualizar los registros que están localmente almacenados.
         * se trae todos los registros que tengan una fecha y hora maypr a la consultada ( el sincronizador en su fecha de consulta para asegurar la sincronización una
         * tiene diferencia de 3 minutos en contra de la hora actual.
         */
        public DataTable traerInhabilitacionesSinSincronizarDesdeNube(string fechaConsulta)
        {
            EjecutoresSql ejecutor = new EjecutoresSql();
            string sql = "select persona_rut, tim_tipoInhabilitacionMarca, fecha, turnoDia, habilitado, comentario, bloqueoMarca,  respaldado,nombreEquipoEdicion,created_at,updated_at " +
                "from inhabilitacionMarca where updated_at >='" + fechaConsulta + "' order by updated_at asc;";
            return ejecutor.traerDatosDataTableServidor(sql);
        }

        public DataTable traerInhabilitacionesSinSincronizarDesdeNubeOtroReloj(string fechaConsulta)
        {
            EjecutoresSql ejecutor = new EjecutoresSql();
            string sql = "select persona_rut, tim_tipoInhabilitacionMarca, fecha, turnoDia, habilitado, comentario, bloqueoMarca,  respaldado,nombreEquipoEdicion,created_at,updated_at " +
                "from inhabilitacionMarca where updated_at >='" + fechaConsulta + "' and nombreEquipoEdicion <>'" + Environment.MachineName +"' order by updated_at asc;";
            return ejecutor.traerDatosDataTableServidor(sql);
        }



        /*
         * 2) Consultamos por datos especificos para ver si ya existe el registro.
         * //retornamos el id de la inhabilitación por si requiere actualización .. y la fecha del updated_at para compararla con el registro de la nube.
         * //si es distinta actualizamos.. de lo contrario omitimos la edición
         * El rut debe consultarse cifrado! (siempre cuando sea consulta por dato local).
         */
        public DataTable consultarRegistroInhabilitacionLocal(DateTime fecha, string rut, int tipoInhabilitacionMarca, int turnoDia)
        {
            EjecutoresSql ejecutor = new EjecutoresSql();
            Herramientas h = new Herramientas();
            string fechaInhabilitacion = h.formatearFechaSinHora(fecha);
            string sql = "select idinhabilitacionMarca,updated_at from inhabilitacionMarca where fecha='" + fechaInhabilitacion +
                "' and  tim_tipoInhabilitacionMarca=" + tipoInhabilitacionMarca + " and turnoDia=" + turnoDia + " and persona_rut='" + rut + "';";
            return ejecutor.traerDatosDataTable(sql);
        }


        /*
        * 3) Función que actualiza el estado de la inhabilitación local indicando que ya está respaldada en el servidor.
        */
        public bool actualizarDatoInhabilitacionMarcaPorDatoTraidoDesdeNube(DateTime updated_at, int habilitado, int bloqueoMarca, int idinhabilitacionMarca)
        {
            Herramientas h = new Herramientas();
            EjecutoresSql ejecutor = new EjecutoresSql();
            string fechaUpdatedFormateada = h.formatearFecha(updated_at, true);
            string sql = "update inhabilitacionMarca set habilitado=" + habilitado + ",bloqueoMarca=" + bloqueoMarca + ",updated_at='" + fechaUpdatedFormateada + "' where idinhabilitacionMarca=" + idinhabilitacionMarca + ";";
            return ejecutor.ejecutarConsulta(sql);
        }


        /**
         * 4)
         * Esta función agrega en local un nuevo registro de inhabilitación de marca registrado en la nube para mantener sincronizadas las inhabilitaciones 
         * y quitarle esta carga al reloj.
         */
        public bool agregarInhabilitacionMarcaTraidoDesdeNube(string rutEncriptado, int tim_tipoInhabilitacionMarca, DateTime fecha, int turnoDia, int habilitado, string comentario, int bloqueoMarca, string nombreEquipoEdicion, DateTime created_at, DateTime updated_at)
        {
            Herramientas h = new Herramientas();
            EjecutoresSql ejecutor = new EjecutoresSql();
            string fechaFormateada = h.formatearFechaSinHora(fecha);
            string fechaCreatedAt = h.formatearFecha(created_at, true);
            string fechaUpdatedAt = h.formatearFecha(updated_at, true);
            //En el sql va un "1" ese corresponde al respaldado... como es un dato obvio (viene de la nube... está mas que claro que está respaldado xD) lo dejaremos en 1.
            string sql = "insert into inhabilitacionMarca(" +
                "persona_rut, tim_tipoInhabilitacionMarca,  fecha,  turnoDia, habilitado, comentario, bloqueoMarca, respaldado, nombreEquipoEdicion, created_at, updated_at )values('" +
                rutEncriptado + "'," + tim_tipoInhabilitacionMarca + ",'" + fechaFormateada + "'," + turnoDia + "," + habilitado + ",'" + comentario + "'," + bloqueoMarca + "," + 1 + ",'" + nombreEquipoEdicion + "','" + fechaCreatedAt + "','" + fechaUpdatedAt + "');";
            return ejecutor.ejecutarConsulta(sql);
        }

        #endregion


        /*
         *Llama a la función cruce de turnos que indica buscando inhabilitaciones y turnos si la persona puede sacar un ticket de casino.
         */
        public string preguntarFuncionCruceTurnos(string rutNormal, int tipoInhabilitacionMarca, int idtipoComida, int turnoDia)
        {
            EjecutoresSql ejecutor = new EjecutoresSql();
            string comentario = "Permitido";
            string sql = "select relojControl.preguntarInhabilitacionesCruceTurnos('" + rutNormal + "'," + tipoInhabilitacionMarca + "," + idtipoComida + "," + turnoDia + "); ";
            DataTable dt = ejecutor.traerDatosDataTableServidor(sql);
            //Si no podemos obtener el dato damos por permitido.
            if (dt != null)
            {
                if (dt.Rows.Count > 0)
                {
                    comentario = dt.Rows[0][0].ToString();
                }
            }
            return comentario;
        }


        public DataTable traerUltimoUpdatedAtInhabilitacionMarca()
        {
            EjecutoresSql ejecutor = new EjecutoresSql();
            string sql = " select max(updated_at) as mayorFechaActualizacion from inhabilitacionMarca where respaldado=1;";
            return ejecutor.traerDatosDataTable(sql);
        }







    }
}
