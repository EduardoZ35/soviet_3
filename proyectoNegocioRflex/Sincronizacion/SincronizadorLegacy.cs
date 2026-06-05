using System;
using System.Data;
using proyectoNegocioRflex.Modelo;
using proyectoNegocioRflex.Utilidades;

namespace proyectoNegocioRflex.Sincronizacion
{
    /// <summary>
    /// Lógica de sincronización extraída de frmSincronizador (WinForms) sin dependencias de UI.
    /// Todas las llamadas a actualizarStatus() fueron reemplazadas por Log().
    /// </summary>
    public class SincronizadorLegacy
    {
        private readonly Action<string> _log;
        private readonly DeviceIdentity _device;

        // Fecha de tolerancia usada cuando no hay registros previos.
        private string _fechaUltimaActualizacion = "2017-01-01 10:00:01";

        public SincronizadorLegacy(DeviceIdentity device, Action<string> log)
        {
            _device = device;
            _log = log ?? (msg => { });
        }

        private void Log(string msg) => _log(msg);

        // ======================== Helpers internos ========================

        private bool comprobarFechasEdicion(DateTime fechaDatoLocal, DateTime fechaDatoNube)
        {
            return fechaDatoLocal.Equals(fechaDatoNube);
        }

        private void generarLog(string error, string tipoLog)
        {
            try
            {
                LogSincronizador l = new LogSincronizador();
                Herramientas h = new Herramientas();
                error = h.formateadorLog(error);
                l.agregarLogError(error, tipoLog);
            }
            catch (Exception)
            {
            }
        }

        // ======================== Tipo Comida ========================

        private string traerUltimaFechaUpdatedRegistrosTipoComidas()
        {
            TipoComida tc = new TipoComida();
            DataTable dtFecha = tc.traerUltimoUpdatedAtTipoComida();
            string ultimoUpdated = _fechaUltimaActualizacion;
            if (dtFecha != null)
            {
                if (dtFecha.Rows.Count > 0)
                {
                    Herramientas h = new Herramientas();
                    try
                    {
                        ultimoUpdated = h.formatearFecha(DateTime.Parse(dtFecha.Rows[0][0].ToString()), true);
                    }
                    catch (Exception)
                    {
                        ultimoUpdated = _fechaUltimaActualizacion;
                    }
                }
            }
            return ultimoUpdated;
        }

        public void SincronizarTipoComida()
        {
            Log("Comprobando tipos de comida...");
            Herramientas h = new Herramientas();
            try
            {
                TipoComida tc = new TipoComida();
                DataTable dtLocal;
                DataTable dtNube = tc.traerTipoComidaNube(traerUltimaFechaUpdatedRegistrosTipoComidas());

                if (dtNube != null)
                {
                    if (dtNube.Rows.Count > 0)
                    {
                        string fechaActualizacion = "";
                        string nombre = "";
                        int idtipoComida = 0;
                        string horaInicioEmision = "";
                        string horaTerminoEmision = "";
                        string horaInicioCobro = "";
                        string horaTerminoCobro = "";
                        int habilitada = 0;
                        string actualizadoPor = "";
                        int diasExpiracionTicket = 0;
                        Log("Actualizando Tipos de comida....");
                        for (int i = 0; i < dtNube.Rows.Count; i++)
                        {
                            idtipoComida = int.Parse(dtNube.Rows[i][0].ToString());
                            dtLocal = traerTipoComidaLocal(idtipoComida);
                            if (dtLocal != null)
                            {
                                if (dtLocal.Rows.Count > 0)
                                {
                                    if (!comprobarFechasEdicion(DateTime.Parse(dtLocal.Rows[0][1].ToString()), DateTime.Parse(dtNube.Rows[i][10].ToString())))
                                    {
                                        nombre = dtNube.Rows[i][1].ToString();
                                        horaInicioEmision = DateTime.Parse(dtNube.Rows[i][2].ToString()).ToLongTimeString();
                                        horaTerminoEmision = DateTime.Parse(dtNube.Rows[i][3].ToString()).ToLongTimeString();
                                        horaInicioCobro = DateTime.Parse(dtNube.Rows[i][4].ToString()).ToLongTimeString();
                                        horaTerminoCobro = DateTime.Parse(dtNube.Rows[i][5].ToString()).ToLongTimeString();
                                        habilitada = int.Parse(dtNube.Rows[i][6].ToString());
                                        actualizadoPor = dtNube.Rows[i][7].ToString();
                                        fechaActualizacion = h.formatearFecha(DateTime.Parse(dtNube.Rows[i][10].ToString()), true);
                                        diasExpiracionTicket = int.Parse(dtNube.Rows[i][11].ToString());
                                        tc.editarTipoDeComida(idtipoComida, nombre, horaInicioEmision, horaTerminoEmision, horaInicioCobro, horaTerminoCobro,
                                            habilitada, actualizadoPor, fechaActualizacion, diasExpiracionTicket);
                                    }
                                }
                                else
                                {
                                    nombre = dtNube.Rows[i][1].ToString();
                                    horaInicioEmision = DateTime.Parse(dtNube.Rows[i][2].ToString()).ToLongTimeString();
                                    horaTerminoEmision = DateTime.Parse(dtNube.Rows[i][3].ToString()).ToLongTimeString();
                                    horaInicioCobro = DateTime.Parse(dtNube.Rows[i][4].ToString()).ToLongTimeString();
                                    horaTerminoCobro = DateTime.Parse(dtNube.Rows[i][5].ToString()).ToLongTimeString();
                                    habilitada = int.Parse(dtNube.Rows[i][6].ToString());
                                    actualizadoPor = dtNube.Rows[i][7].ToString();
                                    string creadoPor = dtNube.Rows[i][8].ToString();
                                    string fechaCreacion = h.formatearFecha(DateTime.Parse(dtNube.Rows[i][9].ToString()), true);
                                    fechaActualizacion = h.formatearFecha(DateTime.Parse(dtNube.Rows[i][10].ToString()), true);
                                    diasExpiracionTicket = int.Parse(dtNube.Rows[i][11].ToString());
                                    tc.agregarNuevoTipoDeComida(idtipoComida, nombre, horaInicioEmision, horaTerminoEmision, horaInicioCobro, horaTerminoCobro,
                                        habilitada, actualizadoPor, creadoPor, fechaCreacion, fechaActualizacion, diasExpiracionTicket);
                                }
                            }
                        }
                    }
                }
                else
                {
                    Log("Error al obtener los Tipos de comida desde la nube...");
                    generarLog("comprobarTipoComida() Error conexión a base de datos ", "ERROR");
                }
                Log("Tipos de comida sincronizados....");
            }
            catch (Exception ex)
            {
                generarLog("comprobarTipoComida() " + ex.ToString(), "ERROR");
            }
        }

        private DataTable traerTipoComidaLocal(int idtipoComida)
        {
            DataTable dt;
            try
            {
                TipoComida tc = new TipoComida();
                dt = tc.traerTipoComidaPorID(idtipoComida);
                if (dt == null)
                {
                    generarLog("traerTipoComidaLocal() Error conexión base de datos", "ERROR");
                }
            }
            catch (Exception ex)
            {
                dt = null;
                generarLog("traerTipoComidaLocal() " + ex.ToString(), "ERROR");
            }
            return dt;
        }

        // ======================== Tipo Inhabilitacion Marca ========================

        public void SincronizarTipoInhabilitacionMarca()
        {
            Herramientas h = new Herramientas();
            Log("Comprobando sincronización de tipos de inhabilitaciones de marca...");
            try
            {
                DataTable dtNube;
                DataTable dtLocal;
                string fechaActualizacion = "";
                TipoInhabilitacionMarca tim = new TipoInhabilitacionMarca();
                dtNube = tim.traerTiposInhabilitacionMarcaNube(_fechaUltimaActualizacion);
                if (dtNube != null)
                {
                    if (dtNube.Rows.Count > 0)
                    {
                        for (int i = 0; i < dtNube.Rows.Count; i++)
                        {
                            dtLocal = comprobarTipoInhabilitacionMarcaLocal(int.Parse(dtNube.Rows[i][0].ToString()));
                            if (dtLocal != null)
                            {
                                if (dtLocal.Rows.Count > 0)
                                {
                                    if (!comprobarFechasEdicion(DateTime.Parse(dtLocal.Rows[0][2].ToString()), (DateTime.Parse(dtNube.Rows[i][2].ToString()))))
                                    {
                                        fechaActualizacion = h.formatearFecha(DateTime.Parse(dtNube.Rows[i][2].ToString()), true);
                                        tim.actualizarTiposInhabilitacionMarcaLocal(int.Parse(dtNube.Rows[i][0].ToString()), dtNube.Rows[i][1].ToString(), fechaActualizacion);
                                    }
                                }
                                else
                                {
                                    fechaActualizacion = h.formatearFecha(DateTime.Parse(dtNube.Rows[i][2].ToString()), true);
                                    tim.agregarTiposInhabilitacionMarcaLocal(int.Parse(dtNube.Rows[i][0].ToString()), dtNube.Rows[i][1].ToString(), fechaActualizacion);
                                }
                            }
                        }
                    }
                    Log("Tipos de inhabilitaciones de marca sincronizados...");
                }
            }
            catch (Exception ex)
            {
                generarLog("comprobarTipoInhabilitacionMarca() " + ex.ToString(), "ERROR");
            }
        }

        private DataTable comprobarTipoInhabilitacionMarcaLocal(int idtipoInhabilitacionMarca)
        {
            DataTable dt;
            TipoInhabilitacionMarca tim = new TipoInhabilitacionMarca();
            try
            {
                dt = tim.traerTiposInhabilitacionMarcaPorID(idtipoInhabilitacionMarca);
            }
            catch (Exception ex)
            {
                dt = null;
                generarLog("comprobarTipoInhabilitacionMarcaLocal() " + Environment.MachineName + " " + ex.ToString(), "ERROR");
            }
            return dt;
        }

        // ======================== Tipo Rechazo ========================

        public void SincronizarTipoRechazo()
        {
            Log("Comprobando sincronización de tipos de rechazos de marca...");
            Herramientas h = new Herramientas();
            try
            {
                DataTable dtNube;
                DataTable dtLocal;
                string fechaActualizacion = "";
                TipoRechazo tr = new TipoRechazo();
                dtNube = tr.traerTipoRechazoNube(_fechaUltimaActualizacion);
                if (dtNube != null)
                {
                    if (dtNube.Rows.Count > 0)
                    {
                        for (int i = 0; i < dtNube.Rows.Count; i++)
                        {
                            dtLocal = comprobarTipoRechazoLocal(int.Parse(dtNube.Rows[i][0].ToString()));
                            if (dtLocal != null)
                            {
                                if (dtLocal.Rows.Count > 0)
                                {
                                    if (!comprobarFechasEdicion(DateTime.Parse(dtLocal.Rows[0][2].ToString()), (DateTime.Parse(dtNube.Rows[i][2].ToString()))))
                                    {
                                        fechaActualizacion = h.formatearFecha(DateTime.Parse(dtNube.Rows[i][2].ToString()), true);
                                        tr.actualizarTipoRechazoLocal(int.Parse(dtNube.Rows[i][0].ToString()), dtNube.Rows[i][1].ToString(), fechaActualizacion);
                                    }
                                }
                                else
                                {
                                    fechaActualizacion = h.formatearFecha(DateTime.Parse(dtNube.Rows[i][2].ToString()), true);
                                    tr.agregarTipoRechazoLocal(int.Parse(dtNube.Rows[i][0].ToString()), dtNube.Rows[i][1].ToString(), fechaActualizacion);
                                }
                            }
                        }
                    }
                    Log("Tipos de rechazos de marca sincronizados...");
                }
            }
            catch (Exception ex)
            {
                generarLog("comprobarTipoRechazo() " + ex.ToString(), "ERROR");
            }
        }

        private DataTable comprobarTipoRechazoLocal(int idtipoRechazo)
        {
            DataTable dt;
            TipoRechazo tr = new TipoRechazo();
            try
            {
                dt = tr.traerTipoRechazoPorID(idtipoRechazo);
            }
            catch (Exception ex)
            {
                dt = null;
                generarLog("comprobarTipoRechazoLocal() " + Environment.MachineName + " " + ex.ToString(), "ERROR");
            }
            return dt;
        }

        // ======================== Tipo Marca ========================

        public void SincronizarTipoMarca()
        {
            Log("Comprobando sincronización de Tipos de marca...");
            Herramientas h = new Herramientas();
            try
            {
                DataTable dtNube;
                DataTable dtLocal;
                string fechaActualizacion = "";
                TipoMarca tm = new TipoMarca();
                dtNube = tm.traerTipoMarcaNube(_fechaUltimaActualizacion);
                if (dtNube != null)
                {
                    if (dtNube.Rows.Count > 0)
                    {
                        for (int i = 0; i < dtNube.Rows.Count; i++)
                        {
                            dtLocal = comprobarTipoMarcaLocal(int.Parse(dtNube.Rows[i][0].ToString()));
                            if (dtLocal != null)
                            {
                                if (dtLocal.Rows.Count > 0)
                                {
                                    if (!comprobarFechasEdicion(DateTime.Parse(dtLocal.Rows[0][2].ToString()), (DateTime.Parse(dtNube.Rows[i][2].ToString()))))
                                    {
                                        fechaActualizacion = h.formatearFecha(DateTime.Parse(dtNube.Rows[i][2].ToString()), true);
                                        tm.actualizarTipoMarcaLocal(int.Parse(dtNube.Rows[i][0].ToString()), dtNube.Rows[i][1].ToString(), 1, fechaActualizacion);
                                    }
                                }
                                else
                                {
                                    fechaActualizacion = h.formatearFecha(DateTime.Parse(dtNube.Rows[i][2].ToString()), true);
                                    tm.agregarTipoMarcaLocal(int.Parse(dtNube.Rows[i][0].ToString()), dtNube.Rows[i][1].ToString(), 1, fechaActualizacion);
                                }
                            }
                        }
                    }
                    Log("Tipos de marca sincronizados...");
                }
            }
            catch (Exception ex)
            {
                generarLog("comprobarTipoMarca() " + ex.ToString(), "ERROR");
            }
        }

        private DataTable comprobarTipoMarcaLocal(int idtipoMarca)
        {
            DataTable dt;
            TipoMarca tm = new TipoMarca();
            try
            {
                dt = tm.traerTipoMarcaPorID(idtipoMarca);
            }
            catch (Exception ex)
            {
                dt = null;
                generarLog("comprobarTipoMarcaLocal() " + Environment.MachineName + " " + ex.ToString(), "ERROR");
            }
            return dt;
        }

        // ======================== Tipo Detalle Marca Comida ========================

        public void SincronizarTipoDetalleMarcaComida()
        {
            Log("Comprobando sincronización de Tipos detalle marcas comidas...");
            Herramientas h = new Herramientas();
            try
            {
                TipoDetalleMarcaComida tdmc = new TipoDetalleMarcaComida();
                DataTable dtNube;
                DataTable dtLocal;
                string fechaActualizacion = "";
                dtNube = tdmc.tiposDetalleMarcaComidaNube(_fechaUltimaActualizacion);
                if (dtNube != null)
                {
                    if (dtNube.Rows.Count > 0)
                    {
                        for (int i = 0; i < dtNube.Rows.Count; i++)
                        {
                            dtLocal = comprobarTipoDetalleMarcaComidaLocal(int.Parse(dtNube.Rows[i][0].ToString()));
                            if (dtLocal != null)
                            {
                                if (dtLocal.Rows.Count > 0)
                                {
                                    if (!comprobarFechasEdicion(DateTime.Parse(dtLocal.Rows[0][2].ToString()), DateTime.Parse(dtNube.Rows[i][2].ToString())))
                                    {
                                        fechaActualizacion = h.formatearFecha(DateTime.Parse(dtNube.Rows[i][2].ToString()), true);
                                        tdmc.editarTipoDetalleMarcaComida(int.Parse(dtNube.Rows[i][0].ToString()), dtNube.Rows[i][1].ToString(), fechaActualizacion);
                                    }
                                }
                                else
                                {
                                    fechaActualizacion = h.formatearFecha(DateTime.Parse(dtNube.Rows[i][2].ToString()), true);
                                    tdmc.crearTipoDetalleMarcaComida(int.Parse(dtNube.Rows[i][0].ToString()), dtNube.Rows[i][1].ToString(), fechaActualizacion);
                                }
                            }
                        }
                    }
                    Log("Tipos detalle marcas comidas sincronizados...");
                }
                else
                {
                    generarLog("comprobarTipoDetalleMarcaComida() Error al traer los datos de la nube. Equipo: " + Environment.MachineName, "ERROR");
                }
            }
            catch (Exception ex)
            {
                generarLog("comprobarTipoDetalleMarcaComida() " + Environment.MachineName + " " + ex.ToString(), "ERROR");
            }
        }

        private DataTable comprobarTipoDetalleMarcaComidaLocal(int idtipoMarcaDetalleLocal)
        {
            DataTable dt;
            TipoDetalleMarcaComida tdmc = new TipoDetalleMarcaComida();
            try
            {
                dt = tdmc.tiposDetalleMarcaComidaLocalPorID(idtipoMarcaDetalleLocal);
            }
            catch (Exception ex)
            {
                dt = null;
                generarLog("comprobarTipoDetalleMarcaComidaLocal() " + Environment.MachineName + " " + ex.ToString(), "ERROR");
            }
            return dt;
        }

        // ======================== Tipo Turno ========================

