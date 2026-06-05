using proyectoNegocioRflex.Modelo;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Deployment.Application;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace proyectoNegocioRflex.Utilidades
{
    public class Herramientas
    {

        /**
         * Función que valida un rut completo.
         */
        public bool validarRut(string rut)
        {

            bool validacion = false;
            try
            {
                rut = rut.ToUpper();
                rut = rut.Replace(".", "");
                rut = rut.Replace("-", "");
                int rutAux = int.Parse(rut.Substring(0, rut.Length - 1));

                char dv = char.Parse(rut.Substring(rut.Length - 1, 1));

                int m = 0, s = 1;
                for (; rutAux != 0; rutAux /= 10)
                {
                    s = (s + rutAux % 10 * (9 - m++ % 6)) % 11;
                }
                if (dv == (char)(s != 0 ? s + 47 : 75))
                {
                    validacion = true;
                }
            }
            catch (Exception)
            {
            }
            return validacion;
        }

        /**
        * Función que administra la cantidad total de caracteres de un campo de texto 
        * con formato material design (no tiene limitador como propiedad.)
        */
        public static string cantidadCaracteres(string caracteres, int cantidadCaracteres)
        {
            return !String.IsNullOrEmpty(caracteres) && caracteres.Length >= 4 ?
                caracteres.Substring(0, cantidadCaracteres) : caracteres;
        }

        /**
         * Función que se encarga de liberar memoria que ya no será utilizada por el programa
         */
        [DllImport("kernel32.dll", EntryPoint = "SetProcessWorkingSetSize", ExactSpelling = true, CharSet = CharSet.Ansi, SetLastError = true)]
        private static extern int SetProcessWorkingSetSize(IntPtr process, int minimumWorkingSetSize, int maximumWorkingSetSize);
        public void liberarMemoria()
        {
            GC.Collect();
            GC.WaitForPendingFinalizers();
            SetProcessWorkingSetSize(System.Diagnostics.Process.GetCurrentProcess().Handle, -1, -1);
        }

        /**
         * Función que retorna el dígito verificador de un RUT.
         */
        public static string calcularDigitoVerificador(string rut)
        {
            int suma = 0;
            for (int x = rut.Length - 1; x >= 0; x--)
                suma += int.Parse(char.IsDigit(rut[x]) ? rut[x].ToString() : "0") * (((rut.Length - (x + 1)) % 6) + 2);
            int numericDigito = (11 - suma % 11);
            return numericDigito == 11 ? "0" : numericDigito == 10 ? "K" : numericDigito.ToString();
        }

        /**
         * Genera un Pin de 4 dígitos.
         */
        public int generarPinAleatorio()
        {
            Random aleatorio = new Random();
            return aleatorio.Next(1000, 9999);
        }

        /**
         * Función que formatea la fecha para poder ser guadada en mysql
         * yyy/mm/dd hh:mm:ss
         * El parámetro opcional indica si se debe respetar la hora de la fecha indicada en el parámetro.
         */
        public string formatearFecha(DateTime fecha, bool mantenerHora)
        {
            string dia = fecha.Day < 10 ? "0" + fecha.Day.ToString() : fecha.Day.ToString();
            string mes = fecha.Month < 10 ? "0" + fecha.Month.ToString() : fecha.Month.ToString();
            string hora = mantenerHora ? fecha.ToLongTimeString() : "00:00:01";
            return fecha.Year.ToString() + "-" + mes + "-" + dia + " " + hora;
        }

        /**
         * Función que devuelve un true o false dependiendo del valor entregado en el parámetro.
         * se ocupa para los checkbox que requieren un valor boolean y 
         * no aceptan unos o ceros traidos de la bd.
         */
        public static bool esVerdadero(string valor)
        {
            return int.Parse(valor) == 1 ? true : false;
        }

        /**
         * Función que permite saber si hay conexión con un equipo en particular dentro o fuera de la red
         * dependiendo de la dirección ingresada en el parámetro.
         */
        public bool pingEquipo(string ip)
        {
            bool respuesta = false;
            try
            {
                //Está conectado a alguna red?
                if (System.Net.NetworkInformation.NetworkInterface.GetIsNetworkAvailable())
                {
                    //Puede hacer ping a alguna ip?
                    Ping HacerPing = new Ping();
                    int iTiempoEspera = 3000; //3 segundos.
                    PingReply RespuestaPing;
                    RespuestaPing = HacerPing.Send(ip, iTiempoEspera);

                    //Comprobamos si se pudo hacer ping..
                    //si no se pudo buscamos en los atributos del IPStatus
                    //la posible causa.
                    if (RespuestaPing.Status == IPStatus.Success)
                    {
                        HacerPing.Dispose();
                        return true;
                    }
                    else
                    {
                        if (RespuestaPing.Status == IPStatus.DestinationPortUnreachable)
                        {
                            generarLogSincronizador("Ping comprobación internet: El puerto del equipo de destino no está disponible. Equipo: " + Environment.MachineName, "ADVERTENCIA");
                        }
                        else if (RespuestaPing.Status == IPStatus.TimedOut)
                        {
                            generarLogSincronizador("Ping comprobación internet: Se supero el tiempo de timeout para hacer ping. Equipo:  " + Environment.MachineName, "ADVERTENCIA");
                        }
                        else if (RespuestaPing.Status == IPStatus.DestinationHostUnreachable)
                        {
                            generarLogSincronizador("Ping comprobación internet: El equipo/dirección de destino no está disponible. Equipo: " + Environment.MachineName, "ADVERTENCIA");
                        }
                        else
                        {
                            generarLogSincronizador("Ping comprobación internet: Error desconocido.  Equipo: " + Environment.MachineName, "ADVERTENCIA");
                        }
                        HacerPing.Dispose();
                        return false;
                    }
                }
            }
            catch (Exception ex)
            {
                generarLogSincronizador("Ping comprobación internet: " + ex.InnerException.ToString() + " " + Environment.MachineName, "ADVERTENCIA");
                return false;
            }
            return respuesta;
        }

        /**
         * Función que retorna la hora del servidor. Si no puede obtenerla retorna el valor de la hora del equipo. (ambas formateadas)
         */
        public string obtenerHoraServidor()
        {
            EjecutoresSql ejecuta = new EjecutoresSql();
            string sql = "select now();";
            string fecha;
            DataTable dt = ejecuta.traerDatosDataTableServidor(sql);
            //Si es null quiere decir que no puedo conectar ojo ahí
            if (dt != null)
            {
                if (dt.Rows.Count > 0)
                {
                    fecha = formatearFecha(DateTime.Parse(dt.Rows[0][0].ToString()), true);
                }
                else
                {
                    //retornamos la hora del equipo formateada
                    fecha = formatearFecha(DateTime.Now, true);
                }
            }
            else
            {
                //retornamos la hora del equipo formateada
                fecha = formatearFecha(DateTime.Now, true);
            }
            return fecha;

        }

        /**
         *Retornaremos un arreglo con:
         * 0 La hora 
         * 1 zona Horaria
         * 2 si fue obtenida desde el servidor 1 para servidor 0 para local.
         */
        public string[] obtenerHoraServidorConTimeZone()
        {
            string[] arregloFechaDeterminada = new string[3];
            string fecha;
            try
            {
                EjecutoresSql ejecuta = new EjecutoresSql();
                string sql = "SELECT now(), TIME_FORMAT(TIMEDIFF(NOW(), UTC_TIMESTAMP),'%h:%i') AS zonaHoraria;";

                DataTable dt = ejecuta.traerDatosDataTableServidor(sql);
                if (dt != null)
                {
                    if (dt.Rows.Count > 0)
                    {
                        //Hora desde servidor
                        fecha = formatearFecha(DateTime.Parse(dt.Rows[0][0].ToString()), true);
                        arregloFechaDeterminada[0] = fecha;
                        arregloFechaDeterminada[1] = dt.Rows[0][1].ToString();
                        arregloFechaDeterminada[2] = "1";
                    }
                    else
                    {
                        //Hora desde local
                        //retornamos la hora del equipo formateada
                        fecha = formatearFecha(DateTime.Now, true);
                        arregloFechaDeterminada[0] = fecha;
                        arregloFechaDeterminada[1] = obtenerZonaHorariaLocal();
                        arregloFechaDeterminada[2] = "0";
                    }
                }
                else
                {
                    //Hora desde local
                    //retornamos la hora del equipo formateada
                    fecha = formatearFecha(DateTime.Now, true);
                    arregloFechaDeterminada[0] = fecha;
                    arregloFechaDeterminada[1] = obtenerZonaHorariaLocal();
                    arregloFechaDeterminada[2] = "0";
                }
            }
            catch (Exception)
            {
                //Hora desde local
                //retornamos la hora del equipo formateada
                fecha = formatearFecha(DateTime.Now, true);
                arregloFechaDeterminada[0] = fecha;
                arregloFechaDeterminada[1] = obtenerZonaHorariaLocal();
                arregloFechaDeterminada[2] = "0";
            }
            return arregloFechaDeterminada;
        }




        /**
         * Retornaremos un arreglo con:
         * 0 La hora 
         * 1 zona Horaria
         * 2 si fue obtenida desde el servidor 1 para servidor 0 para local.
         */
        public string[] obtenerHoraServidorConTimeZoneParaMarca(bool conConexionDetectada)
        {
            string[] arregloFechaDeterminada = new string[4];
            string fecha;
            try
            {
                DataTable dt = null;
                //si se detectó conexión en el equipo consultamos por la hora de servidor
                // de lo contrario preguntamos directamente por la fecha del equipo.
                if (conConexionDetectada)
                {
                    Servidor s = new Servidor();
                    dt = s.traerFechaTimezoneDesdeServidor();
                }

                if (dt != null)
                {
                    if (dt.Rows.Count > 0)
                    {
                        //Hora desde servidor
                        fecha = formatearFecha(DateTime.Parse(dt.Rows[0][0].ToString()), true);
                        arregloFechaDeterminada[0] = fecha; //Fecha que se va a ir guardando en en los registros
                        arregloFechaDeterminada[1] = dt.Rows[0][1].ToString(); //timezone
                        arregloFechaDeterminada[2] = "1"; //es hora servidor 1= si 0 = no.
                    }
                    else
                    {
                        //Hora desde local
                        //retornamos la hora del equipo formateada
                        fecha = formatearFecha(DateTime.Now, true);
                        arregloFechaDeterminada[0] = fecha;
                        arregloFechaDeterminada[1] = obtenerZonaHorariaLocal();
                        arregloFechaDeterminada[2] = "0";
                    }
                }
                else
                {
                    //Hora desde local
                    //retornamos la hora del equipo formateada
                    fecha = formatearFecha(DateTime.Now, true);
                    arregloFechaDeterminada[0] = fecha;
                    arregloFechaDeterminada[1] = obtenerZonaHorariaLocal();
                    arregloFechaDeterminada[2] = "0";
                }
            }
            catch (Exception)
            {
                //Hora desde local
                //retornamos la hora del equipo formateada
                fecha = formatearFecha(DateTime.Now, true);
                arregloFechaDeterminada[0] = fecha;
                arregloFechaDeterminada[1] = obtenerZonaHorariaLocal();
                arregloFechaDeterminada[2] = "0";
            }
            return arregloFechaDeterminada;
        }

        /**
         * Función que se encarga de obtener la zona horaria del equipo.
         */
        public string obtenerZonaHorariaLocal()
        {
            string zonaHoraria = "0";
            string hora;
            string minuto;
            try
            {
                hora = String.Format("{00}", TimeZoneInfo.Local.BaseUtcOffset.Hours);
                minuto = String.Format("{00}", TimeZoneInfo.Local.BaseUtcOffset.Minutes);
                if (minuto.Length == 1)
                {
                    minuto = "0" + minuto;
                }
                zonaHoraria = hora + ":" + minuto;
            }
            catch (Exception ex)
            {
                generarLogReloj("obtenerZonaHorariaLocal() " + Environment.MachineName + " " + ex.ToString(), "ERROR");
            }

            return zonaHoraria;
        }

        /**
         * Obtener la ip del equipo actual
         */
        public static string comprobarIpEquipoActual()
        {
            return Dns.GetHostEntry(Dns.GetHostName()).AddressList.Where(
                ip => ip.AddressFamily.ToString().ToUpper().Equals("INTERNETWORK")).FirstOrDefault().ToString();
        }

        /**
        * Obtener la mac del equipo
        */
        public static string comporbarMacPc()
        {
            string mac = "";
            foreach (NetworkInterface nic in NetworkInterface.GetAllNetworkInterfaces())
            {
                // Only consider Ethernet network interfaces
                if (nic.NetworkInterfaceType == NetworkInterfaceType.Ethernet &&
                    nic.OperationalStatus == OperationalStatus.Up)
                {
                    mac = nic.GetPhysicalAddress().ToString();
                }
            }
            return mac;
        }

        /**
         * Función que retorna true o false indicando si se está dentro de un rango permitido de comidas.
         */
        public bool horarioPermitidoDeComida(DateTime fechaActual)
        {
            Modelo.TipoComida tc = new Modelo.TipoComida();
            DataTable dt = tc.traerTipoComidaPorEstado(1);
            /*
                idtipoComida,        0
                nombre,              1
                horaInicioEmision,   2
                horaTerminoEmision,  3
                habilitada           4
             */
            return comprobarFechasParaTipoComida(dt, fechaActual , true);
        }


        public bool horarioPermitidoDeComidaPorSucursal(int idsucursal)
        {
            //La primera comprobación es para ver si hay alguna especificación en la sucursal.
            DateTime fechaActual = DateTime.Now;
            SucursalTipoComida stc = new SucursalTipoComida();
            DataTable dt = stc.traerSucursalTipoComidaLocalPorEstadoYSucursal(1, idsucursal);

            if (dt != null)
            {
                if (dt.Rows.Count > 0)
                {
                    /*
                       idsucursal_tipoComida,   0 
                       tipoComida_idtipoComida, 1
                       horaInicioEmision,       2
                       horaTerminoEmision,      3
                       habilitada               4
                     */
                    return comprobarFechasParaTipoComida(dt, fechaActual, false);
                }
            }

            //Si no tiene una configuración especifica.. entregamos la por defecto.
            /*
               idtipoComida,        0
               nombre,              1
               horaInicioEmision,   2
               horaTerminoEmision,  3
               habilitada           4
            */
            return horarioPermitidoDeComida(fechaActual);

        }

        /// <summary>
        /// Función que indica si hay a lo menos un tipo de cómida válido para solicitar la ración.
        /// </summary>
        /// <param name="dt"></param>
        /// <param name="fechaActual"></param>
        /// <param name="esHorarioTipoComidaNoSucursal"></param> acá indicamos si el registro es de tipo comida o sucursal_has_tipoComida 
        /// (esto nos diferencia para saber desde que indice obtener el valor del id del tipo de comida)
        /// <returns></returns>
        private bool comprobarFechasParaTipoComida(DataTable dt, DateTime fechaActual, bool esHorarioTipoComidaNoSucursal)
        {
            DateTime fechaInicioEmision;
            DateTime fechaTerminoEmision;
            bool permitido = false;
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                fechaInicioEmision = DateTime.Parse(dt.Rows[i][2].ToString());
                fechaTerminoEmision = DateTime.Parse(dt.Rows[i][3].ToString());
                // Acá revisamos si hay al menos un tipo de comida válido para solicitar el marcaje.
                //No mandamos el nombre del tipo de comida ya que acá no necesitamos, sólo nos sirve saber si el objeto no es null.
                permitido = checkValidoTipoComida(fechaActual, fechaInicioEmision, fechaTerminoEmision,
                    esHorarioTipoComidaNoSucursal ? int.Parse(dt.Rows[i][0].ToString()) : int.Parse(dt.Rows[i][1].ToString())
                    , "") != null;
                if (permitido)
                    break;
            }
            return permitido;
        }

        /// <summary>
        /// FUnción que entrega todos los tipos de comida que están dentro del rango de horas.
        /// </summary>
        /// <returns></returns>
        public List<TipoComida> tiposDeComidaDentroRangoHorario(DateTime fechaActual)
        {
            List<TipoComida> listaTipoComida = new List<TipoComida>();
            TipoComida tc = new TipoComida();
            DataTable dt = tc.traerTipoComidaPorEstado(1);
            //idtipoComida,        0
            //nombre,              1
            //horaInicioEmision,   2
            //horaTerminoEmision,  3
            //habilitada           4
            DateTime horaInicioEmision;
            DateTime horaTerminoEmision;
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                horaInicioEmision = DateTime.Parse(dt.Rows[i][2].ToString());
                horaTerminoEmision = DateTime.Parse(dt.Rows[i][3].ToString());

                //Hacemos la comprobación de los horarios y vemos si es valido.
                //de serlo la función nos retorna el obj de lo contrario será null
                var tipoComidaValido = checkValidoTipoComida(fechaActual, horaInicioEmision, horaTerminoEmision,
                    int.Parse(dt.Rows[i][0].ToString()), dt.Rows[i][1].ToString());

                if (tipoComidaValido != null)
                {
                    listaTipoComida.Add(tipoComidaValido);
                }
            }
            return listaTipoComida;
        }




        /// <summary>
        /// Función que entrega todos los tipos de comida que están dentro del rango de horas pero 
        /// preguntando por las configuraciones de tipos de comida por sucursal.
        /// </summary>
        /// <returns></returns>
        public List<TipoComida> tiposDeComidaDentroRangoHorarioPorSucursal(int idsucursal, DateTime fechaCobro)
        {
            //Primero verificamos una configuración especifica por sucursal para tipos de comida.
            List<TipoComida> listaTipoComida = new List<TipoComida>();
            SucursalTipoComida stc = new SucursalTipoComida();
            DataTable dt = stc.traerSucursalTipoComidaLocalPorEstadoYSucursal(1, idsucursal);
            /**
             * idsucursal_tipoComida,     0
             * tipoComida_idtipoComida,   1
             * horaInicioEmision,         2
             * horaTerminoEmision,        3
             * habilitada                 4
             * nombreTipoComida           5
             */
            if (dt == null)
            {
                return listaTipoComida;
            }

            if (dt.Rows.Count > 0)
            {
                DateTime horaInicioEmision;
                DateTime horaTerminoEmision;
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    horaInicioEmision = DateTime.Parse(dt.Rows[i][2].ToString());
                    horaTerminoEmision = DateTime.Parse(dt.Rows[i][3].ToString());
                    //Hacemos la comprobación de los horarios y vemos si es valido.
                    // de serlo la función nos retorna el obj de lo contrario será null
                    var tipoComidaValido = checkValidoTipoComida(fechaCobro, horaInicioEmision, horaTerminoEmision,
                        int.Parse(dt.Rows[i][1].ToString()), dt.Rows[i][5].ToString());

                    if (tipoComidaValido != null)
                    {
                        listaTipoComida.Add(tipoComidaValido);
                    }
                }
                return listaTipoComida;
            }
            //No se encontraron... revisamos los por defecto.
            return tiposDeComidaDentroRangoHorario(fechaCobro);
        }


        /**
         * Función que se encarga de retornar un tipo de comida validando los horario sde emisión con el horario solicitado.
         * Pueden haber dos casos (tienen distinta forma de comprobación).
         * Caso 1: Son los tipos de comida que tienen un horario de inicio de emisión menor que el de término (Caso Normal).
         * Caso 2: Son los tipos de comida que tienen la hora de inicio mayor que la de término o sea su rango se extiende hasta el día siguiente.
         */
        private TipoComida checkValidoTipoComida(DateTime fechaCobro, DateTime fechaInicioEmision, DateTime fechaTerminoEmision, int idtipoComida, string nombreTipoComida)
        {
            //Si detectamos que la hora de de tèrmino es menor que la de inicio
            //quiere decir que es un caso 2 y ahì cambia la forma de indentificar 
            //si se està en un horario de canje permitido

            if (fechaInicioEmision <= fechaTerminoEmision)
            {
                //Caso 1 
                if (fechaCobro >= fechaInicioEmision && fechaCobro <= fechaTerminoEmision)
                {
                    return setObjTipoComida(idtipoComida, nombreTipoComida, false);
                }
                // Si es un caso 1 y no se cumplió con los requsitos de fechas retornamos null, ya que de lo contrario revisará un falso caso 2
                return null;
            }

            //Acá es un caso 2
            //Revisión casos invertidos marca pm
            if (fechaCobro >= fechaInicioEmision)
            {
                return setObjTipoComida(idtipoComida, nombreTipoComida, false);
            }

            //Revisión casos invertidos AM.
            if (fechaCobro <= fechaTerminoEmision)
            {
                // Acá la ultima variable la retornamos con un true para indicar que la inhabilitación debe ser creada para el día anterior.
                // ya que es un cobro de madrugada, pero que pertenece a una comida del día anterior.
                return setObjTipoComida(idtipoComida, nombreTipoComida, true);
            }
            return null;
        }

        private TipoComida setObjTipoComida(int idtipoComida, string nombreTipoComida, bool horarioInvertido)
        {
            TipoComida tcPermitido = new TipoComida
            {
                IDdtipoComida = idtipoComida,
                NombreTipoComida = nombreTipoComida,
                HorarioInvertido = horarioInvertido // Dependiendo de esta variable sabremos si debemos crear la inhabilitaciòn de marca para el dìa anterior
            };

            return tcPermitido;
        }

        /**
         * Función que actualiza el texto ingresado a UPPERCASE.
         */
        public static string cambiarAMayuscula(string texto)
        {
            return texto.Length > 0 ? texto.ToUpper() : "";
        }

        /**
         *Función que se encarga de formatear la fecha para evitar incompatibilidades.
         * ej: minutos con sólo con un dígito.
         */
        public string formatearFechaSinHora(DateTime fechaAFormatear)
        {
            string dia = fechaAFormatear.Day < 10 ? "0" + fechaAFormatear.Day.ToString() : fechaAFormatear.Day.ToString();
            string mes = fechaAFormatear.Month < 10 ? "0" + fechaAFormatear.Month.ToString() : fechaAFormatear.Month.ToString();
            return fechaAFormatear.Year + "/" + mes + "/" + dia;
        }

        /**
         * Función que se encarga de hacer el formateo de los rut ingresado en el partámetro.
         */
        public string formatearRut(string rutSinFormato)
        {
            string rutFormateado;
            //obtengo la parte numerica del RUT
            string rutTemporal = rutSinFormato.Substring(0, rutSinFormato.Length - 1);
            //obtengo el Digito Verificador del RUT
            string dv = rutSinFormato.Substring(rutSinFormato.Length - 1, 1);
            //aqui convierto a un numero el RUT si ocurre un error lo deja en CERO
            if (!long.TryParse(rutTemporal, out long rut))
            {
                rut = 0;
            }

            //este comando es el que formatea con los separadores de miles
            rutFormateado = rut.ToString("N0");
            if (rutFormateado.Equals("0"))
            {
                rutFormateado = string.Empty;
            }
            else
            {
                //si no hubo problemas con el formateo agrego el DV a la salida
                rutFormateado += "-" + dv;

                //y hago este replace por si el servidor tuviese configuracion anglosajona y reemplazo las comas por puntos
                rutFormateado = rutFormateado.Replace(",", ".");
            }
            return rutFormateado;
        }

        /**
         * Función que verifica si los correo son válidos.
         */
        public bool mailValido(string email)
        {
            try
            {
                //Intentamos crear una instancia con el correo dado .. 
                //si no cae quiere decir que el correo es válido
                new MailAddress(email);
                // '^[A-Z0-9\._%-]+@[A-Z0-9\.-]+\.[A-Z]{2,4}$'
                // ^[A-Za-z0-9_.\-]+@[A-Za-z0-9_\-]+\.([A-Za-z0-9_\-]+\.)*[A-Za-z][A-Za-z]+$

                Regex oRegExp = new Regex(@"[A-Z0-9\._%-]+@[A-Z0-9\.-]+\.[A-Z]{2,4}$", RegexOptions.IgnoreCase);
                return oRegExp.Match(email).Success;
            }
            catch (FormatException)
            {
                //No es válido.
                return false;
            }

        }

        /**
         * Función que se encarga de quitar caractéres a los logs generados por las aplicaciones.
         * evitamos posibles simbolos que pueden causar problemas y se ahorra caracteres de ingreso en la bd.
         */
        public string formateadorLog(string error)
        {
            error = error.Replace("'", "");
            error = error.Replace(",", "");
            error = error.Replace(";", "");
            error = error.Replace("`", "");
            error = error.Replace("\"", "");
            error = error.Replace("+", " ");
            if (error.Length > 1998)
            {
                error = error.Substring(0, 1998);
            }
            return error;
        }

        /**
         * Función qu se encarga de generar el log dependiendo de la aplicación que lo ingrese.
         */
        public void generarLog(string error, string relojOSincronizador, string tipoLog)
        {
            if (relojOSincronizador.Equals("reloj"))
            {
                LogReloj l = new LogReloj();
                error = formateadorLog(error);
                l.agregarLogReloj(Environment.MachineName, error, tipoLog);
            }
            else
            {
                LogSincronizador l = new LogSincronizador();
                error = formateadorLog(error);
                l.agregarLogError(error, tipoLog);
            }
        }

        /**
         * Función que guarda los logs generados por el reloj.
         */
        public void generarLogReloj(string error, string tipoLog)
        {
            LogReloj lr = new LogReloj();
            error = formateadorLog(error.ToString());
            lr.agregarLogReloj(Environment.MachineName, error, tipoLog);
        }

        /**
         * Función que se guarda los logs generados por el sincronizador. 
         */
        public void generarLogSincronizador(string error, string tipoLog)
        {
            LogSincronizador ls = new LogSincronizador();
            error = formateadorLog(error.ToString());
            ls.agregarLogError(error, tipoLog);
        }

        /**
         *Función que se encarga de enviar una notificación al canale de slack del reloj.
         */
        public void enviarMensajeSlackReloj(string mensaje)
        {
            try
            {
                DataTable dtConfigSlack = traerConfigSlack(1);
                if (dtConfigSlack != null)
                {
                    //No encontramos datos..
                    Encriptacion e = new Encriptacion();
                    if (dtConfigSlack.Rows.Count > 0)
                    {
                        IntegracionSlack inte = new IntegracionSlack(e.Desencriptar(dtConfigSlack.Rows[0][1].ToString()));
                        _ = inte.enviarMensajeSlack(mensaje, e.Desencriptar(dtConfigSlack.Rows[0][2].ToString()), e.Desencriptar(dtConfigSlack.Rows[0][3].ToString()));
                        dtConfigSlack.Rows.Clear();
                    }
                    else
                    {
                        //Datos HardCoded
                        IntegracionSlack inte = new IntegracionSlack(""); // webhook removed — configure via DB
                        _ = inte.enviarMensajeSlack(mensaje, "#logreloj", "Alerta Reloj");
                    }
                }
            }
            catch (Exception ex)
            {
                generarLogReloj("enviarMensajeSlackReloj() " + ex.ToString(), "ERROR");
            }
        }

        /**
        * Función que se encarga de enviar una notificación al canale de slack del sincronizador.
        */
        public void enviarMensajeSlackSincronizador(string mensaje)
        {
            try
            {
                DataTable dtConfigSlack = traerConfigSlack(0);
                /**
                 * idconfigSlack,0
                 * webHook,      1 
                 * canal,        2
                 * userName,     3
                 * esCanalReloj, 4
                 * updated_at,   5
                 * habilitado    6
                 */
                if (dtConfigSlack != null)
                {
                    if (dtConfigSlack.Rows.Count > 0)
                    {
                        Encriptacion e = new Encriptacion();
                        IntegracionSlack inte = new IntegracionSlack(e.Desencriptar(dtConfigSlack.Rows[0][1].ToString()));
                        _ = inte.enviarMensajeSlack(mensaje, e.Desencriptar(dtConfigSlack.Rows[0][2].ToString()), e.Desencriptar(dtConfigSlack.Rows[0][3].ToString()));
                        dtConfigSlack.Rows.Clear();
                    }
                    else
                    {
                        //Datos HardCoded
                        IntegracionSlack inte = new IntegracionSlack(""); // webhook removed — configure via DB
                        _ = inte.enviarMensajeSlack(mensaje, "#logsincronizadorreloj", "Alerta Sincronizador Reloj");
                    }
                }

            }
            catch (Exception ex)
            {
                generarLogSincronizador("enviarMensajeSlackSincronizador() " + ex.ToString(), "ERROR");
            }
        }

        /**
         * Función que se encarga de verificar si los directorios de configuración están en el equipo. (rflexapps)
         */
        public bool existeDirectorio(string ruta, bool esSincronizador)
        {
            bool existe = false;
            try
            {
                existe = Directory.Exists(ruta);
                if (!existe)
                {
                    if (esSincronizador)
                    {
                        if (existeArchivoConexion())
                        {
                            //No tiene mucho sentido generar log si no están los archivos de conexión a la base de datos.
                            //generarLogSincronizador("existeDirectorio() No existe el directorio " + ruta + " En el Equipo : " + Environment.MachineName, "127.0.0.1", "ERROR");
                            enviarMensajeSlackSincronizador("existeDirectorio() No existe el directorio " + ruta + " En el Equipo : " + Environment.MachineName);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                if (existeArchivoConexion())
                {
                    generarLogSincronizador("existeDirectorio() " + ex.ToString(), "ERROR");
                    enviarMensajeSlackSincronizador("existeDirectorio(). No Existe la ruta " + ruta + " En el Equipo : " + Environment.MachineName + " " + ex.ToString());
                }
            }
            return existe;
        }

        /**
         * Función que se encarga de revisar si existe un archivo de configuración en especifico.
         */
        public bool existeArchivo(string rutaArchivo, bool esSincronizador)
        {
            bool existe = false;
            try
            {
                existe = File.Exists(rutaArchivo);
                if (!existe)
                {
                    if (esSincronizador)
                    {
                        if (existeArchivoConexion())
                        {
                            generarLogSincronizador("existeArchivo() No existe el archivo " + rutaArchivo + " En el Equipo : " + Environment.MachineName, "ERROR");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                if (esSincronizador)
                {
                    if (existeArchivoConexion())
                    {
                        generarLogSincronizador("existeArchivo() " + ex.ToString(), "ERROR");
                    }

                }

            }
            return existe;
        }

        /**
         * Función que se encarga de revisar si los archivos de conexión están disponibles y existen para ser usados por los programas.
         * @param bool esSincronizador define en caso de que sea necesario que tipo de log o nitificación se debe crear.
         */
        public bool comprobarArchivosBase(bool esSincronizador)
        {
            bool permitidoFuncionamiento = false;
            if (existeDirectorio("C:\\rflexapps", esSincronizador))
            {
                if (existeArchivo("C:\\rflexapps\\pems\\client-cert.pem", esSincronizador) &&
                    existeArchivo("C:\\rflexapps\\config\\conexionLocal.txt", esSincronizador) &&
                    existeArchivo("C:\\rflexapps\\config\\conexionPrincipal.txt", esSincronizador) &&
                    existeArchivo("C:\\rflexapps\\config\\conexionIntegracion.txt", esSincronizador) &&
                    existeArchivo("C:\\rflexapps\\config\\conexionRelojServidor.txt", esSincronizador))
                {
                    permitidoFuncionamiento = true;
                }
            }
            return permitidoFuncionamiento;
        }

        /**
         * Por el momento esta función retorna true. Hasta asegurarnos que los equipos no muestran problemas de permisos para consultar por los archivos de configs base..
         */
        public static bool existeArchivoConexion()
        {
            return true;
        }

        /**
         * Función que busca los datos de los canales de slack para poder hacer notificaciones.
         */
        public DataTable traerConfigSlack(int esCanalReloj)
        {
            DataTable dt;
            ConfigSlack c = new ConfigSlack();
            try
            {
                dt = c.traerConfigSlackPorEsCanalRelojYHabilitado(esCanalReloj);
                //Datos Traidos
                /**
                 * idconfigSlack,0
                 * webHook,      1 
                 * canal,        2
                 * userName,     3
                 * esCanalReloj, 4
                 * updated_at,   5
                 * habilitado    6
                 */
            }
            catch (Exception ex)
            {
                dt = null;
                if (esCanalReloj == 1)
                {
                    generarLogReloj("traerConfigSlack() " + ex.ToString(), "ERROR");
                }
                else
                {
                    generarLogSincronizador("traerConfigSlack() " + ex.ToString(), "ERROR");
                }
            }
            return dt;
        }

        /***
         * Función que lee los archivos de conexión indicados en la ruta del parámetro y retorna su valor desencriptado.
         */
        public string traerCadenaConexion(string rutaConexion)
        {
            Encriptacion e = new Encriptacion();
            string conexion = System.IO.File.ReadAllText(rutaConexion);
            return e.Desencriptar(conexion);
        }

        /**
         * Función utilizada por el codificador. Esta crea los archivos de conexión que serán usados por los programas complementarios del reloj.
         */
        public string generarArchivoConfiguracion(string ruta, string datoAEscribir)
        {
            string resultado;
            try
            {
                // eliminar el fichero si ya existe, ya que puede tener la cadena de conexión de otro cliente.
                if (File.Exists(ruta))
                    File.Delete(ruta);
                // Creamos el nuevo archivo con la cadena de conexión creado.
                using (var fileStream = File.Create(ruta))
                {
                    var texto = new UTF8Encoding(true).GetBytes(datoAEscribir);
                    fileStream.Write(texto, 0, texto.Length);
                    fileStream.Flush();
                }
                resultado = "ok";

            }
            catch (Exception ex)
            {
                resultado = "Error " + ex.ToString();
            }
            return resultado;
        }

        /**
         * Función que retorna el número del dia de la fecha ingresada.
         */
        public int obtenerNumeroDiaFecha(DateTime fecha)
        {
            //Nombre de dia en español
            try
            {
                //Si no existe el es-ES intentamos en el es-CL
                var culture = new System.Globalization.CultureInfo("es-ES");
                string nombreDia = culture.DateTimeFormat.GetDayName(fecha.DayOfWeek);
                return diaSemana(nombreDia);
            }
            catch (Exception)
            {
                var culture = new System.Globalization.CultureInfo("es-CL");
                string nombreDia = culture.DateTimeFormat.GetDayName(fecha.DayOfWeek);
                return diaSemana(nombreDia);
            }
        }

        /**
         * Retorna el número del dia de la semana.
         */
        public static int diaSemana(string dia)
        {
            int numeroDia = 0;
            switch (dia?.ToLower())
            {
                case "lunes":
                    numeroDia = 1;
                    break;
                case "martes":
                    numeroDia = 2;
                    break;
                case "miércoles":
                    numeroDia = 3;
                    break;
                case "jueves":
                    numeroDia = 4;
                    break;
                case "viernes":
                    numeroDia = 5;
                    break;
                case "sábado":
                    numeroDia = 6;
                    break;
                case "domingo":
                    numeroDia = 7;
                    break;
                default:
                    break;
            }
            return numeroDia;
        }

        public bool existeArchivoOpcional(string rutaArchivo)
        {
            bool existe = false;
            try
            {
                existe = File.Exists(rutaArchivo);
            }
            catch (Exception)
            {

            }
            return existe;
        }

        public String[] traerDirectoriosConfiguracionDisponibles(String directorio)
        {
            String[] directorios;
            try
            {
                directorios = System.IO.Directory.GetDirectories(directorio);
            }
            catch
            {
                directorios = null;
            }
            return directorios;
        }

        public string obtenerTipoComidaPorID(int tipoComida)
        {
            string nombreTipoComida = "";
            switch (tipoComida)
            {
                case 1:
                    nombreTipoComida = "desayuno";
                    break;
                case 2:
                    nombreTipoComida = "almuerzo";
                    break;
                case 3:
                    nombreTipoComida = "once";
                    break;
                case 4:
                    nombreTipoComida = "cena";
                    break;
                default:
                    break;
            }
            return nombreTipoComida;
        }

        /**
         * Función que realiza la comprobación si los programas están funcionando.
         * el sincronziador revisa si el reloj está funcionando y viceversa.
         */
        public void saberSiSeEjecutaPrograma(string programa)
        {
            try
            {
                //Si el array tiene algun registro quiere decir que el programa se está ejecutando.
                Process[] procesoEncontrado = Process.GetProcessesByName(programa);
                if (procesoEncontrado.Length == 0)
                {
                    // Si la variable programa = proyectoRelojControlRflex quiere decir que el sincornizador está comprobando.
                    // Si la variable programa = sincronizadorReloj  quiere decir que el programa reloj está comprobando.
                    if (programa.Equals(ConfiguracionesConstantes.PROYECTO_RELOJ_CONTROL))
                    {
                        generarLogSincronizador("El " + programa + " no se estaba ejecutando. Reloj: " + Environment.MachineName, "ADVERTENCIA");
                    }
                    else
                    {
                        generarLogReloj("El " + programa + " no se estaba ejecutando. Reloj: " + Environment.MachineName, "ADVERTENCIA");
                    }
                    ejecutarPrograma(programa);
                }
            }
            catch (Exception ex)
            {
                if (programa.Equals(ConfiguracionesConstantes.PROYECTO_RELOJ_CONTROL))
                {
                    generarLogSincronizador("El " + programa + " no se estaba ejecutando. Reloj: " + Environment.MachineName, "ADVERTENCIA");
                }
                else
                {
                    generarLogReloj("saberSiSeEjecutaPrograma() " + programa + " " + Environment.MachineName + " " + ex.ToString(), "ERROR");
                }
            }
        }

        /*
         * Función que se encarga de ejecutar el programa ingresado en el parámetro.
         * @param programa indica que programa se debe iniciar.
         * @param actualización indica si el reinicio corresponde a una actualización del programa
         */
        public void ejecutarPrograma(string programa, bool actualización = false)
        {
            string agregadoMensaje = actualización ? " (por Actualización)." : ".";
            try
            {
                StringBuilder sb = new StringBuilder();
                sb.Append(Environment.GetFolderPath(Environment.SpecialFolder.Programs));
                sb.Append("\\rFlex\\");

                if (programa.Equals("proyectoRelojControlRflex"))
                {
                    sb.Append("Reloj Control rFlex\\Reloj Control rFlex.appref-ms");
                }
                else if (programa.Equals(ConfiguracionesConstantes.PROYECTO_SINCRONIZADOR_CASINO))
                {
                    sb.Append("Sincronizador Casino rFlex\\Sincronizador Casino rFlex.appref-ms");
                }
                else
                {
                    sb.Append("Sincronizador rFlex\\Sincronizador rFlex.appref-ms");
                }
                string shortcutPath = sb.ToString();
                System.Diagnostics.Process.Start(shortcutPath);
                if (programa.Equals("proyectoRelojControlRflex"))
                {
                    generarLogSincronizador("El programa de reloj ha sido iniciado. Equipo: " + Environment.MachineName + agregadoMensaje, "ADVERTENCIA");
                }
                else if (programa.Equals(ConfiguracionesConstantes.PROYECTO_SINCRONIZADOR_CASINO))
                {
                    generarLogReloj("El sincronizador de casino ha sido iniciado. Equipo: " + Environment.MachineName + agregadoMensaje, "ADVERTENCIA");
                }
                else
                {
                    generarLogReloj("El sincronizador ha sido iniciado. Equipo: " + Environment.MachineName + agregadoMensaje, "ADVERTENCIA");
                }
            }
            catch (Exception ex)
            {
                if (programa.Equals("proyectoRelojControlRflex"))
                {
                    generarLogSincronizador("ejecutarPrograma() " + programa + " " + Environment.MachineName + " " + ex.ToString(), "ERROR");
                }
                else if (programa.Equals(ConfiguracionesConstantes.PROYECTO_SINCRONIZADOR_CASINO))
                {
                    generarLogSincronizador("ejecutarPrograma() " + programa + " " + Environment.MachineName + " " + ex.ToString(), "ERROR");
                }
                else
                {
                    generarLogReloj("ejecutarPrograma() " + programa + " " + Environment.MachineName + " " + ex.ToString(), "ERROR");
                }
            }
        }

        /**
         * Función que se encarga de ejecutar la actualización del programa que la está ejecutando.
         */
        public void actualizarDesatendido(string programa)
        {
            try
            {
                UpdateCheckInfo info = null;
                if ((ApplicationDeployment.IsNetworkDeployed))
                {
                    ApplicationDeployment AD = ApplicationDeployment.CurrentDeployment;
                    try
                    {
                        info = AD.CheckForDetailedUpdate();
                    }
                    catch (DeploymentDownloadException dde)
                    {
                        generarLogReloj("actualizarDesatendido() " + programa + " " + Environment.MachineName + " " + dde.ToString(), "ERROR");
                    }
                    catch (InvalidOperationException ioe)
                    {
                        generarLogReloj("actualizarDesatendido() " + programa + " " + Environment.MachineName + " " + ioe.ToString(), "ERROR");
                    }
                    if ((info.UpdateAvailable))
                    {
                        try
                        {
                            if (programa.Equals(ConfiguracionesConstantes.PROYECTO_SINCRONIZADOR))
                            {
                                generarLogSincronizador("actualizarDesatendido() Sincronzador Actualizando: " + Environment.MachineName, "ADVERTENCIA");
                            }
                            else if (programa.Equals(ConfiguracionesConstantes.PROYECTO_SINCRONIZADOR_CASINO))
                            {
                                generarLogSincronizador("actualizarDesatendido() Sincronzador Casino: Actualizando: " + Environment.MachineName, "ADVERTENCIA");
                            }
                            else
                            {
                                generarLogReloj("actualizarDesatendido() Actualizando: " + Environment.MachineName, "ADVERTENCIA");
                            }
                            AD.Update();
                            ejecutarPrograma(programa, true);
                            Process.GetCurrentProcess().Kill();
                        }
                        catch (DeploymentDownloadException dde)
                        {
                            generarLogReloj("actualizarDesatendido() " + programa + " " + Environment.MachineName + " " + dde.ToString(), "ERROR");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                generarLogReloj("actualizarDesatendido() " + programa + " " + Environment.MachineName + " " + ex.ToString(), "ERROR");
            }

        }


        /**
         * Función que se encarga de comprobar si hay actualizaciones disponibles del programa que la ejecuta.
         */
        public bool comprobacionActualizacionDisponible(string programa)
        {
            bool hayActualizacion = false;
            try
            {
                UpdateCheckInfo info = null;
                if ((ApplicationDeployment.IsNetworkDeployed))
                {
                    ApplicationDeployment AD = ApplicationDeployment.CurrentDeployment;
                    try
                    {
                        info = AD.CheckForDetailedUpdate();
                    }
                    catch (DeploymentDownloadException dde)
                    {
                        generarLogReloj("comprobacionActualizacionDisponible() " + programa + " " + Environment.MachineName + " " + dde.ToString(), "ERROR");
                    }
                    catch (InvalidOperationException ioe)
                    {
                        generarLogReloj("comprobacionActualizacionDisponible() " + programa + " " + Environment.MachineName + " " + ioe.ToString(), "ERROR");
                    }
                    hayActualizacion = info.UpdateAvailable;
                    info = null;
                    AD = null;
                }
            }
            catch (Exception ex)
            {
                generarLogReloj("comprobacionActualizacionDisponible() " + programa + " " + Environment.MachineName + " " + ex.ToString(), "ERROR");
            }
            return hayActualizacion;
        }

        /**
         * Función que se encarga de obtener el número de la versión del programa que la ejecuta.
         * En vista que hay equipos que generan problemas con la comprobación las versiones se irán 
         * agregando manualmente y se obtendrán dependiendo del proyecto.
         */
        public string obtenerNumeroVersion(string proyecto)
        {
            switch (proyecto)
            {
                case ConfiguracionesConstantes.PROYECTO_SINCRONIZADOR:
                    return "v1.0.0.65";
                case ConfiguracionesConstantes.PROYECTO_SINCRONIZADOR_CASINO:
                    return "v1.0.0.15";
                default: //Reloj
                    return "v1.0.0.60";
            }

        }

        /**
         * Función que determina si la marca corresponde al tipo turno noche o día.
         */
        public int determinarTipoTurno(DateTime horaMarca)
        {
            int turnoDia = 1;
            try
            {
                TipoTurno tt = new TipoTurno();
                DataTable dt;
                DateTime horaInicio;
                DateTime horatermino;
                dt = tt.traerTipoTurnoLocal();
                //idtipoTurno,  0
                //turnoDia,     1
                //horaInicio,   2
                //horaTermino   3
                if (dt != null)
                {
                    if (dt.Rows.Count > 0)
                    {
                        for (int i = 0; i < dt.Rows.Count; i++)
                        {
                            horaInicio = DateTime.Parse(dt.Rows[i][2].ToString());
                            horatermino = DateTime.Parse(dt.Rows[i][3].ToString());
                            //Si es turno noche le agregamos un ´día pára reconocer la hora de termino sea mayor a la de la marca
                            // de lo contrario como son horas de madrugada siempre la hora de la marca será mayor
                            //que la del periodo marcado ocmo turno noche.
                            if (dt.Rows[i][1].ToString().Equals("0"))
                            {
                                horatermino = horatermino.AddDays(1);
                            }
                            if (horaInicio <= horaMarca && horatermino >= horaMarca)
                            {
                                turnoDia = dt.Rows[i][1].ToString().Equals("True") || dt.Rows[i][1].ToString().Equals("1") ? 1 : 0;
                                break;
                            }
                        }

                    }
                }
            }
            catch (Exception ex)
            {
                generarLogReloj("determinarTipoTurno() " + Environment.MachineName + " " + ex.ToString(), "ERROR");
            }
            return turnoDia;
        }

        /**
         * 
         * Funcion que retorna el espacio disponible del disco duro 
         * Los retorna en MB.
         */
        public string obtenerEspacioLibreDiscoDuro()
        {
            string espacioDisponible = "0";
            try
            {
                DriveInfo[] drives = DriveInfo.GetDrives();
                foreach (DriveInfo drive in drives)
                {
                    if (drive.Name.Contains("C"))
                    {
                        espacioDisponible = String.Format("{0:##}", drive.TotalFreeSpace / 1048576.0);
                        break;
                    }
                }
            }
            catch (Exception ex)
            {
                espacioDisponible = "0";
                generarLogSincronizador("obtenerEspacioLibreDiscoDuro() " + Environment.MachineName + " " + ex.ToString(), "ERROR");
            }

            return espacioDisponible;
        }

        /**
         *Función que transforma datos en MB a GB 
         */
        public static string convertirMegasAGigas(int megas)
        {
            string gigas;
            try
            {
                gigas = string.Format("{0:0.00}", (megas / 1000));
            }
            catch (Exception)
            {
                gigas = "0";
            }
            return gigas;
        }

        /// <summary>
        /// Función que retorna un true o false que indica a través del dato de tieneConexion
        /// si el equipo tiene conexión a internet o no.
        /// </summary>
        /// <param name="ip"></param>
        /// <returns></returns>
        public bool obtenerInformacionConexionDetectada()
        {
            try
            {
                DataTable dt;
                DetectadaConexion dc = new DetectadaConexion();
                dt = dc.traeRegistroEstadoConexion();
                if (dt != null)
                {
                    if (dt.Rows.Count > 0)
                    {
                        return int.Parse(dt.Rows[0][2].ToString()) == 1;
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                generarLogReloj("obtenerInformacionConexionDetectada() " + Environment.MachineName + " " + ex.ToString(), "ERROR");
                return true;
            }

        }

        /**
         * Función que se encarga de agregar los segundos de marca a la fecha obtenida por consulta.
         */
        public string agregarSegundosAFechaTraida(DateTime fecha, int segundos)
        {
            string fechaFinal;
            try
            {
                fechaFinal = formatearFecha(fecha.AddSeconds(segundos), true);
            }
            catch (Exception ex)
            {
                generarLogReloj("agregarSegundosAFechaTraida() " + Environment.MachineName + " " + ex.ToString(), "ERROR");
                fechaFinal = formatearFecha(fecha, true);
            }
            return fechaFinal;
        }

        /**
         * Función que se encarga de poder comprobar si el equipo tiene internet comprobando 
         * la conexión a una url indicada en el marámetro mURL.
         */
        public bool VerificarConexionPorURL(string mURL)
        {
            bool conConexion = false;

            try
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(mURL);
                request.KeepAlive = false;
                using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                {
                    conConexion = response.StatusCode.ToString().Equals("OK");
                    response.Dispose();
                }
                request.Abort();
            }
            catch (System.Net.WebException ex)
            {
                if (ex.Status == System.Net.WebExceptionStatus.NameResolutionFailure)
                {
                    generarLogSincronizador("VerificarConexionPorURL() No se puede acceder al dominio para comprobación. Equipo: " + Environment.MachineName + " " + ex.ToString(), "ERROR");
                }
            }
            return conConexion;
        }

        /**
         * Función que sirve para consultar si existe un archivo indicado especificamente en el parámetro rutaArchivo.
         */
        public bool preguntarArchivoExistenteGenerico(string rutaArchivo, bool esReloj)
        {
            bool existe = false;
            try
            {
                existe = File.Exists(rutaArchivo);
            }
            catch (Exception ex)
            {
                if (esReloj)
                {
                    generarLogReloj("preguntarArchivoExistenteGenerico() " + rutaArchivo + " " + Environment.MachineName + " " + ex.ToString(), "ERROR");
                }
                else
                {
                    generarLogSincronizador("preguntarArchivoExistenteGenerico() " + rutaArchivo + " " + Environment.MachineName + " " + ex.ToString(), "ERROR");
                }
            }
            return existe;
        }

        /// <summary>
        /// FUnción que se encarga de generar los datos que serán utilizados para armar los códigos de los tickets de casino.
        /// </summary>
        /// <param name="valor"></param>
        /// <param name="cantidadCaracteres"></param>
        /// <returns></returns>
        public static string configurarCodigoParaTicket(string valor, int cantidadCaracteres)
        {
            for (int i = 0; i < cantidadCaracteres; i++)
            {
                if (valor.Length < cantidadCaracteres)
                {
                    valor = "0" + valor;
                }
            }
            return valor;
        }

        /// <summary>
        /// FUnción que retorna la fecha hora del registro sin los caracteres especiales de la fecha y hora
        /// </summary>
        /// <param name="fechaSistema"></param>
        /// <returns></returns>
        public string quitarCaracteresEspecialesFechaParaHash(string fechaSistema)
        {
            string fechaSistemaSinCaracteresEspeciales = fechaSistema.Replace("/", "");
            fechaSistemaSinCaracteresEspeciales = fechaSistemaSinCaracteresEspeciales.Replace("-", "");
            fechaSistemaSinCaracteresEspeciales = fechaSistemaSinCaracteresEspeciales.Replace(":", ":");
            return fechaSistemaSinCaracteresEspeciales;
        }

        /// <summary>
        /// Función que agrega una alerta de error.
        /// </summary>
        /// <param name="mensaje">string</param>
        /// <param name="idreloj">int</param>
        /// <param name="idempresa">int</param>
        /// <param name="idsucursal">int</param>
        /// <param name="idcategoria">int</param>
        public void generarAlertaError(string mensaje, int idreloj, int idempresa, int idsucursal, int idcategoria)
        {
            AlertaError ae = new AlertaError();
            string fecha = formatearFecha(DateTime.Now, true);
            ae.agregarAlertaLocal(mensaje, idreloj, idempresa, idsucursal, 0, 0, fecha, idcategoria);
        }


        /// <summary>
        /// Función que se encarga de registrar localmente los datos de la versión de c/u de los proyectos.
        /// y como es una función que se va a utilizar en todos lo crearé en Herramientas.
        /// </summary>
        /// <param name="idreloj"></param>
        /// <param name="version"></param>
        /// <param name="proyecto"></param>
        public void registrarVersion(int idreloj, string proyecto)
        {
            try
            {
                //Primero vamos verificar que existe algun registro de versiones.
                Modelo.Version v = new Modelo.Version();
                DataTable dt = v.preguntarDatoVersionExistenteLocal(idreloj);
                string versionProyecto = obtenerNumeroVersion(proyecto);

                if (dt == null)
                {
                    generarLogReloj("No se pudo conectar con la tabla version", "Error");
                    return;
                }

                if (dt.Rows.Count > 0)
                {
                    //Si hay un registro tenemos que ir actualizando ya que en local sólo debería haber un registro en la tabla.
                    //verificamos si el dato que se quiere agregar es el mismo registrado.
                    /*
                       reloj_idreloj,         0
                       relojControl,          1
                       sincronizador,         2
                       sincronizadorCasino,   3
                       created_at,            4
                       updated_at             5
                     */

                    //Revisamos por proyecto .. si es proyecto es sincronizador y la versión es distinta... actualizamos
                    if (proyecto.Equals(ConfiguracionesConstantes.PROYECTO_RELOJ_CONTROL) && !dt.Rows[0][1].ToString().Equals(versionProyecto))
                    {
                        guardarVersionLocal(proyecto, versionProyecto, idreloj);
                        return;
                    }

                    if (proyecto.Equals(ConfiguracionesConstantes.PROYECTO_SINCRONIZADOR) && !dt.Rows[0][2].ToString().Equals(versionProyecto))
                    {
                        guardarVersionLocal(proyecto, versionProyecto, idreloj);
                        return;
                    }

                    if (proyecto.Equals(ConfiguracionesConstantes.PROYECTO_SINCRONIZADOR_CASINO) && !dt.Rows[0][3].ToString().Equals(versionProyecto))
                    {
                        guardarVersionLocal(proyecto, versionProyecto, idreloj);
                        return;
                    }
                    return;
                }

                //No existe registro ... bueno lo creamos.
                if (!v.agregarDatosVersionLocal(idreloj, versionProyecto, ConfiguracionesConstantes.PROYECTO_SINCRONIZADOR))
                {
                    generarLogSincronizador("registrarVersion() Equipo: " + Environment.MachineName + "No se pudo guardar un nuevo registro de versión.", "ERROR");
                }
            }
            catch (Exception ex)
            {
                generarLogSincronizador("registrarVersion() Equipo: " + Environment.MachineName + " " + ex.ToString(), "ERROR");
            }
        }

        private void guardarVersionLocal(string proyecto, string version, int idreloj)
        {
            try
            {
                Modelo.Version v = new Modelo.Version();
                string fecha = formatearFecha(DateTime.Now, true);
                if (!v.actualizarRegistroVersionLocal(proyecto, version, fecha, idreloj))
                {
                    generarLogReloj("No se pudo actualizar la tabla version", "Error");
                }
            }
            catch (Exception ex)
            {
                generarLogSincronizador("registrarVersion() Equipo: " + Environment.MachineName + " " + ex.ToString(), "ERROR");
            }

        }

        public static bool isNumeric(string valor)
        {
            int numero;
            return int.TryParse(valor, out numero);
        }

        /// <summary>
        /// FUnción que se encarga de calcular la fecha de vencimiento del ticket de casino generado.
        /// </summary>
        /// <param name="tipoComida"></param>
        /// <param name="idsucursal"></param>
        /// <param name="fechaMarca"></param>
        /// <returns>string</returns>
        public string calcularFechaVencimientoTicket(int tipoComida, int idsucursal, DateTime fechaMarca)
        {
            try
            {
                //Primero verificamos si la sucursal que tiene el reloj se le ha agregado alguna configuración en específico.
                SucursalTipoComida stp = new SucursalTipoComida();
                DataTable dt = stp.traerDiasVencimientoTicketTipoComidaPorIdTipoComidaYSucursal(idsucursal, tipoComida);

                //Lo haré con else, ya que haciendo con if sin anidar queda muy enredado...
                if (dt != null)
                {
                    if (dt.Rows.Count > 0)
                    {
                        int cantidadDias = int.Parse(dt.Rows[0][0].ToString());
                        return formatearFecha(fechaMarca.AddDays(cantidadDias), true);
                    }
                }
                else
                {
                    generarLogReloj("Error al consultar la función traerDiasVencimientoTicketTipoComidaPorIdTipoComidaYSucursal.", "Error");
                }

                //Si llegamos acá preguntamos por el defecto de la tabla tipo comida.
                TipoComida tc = new TipoComida();
                dt = tc.traerTipoComidaPorID(tipoComida);
                /**
                 * idtipoComida,           0
                 * updated_at,             1
                 * diasExpiracionTicket    2
                 */

                //Haremos el mismo procedimiento anterior.

                //Lo haré con else, ya que haciendo con if sin anidar queda muy enredado...
                if (dt != null)
                {
                    if (dt.Rows.Count > 0)
                    {
                        int cantidadDias = int.Parse(dt.Rows[0][2].ToString());
                        return formatearFecha(fechaMarca.AddDays(cantidadDias), true);
                    }
                }
                else
                {
                    generarLogReloj("Error al consultar la función traerTipoComidaPorID (calcularFechaVencimientoTicket).", "Error");
                }

                //Ya, si llegamos a este punto ocupamos como emergencia la constante por defecto de días
                return formatearFecha(fechaMarca.AddDays(ConfiguracionesConstantes.DIAS_VENCIMIENTO_TICKET), true);
            }
            catch (Exception)
            {
                //Ultimo recurso si fallan las 2 tablas.
                generarLogReloj("Error al calcular la fecha de vencimiento. Se han otorgado " + ConfiguracionesConstantes.DIAS_VENCIMIENTO_TICKET + " días por defecto.", "Error");
                return formatearFecha(fechaMarca.AddDays(ConfiguracionesConstantes.DIAS_VENCIMIENTO_TICKET), true);
            }
        }

        /// <summary>
        /// Función que retorna la fecha del dia anterior con la hora al final del día.
        /// </summary>
        /// <returns>Datetime</returns>
        public DateTime getDiaAnterior()
        {
            return DateTime.Now.AddDays(-1).AddTicks(-1);
        }

    }
}
