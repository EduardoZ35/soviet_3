using proyectoNegocioRflex.Utilidades;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace proyectoNegocioRflex.Modelo
{
    public class FuncionarioHasReloj
    {

        public DataTable traerRelojesHabilitadosParaPersona(string persona_rut)
        {
            EjecutoresSql ejecutor = new EjecutoresSql();
            string sql = "select idfuncionario_has_reloj, reloj_idreloj,puedeMarcar from funcionario_has_reloj where persona_rut='" + persona_rut + "'";
            return ejecutor.traerDatosDataTable(sql);
        }

        public DataTable preguntarRelojHabilitadoFuncionario(int reloj_idreloj, string persona_rut)
        {
            EjecutoresSql ejecutor = new EjecutoresSql();
            string sql = "select idfuncionario_has_reloj,puedeMarcar,updated_at from funcionario_has_reloj where reloj_idreloj =" + reloj_idreloj +
                " and persona_rut = '" + persona_rut + "' and puedeMarcar=1 limit 1";
            return ejecutor.traerDatosDataTable(sql);
        }

        public DataTable preguntarEstadoFuncionarioPuedeMarcarEnReloj(int reloj_idreloj, string persona_rut)
        {
            EjecutoresSql ejecutor = new EjecutoresSql();
            string sql = "select idfuncionario_has_reloj,puedeMarcar,updated_at from funcionario_has_reloj where reloj_idreloj =" + reloj_idreloj +
                " and persona_rut = '" + persona_rut + "' limit 1";
            return ejecutor.traerDatosDataTable(sql);
        }

        /**
         *Ocupamos esta función para limpiar los registros de asignación de relojes.
         *cuando se actualicen los permisos se borrarán los antiguos y se crearán los nuevos.
         */
        public bool eliminarHabilitacionReloj(string persona_rut)
        {
            EjecutoresSql ejecutor = new EjecutoresSql();
            string sql = "delete from funcionario_has_reloj where persona_rut ='" + persona_rut + "'";
            return ejecutor.ejecutarConsulta(sql);
        }

        public bool asignarHabilitacionReloj(int reloj_idreloj, string persona_rut, string created_by, string updated_by, int respaldado, string created_at, string updated_at, int puedeMarcar, string nombreEquipoEdicion)
        {
            EjecutoresSql ejecutor = new EjecutoresSql();
            string sql = "insert into funcionario_has_reloj(reloj_idreloj,persona_rut,created_by,updated_by,respaldado,created_at,updated_at,nombreEquipoEdicion,puedeMarcar) values" +
                "(" + reloj_idreloj + ",'" + persona_rut + "','" + created_by + "','" + updated_by + "'," + respaldado + ",'" + created_at + "','" + updated_at + "','" + nombreEquipoEdicion + "'," + puedeMarcar + ")";
            return ejecutor.ejecutarConsulta(sql);
        }


        public bool asignarHabilitacionRelojNube(int reloj_idreloj, string persona_rut, string created_by, string updated_by, int respaldado, string created_at, string updated_at, int puedeMarcar, string nombreEquipoEdicion)
        {
            EjecutoresSql ejecutor = new EjecutoresSql();
            string sql = "insert into funcionario_has_reloj(reloj_idreloj,persona_rut,created_by,updated_by,respaldado,created_at,updated_at,nombreEquipoEdicion,puedeMarcar) values" +
                "(" + reloj_idreloj + ",'" + persona_rut + "','" + created_by + "','" + updated_by + "'," + respaldado + ",'" + created_at + "','" + updated_at + "','" + nombreEquipoEdicion + "'," + puedeMarcar + ")";
            return ejecutor.ejecutarConsultaServidor(sql);
        }

        //public DataTable preguntarRelojHabilitadoFuncionario(int persona_idpersona)
        //{
        //    EjecutoresSql ejecutor = new EjecutoresSql();
        //    string sql = "select funcionario_has_reloj.idfuncionario_has_reloj from funcionario_has_reloj, reloj where reloj.nombre = '"+ Environment.MachineName.ToString().ToUpper() + 
        //        "' and reloj.idreloj = funcionario_has_reloj.reloj_idreloj and funcionario_has_reloj.persona_idpersona = " + persona_idpersona + "";
        //    DataTable dt = ejecutor.traerDatosDataTable(sql);
        //    return dt;
        //}

        public DataTable traerFuncionariosHasRelojLocalSinSincronizar()
        {
            EjecutoresSql ejecutor = new EjecutoresSql();
            string sql = "select idfuncionario_has_reloj, reloj_idreloj,persona_rut,nombreEquipoEdicion,created_by,updated_by, " +
                "created_at,updated_at,puedeMarcar from funcionario_has_reloj where respaldado=0 order by persona_rut asc";
            return ejecutor.traerDatosDataTable(sql);
        }

        public DataTable traerFuncionariosHasRelojNubeSinSincronizar(string updated_at)
        {
            EjecutoresSql ejecutor = new EjecutoresSql();
            string sql = "select idfuncionario_has_reloj, reloj_idreloj,persona_rut,nombreEquipoEdicion,created_by,updated_by, " +
                "created_at,updated_at,puedeMarcar from funcionario_has_reloj where updated_at>='" + updated_at + "'  order by updated_at asc;";
            return ejecutor.traerDatosDataTableServidor(sql);
        }

        /**
         *Ocupamos esta función para limpiar los registros de asignación de relojes.
         *cuando se actualicen los permisos se borrarán los antiguos y se crearán los nuevos.
         */
        public bool eliminarHabilitacionRelojNube(string persona_rut)
        {
            EjecutoresSql ejecutor = new EjecutoresSql();
            string sql = "delete from funcionario_has_reloj where persona_rut ='" + persona_rut + "'";
            return ejecutor.ejecutarConsultaServidor(sql);
        }

        public bool actualizarHabilitacionRelojLocal(int reloj_idreloj, string persona_rut, string updated_by, int respaldado, string updated_at, int puedeMarcar, string nombreEquipoEdicion)
        {
            EjecutoresSql ejecutor = new EjecutoresSql();
            string sql = "update funcionario_has_reloj set updated_by='" + updated_by + "',respaldado=" + respaldado + ",updated_at='" + updated_at + "',nombreEquipoEdicion='" + nombreEquipoEdicion + "',puedeMarcar=" + puedeMarcar +
                        " where reloj_idreloj=" + reloj_idreloj + " and persona_rut='" + persona_rut + "'";
            return ejecutor.ejecutarConsulta(sql);
        }

        public string generadorActualizarHabilitacionRelojLocal(int reloj_idreloj, string persona_rut, string updated_by, int respaldado, string updated_at, int puedeMarcar, string nombreEquipoEdicion)
        {
            return "update funcionario_has_reloj set updated_by='" + updated_by + "',respaldado=" + respaldado + ",updated_at='" + updated_at + "',nombreEquipoEdicion='" + nombreEquipoEdicion + "',puedeMarcar=" + puedeMarcar +
                        " where reloj_idreloj=" + reloj_idreloj + " and persona_rut='" + persona_rut + "';";
        }

        public bool actualizarHabilitacionRelojNube(int reloj_idreloj, string persona_rut, string updated_by, int respaldado, string updated_at, int puedeMarcar, string nombreEquipoEdicion)
        {
            EjecutoresSql ejecutor = new EjecutoresSql();
            string sql = "update funcionario_has_reloj set updated_by='" + updated_by + "',respaldado=" + respaldado + ",updated_at='" + updated_at + "',nombreEquipoEdicion='" + nombreEquipoEdicion + "',puedeMarcar=" + puedeMarcar +
                        " where reloj_idreloj=" + reloj_idreloj + " and persona_rut='" + persona_rut + "'";
            return ejecutor.ejecutarConsultaServidor(sql);
        }


        public DataTable traerRelojesHabilitadosParaPersonaDesdeNube(string persona_rut)
        {
            EjecutoresSql ejecutor = new EjecutoresSql();
            string sql = "select idfuncionario_has_reloj, reloj_idreloj,puedeMarcar,updated_at from funcionario_has_reloj where persona_rut='" + persona_rut + "'";
            return ejecutor.traerDatosDataTableServidor(sql);
        }


        public bool actualizarUpdatedAtHabilitacionReloj(int idfuncionario_has_reloj,string updated_at)
        {
            EjecutoresSql ejecutor = new EjecutoresSql();
            string sql = "update funcionario_has_reloj set updated_at'" + updated_at + "' where idfuncionario_has_reloj=" + idfuncionario_has_reloj + ";";
            return ejecutor.ejecutarConsulta(sql);
        }


        public bool actualizarEstadoRespaldadoHabilitacionReloj(int idfuncionario_has_reloj,string updated_at)
        {
            EjecutoresSql ejecutor = new EjecutoresSql();
            string sql = "update funcionario_has_reloj set respaldado=1, updated_at='" + updated_at + "' where idfuncionario_has_reloj=" + idfuncionario_has_reloj + ";";
            return ejecutor.ejecutarConsulta(sql);
        }

        /**
         * Función que recibe un sql ya formado y que realiza multiples insert o updates  contenidas dentro de una linea del sql
         */
        public bool funcionarioHasRelojInsertUpdateMultipleDinamico(string sql)
        {
            EjecutoresSql ejecutor = new EjecutoresSql();
            return ejecutor.ejecutarConsulta(sql);
        }


        public DataTable traerUltimoUpdatedAtFuncionarioHasReloj()
        {
            EjecutoresSql ejecutor = new EjecutoresSql();
            string sql = " select max(updated_at) as mayorFechaActualizacion from funcionario_has_reloj where respaldado=1;";
            return ejecutor.traerDatosDataTable(sql);
        }




    }
}