        public void SincronizarTipoTurno()
        {
            Log("Comprobando sincronización de tipos de turno...");
            Herramientas h = new Herramientas();
            try
            {
                DataTable dtNube;
                DataTable dtLocal;
                string fechaActualizacion = "";
                TipoTurno tt = new TipoTurno();
                dtNube = tt.traerTipoTurnoNube(_fechaUltimaActualizacion);
                if (dtNube != null)
                {
                    if (dtNube.Rows.Count > 0)
                    {
                        for (int i = 0; i < dtNube.Rows.Count; i++)
                        {
                            dtLocal = comprobarTipoTurnoLocal(int.Parse(dtNube.Rows[i][0].ToString()));
                            if (dtLocal != null)
                            {
                                if (dtLocal.Rows.Count > 0)
                                {
                                    if (!comprobarFechasEdicion(DateTime.Parse(dtLocal.Rows[0][6].ToString()), (DateTime.Parse(dtNube.Rows[i][6].ToString()))))
                                    {
                                        fechaActualizacion = h.formatearFecha(DateTime.Parse(dtNube.Rows[i][6].ToString()), true);
                                        tt.actualizarTipoTurnoLocal(int.Parse(dtNube.Rows[i][0].ToString()), dtNube.Rows[i][1].ToString(), int.Parse(dtNube.Rows[i][2].ToString()),
                                            dtNube.Rows[i][3].ToString(), dtNube.Rows[i][4].ToString(), fechaActualizacion);
                                    }
                                }
                                else
                                {
                                    fechaActualizacion = h.formatearFecha(DateTime.Parse(dtNube.Rows[i][6].ToString()), true);
                                    tt.agregarTipoTurnoLocal(int.Parse(dtNube.Rows[i][0].ToString()), dtNube.Rows[i][1].ToString(), int.Parse(dtNube.Rows[i][2].ToString()),
                                        dtNube.Rows[i][3].ToString(), dtNube.Rows[i][4].ToString(), fechaActualizacion);
                                }
                            }
                        }
                    }
                    Log("Tipos de turno sincronizados...");
                }
            }
            catch (Exception ex)
            {
                generarLog("comprobarTipoTurno() " + ex.ToString(), "ERROR");
            }
        }

        private DataTable comprobarTipoTurnoLocal(int idtipoTurno)
        {
            DataTable dt;
            TipoTurno tt = new TipoTurno();
            try
            {
                dt = tt.traerTipoTurnoPorID(idtipoTurno);
            }
            catch (Exception ex)
            {
                dt = null;
                generarLog("comprobarTipoTurnoLocal() " + Environment.MachineName + " " + ex.ToString(), "ERROR");
            }
            return dt;
        }

        // ======================== Resolucion Marca ========================

        public void SincronizarResolucionMarca()
        {
            Log("Comprobando sincronización de resoluciones...");
            Herramientas h = new Herramientas();
            try
            {
                DataTable dtNube;
                DataTable dtLocal;
                ResolucionMarca rm = new ResolucionMarca();
                dtNube = rm.traerResolucionesMarcaServidor(_fechaUltimaActualizacion);
                if (dtNube != null)
                {
                    if (dtNube.Rows.Count > 0)
                    {
                        for (int i = 0; i < dtNube.Rows.Count; i++)
                        {
                            dtLocal = comprobaResolucionMarcaLocal(int.Parse(dtNube.Rows[i][0].ToString()));
                            if (dtLocal != null)
                            {
                                if (dtLocal.Rows.Count > 0)
                                {
                                    if (!comprobarFechasEdicion(DateTime.Parse(dtLocal.Rows[0][3].ToString()), (DateTime.Parse(dtNube.Rows[i][3].ToString()))))
                                    {
                                        rm.actualizarResolucionMarca(dtNube.Rows[i][1].ToString(), DateTime.Parse(dtNube.Rows[i][3].ToString()), int.Parse(dtNube.Rows[i][0].ToString()));
                                    }
                                }
                                else
                                {
                                    rm.agregarResolucionMarca(dtNube.Rows[i][1].ToString(), DateTime.Parse(dtNube.Rows[i][2].ToString()), DateTime.Parse(dtNube.Rows[i][3].ToString()), int.Parse(dtNube.Rows[i][0].ToString()));
                                }
                            }
                        }
                    }
                    Log("Resoluciones sincronizadas...");
                }
            }
            catch (Exception ex)
            {
                generarLog("comprobarResolucionMarca() " + ex.ToString(), "ERROR");
            }
        }

        private DataTable comprobaResolucionMarcaLocal(int idresolucionMarca)
        {
            DataTable dt;
            ResolucionMarca rm = new ResolucionMarca();
            try
            {
                dt = rm.traerResolucionesMarcaLocalPorID(idresolucionMarca);
            }
            catch (Exception ex)
            {
                dt = null;
                generarLog("comprobaResolucionMarcaLocal() " + Environment.MachineName + " " + ex.ToString(), "ERROR");
            }
            return dt;
        }

        // ======================== Tipo Rol Usuario ========================

        public void SincronizarTipoRolUsuario()
        {
            Log("Comprobando sincronización de Tipos roles de usuario...");
            Herramientas h = new Herramientas();
            try
            {
                DataTable dtNube;
                DataTable dtLocal;
                string fechaActualizacion = "";
                TipoRolUsuario tru = new TipoRolUsuario();
                dtNube = tru.traerTipoRolUsuarioDesdeNube(_fechaUltimaActualizacion);
                if (dtNube != null)
                {
                    if (dtNube.Rows.Count > 0)
                    {
                        for (int i = 0; i < dtNube.Rows.Count; i++)
                        {
                            dtLocal = comprobarTipoRolUsuarioLocal(int.Parse(dtNube.Rows[i][0].ToString()));
                            if (dtLocal != null)
                            {
                                if (dtLocal.Rows.Count > 0)
                                {
                                    if (!comprobarFechasEdicion(DateTime.Parse(dtLocal.Rows[0][1].ToString()), (DateTime.Parse(dtNube.Rows[i][3].ToString()))))
                                    {
                                        fechaActualizacion = h.formatearFecha(DateTime.Parse(dtNube.Rows[i][3].ToString()), true);
                                        tru.editarTipoRolUsuarioLocal(int.Parse(dtNube.Rows[i][0].ToString()), dtNube.Rows[i][1].ToString(), fechaActualizacion);
                                    }
                                }
                                else
                                {
                                    fechaActualizacion = h.formatearFecha(DateTime.Parse(dtNube.Rows[i][3].ToString()), true);
                                    tru.agregarTipoRolUsuarioLocal(int.Parse(dtNube.Rows[i][0].ToString()), dtNube.Rows[i][1].ToString(), fechaActualizacion);
                                }
                            }
                        }
                    }
                    Log("Tipos de roles de usuarios sincronizados...");
                }
            }
            catch (Exception ex)
            {
                generarLog("comprobarTipoRolUsuario() " + ex.ToString(), "ERROR");
            }
        }

        private DataTable comprobarTipoRolUsuarioLocal(int idtipoRolUsuario)
        {
            DataTable dt;
            TipoRolUsuario tru = new TipoRolUsuario();
            try
            {
                dt = tru.traerTipoRolUsuarioPorID(idtipoRolUsuario);
            }
            catch (Exception ex)
            {
                dt = null;
                generarLog("comprobarTipoRolUsuarioLocal() " + Environment.MachineName + " " + ex.ToString(), "ERROR");
            }
            return dt;
        }

        // ======================== Categoria Alerta ========================

        public void SincronizarCategoriaAlerta()
        {
            try
            {
                CategoriaAlerta categoria = new CategoriaAlerta();
                string ultimoUpdatedCategoriaAlerta = getUltimoUpdatedCategoriaAlerta();
                DataTable dtNube = categoria.traerCategoriaAlertaNube(ultimoUpdatedCategoriaAlerta);

                if (dtNube == null)
                {
                    generarLog("comprobarCategoriaAlerta() No se pudo conectar con la nube " + Environment.MachineName, "ERROR");
                    return;
                }

                if (dtNube.Rows.Count == 0)
                {
                    return;
                }

                int idregistroCategoriaNube;
                for (int i = 0; i < dtNube.Rows.Count; i++)
                {
                    idregistroCategoriaNube = int.Parse(dtNube.Rows[i][0].ToString());
                    DataTable dtLocal = comprobarCategoriaLocal(idregistroCategoriaNube);

                    if (dtLocal == null)
                    {
                        continue;
                    }

                    if (dtLocal.Rows.Count > 0 && !comprobarFechasEdicion(DateTime.Parse(dtLocal.Rows[0][0].ToString()), (DateTime.Parse(dtNube.Rows[i][4].ToString()))))
                    {
                        Herramientas h = new Herramientas();
                        string fechaActualizacion = h.formatearFecha(DateTime.Parse(dtNube.Rows[i][3].ToString()), true);
                        categoria.editarCategoriaAlertaLocal(dtNube.Rows[i][1].ToString(), int.Parse(dtNube.Rows[i][2].ToString()), fechaActualizacion, int.Parse(dtNube.Rows[i][0].ToString()));
                        continue;
                    }

                    if (dtLocal.Rows.Count == 0)
                    {
                        Herramientas h = new Herramientas();
                        string fechaCreacion = h.formatearFecha(DateTime.Parse(dtNube.Rows[i][3].ToString()), true);
                        string fechaActualizacion = h.formatearFecha(DateTime.Parse(dtNube.Rows[i][4].ToString()), true);
                        categoria.agregarCategoriaAlertaLocal(int.Parse(dtNube.Rows[i][0].ToString()), dtNube.Rows[i][1].ToString(), int.Parse(dtNube.Rows[i][2].ToString()), fechaCreacion, fechaActualizacion);
                    }
                }
            }
            catch (Exception ex)
            {
                generarLog("comprobarCategoriaAlerta() " + Environment.MachineName + " " + ex.ToString(), "ERROR");
            }
        }

        private string getUltimoUpdatedCategoriaAlerta()
        {
            CategoriaAlerta categoria = new CategoriaAlerta();
            DataTable dtFecha = categoria.traerUltimoUpdatedAtCategoriaAlerta();
            string ultimoUpdated = _fechaUltimaActualizacion;

            if (dtFecha == null)
            {
                return _fechaUltimaActualizacion;
            }

            if (dtFecha.Rows.Count > 0)
            {
                Herramientas h = new Herramientas();
                try
                {
                    ultimoUpdated = h.formatearFecha(DateTime.Parse(dtFecha.Rows[0][0].ToString()), true);
                }
                catch (Exception)
                {
                    ultimoUpdated = _fechaUltimaActualizacion;
                }
            }
            return ultimoUpdated;
        }

        private DataTable comprobarCategoriaLocal(int idcategoriaAlerta)
        {
            DataTable dt;
            CategoriaAlerta categoria = new CategoriaAlerta();
            try
            {
                dt = categoria.traerUpdatedAtCategoriaAlertaPorID(idcategoriaAlerta);
            }
            catch (Exception ex)
            {
                dt = null;
                generarLog("comprobarCategoriaLocal() " + Environment.MachineName + " " + ex.ToString(), "ERROR");
            }
            return dt;
        }

        // ======================== Tipo Reloj ========================

        public void SincronizarTipoReloj()
        {
            try
            {
                Log("Comprobando Sincronización de Tipos de Relojes...");
                TipoReloj tr = new TipoReloj();
                DataTable dtNube = traerDatosTipoRelojDesdeNube();
                if (dtNube == null)
                {
                    return;
                }

                if (dtNube.Rows.Count == 0)
                {
                    Log("Tipos de Relojes Sincronizados...");
                    return;
                }

                Herramientas h = new Herramientas();
                Log("Sincronizando de Tipos de Relojes...");
                for (int i = 0; i < dtNube.Rows.Count; i++)
                {
                    DataTable dtTipoRelojActualizarLocal = tr.traerTipoRelojLocalPorID(int.Parse(dtNube.Rows[i][0].ToString()));

                    if (dtTipoRelojActualizarLocal == null)
                    {
                        continue; // skip this row only — was return (aborting all remaining rows)
                    }

                    if (dtTipoRelojActualizarLocal.Rows.Count == 0)
                    {
                        string created_at = h.formatearFecha(DateTime.Parse(dtNube.Rows[i][2].ToString()), true);
                        string updated_at = h.formatearFecha(DateTime.Parse(dtNube.Rows[i][3].ToString()), true);
                        if (!tr.guardarTipoRelojLocal(int.Parse(dtNube.Rows[i][0].ToString()), dtNube.Rows[i][1].ToString(), created_at, updated_at))
                        {
                            generarLog("comprobarTipoReloj() " + Environment.MachineName, "ERROR");
                        }
                        continue;
                    }

                    if (!comprobarFechasEdicion(DateTime.Parse(dtTipoRelojActualizarLocal.Rows[0][3].ToString()), DateTime.Parse(dtNube.Rows[i][3].ToString())))
                    {
                        string updated_at = h.formatearFecha(DateTime.Parse(dtNube.Rows[i][3].ToString()), true);
                        tr.actualizarTipoRelojLocal(int.Parse(dtNube.Rows[i][0].ToString()), dtNube.Rows[i][1].ToString(), updated_at);
                    }
                }
                Log("Tipos de Relojes Sincronizados...");
            }
            catch (Exception ex)
            {
                generarLog("comprobarTipoReloj() " + Environment.MachineName + " " + ex.ToString(), "ERROR");
            }
        }

        private DataTable traerDatosTipoRelojDesdeNube()
        {
            TipoReloj tr = new TipoReloj();
            DataTable dtFecha = tr.traerUltimoUpdatedAtTipoReloj();
            string ultimoUpdated = _fechaUltimaActualizacion;
            if (dtFecha == null)
            {
                return null;
            }

            if (dtFecha.Rows.Count > 0)
            {
                Herramientas h = new Herramientas();
                try
                {
                    ultimoUpdated = h.formatearFecha(DateTime.Parse(dtFecha.Rows[0][0].ToString()).AddHours(-3), true);
                }
                catch (Exception)
                {
                    ultimoUpdated = _fechaUltimaActualizacion;
                }
            }

            DataTable dt = tr.traerTipoRelojNube(ultimoUpdated);
            return dt;
        }

        // ======================== Perfil Casino ========================

        public void SincronizarPerfilesCasino()
        {
            Log("Comprobando perfiles base de casino...");
            Herramientas h = new Herramientas();
            try
            {
                PerfilCasino pc = new PerfilCasino();
                DataTable dtLocal;
                DataTable dtNube = pc.traerPerfilesDeCasinoNube(traerUltimoUpdatePerfilCasinoLocal());
                if (dtNube != null)
                {
                    if (dtNube.Rows.Count > 0)
                    {
                        string fechaCreacion = "";
                        string fechaActualizacion = "";
                        string nombre = "";
                        int idperfilCasino = 0;
                        int habilitado = 0;
                        Log("Actualizando perfiles base de casino....");
                        for (int i = 0; i < dtNube.Rows.Count; i++)
                        {
                            idperfilCasino = int.Parse(dtNube.Rows[i][0].ToString());
                            dtLocal = traerPerfilCasinoLocal(idperfilCasino);
                            if (dtLocal != null)
                            {
                                if (dtLocal.Rows.Count > 0)
                                {
                                    if (!comprobarFechasEdicion(DateTime.Parse(dtLocal.Rows[0][4].ToString()), DateTime.Parse(dtNube.Rows[i][4].ToString())))
                                    {
                                        nombre = dtNube.Rows[i][1].ToString();
                                        habilitado = int.Parse(dtNube.Rows[i][2].ToString());
                                        fechaActualizacion = h.formatearFecha(DateTime.Parse(dtNube.Rows[i][4].ToString()), true);
                                        pc.editarPerfilDeCasino(idperfilCasino, nombre, habilitado, fechaActualizacion);
                                    }
                                }
                                else
                                {
                                    nombre = dtNube.Rows[i][1].ToString();
                                    habilitado = int.Parse(dtNube.Rows[i][2].ToString());
                                    fechaCreacion = h.formatearFecha(DateTime.Parse(dtNube.Rows[i][3].ToString()), true);
                                    fechaActualizacion = h.formatearFecha(DateTime.Parse(dtNube.Rows[i][4].ToString()), true);
                                    pc.agregarPerfilDeCasino(idperfilCasino, nombre, habilitado, fechaCreacion, fechaActualizacion);
                                }
                            }
                        }
                    }
                }
                else
                {
                    Log("Error al obtener los perfiles base de casino desde la nube...");
                    generarLog("PerfilCasino() Error conexión a base de datos ", "ERROR");
                }
                Log("Perfiles base de casino sincronizados....");
            }
            catch (Exception ex)
            {
                generarLog("comprobarPerfilCasino() " + ex.ToString(), "ERROR");
            }
        }

        private DataTable traerPerfilCasinoLocal(int idperfilCasino)
        {
            DataTable dt;
            try
            {
                PerfilCasino pc = new PerfilCasino();
                dt = pc.traerPerfilCasinoPorID(idperfilCasino);
                if (dt == null)
                {
                    generarLog("traerPerfilCasinoLocal() Error conexión base de datos", "ERROR");
                }
            }
            catch (Exception ex)
            {
                dt = null;
                generarLog("traerPerfilCasinoLocal() " + ex.ToString(), "ERROR");
            }
            return dt;
        }

        private string traerUltimoUpdatePerfilCasinoLocal()
        {
            PerfilCasino pc = new PerfilCasino();
            DataTable dtFecha = pc.traerUltimoUpdatedAtPerfilDeCasino();
            string ultimoUpdated = _fechaUltimaActualizacion;
            if (dtFecha != null)
            {
                if (dtFecha.Rows.Count > 0)
                {
                    Herramientas h = new Herramientas();
                    try
                    {
                        ultimoUpdated = h.formatearFecha(DateTime.Parse(dtFecha.Rows[0][0].ToString()), true);
                    }
                    catch (Exception)
                    {
                        ultimoUpdated = _fechaUltimaActualizacion;
                    }
                }
            }
            return ultimoUpdated;
        }

        // ======================== TipoComida has TipoInhabilitacion ========================

        public void SincronizarTipoComidaHasTipoInhabilitacion()
        {
            Log("Comprobando tipo inhabilitaciones por tipos de comida...");
            Herramientas h = new Herramientas();
            try
            {
                TipoComidaHasTipoInhabilitacionMarca tchtim = new TipoComidaHasTipoInhabilitacionMarca();
                DataTable dtLocal;
                DataTable dtNube = tchtim.traerTipoComidaHasTipoInhabilitacionMarcaNube(traerUltimoUpdateTipoComidaHasTipoInhabilitacionMarcaLocal());
                if (dtNube != null)
                {
                    if (dtNube.Rows.Count > 0)
                    {
                        int idtipoComida_has_tipoInhabilitacionMarca = 0;
                        int tipoComida_idtipoComida = 0;
                        int tipoMarca_idtipoMarca = 0;
                        int tipoInhabilitacionMarca_idtipoInhabilitacionMarca = 0;
                        string fechaCreacion = "";
                        string fechaActualizacion = "";
                        int habilitado = 0;
                        Log("Actualizando tipo inhabilitaciones por tipos de comida....");
                        for (int i = 0; i < dtNube.Rows.Count; i++)
                        {
                            idtipoComida_has_tipoInhabilitacionMarca = int.Parse(dtNube.Rows[i][0].ToString());
                            dtLocal = traerTipoComidaHasTipoInhabilitacionMarcaLocal(idtipoComida_has_tipoInhabilitacionMarca);
                            if (dtLocal != null)
                            {
                                if (dtLocal.Rows.Count > 0)
                                {
                                    if (!comprobarFechasEdicion(DateTime.Parse(dtLocal.Rows[0][6].ToString()), DateTime.Parse(dtNube.Rows[i][6].ToString())))
                                    {
                                        tipoComida_idtipoComida = int.Parse(dtNube.Rows[i][1].ToString());
                                        tipoInhabilitacionMarca_idtipoInhabilitacionMarca = int.Parse(dtNube.Rows[i][2].ToString());
                                        tipoMarca_idtipoMarca = int.Parse(dtNube.Rows[i][3].ToString());
                                        habilitado = int.Parse(dtNube.Rows[i][4].ToString());
                                        fechaActualizacion = h.formatearFecha(DateTime.Parse(dtNube.Rows[i][6].ToString()), true);
                                        tchtim.actualizarTipoComidaHasTipoInhabilitacionMarca(idtipoComida_has_tipoInhabilitacionMarca, tipoComida_idtipoComida, tipoInhabilitacionMarca_idtipoInhabilitacionMarca, tipoMarca_idtipoMarca, habilitado, fechaActualizacion);
                                    }
                                }
                                else
                                {
                                    tipoComida_idtipoComida = int.Parse(dtNube.Rows[i][1].ToString());
                                    tipoInhabilitacionMarca_idtipoInhabilitacionMarca = int.Parse(dtNube.Rows[i][2].ToString());
                                    tipoMarca_idtipoMarca = int.Parse(dtNube.Rows[i][3].ToString());
                                    habilitado = int.Parse(dtNube.Rows[i][4].ToString());
                                    fechaCreacion = h.formatearFecha(DateTime.Parse(dtNube.Rows[i][5].ToString()), true);
                                    fechaActualizacion = h.formatearFecha(DateTime.Parse(dtNube.Rows[i][6].ToString()), true);
                                    tchtim.agregarTipoComidaHasTipoInhabilitacionMarca(idtipoComida_has_tipoInhabilitacionMarca, tipoComida_idtipoComida, tipoInhabilitacionMarca_idtipoInhabilitacionMarca, tipoMarca_idtipoMarca, habilitado, fechaCreacion, fechaActualizacion);
                                }
                            }
                        }
                    }
                }
                else
                {
                    Log("Error al obtener los perfiles base de casino desde la nube...");
                    generarLog("comprobarTipoComidaHasTipoInhabilitacionMarca() Error conexión a base de datos ", "ERROR");
                }
                Log("Tipo inhabilitaciones por tipos de comida sincronizados....");
            }
            catch (Exception ex)
            {
                generarLog("comprobarTipoComidaHasTipoInhabilitacionMarca() " + ex.ToString(), "ERROR");
            }
        }

        private DataTable traerTipoComidaHasTipoInhabilitacionMarcaLocal(int idtipoComida_has_tipoInhabilitacionMarca)
        {
            DataTable dt;
            try
            {
                TipoComidaHasTipoInhabilitacionMarca tchtim = new TipoComidaHasTipoInhabilitacionMarca();
                dt = tchtim.traerTipoComidaHasTipoInhabilitacionMarcaPorID(idtipoComida_has_tipoInhabilitacionMarca);
                if (dt == null)
                {
                    generarLog("traerTipoComidaHasTipoInhabilitacionMarcaLocal() Error conexión base de datos", "ERROR");
                }
            }
            catch (Exception ex)
            {
                dt = null;
                generarLog("traerTipoComidaHasTipoInhabilitacionMarcaLocal() " + ex.ToString(), "ERROR");
            }
            return dt;
        }

        private string traerUltimoUpdateTipoComidaHasTipoInhabilitacionMarcaLocal()
        {
            TipoComidaHasTipoInhabilitacionMarca tchtim = new TipoComidaHasTipoInhabilitacionMarca();
            DataTable dtFecha = tchtim.traerUltimoUpdatedTipoComidaHasTipoInhabilitacionMarca();
            string ultimoUpdated = _fechaUltimaActualizacion;
            if (dtFecha != null)
            {
                if (dtFecha.Rows.Count > 0)
                {
                    Herramientas h = new Herramientas();
                    try
                    {
                        ultimoUpdated = h.formatearFecha(DateTime.Parse(dtFecha.Rows[0][0].ToString()), true);
                    }
                    catch (Exception)
                    {
                        ultimoUpdated = _fechaUltimaActualizacion;
                    }
                }
            }
            return ultimoUpdated;
        }

        // ======================== Detalle Perfil Casino ========================

        public void SincronizarDetallePerfilCasino()
        {
            Log("Comprobando detalles perfiles base de casino...");
            Herramientas h = new Herramientas();
            try
            {
                DetallePerfilCasino dpc = new DetallePerfilCasino();
                DataTable dtLocal;
                DataTable dtNube = dpc.traerDetallePerfilCasinoNube(traerUltimoUpdateDetallePerfilCasinoLocal());
                if (dtNube != null)
                {
                    if (dtNube.Rows.Count > 0)
                    {
                        int iddetallePerfilCasino = 0;
                        int perfilCasino_idperfilCasino = 0;
                        int tipoComida_idtipoComida = 0;
                        int numeroDia = 0;
                        int habilitado = 0;
                        string fechaCreacion = "";
                        string fechaActualizacion = "";
                        Log("Actualizando detalles perfiles base de casino....");
                        for (int i = 0; i < dtNube.Rows.Count; i++)
                        {
                            iddetallePerfilCasino = int.Parse(dtNube.Rows[i][0].ToString());
                            dtLocal = traerDetallePerfilCasinoLocal(iddetallePerfilCasino);
                            if (dtLocal != null)
                            {
                                if (dtLocal.Rows.Count > 0)
                                {
                                    if (!comprobarFechasEdicion(DateTime.Parse(dtLocal.Rows[0][6].ToString()), DateTime.Parse(dtNube.Rows[i][6].ToString())))
                                    {
                                        perfilCasino_idperfilCasino = int.Parse(dtNube.Rows[i][1].ToString());
                                        tipoComida_idtipoComida = int.Parse(dtNube.Rows[i][2].ToString());
                                        numeroDia = int.Parse(dtNube.Rows[i][3].ToString());
                                        habilitado = int.Parse(dtNube.Rows[i][4].ToString());
                                        fechaActualizacion = h.formatearFecha(DateTime.Parse(dtNube.Rows[i][6].ToString()), true);
                                        dpc.editarDetallePerfilCasino(iddetallePerfilCasino, perfilCasino_idperfilCasino, tipoComida_idtipoComida, numeroDia, habilitado, fechaActualizacion);
                                    }
                                }
                                else
                                {
                                    perfilCasino_idperfilCasino = int.Parse(dtNube.Rows[i][1].ToString());
                                    tipoComida_idtipoComida = int.Parse(dtNube.Rows[i][2].ToString());
                                    numeroDia = int.Parse(dtNube.Rows[i][3].ToString());
                                    habilitado = int.Parse(dtNube.Rows[i][4].ToString());
                                    fechaCreacion = h.formatearFecha(DateTime.Parse(dtNube.Rows[i][5].ToString()), true);
                                    fechaActualizacion = h.formatearFecha(DateTime.Parse(dtNube.Rows[i][6].ToString()), true);
                                    dpc.agregarDetallePerfilCasino(iddetallePerfilCasino, perfilCasino_idperfilCasino, tipoComida_idtipoComida, numeroDia, habilitado, fechaCreacion, fechaActualizacion);
                                }
                            }
                        }
                    }
                }
                else
                {
                    Log("Error al obtener los detalles perfiles base de casino desde la nube...");
                    generarLog("PerfilCasino() Error conexión a base de datos ", "ERROR");
                }
                Log("Detalles de Perfiles base de casino sincronizados....");
            }
            catch (Exception ex)
            {
                generarLog("comprobarDetallePerfilCasino() " + ex.ToString(), "ERROR");
            }
        }

        private DataTable traerDetallePerfilCasinoLocal(int idperfilCasino)
        {
            DataTable dt;
            try
            {
                DetallePerfilCasino dpc = new DetallePerfilCasino();
                dt = dpc.traerDetallePerfilCasinoPorID(idperfilCasino);
                if (dt == null)
                {
                    generarLog("traerDetallePerfilCasinoLocal() Error conexión base de datos", "ERROR");
                }
            }
            catch (Exception ex)
            {
                dt = null;
                generarLog("traerDetallePerfilCasinoLocal() " + ex.ToString(), "ERROR");
            }
            return dt;
        }

        private string traerUltimoUpdateDetallePerfilCasinoLocal()
        {
            DetallePerfilCasino dpc = new DetallePerfilCasino();
            DataTable dtFecha = dpc.traerUltimoUpdatedAtDetallePerfilDeCasino();
            string ultimoUpdated = _fechaUltimaActualizacion;
            if (dtFecha != null)
            {
                if (dtFecha.Rows.Count > 0)
                {
                    Herramientas h = new Herramientas();
                    try
                    {
                        ultimoUpdated = h.formatearFecha(DateTime.Parse(dtFecha.Rows[0][0].ToString()), true);
                    }
                    catch (Exception)
                    {
                        ultimoUpdated = _fechaUltimaActualizacion;
                    }
                }
            }
            return ultimoUpdated;
        }

        // ======================== Correo Alerta ========================

        public void SincronizarCorreosAlertas()
        {
            Log("Comprobando datos de correos de alerta...");
            try
            {
                DataTable dtNube;
                CorreoAlerta ca = new CorreoAlerta();
                Herramientas h = new Herramientas();
                DataTable dtLocal;
                string fechaFormateada = "";
                dtNube = ca.traerCorreosAlertasNube(_fechaUltimaActualizacion);
                if (dtNube != null)
                {
                    if (dtNube.Rows.Count > 0)
                    {
                        Log("Sincronizando datos de correos de alerta...");
                        for (int i = 0; i < dtNube.Rows.Count; i++)
                        {
                            dtLocal = consultarCorreoAlertaLocal(int.Parse(dtNube.Rows[i][0].ToString()));
                            if (dtLocal != null)
                            {
                                if (dtLocal.Rows.Count > 0)
                                {
                                    if (!comprobarFechasEdicion(DateTime.Parse(dtLocal.Rows[0][4].ToString()), DateTime.Parse(dtNube.Rows[i][4].ToString())))
                                    {
                                        fechaFormateada = h.formatearFecha(DateTime.Parse(dtNube.Rows[i][4].ToString()), true);
                                        ca.actualizarCorreoAlertaLocal(int.Parse(dtNube.Rows[i][0].ToString()), dtNube.Rows[i][1].ToString(), dtNube.Rows[i][2].ToString(), int.Parse(dtNube.Rows[i][3].ToString()), fechaFormateada);
                                    }
                                }
                                else
                                {
                                    fechaFormateada = h.formatearFecha(DateTime.Parse(dtNube.Rows[i][4].ToString()), true);
                                    ca.agregarCorreoAlertaLocal(int.Parse(dtNube.Rows[i][0].ToString()), dtNube.Rows[i][1].ToString(), dtNube.Rows[i][2].ToString(), int.Parse(dtNube.Rows[i][3].ToString()), fechaFormateada);
                                }
                            }
                        }
                    }
                    Log("Datos de correos de alerta sincronizados...");
                }
            }
            catch (Exception ex)
            {
                generarLog("comprobarSincronizacionCorreosAlertas() " + ex.ToString(), "ERROR");
            }
        }

        private DataTable consultarCorreoAlertaLocal(int idcorreoAlerta)
        {
            DataTable dt;
            try
            {
                CorreoAlerta ca = new CorreoAlerta();
                dt = ca.traerCorreosAlertasPorIDLocal(idcorreoAlerta);
            }
            catch (Exception ex)
            {
                dt = null;
                generarLog("consultarCorreoAlertaLocal() " + ex.ToString(), "ERROR");
            }
            return dt;
        }

        // ======================== Alerta Error ========================

        public void SincronizarAlertasError()
        {
            Log("Comprobando sincronización de alertas...");
            try
            {
                DataTable dt;
                AlertaError ar = new AlertaError();
                Herramientas h = new Herramientas();
                dt = ar.traerAlertasSinRespaldar();
                string fechaFormateada = "";
                if (dt != null)
                {
                    if (dt.Rows.Count > 0)
                    {
                        Log("Sincronizando alertas...");
                        int categoriaAlerta;
                        for (int i = 0; i < dt.Rows.Count; i++)
                        {
                            fechaFormateada = h.formatearFecha(DateTime.Parse(dt.Rows[i][7].ToString()), true);

                            try
                            {
                                categoriaAlerta = int.Parse(dt.Rows[i][9].ToString());
                            }
                            catch (Exception)
                            {
                                categoriaAlerta = ConfiguracionesConstantes.CATEGORIA_NOTIFICACION_OTROS;
                            }

                            if (ar.agregarAlertaNube(dt.Rows[i][1].ToString(), int.Parse(dt.Rows[i][2].ToString()), int.Parse(dt.Rows[i][3].ToString()), int.Parse(dt.Rows[i][4].ToString()), 1, int.Parse(dt.Rows[i][6].ToString()), fechaFormateada, dt.Rows[i][8].ToString(), categoriaAlerta))
                            {
                                ar.actualizarEstadoRespaldadoAlertaLocal(int.Parse(dt.Rows[i][0].ToString()));
                            }
                        }
                    }
                    Log("Alertas Sincronizadas...");
                }
            }
            catch (Exception ex)
            {
                generarLog("comprobarSincronizacionAlertasError() " + ex.ToString(), "ERROR");
            }
        }

        // ======================== Empresa ========================

        public void SincronizarEmpresas()
        {
            Log("Comprobando datos de Empresa...");
            try
            {
                Empresa emp = new Empresa();
                Herramientas h = new Herramientas();
                DataTable dt = emp.traerDatosEmpresa();
                DataTable dtNube = emp.traerDatosEmpresaDesdeServidor(traerUltimoUpdateEmpresa());

                if (dt != null)
                {
                    if (dt.Rows.Count > 0)
                    {
                        if (dtNube != null)
                        {
                            if (dtNube.Rows.Count > 0)
                            {
                                for (int i = 0; i < dtNube.Rows.Count; i++)
                                {
                                    DataRow[] filasEncontradas = empresaExisteLocalmente(int.Parse(dtNube.Rows[i][0].ToString()), dt);
                                    if (filasEncontradas.Length > 0)
                                    {
                                        if (!comprobarFechasEdicion(DateTime.Parse(dtNube.Rows[i][10].ToString()), DateTime.Parse(filasEncontradas[0][10].ToString())))
                                        {
                                            emp.editarDatosEmpresa(dtNube.Rows[i][1].ToString(), dtNube.Rows[i][2].ToString(), dtNube.Rows[i][3].ToString(), dtNube.Rows[i][4].ToString(),
                                            dtNube.Rows[i][5].ToString(), dtNube.Rows[i][6].ToString(), dtNube.Rows[i][7].ToString(),
                                            dtNube.Rows[i][8].ToString(), dtNube.Rows[i][9].ToString(), 1,
                                            int.Parse(dtNube.Rows[i][0].ToString()),
                                            h.formatearFecha(DateTime.Parse(dtNube.Rows[i][10].ToString()), true),
                                            dtNube.Rows[i][11].ToString());
                                        }
                                    }
                                    else
                                    {
                                        emp.agregarDatosEmpresa(dtNube.Rows[i][1].ToString(), dtNube.Rows[i][2].ToString(), dtNube.Rows[i][3].ToString(), dtNube.Rows[i][4].ToString(),
                                                          dtNube.Rows[i][5].ToString(), dtNube.Rows[i][6].ToString(), dtNube.Rows[i][7].ToString(), dtNube.Rows[i][8].ToString(),
                                                          dtNube.Rows[i][9].ToString(), h.formatearFecha(DateTime.Parse(dtNube.Rows[i][10].ToString()), true),
                                                          dtNube.Rows[i][11].ToString());
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        if (dtNube != null)
                        {
                            if (dtNube.Rows.Count > 0)
                            {
                                for (int i = 0; i < dtNube.Rows.Count; i++)
                                {
                                    emp.agregarDatosEmpresa(dtNube.Rows[i][1].ToString(), dtNube.Rows[i][2].ToString(), dtNube.Rows[i][3].ToString(), dtNube.Rows[i][4].ToString(),
                                                             dtNube.Rows[i][5].ToString(), dtNube.Rows[i][6].ToString(), dtNube.Rows[i][7].ToString(), dtNube.Rows[i][8].ToString(),
                                                             dtNube.Rows[i][9].ToString(), h.formatearFecha(DateTime.Parse(dtNube.Rows[i][10].ToString()), true),
                                                             dtNube.Rows[i][11].ToString());
                                }
                            }
                        }
                    }
                    Log("Datos de Empresa sincronizados...");
                }
                else
                {
                    Log("Error al comprobar sincronización de Empresa...");
                }
            }
            catch (Exception ex)
            {
                generarLog("comprobarSincronizacionEmpresa() " + ex.ToString(), "ERROR");
            }
        }

        private string traerUltimoUpdateEmpresa()
        {
            Empresa e = new Empresa();
            DataTable dtFecha = e.traerUltimoUpdatedAtEmpresa();
            string ultimoUpdated = _fechaUltimaActualizacion;
            if (dtFecha != null)
            {
                if (dtFecha.Rows.Count > 0)
                {
                    Herramientas h = new Herramientas();
                    try
                    {
                        ultimoUpdated = h.formatearFecha(DateTime.Parse(dtFecha.Rows[0][0].ToString()), true);
                    }
                    catch (Exception)
                    {
                        ultimoUpdated = _fechaUltimaActualizacion;
                    }
                }
            }
            return ultimoUpdated;
        }

        private DataRow[] empresaExisteLocalmente(int idempresa, DataTable dt)
        {
            return dt.Select("idempresa=" + idempresa);
        }

        // ======================== Sucursales ========================

        public void SincronizarSucursales()
        {
            Log("Comprobando actualización de datos sucursales...");
            try
            {
                Sucursal s = new Sucursal();
                DataTable dtNube;
                DataTable dtLocal;
                Herramientas h = new Herramientas();
                dtNube = s.traerSucursalesDesdeNube();
                if (dtNube != null)
                {
                    if (dtNube.Rows.Count > 0)
                    {
                        Log("Actualizando datos sucursales...");
                        for (int i = 0; i < dtNube.Rows.Count; i++)
                        {
                            dtLocal = s.traerSucursalLocalPorID(int.Parse(dtNube.Rows[i][0].ToString()));
                            if (dtLocal != null)
                            {
                                if (dtLocal.Rows.Count > 0)
                                {
                                    if (!comprobarFechasEdicion(DateTime.Parse(dtLocal.Rows[0][1].ToString()), DateTime.Parse(dtNube.Rows[i][13].ToString())))
                                    {
                                        s.editarDatosSucursal(dtNube.Rows[i][2].ToString(), dtNube.Rows[i][3].ToString(), dtNube.Rows[i][4].ToString(),
                                            dtNube.Rows[i][5].ToString(), dtNube.Rows[i][6].ToString(), dtNube.Rows[i][7].ToString(), dtNube.Rows[i][8].ToString(),
                                            dtNube.Rows[i][9].ToString(), dtNube.Rows[i][10].ToString(), int.Parse(dtNube.Rows[i][11].ToString()), 1, int.Parse(dtNube.Rows[i][0].ToString()),
                                             h.formatearFecha(DateTime.Parse(dtNube.Rows[i][13].ToString()), true));
                                    }
                                }
                                else
                                {
                                    s.agregarSucursal(int.Parse(dtNube.Rows[i][0].ToString()), int.Parse(dtNube.Rows[i][1].ToString()), dtNube.Rows[i][2].ToString(), dtNube.Rows[i][3].ToString(), dtNube.Rows[i][4].ToString(),
                                        dtNube.Rows[i][5].ToString(), dtNube.Rows[i][6].ToString(), dtNube.Rows[i][7].ToString(), dtNube.Rows[i][8].ToString(), dtNube.Rows[i][9].ToString(),
                                        dtNube.Rows[i][10].ToString(), int.Parse(dtNube.Rows[i][11].ToString()), 1, h.formatearFecha(DateTime.Parse(dtNube.Rows[i][13].ToString()), true));
                                }
                            }
                        }
                        Log("Datos de sucursales actualizados...");
                    }
                    else
                    {
                        Log("No se encontraron sucursales favor contactar a soporte rFlex...");
                        generarLog("Error de configuración... no hay sucursales ingresadas en la base de datos.", "ADVERTENCIA");
                    }
                }
            }
            catch (Exception ex)
            {
                generarLog("comprobarSucursales() " + ex.ToString(), "ERROR");
            }
        }

        // ======================== Sucursal Tipo Comida ========================

        public void SincronizarSucursalTipoComida()
        {
            try
            {
                Log("Comprobando Sincronización de Tipos de Comida por Sucursal...");
                SucursalTipoComida stc = new SucursalTipoComida();
                DataTable dtNube = traerDatosSucursalTipoComidaDesdeNube();
                if (dtNube == null)
                {
                    return;
                }

                if (dtNube.Rows.Count == 0)
                {
                    Log("Tipos de Comida por Sucursal Sincronizados...");
                    return;
                }

                Herramientas h = new Herramientas();
                Log("Sincronizando de Tipos de Comida por Sucursal...");
                for (int i = 0; i < dtNube.Rows.Count; i++)
                {
                    DataTable dtSucursalTipoComidaActualizarLocal = stc.traerSucursalTipoComidaPorID(int.Parse(dtNube.Rows[i][0].ToString()));

                    if (dtSucursalTipoComidaActualizarLocal == null)
                    {
                        continue; // skip this row only — was return (aborting all remaining rows)
                    }

                    if (dtSucursalTipoComidaActualizarLocal.Rows.Count == 0)
                    {
                        string created_at = h.formatearFecha(DateTime.Parse(dtNube.Rows[i][8].ToString()), true);
                        string updated_at = h.formatearFecha(DateTime.Parse(dtNube.Rows[i][9].ToString()), true);

                        if (!stc.guardarSucursalTipoComidaLocal(int.Parse(dtNube.Rows[i][0].ToString()), int.Parse(dtNube.Rows[i][1].ToString()),
                            int.Parse(dtNube.Rows[i][2].ToString()), dtNube.Rows[i][3].ToString(),
                            dtNube.Rows[i][4].ToString(), dtNube.Rows[i][5].ToString(), dtNube.Rows[i][6].ToString(), int.Parse(dtNube.Rows[i][7].ToString()), created_at, updated_at,
                            int.Parse(dtNube.Rows[i][10].ToString())))
                        {
                            generarLog("comprobarSucursalTipoComida() " + Environment.MachineName, "ERROR");
                        }

                        continue;
                    }

                    if (!comprobarFechasEdicion(DateTime.Parse(dtSucursalTipoComidaActualizarLocal.Rows[0][9].ToString()), DateTime.Parse(dtNube.Rows[i][9].ToString())))
                    {
                        string updated_at = h.formatearFecha(DateTime.Parse(dtNube.Rows[i][9].ToString()), true);
                        stc.actualizarSucursalTipoComidaLocal(int.Parse(dtNube.Rows[i][0].ToString()), int.Parse(dtNube.Rows[i][1].ToString()), int.Parse(dtNube.Rows[i][2].ToString()),
                            dtNube.Rows[i][3].ToString(), dtNube.Rows[i][4].ToString(), dtNube.Rows[i][5].ToString(), dtNube.Rows[i][6].ToString(), int.Parse(dtNube.Rows[i][7].ToString()),
                            updated_at, int.Parse(dtNube.Rows[i][10].ToString()));
                    }
                }
                Log("Tipos de Comida por Sucursal Sincronizados...");
            }
            catch (Exception ex)
            {
                generarLog("comprobarSucursalTipoComida() " + Environment.MachineName + " " + ex.ToString(), "ERROR");
            }
        }

        private DataTable traerDatosSucursalTipoComidaDesdeNube()
        {
            SucursalTipoComida stc = new SucursalTipoComida();
            DataTable dtFecha = stc.traerUltimoUpdatedAtSucursalTipoComida();
            string ultimoUpdated = _fechaUltimaActualizacion;
            if (dtFecha == null)
            {
                return null;
            }

            if (dtFecha.Rows.Count > 0)
            {
                Herramientas h = new Herramientas();
                try
                {
                    ultimoUpdated = h.formatearFecha(DateTime.Parse(dtFecha.Rows[0][0].ToString()), true);
                }
                catch (Exception)
                {
                    ultimoUpdated = _fechaUltimaActualizacion;
                }
            }

            DataTable dt = stc.traerSucursalTipoComidaNube(ultimoUpdated);
            return dt;
        }

        // ======================== Reloj ========================

        public void SincronizarReloj()
        {
            Log("Sincronizando datos reloj...");
            Herramientas h = new Herramientas();
            try
            {
                Reloj r = new Reloj();
                DataTable dtNube;
                dtNube = r.traerDatosRelojEnNubePorSincronizar(_fechaUltimaActualizacion);
                if (dtNube != null)
                {
                    if (dtNube.Rows.Count > 0)
                    {
                        DataTable dtRelojActualizarLocal;
                        for (int i = 0; i < dtNube.Rows.Count; i++)
                        {
                            dtRelojActualizarLocal = traerDatosRelojPorIdReloj(int.Parse(dtNube.Rows[i][0].ToString()));
                            if (dtRelojActualizarLocal != null)
                            {
                                if (dtRelojActualizarLocal.Rows.Count == 0)
                                {
                                    r.agregarReloj(int.Parse(dtNube.Rows[i][0].ToString()), int.Parse(dtNube.Rows[i][1].ToString()), int.Parse(dtNube.Rows[i][2].ToString()),
                                     dtNube.Rows[i][3].ToString(), dtNube.Rows[i][4].ToString(), int.Parse(dtNube.Rows[i][5].ToString()),
                                     dtNube.Rows[i][6].ToString(), dtNube.Rows[i][7].ToString(), DateTime.Parse(dtNube.Rows[i][8].ToString()),
                                     int.Parse(dtNube.Rows[i][16].ToString()), DateTime.Parse(dtNube.Rows[i][17].ToString()),
                                     int.Parse(dtNube.Rows[i][9].ToString()), int.Parse(dtNube.Rows[i][10].ToString()), int.Parse(dtNube.Rows[i][11].ToString()),
                                     int.Parse(dtNube.Rows[i][12].ToString()), 1, int.Parse(dtNube.Rows[i][14].ToString()), dtNube.Rows[i][15].ToString(),
                                     int.Parse(dtNube.Rows[i][18].ToString()), int.Parse(dtNube.Rows[i][19].ToString()), int.Parse(dtNube.Rows[i][20].ToString()),
                                     int.Parse(dtNube.Rows[i][21].ToString()), int.Parse(dtNube.Rows[i][22].ToString()), int.Parse(dtNube.Rows[i][23].ToString()),
                                     int.Parse(dtNube.Rows[i][24].ToString()), int.Parse(dtNube.Rows[i][25].ToString()));
                                }
                                else
                                {
                                    if (!comprobarFechasEdicion(DateTime.Parse(dtRelojActualizarLocal.Rows[0][17].ToString()), DateTime.Parse(dtNube.Rows[i][17].ToString())))
                                    {
                                        r.actualizarConfiguracionRelojPorID(int.Parse(dtNube.Rows[i][5].ToString()), int.Parse(dtNube.Rows[i][9].ToString()), int.Parse(dtNube.Rows[i][10].ToString()),
                                         int.Parse(dtNube.Rows[i][11].ToString()), int.Parse(dtNube.Rows[i][12].ToString()),
                                         int.Parse(dtNube.Rows[i][16].ToString()), int.Parse(dtNube.Rows[i][2].ToString()), dtNube.Rows[i][3].ToString(),
                                         DateTime.Parse(dtNube.Rows[i][17].ToString()), 1, dtNube.Rows[i][4].ToString(), int.Parse(dtNube.Rows[i][18].ToString()),
                                         dtNube.Rows[i][3].ToString(), dtNube.Rows[i][7].ToString(), dtNube.Rows[i][6].ToString(), int.Parse(dtNube.Rows[i][19].ToString()),
                                         int.Parse(dtNube.Rows[i][20].ToString()), int.Parse(dtNube.Rows[i][21].ToString()), int.Parse(dtNube.Rows[i][22].ToString()),
                                         int.Parse(dtNube.Rows[i][0].ToString()), int.Parse(dtNube.Rows[i][23].ToString()), int.Parse(dtNube.Rows[i][24].ToString()),
                                         int.Parse(dtNube.Rows[i][1].ToString()), int.Parse(dtNube.Rows[i][25].ToString()));
                                    }
                                }
                            }
                        }
                    }
                    Log("Datos relojes Nube a local sincronizados");
                }
                else
                {
                    Log("Error al obtener datos de los relojes desde la Nube...");
                    generarLog("Error al obtener datos desde la Nube de los relojes sincronizarDatosRelojLocalesANube() dtNube = NULL ", "ADVERTENCIA");
                }
            }
            catch (Exception ex)
            {
                generarLog("sincronizarDatosRelojLocalesANube()" + ex.ToString(), "ERROR");
            }
            Log("Fin sincronización relojes...");
        }

        private DataTable traerDatosRelojPorIdReloj(int idreloj)
        {
            DataTable dt;
            Reloj r = new Reloj();
            dt = r.traerDatosRelojPorIdReloj(idreloj);
            return dt;
        }

        // ======================== Personas ========================

        public void SincronizarPersonas()
        {
            Herramientas h = new Herramientas();
            try
            {
                Encriptacion e = new Encriptacion();
                Log("Comprobando sincronización de personas...");
                DataTable dtPersonasNube = traerDatosPersonaNube();
                DataTable dtPersonasLocal = traerDatosPersonaLocal();

                if (dtPersonasLocal != null)
                {
                    if (dtPersonasLocal.Rows.Count > 0)
                    {
                        Log("Sincronizando datos de trabajadores Local a Nube...");
                        int totalActualizar = dtPersonasLocal.Rows.Count;
                        int valorActual = 0;
                        for (int i = 0; i < dtPersonasLocal.Rows.Count; i++)
                        {
                            valorActual = i + 1;
                            Log("Actualizando datos de trabajadores local a nube: " + valorActual.ToString() + "/" + totalActualizar.ToString() + "...");
                            if (personaYaExisteEnNube(e.Desencriptar(dtPersonasLocal.Rows[i][1].ToString())))
                            {
                                DateTime nuevoUpdated = DateTime.Now;
                                subirDatosActualizadosANubePersona(e.Desencriptar(dtPersonasLocal.Rows[i][1].ToString()),
                                    e.Desencriptar(dtPersonasLocal.Rows[i][2].ToString()), e.Desencriptar(dtPersonasLocal.Rows[i][3].ToString()),
                                    e.Desencriptar(dtPersonasLocal.Rows[i][4].ToString()), e.Desencriptar(dtPersonasLocal.Rows[i][5].ToString()),
                                    e.Desencriptar(dtPersonasLocal.Rows[i][6].ToString()), e.Desencriptar(dtPersonasLocal.Rows[i][7].ToString()),
                                    e.Desencriptar(dtPersonasLocal.Rows[i][8].ToString()), e.Desencriptar(dtPersonasLocal.Rows[i][9].ToString()),
                                    DateTime.Parse(e.Desencriptar(dtPersonasLocal.Rows[i][10].ToString())), e.Desencriptar(dtPersonasLocal.Rows[i][11].ToString()),
                                    e.Desencriptar(dtPersonasLocal.Rows[i][12].ToString()), DateTime.Parse(dtPersonasLocal.Rows[i][13].ToString()),
                                    int.Parse(dtPersonasLocal.Rows[i][14].ToString()), nuevoUpdated,
                                    int.Parse(dtPersonasLocal.Rows[i][19].ToString()));
                            }
                            else
                            {
                                DateTime nuevoUpdated = DateTime.Now;
                                subirDatosANubePersonaSinSincronizar(e.Desencriptar(dtPersonasLocal.Rows[i][1].ToString()),
                                    e.Desencriptar(dtPersonasLocal.Rows[i][2].ToString()), e.Desencriptar(dtPersonasLocal.Rows[i][3].ToString()),
                                    e.Desencriptar(dtPersonasLocal.Rows[i][4].ToString()), e.Desencriptar(dtPersonasLocal.Rows[i][5].ToString()),
                                    e.Desencriptar(dtPersonasLocal.Rows[i][6].ToString()), e.Desencriptar(dtPersonasLocal.Rows[i][7].ToString()),
                                    e.Desencriptar(dtPersonasLocal.Rows[i][8].ToString()), e.Desencriptar(dtPersonasLocal.Rows[i][9].ToString()),
                                    DateTime.Parse(e.Desencriptar(dtPersonasLocal.Rows[i][10].ToString())), e.Desencriptar(dtPersonasLocal.Rows[i][11].ToString()),
                                    e.Desencriptar(dtPersonasLocal.Rows[i][12].ToString()), DateTime.Parse(dtPersonasLocal.Rows[i][13].ToString()),
                                    int.Parse(dtPersonasLocal.Rows[i][14].ToString()), 1, DateTime.Parse(dtPersonasLocal.Rows[i][16].ToString()),
                                    nuevoUpdated, int.Parse(dtPersonasLocal.Rows[i][19].ToString()));
                            }
                        }
                    }
                    else
                    {
                        Log("Datos Sincronizados...");
                    }

                    if (dtPersonasNube != null)
                    {
                        if (dtPersonasNube.Rows.Count > 0)
                        {
                            int totalActualizar = dtPersonasNube.Rows.Count;
                            int valorActual = 0;
                            Log("Datos de trabajadores Local a Nube... Sincronizados");
                            Log("Sincronizando datos de trabajadores Nube a Local...");
                            DataTable dtPersonaLocalEncontrada;
                            for (int i = 0; i < dtPersonasNube.Rows.Count; i++)
                            {
                                valorActual = i + 1;
                                Log("Actualizando datos de trabajadores nube a local: " + valorActual.ToString() + "/" + totalActualizar.ToString() + "...");
                                dtPersonaLocalEncontrada = null;
                                dtPersonaLocalEncontrada = personaYaExisteEnLocal(e.Encriptar(dtPersonasNube.Rows[i][1].ToString()));
                                if (dtPersonaLocalEncontrada.Rows.Count > 0)
                                {
                                    if (!comprobarFechasEdicion(DateTime.Parse(dtPersonaLocalEncontrada.Rows[0][1].ToString()), DateTime.Parse(dtPersonasNube.Rows[i][17].ToString())))
                                    {
                                        actualizarDatoPersonaLocalConDatoDeNube(e.Encriptar(dtPersonasNube.Rows[i][2].ToString()), e.Encriptar(dtPersonasNube.Rows[i][3].ToString()),
                                        e.Encriptar(dtPersonasNube.Rows[i][4].ToString()), e.Encriptar(dtPersonasNube.Rows[i][5].ToString()),
                                        e.Encriptar(dtPersonasNube.Rows[i][8].ToString()), e.Encriptar(dtPersonasNube.Rows[i][9].ToString()),
                                        DateTime.Parse(dtPersonasNube.Rows[i][10].ToString()), e.Encriptar(dtPersonasNube.Rows[i][11].ToString()),
                                        e.Encriptar(dtPersonasNube.Rows[i][12].ToString()), DateTime.Parse(dtPersonasNube.Rows[i][13].ToString()),
                                        e.Encriptar(dtPersonasNube.Rows[i][6].ToString()), e.Encriptar(dtPersonasNube.Rows[i][7].ToString()),
                                        e.Encriptar(dtPersonasNube.Rows[i][1].ToString()),
                                        dtPersonasNube.Rows[i][18].ToString(), DateTime.Parse(dtPersonasNube.Rows[i][17].ToString()),
                                        int.Parse(dtPersonasNube.Rows[i][14].ToString()), int.Parse(dtPersonasNube.Rows[i][19].ToString()));
                                    }
                                }
                                else
                                {
                                    agregarDatoPersonaTraidoDesdeNubeALocal(
                                        e.Encriptar(dtPersonasNube.Rows[i][2].ToString()), e.Encriptar(dtPersonasNube.Rows[i][3].ToString()),
                                        e.Encriptar(dtPersonasNube.Rows[i][4].ToString()), e.Encriptar(dtPersonasNube.Rows[i][5].ToString()),
                                        e.Encriptar(dtPersonasNube.Rows[i][1].ToString()), e.Encriptar(dtPersonasNube.Rows[i][8].ToString()),
                                        e.Encriptar(dtPersonasNube.Rows[i][9].ToString()), DateTime.Parse(dtPersonasNube.Rows[i][10].ToString()),
                                        e.Encriptar(dtPersonasNube.Rows[i][11].ToString()), e.Encriptar(dtPersonasNube.Rows[i][12].ToString()), 1,
                                        DateTime.Parse(dtPersonasNube.Rows[i][13].ToString()), e.Encriptar(dtPersonasNube.Rows[i][6].ToString()),
                                        e.Encriptar(dtPersonasNube.Rows[i][7].ToString()), dtPersonasNube.Rows[i][18].ToString(),
                                        DateTime.Parse(dtPersonasNube.Rows[i][16].ToString()), DateTime.Parse(dtPersonasNube.Rows[i][17].ToString()),
                                        int.Parse(dtPersonasNube.Rows[i][14].ToString()), int.Parse(dtPersonasNube.Rows[i][19].ToString()));
                                }
                            }
                            Log("Datos de trabajadores Nube a Local... Sincronizados");
                        }

                        // Save cloud watermark — updated_at is column 17 in traerDatosPersonasDesdeNube
                        GuardarWatermarkDeNube("Persona", dtPersonasNube, 17);
                    }
                }
                else
                {
                    Log("Error al comprobar personas favor contactar a soporte...");
                    generarLog("comprobarPersonasSinSincronizar() Error al comprobar personas Local", "ERROR");
                }
            }
            catch (Exception ex)
            {
                generarLog("comprobarPersonasSinSincronizar() " + ex.ToString(), "ERROR");
            }
        }

        /// <summary>
        /// Extracts MAX(column[colIdx]) from the DataTable and saves it as the sync_watermark
        /// for the given table name. Call after processing each cloud download batch.
        /// </summary>
        private void GuardarWatermarkDeNube(string tabla, DataTable dt, int colIdx)
        {
            try
            {
                if (dt == null || dt.Rows.Count == 0) return;
                DateTime maxFecha = DateTime.MinValue;
                foreach (System.Data.DataRow row in dt.Rows)
                {
                    if (row[colIdx] == null || row[colIdx] == DBNull.Value) continue;
                    if (DateTime.TryParse(row[colIdx].ToString(), out DateTime f) && f > maxFecha)
                        maxFecha = f;
                }
                if (maxFecha == DateTime.MinValue) return;
                new SyncWatermark().GuardarWatermark(tabla, maxFecha.ToString("yyyy-MM-dd HH:mm:ss"));
            }
            catch (Exception ex)
            {
                generarLog("GuardarWatermarkDeNube(" + tabla + ") " + ex.Message, "ADVERTENCIA");
            }
        }

        private void subirDatosANubePersonaSinSincronizar(string rut, string nombre, string segundoNombre,
            string apellidoPaterno, string apellidoMaterno, string centroCosto,
            string puesto, string correo, string alias,
            DateTime fechaNacimiento, string telefonoUno, string telefonoDos,
            DateTime activoDesde, int habilitada, int respaldado, DateTime created_at,
            DateTime updated_at, int empresa_idempresa)
        {
            try
            {
                Persona p = new Persona();
                if (p.agregarPersonaNube(nombre, segundoNombre, apellidoPaterno, apellidoMaterno, rut,
                    correo, alias, fechaNacimiento, telefonoUno, telefonoDos, activoDesde, centroCosto, puesto, habilitada, created_at, updated_at, empresa_idempresa))
                    p.actualizarEstadoRespaldadoPersonaLocal(rut, updated_at);
            }
            catch (Exception ex)
            {
                generarLog("subirDatosANubePersonaSinSincronizar() " + ex.ToString(), "ERROR");
            }
        }

        private void subirDatosActualizadosANubePersona(string rut, string nombre, string segundoNombre,
            string apellidoPaterno, string apellidoMaterno, string centroCosto,
            string puesto, string correo, string alias,
            DateTime fechaNacimiento, string telefonoUno, string telefonoDos,
            DateTime activoDesde, int habilitada,
            DateTime updated_at, int empresa_idempresa)
        {
            try
            {
                Persona p = new Persona();
                if (p.editarPersonaNube(nombre, segundoNombre, apellidoPaterno, apellidoMaterno, correo, alias, fechaNacimiento, telefonoUno, telefonoDos, activoDesde, centroCosto, puesto, rut, habilitada, empresa_idempresa))
                    p.actualizarEstadoRespaldadoPersonaLocal(rut, updated_at);
            }
            catch (Exception ex)
            {
                generarLog("subirDatosActualizadosANubePersona() " + ex.ToString(), "ERROR");
            }
        }

        private DataTable traerDatosPersonaNube()
        {
            // Use cloud-timezone watermark instead of local MAX.AddHours(-3).
            // The .AddHours(-3) hack caused timezone drift: it shifted the cutoff backward
            // by 3h every sync cycle, eventually downloading the full history on each run.
            string cutoff = new SyncWatermark().TraerWatermark("Persona");
            Persona p = new Persona();
            DataTable dt = p.traerDatosPersonasDesdeNube(cutoff);
            return dt;
        }

        private DataTable traerDatosPersonaLocal()
        {
            Persona p = new Persona();
            DataTable dt = p.personasSinSincronizarLocal();
            return dt;
        }

        private bool personaYaExisteEnNube(string rut)
        {
            Persona p = new Persona();
            DataTable dt = p.traerDatosPersonasDesdeNubePorRut(rut);
            bool respuesta = false;
            if (dt != null)
            {
                respuesta = dt.Rows.Count > 0;
            }
            return respuesta;
        }

        private DataTable personaYaExisteEnLocal(string rut)
        {
            Persona p = new Persona();
            DataTable dt = p.existeRegistroPersona(rut);
            return dt;
        }

        private void agregarDatoPersonaTraidoDesdeNubeALocal(string nombre, string segundoNombre, string apellidoPaterno, string apellidoMaterno, string rut,
            string correo, string alias, DateTime fechaNacimiento, string telefonoUno, string telefonoDos,
            int respaldado, DateTime activoDesde, string centroCosto, string puesto, string nombreEquipo,
            DateTime created_at, DateTime updated_at, int habilitada, int empresa_idempresa)
        {
            try
            {
                Persona p = new Persona();
                p.agregarPersona(nombre, segundoNombre, apellidoPaterno, apellidoMaterno,
                    rut, correo, alias, fechaNacimiento,
                    telefonoUno, telefonoDos, 1,
                    activoDesde, centroCosto, puesto, nombreEquipo, created_at, updated_at, habilitada, empresa_idempresa);
            }
            catch (Exception ex)
            {
                generarLog("agregarDatoPersonaTraidoDesdeNubeALocal() " + ex.ToString(), "ERROR");
            }
        }

        private void actualizarDatoPersonaLocalConDatoDeNube(string nombre, string segundoNombre, string apellidoPaterno, string apellidoMaterno,
            string correo, string alias, DateTime fechaNacimiento, string telefonoUno, string telefonoDos,
            DateTime activoDesde, string centroCosto, string puesto, string rut, string nombreEquipo, DateTime updated_at, int habilitada, int empresa_idempresa)
        {
            try
            {
                Persona p = new Persona();
                Herramientas h = new Herramientas();
                string fechaFormateada = h.formatearFecha(updated_at, true);
                p.editarPersona(nombre, segundoNombre, apellidoPaterno, apellidoMaterno, correo,
                    alias, fechaNacimiento, telefonoUno, telefonoDos, activoDesde, 1,
                    centroCosto, puesto, rut, nombreEquipo, fechaFormateada, habilitada, empresa_idempresa);
            }
            catch (Exception ex)
            {
                generarLog("actualizarDatoPersonaLocalConDatoDeNube() " + ex.ToString(), "ERROR");
            }
        }

        // ======================== Imagen Huellas ========================

        private string traerUltimaFechaUpdatedRegistrosHuellasLocales()
        {
            ImagenHuella ih = new ImagenHuella();
            DataTable dtFecha = ih.traerUltimoUpdatedAtImagenHuella();
            string ultimoUpdated = _fechaUltimaActualizacion;
            if (dtFecha != null)
            {
                if (dtFecha.Rows.Count > 0)
                {
                    Herramientas h = new Herramientas();
                    try
                    {
                        ultimoUpdated = h.formatearFecha(DateTime.Parse(dtFecha.Rows[0][0].ToString()), true);
                    }
                    catch (Exception)
                    {
                        ultimoUpdated = _fechaUltimaActualizacion;
                    }
                }
            }
            return ultimoUpdated;
        }

        public void SincronizarImagenesHuella()
        {
            try
            {
                ImagenHuella ih = new ImagenHuella();
                Encriptacion e = new Encriptacion();
                DataTable dtLocal = ih.traerHuellasSinSincronizar();
                DataTable dtNube = ih.traerIndiciosDeRegistrosDeHuellaSinSincronizarServidorSimplificado(traerUltimaFechaUpdatedRegistrosHuellasLocales());
                string rutActual;
                string rutYaActualizado = "";
                if (dtLocal != null)
                {
                    Log("Sincronizando huellas Local a Nube...");

                    if (dtLocal.Rows.Count > 0)
                    {
                        for (int i = 0; i < dtLocal.Rows.Count; i++)
                        {
                            rutActual = dtLocal.Rows[i][2].ToString();
                            if (!rutActual.Equals(rutYaActualizado))
                            {
                                procesarRegistroHuellasLocalANube(rutActual);
                                rutYaActualizado = rutActual;
                            }
                        }
                        Log("Registro de huellas Local a Nube ... sincronizado");
                    }
                    else
                    {
                        Log("Registros de huellas Local a Nube ... sincronizado");
                    }
                }
                else
                {
                    Log("No se ha podido obtener el registro de huellas Local a Nube...");
                    generarLog(" No se ha podido obtener el registro de huellas Local a Nube comprobarImagenHuellasSinSincronizar() ", "ADVERTENCIA");
                }

                if (dtNube != null)
                {
                    Log("Sincronizando huellas Nube a Local...");

                    if (dtNube.Rows.Count > 0)
                    {
                        rutYaActualizado = "";
                        rutActual = "";
                        int totalActualizar = dtNube.Rows.Count;
                        int valorActual = 0;
                        for (int i = 0; i < dtNube.Rows.Count; i++)
                        {
                            valorActual = i + 1;
                            Log("Actualizando huellas Nube a Local: " + valorActual.ToString() + "/" + totalActualizar.ToString() + "...");
                            rutActual = dtNube.Rows[i][1].ToString();
                            if (!rutActual.Equals(rutYaActualizado))
                            {
                                procesarRegistroHuellasNubeALocal(rutActual);
                                rutYaActualizado = rutActual;
                            }
                        }
                    }
                }
                else
                {
                    Log("No se ha podido obtener el registro de huellas Nube a Local...");
                    generarLog(" No se ha podido obtener el registro de huellas Nube a Local comprobarImagenHuellasSinSincronizar() ", "ADVERTENCIA");
                }
                Log("Registros de huellas... sincronizados");
            }
            catch (Exception ex)
            {
                generarLog("comprobarImagenHuellasSinSincronizar() " + ex.ToString(), "ERROR");
            }
        }

        private void procesarRegistroHuellasLocalANube(string rut)
        {
            // Uses upsert (check-exists → update or insert) instead of delete+reinsert.
            // The cloud DB user does not have DELETE permission on imagenHuella, so the old
            // eliminarHuellasTrabajadorRutServidor() was silently failing, causing 167k+ duplicates.
            try
            {
                ImagenHuella ih = new ImagenHuella();
                DataTable dt = ih.traerHuellaPorRutPersonaCompleto(rut);
                Encriptacion e = new Encriptacion();
                if (dt == null)
                {
                    Log("No se ha podido obtener el registro de huellas Local a Nube...");
                    generarLog("procesarRegistroHuellasLocalANube() No se ha podido obtener el registro de huellas Local", "ERROR");
                    return;
                }
                if (dt.Rows.Count == 0) return;

                string rutDesencriptado = e.Desencriptar(rut);
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    int numeroDedo = int.Parse(dt.Rows[i][3].ToString());
                    byte[] bytData = (byte[])dt.Rows[i][4];
                    DateTime nuevoUpdate = DateTime.Now;

                    // Upsert: check if (rut, numeroDedo) already exists in cloud
                    DataTable dtExiste = ih.consultarRegistroHuellaExistenteServidorPorRutNumeroDedo(rutDesencriptado, numeroDedo);
                    bool guardado;
                    if (dtExiste != null && dtExiste.Rows.Count > 0)
                    {
                        // Row exists — update image and mark respaldado=1
                        int idCloud = int.Parse(dtExiste.Rows[0][0].ToString());
                        guardado = ih.actualizarHuellaServidor(idCloud, bytData, 1, nuevoUpdate);
                    }
                    else
                    {
                        // Row doesn't exist — insert
                        guardado = ih.guardarHuellaServidor(
                            int.Parse(dt.Rows[i][1].ToString()), rutDesencriptado,
                            numeroDedo, bytData,
                            e.Desencriptar(dt.Rows[i][6].ToString()),
                            e.Desencriptar(dt.Rows[i][7].ToString()),
                            1, DateTime.Parse(dt.Rows[i][8].ToString()),
                            nuevoUpdate, dt.Rows[i][5].ToString());
                    }

                    if (guardado)
                        ih.actualizarEstadoRegistroHuellaLocal(int.Parse(dt.Rows[i][0].ToString()), nuevoUpdate);
                }
            }
            catch (Exception ex)
            {
                generarLog("procesarRegistroHuellasLocalANube() " + ex.ToString(), "ERROR");
            }
        }

        /// <summary>
        /// Set-diff download: downloads fingerprints present in cloud (for this device) but missing locally.
        /// Safe to run repeatedly — never overwrites existing local records.
        /// For Enrolador (all fingerprints): pass idReloj=0 to skip FHR filter.
        /// </summary>
        public void DescargarHuellasNuevasDeNube(int idReloj)
        {
            Log("Descargando huellas nuevas desde nube (set-diff)...");
            try
            {
                ImagenHuella ih = new ImagenHuella();
                Encriptacion e = new Encriptacion();

                // Build local set: decrypt each local rut for comparison
                DataTable dtLocal = ih.traerCombinacionesRutDedoLocal();
                var localSet = new System.Collections.Generic.HashSet<string>(StringComparer.Ordinal);
                if (dtLocal != null)
                {
                    foreach (System.Data.DataRow row in dtLocal.Rows)
                    {
                        string rutEnc = row[0].ToString();
                        int dedo = int.Parse(row[1].ToString());
                        string rutPlain = e.Desencriptar(rutEnc);
                        localSet.Add(rutPlain + "|" + dedo);
                    }
                }

                // Get cloud combos — FHR-filtered for Asistencia/Casino, unfiltered for Enrolador
                DataTable dtNube = (idReloj > 0)
                    ? ih.traerCombinacionesRutDedoEnNubePorReloj(idReloj)
                    : ih.traerCombinacionesRutDedoEnNube();

                if (dtNube == null || dtNube.Rows.Count == 0)
                {
                    Log("Sin huellas en nube para este dispositivo.");
                    return;
                }

                int descargadas = 0;
                for (int i = 0; i < dtNube.Rows.Count; i++)
                {
                    string rutCloud = dtNube.Rows[i][0].ToString();
                    int dedo = int.Parse(dtNube.Rows[i][1].ToString());
                    if (!localSet.Contains(rutCloud + "|" + dedo))
                    {
                        DataTable dtHuella = ih.traerHuellaCompletaServidorPorRutDedo(rutCloud, dedo);
                        if (dtHuella != null && dtHuella.Rows.Count > 0)
                        {
                            procesarHuellaDescargadaDeNube(dtHuella.Rows[0], e);
                            descargadas++;
                        }
                    }
                }
                Log($"Huellas descargadas desde nube: {descargadas} de {dtNube.Rows.Count} combos en nube.");
            }
            catch (Exception ex)
            {
                generarLog("DescargarHuellasNuevasDeNube() " + ex.ToString(), "ERROR");
            }
        }

        private void procesarHuellaDescargadaDeNube(System.Data.DataRow row, Encriptacion e)
        {
            // Columns from traerHuellaCompletaServidorPorRutDedo:
            // [0]=idimagenHuella,[1]=empresa_idempresa,[2]=persona_rut(plain),[3]=numeroDedo,
            // [4]=imagen_huella,[5]=nombreEquipoEdicion,[6]=created_by,[7]=updated_by,[8]=created_at,[9]=updated_at
            try
            {
                ImagenHuella ih = new ImagenHuella();
                string rutPlain = row[2].ToString();
                string rutEnc = e.Encriptar(rutPlain);
                int numeroDedo = int.Parse(row[3].ToString());
                byte[] bytData = (byte[])row[4];
                int empresaId = int.Parse(row[1].ToString());
                string createdBy = e.Encriptar(row[6].ToString());
                string updatedBy = e.Encriptar(row[7].ToString());
                string equipo = row[5].ToString();
                DateTime created_at = DateTime.Parse(row[8].ToString());
                DateTime updated_at = DateTime.Parse(row[9].ToString());

                ih.guardarHuella(empresaId, rutEnc, numeroDedo, bytData,
                    createdBy, updatedBy, 1, equipo, created_at, updated_at);
            }
            catch (Exception ex)
            {
                generarLog("procesarHuellaDescargadaDeNube() " + ex.ToString(), "ERROR");
            }
        }

        private void procesarRegistroHuellasNubeALocal(string rut)
        {
            try
            {
                ImagenHuella ih = new ImagenHuella();
                DataTable dt = ih.traerHuellaPorRutPersonaCompletoServidor(rut);
                Encriptacion e = new Encriptacion();
                Herramientas h = new Herramientas();
                object objByte;
                byte[] bytData;
                Object fpt_template;
                string rutEncriptado = "";
                fpt_template = "";
                if (dt != null)
                {
                    if (dt.Rows.Count > 0)
                    {
                        rutEncriptado = e.Encriptar(rut);
                        ih.eliminarHuellasTrabajadorRutLocal(rutEncriptado);
                        for (int i = 0; i < dt.Rows.Count; i++)
                        {
                            fpt_template = dt.Rows[i][4];
                            objByte = (byte[])(fpt_template);
                            bytData = (byte[])objByte;
                            ih.guardarHuella(int.Parse(dt.Rows[i][1].ToString()), rutEncriptado,
                               int.Parse(dt.Rows[i][3].ToString()), bytData, e.Encriptar(dt.Rows[i][6].ToString()),
                               e.Encriptar(dt.Rows[i][7].ToString()), 1, dt.Rows[i][5].ToString(), DateTime.Parse(dt.Rows[i][8].ToString()),
                               DateTime.Parse(dt.Rows[i][9].ToString()));
                        }
                    }
                }
                else
                {
                    Log("No se ha podido obtener el registro de huellas Local a Nube...");
                }
            }
            catch (Exception ex)
            {
                generarLog("procesarRegistroHuellasNubeALocal() " + ex.ToString(), "ERROR");
            }
        }

        // ======================== Configuracion Funcionarios ========================

        public void SincronizarConfigFuncionarios()
        {
            try
            {
                Encriptacion e = new Encriptacion();
                Log("Comprobando sincronización de configuración de personas...");
                DataTable dtConfiguracionPersonaNube = traerDatosConfiguracionPersonaNube();
                DataTable dtConfiguracionPersonaLocal = traerDatosConfiguracionPersonaLocal();
                DataTable dtRegistroEspecifico;
                string rutSinCifrar = "";
                ConfigFuncionarioReloj cfr = new ConfigFuncionarioReloj();

                if (dtConfiguracionPersonaLocal != null)
                {
                    if (dtConfiguracionPersonaLocal.Rows.Count > 0)
                    {
                        int totalActualizar = dtConfiguracionPersonaLocal.Rows.Count;
                        int valorActual = 0;
                        Log("Sincronizado configuración de personas Local a Nube...");
                        for (int i = 0; i < dtConfiguracionPersonaLocal.Rows.Count; i++)
                        {
                            valorActual = i + 1;
                            Log("Actualizando configuraciones de personas local a nube: " + valorActual.ToString() + "/" + totalActualizar.ToString() + "...");
                            rutSinCifrar = e.Desencriptar(dtConfiguracionPersonaLocal.Rows[i][0].ToString());
                            dtRegistroEspecifico = traerConfigPersonaNubePorRut(rutSinCifrar);
                            if (dtRegistroEspecifico != null)
                            {
                                if (dtRegistroEspecifico.Rows.Count > 0)
                                {
                                    DateTime nuevoUpdated = DateTime.Now;
                                    if (cfr.editarConfigPersonaNube(int.Parse(dtConfiguracionPersonaLocal.Rows[i][1].ToString()),
                                        int.Parse(dtConfiguracionPersonaLocal.Rows[i][2].ToString()), int.Parse(dtConfiguracionPersonaLocal.Rows[i][3].ToString()),
                                         dtConfiguracionPersonaLocal.Rows[i][4].ToString(), e.Desencriptar(dtConfiguracionPersonaLocal.Rows[i][11].ToString()),
                                         1, dtConfiguracionPersonaLocal.Rows[i][7].ToString(), rutSinCifrar, int.Parse(dtConfiguracionPersonaLocal.Rows[i][8].ToString()),
                                         int.Parse(dtConfiguracionPersonaLocal.Rows[i][10].ToString()), nuevoUpdated,
                                         int.Parse(dtConfiguracionPersonaLocal.Rows[i][15].ToString()), int.Parse(dtConfiguracionPersonaLocal.Rows[i][16].ToString()),
                                         int.Parse(dtConfiguracionPersonaLocal.Rows[i][17].ToString()), int.Parse(dtConfiguracionPersonaLocal.Rows[i][18].ToString())))
                                    {
                                        cfr.editarConfigPersonaActualizarEstadoRespaldado(int.Parse(dtRegistroEspecifico.Rows[0][12].ToString()), nuevoUpdated);
                                    }
                                }
                                else
                                {
                                    DateTime nuevoUpdated = DateTime.Now;
                                    if (cfr.agregarConfigPersonaNube(rutSinCifrar, int.Parse(dtConfiguracionPersonaLocal.Rows[i][1].ToString()),
                                         int.Parse(dtConfiguracionPersonaLocal.Rows[i][2].ToString()),
                                         int.Parse(dtConfiguracionPersonaLocal.Rows[i][3].ToString()),
                                         dtConfiguracionPersonaLocal.Rows[i][4].ToString(), e.Desencriptar(dtConfiguracionPersonaLocal.Rows[i][5].ToString()),
                                         1, dtConfiguracionPersonaLocal.Rows[i][7].ToString(), int.Parse(dtConfiguracionPersonaLocal.Rows[i][8].ToString()),
                                         e.Desencriptar(dtConfiguracionPersonaLocal.Rows[i][11].ToString()), int.Parse(dtConfiguracionPersonaLocal.Rows[i][10].ToString()),
                                         DateTime.Parse(dtConfiguracionPersonaLocal.Rows[i][14].ToString()),
                                         nuevoUpdated,
                                         int.Parse(dtConfiguracionPersonaLocal.Rows[i][15].ToString()), int.Parse(dtConfiguracionPersonaLocal.Rows[i][16].ToString()),
                                         int.Parse(dtConfiguracionPersonaLocal.Rows[i][17].ToString()), int.Parse(dtConfiguracionPersonaLocal.Rows[i][18].ToString())))
                                    {
                                        cfr.editarConfigPersonaActualizarEstadoRespaldado(int.Parse(dtConfiguracionPersonaLocal.Rows[0][12].ToString()), nuevoUpdated);
                                    }
                                }
                            }
                            else
                            {
                                Log("Error al obtener configuración de persona Local a Nube...");
                            }
                        }
                    }

                    Log("Sincronizando configuraciones de persona Nube a local...");

                    if (dtConfiguracionPersonaNube != null)
                    {
                        if (dtConfiguracionPersonaNube.Rows.Count > 0)
                        {
                            int totalActualizar = dtConfiguracionPersonaNube.Rows.Count;
                            int valorActual = 0;
                            for (int i = 0; i < dtConfiguracionPersonaNube.Rows.Count; i++)
                            {
                                valorActual = i + 1;
                                Log("Actualizando configuraciones de personas nube a local: " + valorActual.ToString() + "/" + totalActualizar.ToString() + "...");
                                dtRegistroEspecifico = traerConfigPersonaLocalPorRut(e.Encriptar(dtConfiguracionPersonaNube.Rows[i][0].ToString()));
                                if (dtRegistroEspecifico != null)
                                {
                                    if (dtRegistroEspecifico.Rows.Count > 0)
                                    {
                                        if (!comprobarFechasEdicion(DateTime.Parse(dtRegistroEspecifico.Rows[0][13].ToString()), DateTime.Parse(dtConfiguracionPersonaNube.Rows[i][13].ToString())))
                                        {
                                            cfr.editarConfigPersona(int.Parse(dtConfiguracionPersonaNube.Rows[i][1].ToString()), int.Parse(dtConfiguracionPersonaNube.Rows[i][2].ToString()),
                                            int.Parse(dtConfiguracionPersonaNube.Rows[i][3].ToString()), dtConfiguracionPersonaNube.Rows[i][4].ToString(),
                                            e.Encriptar(dtConfiguracionPersonaNube.Rows[i][11].ToString()), 1, dtConfiguracionPersonaNube.Rows[i][7].ToString(),
                                            e.Encriptar(dtConfiguracionPersonaNube.Rows[i][0].ToString()), int.Parse(dtConfiguracionPersonaNube.Rows[i][8].ToString()),
                                            int.Parse(dtConfiguracionPersonaNube.Rows[i][10].ToString()), dtConfiguracionPersonaNube.Rows[i][9].ToString(),
                                            (DateTime.Parse(dtConfiguracionPersonaNube.Rows[i][13].ToString())), int.Parse(dtConfiguracionPersonaNube.Rows[i][15].ToString()),
                                            int.Parse(dtConfiguracionPersonaNube.Rows[i][16].ToString()), int.Parse(dtConfiguracionPersonaNube.Rows[i][17].ToString()), int.Parse(dtConfiguracionPersonaNube.Rows[i][18].ToString()));
                                        }
                                    }
                                    else
                                    {
                                        cfr.agregarConfigPersona(e.Encriptar(dtConfiguracionPersonaNube.Rows[i][0].ToString()), int.Parse(dtConfiguracionPersonaNube.Rows[i][1].ToString()),
                                               int.Parse(dtConfiguracionPersonaNube.Rows[i][2].ToString()), int.Parse(dtConfiguracionPersonaNube.Rows[i][3].ToString()),
                                               dtConfiguracionPersonaNube.Rows[i][4].ToString(), e.Encriptar(dtConfiguracionPersonaNube.Rows[i][5].ToString()), 1,
                                               dtConfiguracionPersonaNube.Rows[i][7].ToString(), int.Parse(dtConfiguracionPersonaNube.Rows[i][8].ToString()),
                                               e.Encriptar(dtConfiguracionPersonaNube.Rows[i][11].ToString()), int.Parse(dtConfiguracionPersonaNube.Rows[i][10].ToString()), DateTime.Parse(dtConfiguracionPersonaNube.Rows[i][14].ToString()),
                                               DateTime.Parse(dtConfiguracionPersonaNube.Rows[i][13].ToString()), dtConfiguracionPersonaNube.Rows[i][9].ToString(),
                                               int.Parse(dtConfiguracionPersonaNube.Rows[i][15].ToString()), int.Parse(dtConfiguracionPersonaNube.Rows[i][16].ToString()),
                                               int.Parse(dtConfiguracionPersonaNube.Rows[i][17].ToString()), int.Parse(dtConfiguracionPersonaNube.Rows[i][17].ToString()));
                                    }
                                }
                                else
                                {
                                    Log("Error al consultar las Configuraciones de personas Nube a Local");
                                }
                            }
                        }
                    }
                    Log("Configuraciones de personas sincronizadas...");
                }
                else
                {
                    Log("Error al obtener configuración de personas Local a Nube...");
                }
            }
            catch (Exception ex)
            {
                generarLog("comprobarConfiguracionesFuncionariosSinSincronizar()" + ex.ToString(), "ERROR");
            }
        }

        private DataTable traerDatosConfiguracionPersonaLocal()
        {
            ConfigFuncionarioReloj cfr = new ConfigFuncionarioReloj();
            DataTable dt = cfr.traerConfigsPersonaLocalSinSincronizar();
            return dt;
        }

        private DataTable traerDatosConfiguracionPersonaNube()
        {
            ConfigFuncionarioReloj cfr = new ConfigFuncionarioReloj();
            DataTable dtFecha = cfr.traerUltimoUpdatedAtConfigFuncionarioReloj();
            string ultimoUpdated = _fechaUltimaActualizacion;
            if (dtFecha != null)
            {
                if (dtFecha.Rows.Count > 0)
                {
                    try
                    {
                        Herramientas h = new Herramientas();
                        ultimoUpdated = h.formatearFecha(DateTime.Parse(dtFecha.Rows[0][0].ToString()).AddHours(-3), true);
                    }
                    catch (Exception)
                    {
                        ultimoUpdated = _fechaUltimaActualizacion;
                    }
                }
            }

            DataTable dt = cfr.traerConfigsPersonaNube(ultimoUpdated);
            return dt;
        }

        private DataTable traerConfigPersonaLocalPorRut(string rut)
        {
            ConfigFuncionarioReloj cfr = new ConfigFuncionarioReloj();
            DataTable dt = cfr.traerConfigPersonaLocalPorRut(rut);
            return dt;
        }

        private DataTable traerConfigPersonaNubePorRut(string rut)
        {
            ConfigFuncionarioReloj cfr = new ConfigFuncionarioReloj();
            DataTable dt = cfr.traerConfigPersonaNubePorRut(rut);
            return dt;
        }

        // ======================== Funcionario has reloj ========================

        public void SincronizarFuncionarioHasReloj()
        {
            try
            {
                Log("Comprobando sincronización personas habilitadas para marca...");
                FuncionarioHasReloj fhr = new FuncionarioHasReloj();

                DataTable dtFecha = fhr.traerUltimoUpdatedAtFuncionarioHasReloj();
                string ultimoUpdated = _fechaUltimaActualizacion;
                if (dtFecha != null)
                {
                    if (dtFecha.Rows.Count > 0)
                    {
                        try
                        {
                            Herramientas h = new Herramientas();
                            ultimoUpdated = h.formatearFecha(DateTime.Parse(dtFecha.Rows[0][0].ToString()).AddHours(-3), true);
                        }
                        catch (Exception)
                        {
                            ultimoUpdated = _fechaUltimaActualizacion;
                        }
                    }
                }
                DataTable dtLocal = fhr.traerFuncionariosHasRelojLocalSinSincronizar();
                DataTable dtNube = fhr.traerFuncionariosHasRelojNubeSinSincronizar(ultimoUpdated);
                if (dtLocal != null)
                {
                    if (dtLocal.Rows.Count > 0)
                    {
                        Log("Sincronizando Personas habilitadas para marca Local a Nube...");
                        procesarPermisosMarcasLocalesANube(dtLocal);
                    }
                    else
                    {
                        Log("Personas habilitadas para marca Local a Nube sincronizadas...");
                    }
                    if (dtNube != null)
                    {
                        Log("Sincronizando Personas habilitadas para marca Nube a Local...");
                        procesarPermisosMarcasNubeALocal(dtNube);
                        Log("Personas habilitadas para marca sincronizadas...");
                    }
                    else
                    {
                        Log("Error de conexión a bd Nube de sincronización personas habilitadas para marca...");
                        generarLog("comprobarFuncionarioHasReloj() Error de conexión a bd Nube de sincronización personas habilitadas para marca", "ADVERTENCIA");
                    }
                }
                else
                {
                    Log("Error de conexión a bd de sincronización personas habilitadas para marca...");
                    generarLog("comprobarFuncionarioHasReloj() Error de conexión a bd de sincronización personas habilitadas para marca LOCAL", "ERROR");
                }
            }
            catch (Exception ex)
            {
                generarLog("comprobarFuncionarioHasReloj()" + ex.ToString(), "ERROR");
            }
        }

        // Upload-only variant for imagenHuella: used by fast delta loop (no download from cloud)
        // The download direction causes a timezone-driven loop: cloud updated_at > local updated_at always
        // → download fires every cycle, deletes+reinserts local with new IDs → runaway growth
        public void SubirHuellasLocalesANube()
        {
            try
            {
                ImagenHuella ih = new ImagenHuella();
                DataTable dtLocal = ih.traerHuellasSinSincronizar(); // respaldado=0 only
                if (dtLocal != null && dtLocal.Rows.Count > 0)
                {
                    Log("Subiendo huellas locales a nube...");
                    string rutYaActualizado = "";
                    int rutsProcesados = 0;
                    // Limit per delta cycle: each RUT fires up to 10×2 cloud queries.
                    // 500 RUTs × 10 fingers × 2 queries = 10k sequential queries → watchdog breach.
                    // With MaxRutsPorCiclo=50: worst case ~1000 queries per 90s cycle — safe margin.
                    // Remaining RUTs (respaldado=0) are processed on the next delta cycle.
                    const int MaxRutsPorCiclo = 50;
                    for (int i = 0; i < dtLocal.Rows.Count; i++)
                    {
                        string rutActual = dtLocal.Rows[i][2].ToString();
                        if (!rutActual.Equals(rutYaActualizado))
                        {
                            if (rutsProcesados >= MaxRutsPorCiclo)
                            {
                                Log($"Huellas: límite de {MaxRutsPorCiclo} RUTs/ciclo alcanzado — restantes en próximo ciclo.");
                                break;
                            }
                            procesarRegistroHuellasLocalANube(rutActual);
                            rutYaActualizado = rutActual;
                            rutsProcesados++;
                        }
                    }
                    Log("Huellas locales subidas a nube.");
                }
                else
                {
                    Log("Sin huellas locales pendientes de subir.");
                }
            }
            catch (Exception ex)
            {
                generarLog("SubirHuellasLocalesANube() " + ex.ToString(), "ERROR");
            }
        }

        // Upload-only variant: used by fast delta loop (no download from cloud)
        public void SubirPermisosLocalesANube()
        {
            try
            {
                FuncionarioHasReloj fhr = new FuncionarioHasReloj();
                DataTable dtLocal = fhr.traerFuncionariosHasRelojLocalSinSincronizar();
                if (dtLocal != null && dtLocal.Rows.Count > 0)
                {
                    Log("Subiendo permisos locales a nube...");
                    procesarPermisosMarcasLocalesANube(dtLocal);
                }
                else
                {
                    Log("Sin permisos locales pendientes de subir.");
                }
            }
            catch (Exception ex)
            {
                generarLog("SubirPermisosLocalesANube() " + ex.ToString(), "ERROR");
            }
        }

        private void procesarPermisosMarcasLocalesANube(DataTable dtDatosLocales)
        {
            try
            {
                Encriptacion e = new Encriptacion();
                FuncionarioHasReloj fhr = new FuncionarioHasReloj();
                Herramientas h = new Herramientas();
                string rutAnterior = "";
                string rutDesencriptado = "";
                DataTable dtDatosNube = null;
                int idreloj = 0;

                for (int i = 0; i < dtDatosLocales.Rows.Count; i++)
                {
                    rutDesencriptado = e.Desencriptar(dtDatosLocales.Rows[i][2].ToString());
                    if (!rutDesencriptado.Equals(rutAnterior))
                    {
                        dtDatosNube = fhr.traerRelojesHabilitadosParaPersonaDesdeNube(rutDesencriptado);
                    }

                    idreloj = int.Parse(dtDatosLocales.Rows[i][1].ToString());

                    if (dtDatosNube != null)
                    {
                        if (dtDatosNube.Rows.Count > 0)
                        {
                            if (preguntarConfiguracionesPermisoRelojExistente(idreloj, dtDatosNube))
                            {
                                string nuevoUpdated = h.formatearFecha(DateTime.Now, true);
                                if (fhr.actualizarHabilitacionRelojNube(idreloj, rutDesencriptado, e.Desencriptar(dtDatosLocales.Rows[i][5].ToString()),
                                    1, nuevoUpdated, int.Parse(dtDatosLocales.Rows[i][8].ToString()),
                                 dtDatosLocales.Rows[i][3].ToString()))
                                {
                                    fhr.actualizarEstadoRespaldadoHabilitacionReloj(int.Parse(dtDatosLocales.Rows[i][0].ToString()), nuevoUpdated);
                                }
                            }
                            else
                            {
                                string nuevoUpdated = h.formatearFecha(DateTime.Now, true);
                                if (fhr.asignarHabilitacionRelojNube(idreloj, rutDesencriptado,
                                      e.Desencriptar(dtDatosLocales.Rows[i][4].ToString()),
                                      e.Desencriptar(dtDatosLocales.Rows[i][5].ToString()), 1,
                                      h.formatearFecha(DateTime.Parse(dtDatosLocales.Rows[i][6].ToString()), true),
                                      nuevoUpdated,
                                      int.Parse(dtDatosLocales.Rows[i][8].ToString()),
                                      Environment.MachineName))
                                {
                                    fhr.actualizarEstadoRespaldadoHabilitacionReloj(int.Parse(dtDatosLocales.Rows[i][0].ToString()), nuevoUpdated);
                                }
                            }
                        }
                        else
                        {
                            string nuevoUpdated = h.formatearFecha(DateTime.Now, true);
                            if (fhr.asignarHabilitacionRelojNube(idreloj, rutDesencriptado,
                                    e.Desencriptar(dtDatosLocales.Rows[i][4].ToString()),
                                    e.Desencriptar(dtDatosLocales.Rows[i][5].ToString()), 1,
                                    h.formatearFecha(DateTime.Parse(dtDatosLocales.Rows[i][6].ToString()), true),
                                    h.formatearFecha(DateTime.Parse(dtDatosLocales.Rows[i][7].ToString()), true),
                                    int.Parse(dtDatosLocales.Rows[i][8].ToString()),
                                    Environment.MachineName))
                            {
                                fhr.actualizarEstadoRespaldadoHabilitacionReloj(int.Parse(dtDatosLocales.Rows[i][0].ToString()), nuevoUpdated);
                            }
                        }
                    }
                    else
                    {
                        Log("Error de conexión a bd nube sincronización personas habilitadas para marca...");
                        generarLog("procesarPermisosMarcasLocalesANube() Error de conexión a bd nube sincronización personas habilitadas para marca ", "ADVERTENCIA");
                    }
                    rutAnterior = rutDesencriptado;
                }
            }
            catch (Exception ex)
            {
                generarLog("procesarPermisosMarcasLocalesANube() " + ex.ToString(), "ERROR");
            }
        }

        private void procesarPermisosMarcasNubeALocal(DataTable dtDatosNube)
        {
            try
            {
                Herramientas h = new Herramientas();
                Encriptacion e = new Encriptacion();
                DataTable dtLocales;
                string rutPersonaEncriptado;
                int idreloj = 0;
                FuncionarioHasReloj fhr = new FuncionarioHasReloj();
                if (dtDatosNube != null)
                {
                    if (dtDatosNube.Rows.Count > 0)
                    {
                        int totalActualizar = dtDatosNube.Rows.Count;
                        int valorActual = 0;
                        for (int i = 0; i < dtDatosNube.Rows.Count; i++)
                        {
                            valorActual = i + 1;
                            Log("Actualizando permisos de marca: " + valorActual.ToString() + "/" + totalActualizar.ToString() + "...");

                            rutPersonaEncriptado = e.Encriptar(dtDatosNube.Rows[i][2].ToString());
                            idreloj = int.Parse(dtDatosNube.Rows[i][1].ToString());
                            dtLocales = fhr.preguntarEstadoFuncionarioPuedeMarcarEnReloj(idreloj, rutPersonaEncriptado);
                            if (dtLocales != null)
                            {
                                if (dtLocales.Rows.Count > 0)
                                {
                                    if (!comprobarFechasEdicion(DateTime.Parse(dtLocales.Rows[0][2].ToString()), DateTime.Parse(dtDatosNube.Rows[i][7].ToString())))
                                    {
                                        fhr.actualizarHabilitacionRelojLocal(idreloj, e.Encriptar(dtDatosNube.Rows[i][2].ToString()),
                                            e.Encriptar(dtDatosNube.Rows[i][5].ToString()), 1, h.formatearFecha(DateTime.Parse(dtDatosNube.Rows[i][7].ToString()), true),
                                            int.Parse(dtDatosNube.Rows[i][8].ToString()), dtDatosNube.Rows[i][3].ToString());
                                    }
                                }
                                else
                                {
                                    fhr.asignarHabilitacionReloj(idreloj, e.Encriptar(dtDatosNube.Rows[i][2].ToString()), e.Encriptar(dtDatosNube.Rows[i][4].ToString()),
                                        e.Encriptar(dtDatosNube.Rows[i][5].ToString()), 1, h.formatearFecha(DateTime.Parse(dtDatosNube.Rows[i][6].ToString()), true),
                                         h.formatearFecha(DateTime.Parse(dtDatosNube.Rows[i][7].ToString()), true), int.Parse(dtDatosNube.Rows[i][8].ToString()), dtDatosNube.Rows[i][3].ToString());
                                }
                            }
                            else
                            {
                                Log("Error de conexión bd local personas habilitadas para marca...");
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                generarLog("procesarPermisosMarcasNubeALocal(): " + ex.ToString(), "ERROR");
            }
        }

        private bool preguntarConfiguracionesPermisoRelojExistente(int idreloj, DataTable dt)
        {
            bool hayConfigRegistrada = false;
            if (dt != null)
            {
                DataRow[] filasEncontradas;
                filasEncontradas = dt.Select("reloj_idreloj=" + idreloj);
                hayConfigRegistrada = filasEncontradas.Length > 0;
            }
            return hayConfigRegistrada;
        }

        // ======================== Raciones Fijas ========================

        public void SincronizarRacionesFijas()
        {
            comprobarRacionesFijasLocalesSinSubir();
            comprobarRacionesFijasNubeSinSincronizar();
        }

        private void comprobarRacionesFijasLocalesSinSubir()
        {
            Log("Comprobando Raciones Fijas locales sin sincronizar...");
            Herramientas h = new Herramientas();
            try
            {
                DataTable dtLocal;
                RacionFija rf = new RacionFija();
                dtLocal = rf.traerRacionesFijasLocalesSinSincronizar();

                if (dtLocal != null)
                {
                    if (dtLocal.Rows.Count > 0)
                    {
                        Log("Sincronizando Raciones Fijas locales ...");
                        Encriptacion e = new Encriptacion();
                        int idregistroNube = 0;
                        int numeroDia = 0;
                        int tipoComida = 0;
                        string rutDesencriptado;
                        int habilitado = 0;
                        string createdDesencriptado = "";
                        string updatedDesencriptado = "";
                        string fechaCreated = "";
                        string fechaUpdated = "";
                        for (int i = 0; i < dtLocal.Rows.Count; i++)
                        {
                            numeroDia = int.Parse(dtLocal.Rows[i][1].ToString());
                            rutDesencriptado = e.Desencriptar(dtLocal.Rows[i][2].ToString());
                            tipoComida = int.Parse(dtLocal.Rows[i][3].ToString());
                            habilitado = int.Parse(dtLocal.Rows[i][4].ToString());
                            idregistroNube = traerIDRacionFijaNube(rutDesencriptado, numeroDia, tipoComida);
                            createdDesencriptado = e.Desencriptar(dtLocal.Rows[i][7].ToString());
                            updatedDesencriptado = e.Desencriptar(dtLocal.Rows[i][8].ToString());
                            fechaCreated = h.formatearFecha(DateTime.Parse(dtLocal.Rows[i][9].ToString()), true);
                            fechaUpdated = h.formatearFecha(DateTime.Parse(dtLocal.Rows[i][10].ToString()), true);
                            if (idregistroNube == 0)
                            {
                                DateTime nuevoUpdated = DateTime.Now;
                                string nuevoUpdatedFormateado = h.formatearFecha(nuevoUpdated, true);
                                if (rf.agregarRacionFijaNube(numeroDia, rutDesencriptado, tipoComida, habilitado, Environment.MachineName, 1, createdDesencriptado, updatedDesencriptado, fechaCreated, nuevoUpdatedFormateado))
                                    rf.editarEstadoRespaldadoRacionFijaLocal(int.Parse(dtLocal.Rows[i][0].ToString()), nuevoUpdated);
                            }
                            else
                            {
                                DateTime nuevoUpdated = DateTime.Now;
                                string nuevoUpdatedFormateado = h.formatearFecha(nuevoUpdated, true);
                                if (rf.editarRacionFijaNube(habilitado, Environment.MachineName, updatedDesencriptado, nuevoUpdatedFormateado, idregistroNube))
                                    rf.editarEstadoRespaldadoRacionFijaLocal(int.Parse(dtLocal.Rows[i][0].ToString()), nuevoUpdated);
                            }
                        }
                    }
                    Log("Raciones Fijas locales sincronizadas...");
                }
            }
            catch (Exception ex)
            {
                generarLog("comprobarRacionesFijasLocalesSinSubir() " + Environment.MachineName + " " + ex.ToString(), "ERROR");
            }
        }

        private int traerIDRacionFijaNube(string persona_rut, int numeroDia, int tipoComida)
        {
            int idracionFija = 0;
            try
            {
                DataTable dtNube;
                RacionFija rf = new RacionFija();
                dtNube = rf.preguntarRegistroRacionFijaExistenteNube(persona_rut, numeroDia, tipoComida);
                if (dtNube != null)
                {
                    if (dtNube.Rows.Count > 0)
                    {
                        idracionFija = int.Parse(dtNube.Rows[0][0].ToString());
                    }
                }
            }
            catch (Exception ex)
            {
                generarLog("traerIDRacionFijaNube() " + Environment.MachineName + " " + ex.ToString(), "ERROR");
            }
            return idracionFija;
        }

        private void comprobarRacionesFijasNubeSinSincronizar()
        {
            Log("Comprobando sincronizaciones Raciones Fijas nube ...");
            Herramientas h = new Herramientas();
            try
            {
                DataTable dtNube;
                RacionFija rf = new RacionFija();
                DataTable dtFecha = rf.traerUltimoUpdatedAtRacionFija();
                string ultimoUpdated = _fechaUltimaActualizacion;
                if (dtFecha != null)
                {
                    if (dtFecha.Rows.Count > 0)
                    {
                        try
                        {
                            ultimoUpdated = h.formatearFecha(DateTime.Parse(dtFecha.Rows[0][0].ToString()).AddHours(-3), true);
                        }
                        catch (Exception)
                        {
                            ultimoUpdated = _fechaUltimaActualizacion;
                        }
                    }
                }

                dtNube = rf.traerRacionFijaNubeSinRespaldoLocal(ultimoUpdated);
                if (dtNube != null)
                {
                    if (dtNube.Rows.Count > 0)
                    {
                        Log("Sincronizando Raciones Fijas nube ...");
                        Encriptacion e = new Encriptacion();
                        DataTable dtRegistroLocal;
                        int numeroDia = 0;
                        int tipoComida = 0;
                        string rutEncriptado;
                        int habilitado = 0;
                        string createdEncriptado = "";
                        string updatedEncriptado = "";
                        string fechaCreated = "";
                        string fechaUpdated = "";
                        string equipoEdicion = "";
                        for (int i = 0; i < dtNube.Rows.Count; i++)
                        {
                            numeroDia = int.Parse(dtNube.Rows[i][1].ToString());
                            rutEncriptado = e.Encriptar(dtNube.Rows[i][2].ToString());
                            tipoComida = int.Parse(dtNube.Rows[i][3].ToString());
                            habilitado = int.Parse(dtNube.Rows[i][4].ToString());
                            dtRegistroLocal = traerDatoRacionesFijasLocal(rutEncriptado, numeroDia, tipoComida);
                            equipoEdicion = dtNube.Rows[i][5].ToString();

                            if (dtRegistroLocal != null)
                            {
                                if (dtRegistroLocal.Rows.Count > 0)
                                {
                                    if (!comprobarFechasEdicion(DateTime.Parse(dtRegistroLocal.Rows[0][1].ToString()), DateTime.Parse(dtNube.Rows[i][10].ToString())))
                                    {
                                        createdEncriptado = e.Encriptar(dtNube.Rows[i][7].ToString());
                                        updatedEncriptado = e.Encriptar(dtNube.Rows[i][8].ToString());
                                        fechaCreated = h.formatearFecha(DateTime.Parse(dtNube.Rows[i][9].ToString()), true);
                                        fechaUpdated = h.formatearFecha(DateTime.Parse(dtNube.Rows[i][10].ToString()), true);
                                        rf.editarRacionFijaLocal(habilitado, equipoEdicion, 1, updatedEncriptado, fechaUpdated, int.Parse(dtRegistroLocal.Rows[0][0].ToString()));
                                    }
                                }
                                else
                                {
                                    createdEncriptado = e.Encriptar(dtNube.Rows[i][7].ToString());
                                    updatedEncriptado = e.Encriptar(dtNube.Rows[i][8].ToString());
                                    fechaCreated = h.formatearFecha(DateTime.Parse(dtNube.Rows[i][9].ToString()), true);
                                    fechaUpdated = h.formatearFecha(DateTime.Parse(dtNube.Rows[i][10].ToString()), true);
                                    rf.agregarRacionFijaLocal(numeroDia, rutEncriptado, tipoComida, habilitado, equipoEdicion, 1, createdEncriptado, updatedEncriptado, fechaCreated, fechaUpdated);
                                }
                            }
                        }
                    }
                    Log("Raciones Fijas nube sincronizadas...");
                }
            }
            catch (Exception ex)
            {
                generarLog("comprobarRacionesFijasNubeSinSincronizar() " + Environment.MachineName + " " + ex.ToString(), "ERROR");
            }
        }

        private DataTable traerDatoRacionesFijasLocal(string persona_rutEncriptado, int numeroDia, int tipoComida)
        {
            DataTable dtlocal;
            try
            {
                RacionFija rf = new RacionFija();
                dtlocal = rf.preguntarRegistroRacionFijaExistentLocal(persona_rutEncriptado, numeroDia, tipoComida);
            }
            catch (Exception ex)
            {
                dtlocal = null;
                generarLog("traerDatoRacionesFijasLocal() " + Environment.MachineName + " " + ex.ToString(), "ERROR");
            }
            return dtlocal;
        }

        // ======================== Inhabilitaciones Marcas ========================

        public void SincronizarInhabilitaciones()
        {
            comprobarInhabilitacionesLocalesSinSincronizar();
            sincronizarInhabilitacionesNubeALocal();
        }

        private void comprobarInhabilitacionesLocalesSinSincronizar()
        {
            try
            {
                bool actualizadoEnServidor = false;
                Log("Comprobando inhabilitaciones sin sincronizar...");
                InhabilitacionMarca im = new InhabilitacionMarca();
                Encriptacion en = new Encriptacion();
                DataTable dt;
                string rut_desencriptado = "";
                dt = im.traerInhabilitacionesSinSincronizar();

                if (dt != null)
                {
                    if (dt.Rows.Count > 0)
                    {
                        int valorActual = 0;
                        int totalRegistros = dt.Rows.Count;
                        Log("Sincronizando inhabilitaciones...");

                        for (int i = 0; i < dt.Rows.Count; i++)
                        {
                            valorActual = i + 1;
                            Log("Actualizando inhabilitaciones de marca local a nube: " + valorActual.ToString() + "/" + totalRegistros.ToString() + "...");
                            actualizadoEnServidor = false;
                            rut_desencriptado = en.Desencriptar(dt.Rows[i][1].ToString());
                            actualizadoEnServidor = agregarRegistroInhabilitacionEnNube(
                                rut_desencriptado,
                                int.Parse(dt.Rows[i][2].ToString()),
                                DateTime.Parse(dt.Rows[i][3].ToString()),
                                int.Parse(dt.Rows[i][4].ToString()),
                                dt.Rows[i][5].ToString(),
                                int.Parse(dt.Rows[i][9].ToString()));

                            if (actualizadoEnServidor)
                                actualizarEstadoRespaldadoInhabilitacionMarca(int.Parse(dt.Rows[i][0].ToString()));
                        }
                    }
                    else
                    {
                        Log("Inhabilitaciones Sincronizadas...");
                    }
                }
                else
                {
                    Log("Comprobando inhabilitaciones sin sincronizar... Error ,favor contactar son soporte rFlex");
                }
            }
            catch (Exception ex)
            {
                Log("Comprobando inhabilitaciones sin sincronizar... Error ,favor contactar son soporte rFlex");
                generarLog("comprobarInhabilitacionesLocalesSinSincronizar() " + ex.ToString(), "ERROR");
            }
        }

        private bool agregarRegistroInhabilitacionEnNube(string rut, int tim_tipoInhabilitacionMarca, DateTime fecha,
            int habilitado, string comentario, int turnoDia)
        {
            bool respuesta = false;
            try
            {
                InhabilitacionMarca im = new InhabilitacionMarca();
                respuesta = im.agregarInhabilitacionMarcaServidor(rut, tim_tipoInhabilitacionMarca,
                    fecha, habilitado, comentario, turnoDia);
            }
            catch (Exception ex)
            {
                generarLog("agregarRegistroInhabilitacionEnNube() " + ex.ToString(), "ERROR");
            }
            return respuesta;
        }

        private bool actualizarEstadoRespaldadoInhabilitacionMarca(int idinhbabilitacionMarca)
        {
            bool respuesta = false;
            try
            {
                InhabilitacionMarca im = new InhabilitacionMarca();
                respuesta = im.actualizarEstadoInhabilitacionMarcaLocal(idinhbabilitacionMarca);
            }
            catch (Exception ex)
            {
                generarLog("actualizarEstadoRespaldadoInhabilitacionMarca() " + ex.ToString(), "ERROR");
            }
            return respuesta;
        }

        private void sincronizarInhabilitacionesNubeALocal()
        {
            Log("Sincronizando inhabilitaciones de marca desde nube...");
            InhabilitacionMarca im = new InhabilitacionMarca();
            DataTable dtNube;

            try
            {
                DataTable dtFecha = im.traerUltimoUpdatedAtInhabilitacionMarca();
                string ultimoUpdated = _fechaUltimaActualizacion;
                if (dtFecha != null)
                {
                    if (dtFecha.Rows.Count > 0)
                    {
                        try
                        {
                            Herramientas h = new Herramientas();
                            ultimoUpdated = h.formatearFecha(DateTime.Parse(dtFecha.Rows[0][0].ToString()).AddHours(-3), true);
                        }
                        catch (Exception)
                        {
                            ultimoUpdated = _fechaUltimaActualizacion;
                        }
                    }
                }

                dtNube = im.traerInhabilitacionesSinSincronizarDesdeNubeOtroReloj(ultimoUpdated);
                if (dtNube != null)
                {
                    if (dtNube.Rows.Count > 0)
                    {
                        DataTable dtLocal;
                        Herramientas h = new Herramientas();
                        Encriptacion e = new Encriptacion();
                        string rutCifrado;
                        int valorActual = 0;
                        int totalRegistros = dtNube.Rows.Count;
                        for (int i = 0; i < dtNube.Rows.Count; i++)
                        {
                            valorActual = i + 1;
                            Log("Actualizando inhabilitaciones de marca nube a local: " + valorActual.ToString() + "/" + totalRegistros.ToString() + "...");
                            rutCifrado = e.Encriptar(dtNube.Rows[i][0].ToString());
                            dtLocal = comprobarExistenciaInhabilitacionDeMarcaLocal(
                                DateTime.Parse(dtNube.Rows[i][2].ToString()),
                                rutCifrado,
                                int.Parse(dtNube.Rows[i][1].ToString()),
                                int.Parse(dtNube.Rows[i][3].ToString()));
                            if (dtLocal != null)
                            {
                                if (dtLocal.Rows.Count > 0)
                                {
                                    DateTime fechaNube = DateTime.Parse(dtNube.Rows[i][10].ToString());
                                    if (!comprobarFechasEdicion(DateTime.Parse(dtLocal.Rows[0][1].ToString()), fechaNube))
                                    {
                                        im.actualizarDatoInhabilitacionMarcaPorDatoTraidoDesdeNube(fechaNube,
                                            int.Parse(dtNube.Rows[i][4].ToString()),
                                            int.Parse(dtNube.Rows[i][6].ToString()),
                                            int.Parse(dtLocal.Rows[0][0].ToString()));
                                    }
                                }
                                else
                                {
                                    im.agregarInhabilitacionMarcaTraidoDesdeNube(
                                        rutCifrado,
                                        int.Parse(dtNube.Rows[i][1].ToString()),
                                        DateTime.Parse(dtNube.Rows[i][2].ToString()),
                                        int.Parse(dtNube.Rows[i][3].ToString()),
                                        int.Parse(dtNube.Rows[i][4].ToString()),
                                        dtNube.Rows[i][5].ToString(),
                                        int.Parse(dtNube.Rows[i][6].ToString()),
                                        dtNube.Rows[i][8].ToString(),
                                        DateTime.Parse(dtNube.Rows[i][9].ToString()),
                                        DateTime.Parse(dtNube.Rows[i][10].ToString()));
                                }
                            }
                        }
                    }
                    Log("Inhabilitaciones de marca desde nube sincronizadas...");
                }
                else
                {
                    Log("Error Sincronizar inhabilitaciones de marca desde nube...");
                    generarLog("Error no se pudo obtener los datos de inhabilitaciones desde la nube. Nombre Equipo: " + Environment.MachineName, "ADVERTENCIA");
                }
            }
            catch (Exception ex)
            {
                generarLog("sincronizarInhabilitacionesNubeALocal() " + ex.ToString(), "ERROR");
            }
        }

        private DataTable comprobarExistenciaInhabilitacionDeMarcaLocal(DateTime fecha, string rut, int tipoInhabilitacionMarca, int turnoDia)
        {
            try
            {
                InhabilitacionMarca im = new InhabilitacionMarca();
                return im.consultarRegistroInhabilitacionLocal(fecha, rut, tipoInhabilitacionMarca, turnoDia);
            }
            catch (Exception ex)
            {
                generarLog("comprobarExistenciaInhabilitacionDeMarcaLocal() " + ex.ToString(), "ERROR");
                return null;
            }
        }

        // ======================== Logs ========================

        public void SincronizarLogs()
        {
            comprobarLogsSinSincronizar();
            comprobarLogsRelojSinSincronizar();
            // Purge local table: INFO logs (never go to cloud) older than 1 day,
            // and all processed logs older than 7 days.
            try { new LogSincronizador().purgarLogsInfoSinRespaldar(); }
            catch (Exception ex) { generarLog("purgarLogsInfo: " + ex.Message, "ADVERTENCIA"); }
            try { new LogSincronizador().eliminarLogsSincronizadorAntiguosYaRespaldados(); }
            catch (Exception ex) { generarLog("eliminarLogsAntiguos: " + ex.Message, "ADVERTENCIA"); }
        }

        private void comprobarLogsSinSincronizar()
        {
            try
            {
                Log("Comprobando logs de incidencias sin sincronizar");
                LogSincronizador ls = new LogSincronizador();
                DataTable dt = ls.traerLogsSinSincronizar();
                if (dt != null)
                {
                    if (dt.Rows.Count > 0)
                    {
                        for (int i = 0; i < dt.Rows.Count; i++)
                        {
                            try
                            {
                                string tipoLog = dt.Rows[i][4].ToString();
                                int id = int.Parse(dt.Rows[i][0].ToString());
                                // INFO logs stay local-only — mark done without uploading to cloud
                                if (tipoLog != "ERROR" && tipoLog != "ADVERTENCIA")
                                {
                                    actualizarEstadoLog(id);
                                    continue;
                                }
                                if (agregarLogsEnNube(dt.Rows[i][1].ToString(), dt.Rows[i][2].ToString(), DateTime.Parse(dt.Rows[i][3].ToString()), tipoLog))
                                {
                                    actualizarEstadoLog(id);
                                }
                            }
                            catch (Exception exRow)
                            {
                                generarLog("Error procesando log fila " + i + ": " + exRow.Message, "ADVERTENCIA");
                            }
                        }
                    }
                }
                else
                {
                    Log("Comprobando logs sin sincronizar... Error ,favor contactar son soporte rFlex");
                    generarLog("Error Comprobación logs frmSincronizador comprobarLogsSinSincronizar()", "ERROR");
                }
            }
            catch (Exception ex)
            {
                generarLog("Error Comprobación logs " + ex.ToString(), "ERROR");
            }
        }

        private bool agregarLogsEnNube(string equipo, string incidencia, DateTime fecha, string tipoError)
        {
            bool respuesta = false;
            try
            {
                LogSincronizador ls = new LogSincronizador();
                respuesta = ls.agregarLogErrorServidor(incidencia, equipo, fecha, tipoError);
            }
            catch (Exception ex)
            {
                generarLog("agregarLogsEnNube() Error Comprobación logs " + ex.ToString(), "ERROR");
            }
            return respuesta;
        }

        private bool actualizarEstadoLog(int idlogSincronizador)
        {
            bool respuesta = false;
            try
            {
                LogSincronizador ls = new LogSincronizador();
                respuesta = ls.actualizarEstadoLogErrorLocal(idlogSincronizador);
            }
            catch (Exception ex)
            {
                generarLog("actualizarEstadoLog() Error Comprobación logs " + ex.ToString(), "ERROR");
            }
            return respuesta;
        }

        private void comprobarLogsRelojSinSincronizar()
        {
            try
            {
                Log("Comprobando logs de reloj sin sincronizar");
                LogReloj lr = new LogReloj();
                DataTable dt = lr.traerLogsSinRespaldar();
                if (dt != null)
                {
                    if (dt.Rows.Count > 0)
                    {
                        for (int i = 0; i < dt.Rows.Count; i++)
                        {
                            if (agregarLogsRelojEnNube(dt.Rows[i][1].ToString(), dt.Rows[i][2].ToString(), dt.Rows[i][3].ToString(), dt.Rows[i][4].ToString()))
                            {
                                actualizarEstadoLogReloj(int.Parse(dt.Rows[i][0].ToString()));
                            }
                        }
                    }
                    Log("Logs de reloj sincronizados...");
                }
                else
                {
                    Log("Comprobando logs de reloj sin sincronizar... Error ,favor contactar son soporte rFlex");
                    generarLog("Error Comprobación logs frmSincronizador comprobarLogsRelojSinSincronizar()", "ERROR");
                }
            }
            catch (Exception ex)
            {
                generarLog("comprobarLogsRelojSinSincronizar() Error Comprobación logs " + ex.ToString(), "ERROR");
            }
        }

        private bool agregarLogsRelojEnNube(string equipo, string detalleLog, string fecha, string tipoLog)
        {
            bool respuesta = false;
            try
            {
                LogReloj lr = new LogReloj();
                respuesta = lr.agregarLogRelojNube(equipo, detalleLog, tipoLog);
            }
            catch (Exception ex)
            {
                generarLog("agregarLogsRelojEnNube() " + ex.ToString(), "ERROR");
            }
            return respuesta;
        }

        private bool actualizarEstadoLogReloj(int idlogReloj)
        {
            bool respuesta = false;
            try
            {
                LogReloj lr = new LogReloj();
                respuesta = lr.actualizarEstadoRespaldadoLogReloj(idlogReloj);
            }
            catch (Exception ex)
            {
                generarLog("actualizarEstadoLogReloj() " + ex.ToString(), "ERROR");
            }
            return respuesta;
        }

        // ======================== Marcas ========================

        public void SincronizarMarcas()
        {
            try
            {
                Marca m = new Marca();
                DataTable dt = m.traerMarcasAsistenciaSinRespaldar();

                Log("Comprobando sincronización de marcas...");
                if (dt != null)
                {
                    if (dt.Rows.Count > 0)
                    {
                        string numeroDedo;
                        int codigoMarca = 0;
                        Log("Actualizando sincronización de marcas...");
                        for (int i = 0; i < dt.Rows.Count; i++)
                        {
                            try
                            {
                                codigoMarca = int.Parse(dt.Rows[i][18].ToString());
                            }
                            catch (Exception)
                            {
                                codigoMarca = 0;
                            }
                            numeroDedo = dt.Rows[i][11].ToString().Equals("1") ? "marcaSinHuella" : dt.Rows[i][5].ToString();
                            if (agregarRegistroMarcaEnNube(
                                  int.Parse(dt.Rows[i][1].ToString()),
                                  dt.Rows[i][2].ToString(),
                                  int.Parse(dt.Rows[i][3].ToString()),
                                  int.Parse(dt.Rows[i][4].ToString()),
                                  numeroDedo,
                                  dt.Rows[i][7].ToString(),
                                  dt.Rows[i][8].ToString(),
                                  int.Parse(dt.Rows[i][9].ToString()),
                                  int.Parse(dt.Rows[i][11].ToString()),
                                  DateTime.Parse(dt.Rows[i][6].ToString()),
                                  dt.Rows[i][10].ToString(),
                                  int.Parse(dt.Rows[i][13].ToString()),
                                  dt.Rows[i][15].ToString(),
                                  int.Parse(dt.Rows[i][16].ToString()),
                                  dt.Rows[i][17].ToString(), codigoMarca))
                            {
                                actualizarEstadoDeRespadadoDeMarca(int.Parse(dt.Rows[i][0].ToString()));
                            }
                        }
                    }
                    Log("Marcas sincronizadas...");
                }
                else
                {
                    Log("Error al obtener datos de marcas... si el problema persiste favor contactar a soporte");
                    generarLog("comprobarMarcasSinSincronizar() Error al obtener datos de marcas... ", "ERROR");
                }
            }
            catch (Exception ex)
            {
                generarLog("comprobarMarcasSinSincronizar() " + ex.ToString(), "ERROR");
            }
        }

        private bool agregarRegistroMarcaEnNube(int tipoMarca, string persona_rut, int empresa_idempresa, int sucursal_idsucursal,
         string numero_dedo, string ingresador, string actualizador, int reloj_idreloj, int marcaConPin, DateTime fecha, string hash, int notificada, string zonaHoraria, int esHoraServidor, string incidencia, int codigoMarca)
        {
            bool respuesta = false;
            try
            {
                Marca m = new Marca();
                respuesta = m.agregarMarcaNube(tipoMarca, persona_rut,
                    empresa_idempresa, sucursal_idsucursal,
                    numero_dedo, ingresador,
                    actualizador, reloj_idreloj, marcaConPin, hash, fecha, notificada, zonaHoraria, esHoraServidor, incidencia, codigoMarca);
            }
            catch (Exception ex)
            {
                generarLog("agregarRegistroMarcaEnNube() " + ex.ToString(), "ERROR");
            }
            return respuesta;
        }

        private bool actualizarEstadoDeRespadadoDeMarca(int idmarca)
        {
            bool respuesta = false;
            try
            {
                Marca m = new Marca();
                respuesta = m.actualizarEstadoRespaldoMarca(idmarca);
            }
            catch (Exception ex)
            {
                generarLog("actualizarEstadoDeRespadadoDeMarca() " + ex.ToString(), "ERROR");
            }
            return respuesta;
        }

        // ======================== Rechazos ========================

        public void SincronizarRechazos()
        {
            Log("Comprobando rechazos de marca sin sincronizar...");
            try
            {
                DataTable dt;
                Rechazo r = new Rechazo();
                Encriptacion e = new Encriptacion();
                Herramientas h = new Herramientas();
                dt = r.traerRechazosSinRespaldar();
                if (dt != null)
                {
                    if (dt.Rows.Count > 0)
                    {
                        Log("Sincronizando rechazos de marca...");
                        for (int i = 0; i < dt.Rows.Count; i++)
                        {
                            if (r.agregarRechazoNube(int.Parse(dt.Rows[i][1].ToString()), int.Parse(dt.Rows[i][2].ToString()),
                                e.Desencriptar(dt.Rows[i][3].ToString()), int.Parse(dt.Rows[i][4].ToString()), int.Parse(dt.Rows[i][5].ToString()),
                                  e.Desencriptar(dt.Rows[i][6].ToString()), e.Desencriptar(dt.Rows[i][7].ToString()), 1, h.formatearFecha(DateTime.Parse(dt.Rows[i][8].ToString()), true), dt.Rows[i][9].ToString(),
                                  dt.Rows[i][11].ToString(), int.Parse(dt.Rows[i][12].ToString()), dt.Rows[i][13].ToString(), int.Parse(dt.Rows[i][14].ToString())))
                            {
                                r.actualizarEstadoRespaldoRechazo(int.Parse(dt.Rows[i][0].ToString()));
                            }
                        }
                        Log("Rechazos de marca sincronizados...");
                    }
                    else
                    {
                        Log("Rechazos de marca sincronizados...");
                    }
                }
                else
                {
                    generarLog("comprobarRechazosSinSincronizar() DtNull ", "ERROR");
                }
            }
            catch (Exception ex)
            {
                generarLog("comprobarRechazosSinSincronizar() " + ex.ToString(), "ERROR");
            }
        }
    }
}
