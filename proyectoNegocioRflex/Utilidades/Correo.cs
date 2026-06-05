using proyectoNegocioRflex.Modelo;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Net.Mail;
using System.Net.Mime;
using System.Reflection;
using System.Text;

namespace proyectoNegocioRflex.Utilidades
{
    public class Correo
    {


        #region Tipos de Notificacion Antiguos (respaldo)
        public bool enviarReportePorCorreo(string correoTrabajador, string rut, string nombre, string fechaMarca, string checksum, string sentido, string correoNotificacion,
    string hostNotificacion, string puerto, string pass, string usuario)
        {
            ConfiguracionNotificacion cn = new ConfiguracionNotificacion();
            try
            {
                System.Net.Mail.MailMessage _Message = new System.Net.Mail.MailMessage();
                System.Net.Mail.SmtpClient _SMTP = new System.Net.Mail.SmtpClient();
                bool respuesta = false;
                //CONFIGURACIÓN DEL STMP
                //si son correos por servidores propios (no gmail, outlook,etc) el usuario debe ser de la credencial de la cuenta
                //Si son correos tradicionales el usuario debe ser la misma cuenta de correo.
                _SMTP.Credentials = new System.Net.NetworkCredential(usuario, pass);
                _SMTP.Host = hostNotificacion.Trim();
                _SMTP.Port = int.Parse(puerto);
                //_SMTP.Credentials = new System.Net.NetworkCredential("alejandro@rflex.cl", enc.Desencriptar("aS2L/ZEYtuzbF0rzFfO3lg=="));
                //_SMTP.Host = "smtp.gmail.com";
                //_SMTP.Port = 587;
                _SMTP.EnableSsl = true;
                // CONFIGURACIÓN DEL MENSAJE
                _Message.To.Add(correoTrabajador); //Cuenta de Correo al que se le quiere enviar el e-mail
                //_Message.From = new System.Net.Mail.MailAddress("alejandro@rflex.cl", "rFlex - Notificaciones", System.Text.Encoding.UTF8); //Quien lo envía
                _Message.From = new System.Net.Mail.MailAddress(correoNotificacion, "rFlex - Notificaciones", System.Text.Encoding.UTF8); //Quien lo envía
                _Message.Subject = "Notificación de marca";
                _Message.SubjectEncoding = System.Text.Encoding.UTF8; //Codificacion
                _Message.Body = crearHtmlCorreo(rut, nombre, fechaMarca, checksum, sentido);
                _Message.BodyEncoding = System.Text.Encoding.UTF8;
                _Message.Priority = System.Net.Mail.MailPriority.Normal;
                _Message.IsBodyHtml = true;
                //ENVIO
                try
                {
                    _SMTP.Send(_Message);
                    respuesta = true;
                    _SMTP.Dispose();
                }
                catch (System.Net.Mail.SmtpException ex)
                {
                    generarLog("enviarReportePorCorreo() " + ex.ToString(), "ERROR");
                    respuesta = false;
                }
                return respuesta;
            }
            catch (Exception ex)
            {
                generarLog("enviarReportePorCorreo() SEGUNDO CATCH " + ex.ToString(), "ERROR");
                return false;
            }
        }

        public static string crearHtmlCorreo(string rut, string nombre, string fechaMarca, string checksum, string sentido)
        {
            try
            {
                string myHtmlFile = "";
                //Creamos el Objeto.
                StringBuilder myBuilder = new StringBuilder();
                //Comienza la elaboración del HTML que irá en el correo.
                myBuilder.Append("<html xmlns='http://www.w3.org/1999/xhtml'>");
                myBuilder.Append("<head>");
                myBuilder.Append("<meta http-equiv=\"Content-Type\" content=\"text/html; charset=utf-8\"/>");
                myBuilder.Append("<title>");
                myBuilder.Append("Page-");
                myBuilder.Append(Guid.NewGuid().ToString());
                myBuilder.Append("</title>");
                myBuilder.Append("</head>");
                myBuilder.Append("<body>");
                myBuilder.Append("<p> Estimado(a) " + nombre + " (RUT: " + rut + ") enviamos detalle de marca reciente.</p></br>");
                myBuilder.Append("<table border='1px' cellpadding='5' cellspacing='0' ");
                myBuilder.Append("style='border: solid 1px Silver; font-size: small;'>");
                //Agregando Cabeceras.
                myBuilder.Append("<tr align='left' valign='top'>");
                myBuilder.Append("<td align='left' valign='top'>");
                myBuilder.Append("Fecha y hora de la marca: " + fechaMarca);
                myBuilder.Append("</td>");

                myBuilder.Append("<td align='left' valign='top'>");
                myBuilder.Append("Sentido de la Marca: " + sentido);
                myBuilder.Append("</td>");

                myBuilder.Append("<td align='left' valign='top'>");
                myBuilder.Append("Checksum: " + checksum);
                myBuilder.Append("</td>");

                myBuilder.Append("</tr>");
                //Se cierran los Tags.
                myBuilder.Append("</table>");
                myBuilder.Append("<p>Si encuentra inconsistencias en los datos o errores de sistema, favor avisar al administrador o contáctenos en http://www.soporte.rflex.cl.</p>");
                myBuilder.Append("<p>Saluda Atte. a Ud.</p>");
                myBuilder.Append("<p>rFlex.</p>");
                myBuilder.Append("</body>");
                myBuilder.Append("</html>");
                //Documento Generado.
                myHtmlFile = myBuilder.ToString();
                return myHtmlFile;
            }
            catch (Exception ex)
            {
                string error = "crearHtmlCorreo() " + ex.ToString();
                LogSincronizador l = new LogSincronizador();
                Herramientas h = new Herramientas();
                error = h.formateadorLog(error);
                l.agregarLogError(error, "ERROR");
                return "";
            }
        }

        public static string crearHtmlCorreoMarca(string correoTrabajador, string rutFormateadoTrabajador, string nombreCompleto, string rutEmpresaFormateada,
              string razonSocial, string sentido, string sucursal, string direccion, string ubicacionReloj, string datoReloj, string hash, string fechaMarcaFormateada)
        {
            try
            {
                string myHtmlFile = "";
                //Creamos el Objeto.
                StringBuilder myBuilder = new StringBuilder();
                //Comienza la elaboración del HTML que irá en el correo.
                myBuilder.Append("<html xmlns='http://www.w3.org/1999/xhtml'>");
                myBuilder.Append("<head>");
                myBuilder.Append("<meta http-equiv=\"Content-Type\" content=\"text/html; charset=utf-8\"/>");
                myBuilder.Append("<title>");
                myBuilder.Append("Page-");
                myBuilder.Append(Guid.NewGuid().ToString());
                myBuilder.Append("</title>");
                myBuilder.Append("</head>");
                myBuilder.Append("<body>");
                myBuilder.Append("<p> Estimado(a) " + nombreCompleto + " (RUT: " + rutFormateadoTrabajador + ") enviamos detalle de marca reciente.</p></br>");
                myBuilder.Append("<table>");
                myBuilder.Append("<style='font-size: small;'>");
                //Agregando Cabeceras.
                myBuilder.Append("<tr align='left' valign='top'>");
                myBuilder.Append("<td align='left' valign='top'>");
                myBuilder.Append("Razón social: " + razonSocial);
                myBuilder.Append("</td>");
                myBuilder.Append("</tr>");

                myBuilder.Append("<tr align='left' valign='top'>");
                myBuilder.Append("<td align='left' valign='top'>");
                myBuilder.Append("RUT Empresa: " + rutEmpresaFormateada);
                myBuilder.Append("</td>");
                myBuilder.Append("</tr>");

                myBuilder.Append("<tr align='left' valign='top'>");
                myBuilder.Append("<td align='left' valign='top'>");
                myBuilder.Append("Sucursal: " + sucursal);
                myBuilder.Append("</td>");
                myBuilder.Append("</tr>");

                myBuilder.Append("<tr align='left' valign='top'>");
                myBuilder.Append("<td align='left' valign='top'>");
                myBuilder.Append("Dirección Sucursal: " + direccion);
                myBuilder.Append("</td>");
                myBuilder.Append("</tr>");

                myBuilder.Append("<tr align='left' valign='top'>");
                myBuilder.Append("<td align='left' valign='top'>");
                myBuilder.Append("Ubicación Reloj: " + ubicacionReloj);
                myBuilder.Append("</td>");
                myBuilder.Append("</tr>");

                myBuilder.Append("<tr align='left' valign='top'>");
                myBuilder.Append("<td align='left' valign='top'>");
                myBuilder.Append("Fecha y Hora de la Marca: " + fechaMarcaFormateada);
                myBuilder.Append("</td>");
                myBuilder.Append("</tr>");

                myBuilder.Append("<tr align='left' valign='top'>");
                myBuilder.Append("<td align='left' valign='top'>");
                myBuilder.Append("Sentido de la Marca: " + sentido);
                myBuilder.Append("</td>");
                myBuilder.Append("</tr>");

                myBuilder.Append("<tr align='left' valign='top'>");
                myBuilder.Append("<td align='left' valign='top'>");
                myBuilder.Append("Reloj: " + datoReloj);
                myBuilder.Append("</td>");
                myBuilder.Append("</tr>");

                myBuilder.Append("<tr align='left' valign='top'>");
                myBuilder.Append("<td align='left' valign='top'>");
                myBuilder.Append("Checksum: " + hash);
                myBuilder.Append("</td>");
                myBuilder.Append("</tr>");
                //Se cierran los Tags.
                myBuilder.Append("</table>");
                myBuilder.Append("<p>Si encuentra inconsistencias en los datos o errores de sistema, favor avisar al administrador o contáctenos en http://www.soporte.rflex.cl.</p>");
                myBuilder.Append("<p>Saluda Atte. a Ud.</p>");
                myBuilder.Append("<p>rFlex.</p>");
                myBuilder.Append("</body>");
                myBuilder.Append("</html>");
                //Documento Generado.
                myHtmlFile = myBuilder.ToString();
                return myHtmlFile;
            }
            catch (Exception ex)
            {
                string error = "crearHtmlCorreoMarca() " + ex.ToString();
                LogSincronizador l = new LogSincronizador();
                Herramientas h = new Herramientas();
                error = h.formateadorLog(error);
                l.agregarLogError(error, "ERROR");
                return "";
            }
        }




        public static string crearHtmlCorreoNotificacionIntentoFallido(string rutFormateadoTrabajador, string nombreCompleto,
        string rechazo, string sucursal, string ubicacionReloj, string datoReloj, string hash, string fechaMarcaFormateada)
        {
            try
            {
                string myHtmlFile = "";
                //Creamos el Objeto.
                StringBuilder myBuilder = new StringBuilder();
                //Comienza la elaboración del HTML que irá en el correo.
                myBuilder.Append("<html xmlns='http://www.w3.org/1999/xhtml'>");
                myBuilder.Append("<head>");
                myBuilder.Append("<meta http-equiv=\"Content-Type\" content=\"text/html; charset=utf-8\"/>");
                myBuilder.Append("<title>");
                myBuilder.Append("Page-");
                myBuilder.Append(Guid.NewGuid().ToString());
                myBuilder.Append("</title>");
                myBuilder.Append("</head>");
                myBuilder.Append("<body>");
                myBuilder.Append("<p> Estimado(a) " + nombreCompleto + " (RUT: " + rutFormateadoTrabajador + ") enviamos detalle de intento fallido de marca reciente.</p></br>");
                myBuilder.Append("<table>");
                myBuilder.Append("<style='font-size: small;'>");
                //Agregando Cabeceras.


                myBuilder.Append("<tr align='left' valign='top'>");
                myBuilder.Append("<td align='left' valign='top'>");
                myBuilder.Append("Sucursal Marca Fallida: " + sucursal);
                myBuilder.Append("</td>");
                myBuilder.Append("</tr>");


                myBuilder.Append("<tr align='left' valign='top'>");
                myBuilder.Append("<td align='left' valign='top'>");
                myBuilder.Append("Fecha Marca Fallida: " + fechaMarcaFormateada);
                myBuilder.Append("</td>");
                myBuilder.Append("</tr>");

                myBuilder.Append("<tr align='left' valign='top'>");
                myBuilder.Append("<td align='left' valign='top'>");
                myBuilder.Append("Tipo rechazo de marca: " + rechazo);
                myBuilder.Append("</td>");
                myBuilder.Append("</tr>");

                myBuilder.Append("<tr align='left' valign='top'>");
                myBuilder.Append("<td align='left' valign='top'>");
                myBuilder.Append("Reloj: " + datoReloj);
                myBuilder.Append("</td>");
                myBuilder.Append("</tr>");

                myBuilder.Append("<tr align='left' valign='top'>");
                myBuilder.Append("<td align='left' valign='top'>");
                myBuilder.Append("Ubicación reloj: " + ubicacionReloj);
                myBuilder.Append("</td>");
                myBuilder.Append("</tr>");

                myBuilder.Append("<tr align='left' valign='top'>");
                myBuilder.Append("<td align='left' valign='top'>");
                myBuilder.Append("Checksum : " + hash);
                myBuilder.Append("</td>");
                myBuilder.Append("</tr>");
                //Se cierran los Tags.
                myBuilder.Append("</table>");
                myBuilder.Append("<p>Si encuentra inconsistencias en los datos o errores de sistema, favor avisar al administrador o contáctenos en http://www.soporte.rflex.cl.</p>");
                myBuilder.Append("<p>Saluda Atte. a Ud.</p>");
                myBuilder.Append("<p>rFlex.</p>");
                myBuilder.Append("</body>");
                myBuilder.Append("</html>");
                //Documento Generado.
                myHtmlFile = myBuilder.ToString();
                return myHtmlFile;
            }
            catch (Exception ex)
            {
                string error = "crearHtmlCorreoNotificacionIntentoFallido() " + ex.ToString();
                LogSincronizador l = new LogSincronizador();
                Herramientas h = new Herramientas();
                error = h.formateadorLog(error);
                l.agregarLogError(error, "ERROR");
                return "";
            }
        }


        #endregion

        private void generarLog(string error, string tipoLog)
        {
            LogSincronizador l = new LogSincronizador();
            Herramientas h = new Herramientas();
            error = h.formateadorLog(error);
            l.agregarLogError(error, tipoLog);
        }


        #region notificacionMarca

        public bool enviarReporteMarca(string correoTrabajador, string rutFormateadoTrabajador, string nombreCompleto, string rutEmpresaFormateada,
string razonSocial, string sentido, string sucursal, string direccion, string ubicacionReloj, string datoReloj, string hash, string fechaMarcaFormateada,
string correoNotificacion, string hostNotificacion, string puerto, string pass, string usuario, string incidencia = "")
        {
            string cuerpoHtml = "";
            try
            {
                System.Net.Mail.MailMessage _Message = new System.Net.Mail.MailMessage();
                System.Net.Mail.SmtpClient _SMTP = new System.Net.Mail.SmtpClient();
                bool respuesta = false;
                //CONFIGURACIÓN DEL STMP
                //si son correos por servidores propios (no gmail, outlook,etc) el usuario debe ser de la credencial de la cuenta
                //Si son correos tradicionales el usuario debe ser la misma cuenta de correo.
                _SMTP.Credentials = new System.Net.NetworkCredential(usuario, pass);
                _SMTP.Host = hostNotificacion;
                _SMTP.Port = int.Parse(puerto);
                _SMTP.EnableSsl = true;
                // CONFIGURACIÓN DEL MENSAJE
                _Message.To.Add(correoTrabajador); //Cuenta de Correo al que se le quiere enviar el e-mail
                _Message.From = new System.Net.Mail.MailAddress(correoNotificacion, "rFlex - Notificaciones", System.Text.Encoding.UTF8); //Quien lo envía
                _Message.Subject = "Notificación de marca";
                _Message.SubjectEncoding = System.Text.Encoding.UTF8; //Codificacion

                _Message.BodyEncoding = System.Text.Encoding.UTF8;
                _Message.Priority = System.Net.Mail.MailPriority.Normal;
                _Message.IsBodyHtml = true;

                cuerpoHtml = crearHtmlMarcaConPlantilla(rutFormateadoTrabajador, nombreCompleto, rutEmpresaFormateada, razonSocial, sentido, sucursal, direccion, ubicacionReloj, datoReloj, hash, fechaMarcaFormateada, incidencia);

                //   _Message.Body = crearHtmlCorreoMarca(correoTrabajador, rutFormateadoTrabajador, nombreCompleto, rutEmpresaFormateada,
                //  razonSocial, sentido, sucursal, direccion, ubicacionReloj, datoReloj, hash, fechaMarcaFormateada);


                string rutaLogo = System.IO.Directory.GetCurrentDirectory() + @"\2a6cea3e-6c30-4035-90d9-af10203d0014.png";
                string rutaFondo = System.IO.Directory.GetCurrentDirectory() + @"\sayagata-200px.gif";
                AlternateView htmlView = AlternateView.CreateAlternateViewFromString(cuerpoHtml, Encoding.UTF8, MediaTypeNames.Text.Html);

                LinkedResource img = new LinkedResource(rutaLogo)
                {
                    ContentId = "logoRflex"
                };
                img.ContentType.Name = "logoRflex.png";
                htmlView.LinkedResources.Add(img);
                LinkedResource backgroundLink = new LinkedResource(rutaFondo)
                {
                    ContentId = "BackgroundImage",
                    TransferEncoding = System.Net.Mime.TransferEncoding.Base64
                };
                backgroundLink.ContentType.Name = "background.gif";
                htmlView.LinkedResources.Add(backgroundLink);
                _Message.AlternateViews.Add(htmlView);

                //ENVIO
                try
                {
                    _SMTP.Send(_Message);
                    respuesta = true;
                    _SMTP.Dispose();
                }
                catch (System.Net.Mail.SmtpException ex)
                {
                    generarLog("enviarReporteMarca() Equipo: " + Environment.MachineName + " Correo: " + correoTrabajador + " " + ex.ToString(), "ERROR");
                    respuesta = false;
                    _SMTP.Dispose();
                }
                return respuesta;
            }
            catch (Exception ex)
            {
                generarLog("enviarReporteMarca() SEGUNDO CATCH " + ex.ToString(), "ERROR");
                return false;
            }
        }


        private string crearHtmlMarcaConPlantilla(string rutFormateadoTrabajador, string nombreCompleto, string rutEmpresaFormateada,
              string razonSocial, string sentido, string sucursal, string direccion, string ubicacionReloj, string datoReloj, string hash, string fechaMarcaFormateada, string incidencia)
        {
            string ruta = System.IO.Directory.GetCurrentDirectory() + @"\notificacionMarca.html";
            StringBuilder emailHtml = new StringBuilder(File.ReadAllText(ruta));
            emailHtml.Replace("{sentidoMarca}", sentido);
            emailHtml.Replace("{nombreTrabajador}", nombreCompleto + " (RUT: " + rutFormateadoTrabajador + ")");
            emailHtml.Replace("{razonSocial}", razonSocial.ToUpper());
            emailHtml.Replace("{rutEmpresa}", rutEmpresaFormateada);
            emailHtml.Replace("{sucursal}", sucursal.ToUpper());
            emailHtml.Replace("{direccion}", direccion.ToUpper());
            emailHtml.Replace("{ubicacionReloj}", ubicacionReloj.ToUpper());
            emailHtml.Replace("{datoReloj}", datoReloj.ToUpper());
            emailHtml.Replace("{checksum}", hash);
            emailHtml.Replace("{fechaMarca}", fechaMarcaFormateada);
            emailHtml.Replace("{rutFuncionario}", rutFormateadoTrabajador);
            emailHtml.Replace("{anho}", DateTime.Now.Year.ToString());
            emailHtml.Replace("{incidencia}", incidencia.ToUpper());

            if (string.IsNullOrWhiteSpace(incidencia))
            {
                emailHtml.Replace("{estiloDivIncidencia}", "display: none;");
            }
            else
            {
                emailHtml.Replace("{estiloDivIncidencia}", "background-image:url(cid:BackgroundImage);background-position:top left;background-repeat:repeat;background-color:transparent;");
            }


            return emailHtml.ToString();
        }

        #endregion


        #region Notificación Rechazos

        //Reporte de Rechazos
        public bool enviarRechazoPorCorreo(string correoTrabajador, string rutFormateadoTrabajador, string nombreCompleto,
        string rechazo, string sucursal, string ubicacionReloj, string datoReloj, string hash, string fechaMarcaFormateada,
        string correoNotificacion, string hostNotificacion, string puerto, string pass, string usuario, string razonSocial, string rutEmpresa, string direccionSucursal)
        {
            string cuerpoHtml = "";
            try
            {
                System.Net.Mail.MailMessage _Message = new System.Net.Mail.MailMessage();
                System.Net.Mail.SmtpClient _SMTP = new System.Net.Mail.SmtpClient();
                bool respuesta = false;
                //CONFIGURACIÓN DEL STMP
                //si son correos por servidores propios (no gmail, outlook,etc) el usuario debe ser de la credencial de la cuenta
                //Si son correos tradicionales el usuario debe ser la misma cuenta de correo.
                _SMTP.Credentials = new System.Net.NetworkCredential(usuario, pass);
                _SMTP.Host = hostNotificacion;
                _SMTP.Port = int.Parse(puerto);
                _SMTP.EnableSsl = true;
                // CONFIGURACIÓN DEL MENSAJE
                _Message.To.Add(correoTrabajador); //Cuenta de Correo al que se le quiere enviar el e-mail
                _Message.From = new System.Net.Mail.MailAddress(correoNotificacion, "rFlex - Notificaciones", System.Text.Encoding.UTF8); //Quien lo envía
                _Message.Subject = "Notificación de rechazo de marca";
                _Message.SubjectEncoding = System.Text.Encoding.UTF8; //Codificacion
                                                                      //     _Message.Body = crearHtmlCorreoNotificacionIntentoFallido(rutFormateadoTrabajador, nombreCompleto, rechazo, sucursal, ubicacionReloj, datoReloj, hash, fechaMarcaFormateada);
                _Message.BodyEncoding = System.Text.Encoding.UTF8;
                _Message.Priority = System.Net.Mail.MailPriority.Normal;
                _Message.IsBodyHtml = true;

                cuerpoHtml = crearHtmlRechazoPlantilla(rutFormateadoTrabajador, nombreCompleto, rechazo, sucursal, ubicacionReloj, datoReloj, hash, fechaMarcaFormateada, razonSocial, rutEmpresa, direccionSucursal);
                string rutaLogo = System.IO.Directory.GetCurrentDirectory() + @"\2a6cea3e-6c30-4035-90d9-af10203d0014.png";
                string rutaFondo = System.IO.Directory.GetCurrentDirectory() + @"\sayagata-200px.gif";
                AlternateView htmlView = AlternateView.CreateAlternateViewFromString(cuerpoHtml, Encoding.UTF8,
                             MediaTypeNames.Text.Html);

                LinkedResource img = new LinkedResource(rutaLogo)
                {
                    ContentId = "logoRflex"
                };
                img.ContentType.Name = "logoRflex.png";
                htmlView.LinkedResources.Add(img);
                LinkedResource backgroundLink = new LinkedResource(rutaFondo)
                {
                    ContentId = "BackgroundImage",
                    TransferEncoding = System.Net.Mime.TransferEncoding.Base64
                };
                backgroundLink.ContentType.Name = "background.gif";
                htmlView.LinkedResources.Add(backgroundLink);
                _Message.AlternateViews.Add(htmlView);

                //ENVIO
                try
                {
                    _SMTP.Send(_Message);
                    respuesta = true;
                    _SMTP.Dispose();
                }
                catch (System.Net.Mail.SmtpException ex)
                {
                    generarLog("enviarRechazoPorCorreo() " + ex.ToString(), "ERROR");
                    respuesta = false;
                    _SMTP.Dispose();
                }
                return respuesta;
            }
            catch (Exception ex)
            {
                generarLog("enviarRechazoPorCorreo() SEGUNDO CATCH " + ex.ToString(), "ERROR");
                return false;
            }
        }


        private string crearHtmlRechazoPlantilla(string rutFormateadoTrabajador, string nombreCompleto,
        string rechazo, string sucursal, string ubicacionReloj, string datoReloj, string hash, string fechaMarcaFormateada, string razonSocial, string rutEmpresa, string direccionSucursal)
        {
            string ruta = System.IO.Directory.GetCurrentDirectory() + @"\notificacionRechazo.html";
            StringBuilder emailHtml = new StringBuilder(File.ReadAllText(ruta));
            emailHtml.Replace("{nombreTrabajador}", nombreCompleto + " (RUT: " + rutFormateadoTrabajador + ")");
            emailHtml.Replace("{razonSocial}", razonSocial.ToUpper());
            emailHtml.Replace("{rutEmpresa}", rutEmpresa);
            emailHtml.Replace("{sucursal}", sucursal.ToUpper());
            emailHtml.Replace("{direccionSucursal}", direccionSucursal.ToUpper());
            emailHtml.Replace("{ubicacionReloj}", ubicacionReloj.ToUpper());
            emailHtml.Replace("{datoReloj}", datoReloj);
            emailHtml.Replace("{rechazo}", rechazo);
            emailHtml.Replace("{checksum}", hash);
            emailHtml.Replace("{fechaMarca}", fechaMarcaFormateada);
            emailHtml.Replace("{anho}", DateTime.Now.Year.ToString());
            return emailHtml.ToString();
        }
        #endregion


        #region Alerta Errores

        //Alerta De Errores
        public bool enviarNotificacionAlertasErrores(string correoNotificacion, string hostNotificacion, int puerto, string pass, string usuario, string alerta, string datoReloj,
        string razonSocial, string ubicacionReloj, string sucursal, string hash, string fechaAlerta, DataTable dtDestinatarios, string rutEmpresa, string direccionSucursal)
        {
            string cuerpoHtml = "";
            string correosANotificar = cuentasDeCorreoAEnviarNotificacionError(dtDestinatarios);
            if (!correosANotificar.Equals(""))
            {
                try
                {
                    System.Net.Mail.MailMessage _Message = new System.Net.Mail.MailMessage();
                    System.Net.Mail.SmtpClient _SMTP = new System.Net.Mail.SmtpClient();
                    bool respuesta = false;
                    //CONFIGURACIÓN DEL STMP
                    //si son correos por servidores propios (no gmail, outlook,etc) el usuario debe ser de la credencial de la cuenta
                    //Si son correos tradicionales el usuario debe ser la misma cuenta de correo.
                    _SMTP.Credentials = new System.Net.NetworkCredential(usuario, pass);
                    _SMTP.Host = hostNotificacion;
                    _SMTP.Port = puerto;
                    _SMTP.EnableSsl = true;
                    // CONFIGURACIÓN DEL MENSAJE
                    _Message.To.Add(correosANotificar); //Cuenta de Correo al que se le quiere enviar el e-mail
                    _Message.From = new System.Net.Mail.MailAddress(correoNotificacion, "rFlex - Notificaciones", System.Text.Encoding.UTF8); //Quien lo envía
                    _Message.Subject = "Notificación de Alerta";
                    _Message.SubjectEncoding = System.Text.Encoding.UTF8; //Codificacion
                    _Message.BodyEncoding = System.Text.Encoding.UTF8;
                    _Message.Priority = System.Net.Mail.MailPriority.Normal;
                    _Message.IsBodyHtml = true;

                    cuerpoHtml = crearHtmlAlertaError(alerta, datoReloj, razonSocial, ubicacionReloj, sucursal, hash, fechaAlerta, rutEmpresa, direccionSucursal);
                    string rutaLogo = System.IO.Directory.GetCurrentDirectory() + @"\2a6cea3e-6c30-4035-90d9-af10203d0014.png";
                    string rutaFondo = System.IO.Directory.GetCurrentDirectory() + @"\sayagata-200px.gif";
                    AlternateView htmlView = AlternateView.CreateAlternateViewFromString(cuerpoHtml, Encoding.UTF8,
                                 MediaTypeNames.Text.Html);

                    LinkedResource img = new LinkedResource(rutaLogo)
                    {
                        ContentId = "logoRflex"
                    };
                    img.ContentType.Name = "logoRflex.png";
                    htmlView.LinkedResources.Add(img);
                    LinkedResource backgroundLink = new LinkedResource(rutaFondo)
                    {
                        ContentId = "BackgroundImage",
                        TransferEncoding = System.Net.Mime.TransferEncoding.Base64
                    };
                    backgroundLink.ContentType.Name = "background.gif";
                    htmlView.LinkedResources.Add(backgroundLink);
                    _Message.AlternateViews.Add(htmlView);

                    //ENVIO
                    try
                    {
                        _SMTP.Send(_Message);
                        respuesta = true;
                    }
                    catch (System.Net.Mail.SmtpException ex)
                    {
                        generarLog("enviarNotificacionAlertasErrores() " + ex.ToString(), "ERROR");
                        respuesta = false;
                    }
                    return respuesta;
                }
                catch (Exception ex)
                {
                    generarLog("enviarNotificacionAlertasErrores() SEGUNDO CATCH " + ex.ToString(), "ERROR");
                    return false;
                }
            }
            else
            {
                generarLog("enviarNotificacionAlertasErrores() No hay correos válidos para envío de alertas de errores", "ADVERTENCIA");
                return false;
            }

        }


        private string crearHtmlAlertaError(string alerta, string datoReloj,
        string razonSocial, string ubicacionReloj, string sucursal, string hash, string fechaAlerta, string rutEmpresa, string direccionSucursal)
        {
            string ruta = System.IO.Directory.GetCurrentDirectory() + @"\notificacionAlertaError.html";
            StringBuilder emailHtml = new StringBuilder(File.ReadAllText(ruta));
            emailHtml.Replace("{alerta}", alerta);
            emailHtml.Replace("{razonSocial}", razonSocial.ToUpper());
            emailHtml.Replace("{rutEmpresa}", rutEmpresa);
            emailHtml.Replace("{sucursal}", sucursal.ToUpper());
            emailHtml.Replace("{direccionSucursal}", direccionSucursal.ToUpper());
            emailHtml.Replace("{ubicacionReloj}", ubicacionReloj.ToUpper());
            emailHtml.Replace("{datoReloj}", datoReloj.ToUpper());
            emailHtml.Replace("{checksum}", hash);
            emailHtml.Replace("{fechaAlerta}", fechaAlerta);
            emailHtml.Replace("{anho}", DateTime.Now.Year.ToString());
            return emailHtml.ToString();
        }

        private string cuentasDeCorreoAEnviarNotificacionError(DataTable dtNotificar)
        {
            string listaCorreos = "";
            string correo = "";
            Herramientas h = new Herramientas();
            Encriptacion e = new Encriptacion();
            //Datos Traidos
            /**
             *idcorreoAlerta,           0
             *nombreEncargadoCorreo,    1
             *correo,                   2
             *habilitado,               3
             *updated_at                4
             */
            try
            {
                for (int i = 0; i < dtNotificar.Rows.Count; i++)
                {
                    correo = e.Desencriptar(dtNotificar.Rows[i][2].ToString());
                    if (h.mailValido(correo))
                    {
                        listaCorreos = listaCorreos.Equals("") ? correo : listaCorreos + "," + correo;
                    }
                }
            }
            catch (Exception ex)
            {
                generarLog("cuentasDeCorreoAEnviarNotificacionError() SEGUNDO CATCH " + ex.ToString(), "ERROR");
            }
            return listaCorreos;
        }




        #endregion


        //De Prueba
        public bool enviarReportePorCorreoPruebaHtmlExtendido()
        {
            Encriptacion enc = new Encriptacion();
            ConfiguracionNotificacion cn = new ConfiguracionNotificacion();
            string cuerpoHtml = "";
            try
            {
                System.Net.Mail.MailMessage _Message = new System.Net.Mail.MailMessage();
                System.Net.Mail.SmtpClient _SMTP = new System.Net.Mail.SmtpClient();
                bool respuesta = false;
                //CONFIGURACIÓN DEL STMP
                //si son correos por servidores propios (no gmail, outlook,etc) el usuario debe ser de la credencial de la cuenta
                //Si son correos tradicionales el usuario debe ser la misma cuenta de correo.
                //   _SMTP.Credentials = new System.Net.NetworkCredential(usuario, pass);
                //  _SMTP.Host = hostNotificacion.Trim();
                //  _SMTP.Port = int.Parse(puerto);
                _SMTP.Credentials = new System.Net.NetworkCredential("alejandro@rflex.cl", enc.Desencriptar("aS2L/ZEYtuzbF0rzFfO3lg=="));
                _SMTP.Host = "smtp.gmail.com";
                _SMTP.Port = 587;
                _SMTP.EnableSsl = true;
                // CONFIGURACIÓN DEL MENSAJE
                _Message.To.Add("alejandro@rflex.cl"); //Cuenta de Correo al que se le quiere enviar el e-mail
                _Message.From = new System.Net.Mail.MailAddress("alejandro@rflex.cl", "rFlex - Notificaciones", System.Text.Encoding.UTF8); //Quien lo envía
                                                                                                                                            //  _Message.From = new System.Net.Mail.MailAddress(correoNotificacion, "rFlex - Notificaciones", System.Text.Encoding.UTF8); //Quien lo envía
                _Message.Subject = "Notificación de marca";
                _Message.SubjectEncoding = System.Text.Encoding.UTF8; //Codificacion

                _Message.BodyEncoding = System.Text.Encoding.UTF8;
                _Message.Priority = System.Net.Mail.MailPriority.Normal;
                _Message.IsBodyHtml = true;
                cuerpoHtml = crearHtmlMarcaConPlantilla("16944756k", "ALEJANDRO FUENTES", "1-9", "rflex", "Marca de prueba", "Oficina Rosal", "Calle rosal", "Oficina desarrollo", "Pc Alejandro", "12345678aaaaa", "23/05/18 16:37:01", "qwerty");
                string rutaLogo = System.IO.Directory.GetCurrentDirectory() + @"\images\2a6cea3e-6c30-4035-90d9-af10203d0014.png";
                string rutaFondo = System.IO.Directory.GetCurrentDirectory() + @"\images\sayagata-200px.gif";
                AlternateView htmlView = AlternateView.CreateAlternateViewFromString(cuerpoHtml, Encoding.UTF8,
                             MediaTypeNames.Text.Html);

                LinkedResource img = new LinkedResource(rutaLogo)
                {
                    ContentId = "logoRflex"
                };
                htmlView.LinkedResources.Add(img);


                LinkedResource backgroundLink = new LinkedResource(rutaFondo)
                {
                    ContentId = "BackgroundImage",
                    TransferEncoding = System.Net.Mime.TransferEncoding.Base64
                };
                htmlView.LinkedResources.Add(backgroundLink);



                _Message.AlternateViews.Add(htmlView);

                // _Message.Body =cuerpoHtml;
                //ENVIO
                try
                {
                    _SMTP.Send(_Message);
                    _SMTP.Dispose();
                    respuesta = true;
                }
                catch (System.Net.Mail.SmtpException ex)
                {
                    generarLog("enviarReportePorCorreo() " + ex.ToString(), "ERROR");
                    respuesta = false;
                }
                return respuesta;
            }
            catch (Exception ex)
            {
                generarLog("enviarReportePorCorreo() SEGUNDO CATCH " + ex.ToString(), "ERROR");
                return false;
            }
        }





        #region notificacionMarcaTicket

        public bool enviarReporteMarcaCasino(string correoTrabajador, string rutFormateadoTrabajador, string nombreCompleto, string rutEmpresaFormateada,
string razonSocial, string sentido, string sucursal, string direccion, string ubicacionReloj, string datoReloj, string hash, string fechaMarcaFormateada,
string correoNotificacion, string hostNotificacion, string puerto, string pass, string usuario, string codigoTicket, bool tieneETicket, string incidencia = "")
        {
            string cuerpoHtml = "";
            try
            {
                System.Net.Mail.MailMessage _Message = new System.Net.Mail.MailMessage();
                System.Net.Mail.SmtpClient _SMTP = new System.Net.Mail.SmtpClient();
                bool respuesta = false;
                //CONFIGURACIÓN DEL STMP
                //si son correos por servidores propios (no gmail, outlook,etc) el usuario debe ser de la credencial de la cuenta
                //Si son correos tradicionales el usuario debe ser la misma cuenta de correo.
                _SMTP.Credentials = new System.Net.NetworkCredential(usuario, pass);
                _SMTP.Host = hostNotificacion;
                _SMTP.Port = int.Parse(puerto);
                _SMTP.EnableSsl = true;
                // CONFIGURACIÓN DEL MENSAJE
                _Message.To.Add(correoTrabajador); //Cuenta de Correo al que se le quiere enviar el e-mail
                _Message.From = new System.Net.Mail.MailAddress(correoNotificacion, "rFlex - Notificaciones", System.Text.Encoding.UTF8); //Quien lo envía
                _Message.Subject = "Notificación de marca";
                _Message.SubjectEncoding = System.Text.Encoding.UTF8; //Codificacion

                _Message.BodyEncoding = System.Text.Encoding.UTF8;
                _Message.Priority = System.Net.Mail.MailPriority.Normal;
                _Message.IsBodyHtml = true;

                cuerpoHtml = crearHtmlMarcaConPlantillaCasino(rutFormateadoTrabajador, nombreCompleto, rutEmpresaFormateada, razonSocial, sentido, sucursal, direccion, ubicacionReloj, datoReloj, hash, fechaMarcaFormateada, incidencia, codigoTicket);

                string rutaLogo = System.IO.Directory.GetCurrentDirectory() + @"\lrf.png";
                string rutaFondo = System.IO.Directory.GetCurrentDirectory() + @"\fondo.gif";
                AlternateView htmlView = AlternateView.CreateAlternateViewFromString(cuerpoHtml, Encoding.UTF8, MediaTypeNames.Text.Html);

                LinkedResource img = new LinkedResource(rutaLogo)
                {
                    ContentId = "logoRflex"
                };
                img.ContentType.Name = "logoRflex.png";
                htmlView.LinkedResources.Add(img);

                LinkedResource backgroundLink = new LinkedResource(rutaFondo)
                {
                    ContentId = "BackgroundImage",
                    TransferEncoding = System.Net.Mime.TransferEncoding.Base64
                };

                backgroundLink.ContentType.Name = "background.gif";
                htmlView.LinkedResources.Add(backgroundLink);
                _Message.AlternateViews.Add(htmlView);

                if (tieneETicket)
                {
                    string rutaETicket = @"C:\rflexapps\eticket.pdf";
                    LinkedResource eTicket = new LinkedResource(rutaETicket)
                    {
                        ContentId = "eTicketCasino",
                        TransferEncoding = System.Net.Mime.TransferEncoding.Base64
                    };
                    eTicket.ContentType.Name = "eticket.pdf";
                    htmlView.LinkedResources.Add(eTicket);
                }

                //ENVIO
                try
                {
                    _SMTP.Send(_Message);
                    respuesta = true;
                    try
                    {
                        _SMTP.Dispose();
                        htmlView.LinkedResources.Dispose();
                        htmlView.Dispose();
                    }
                    catch (Exception)
                    {
                        respuesta = true;
                    }

                }
                catch (System.Net.Mail.SmtpException ex)
                {
                    generarLog("enviarReporteMarca() Equipo: " + Environment.MachineName + " Correo: " + correoTrabajador + " " + ex.ToString(), "ERROR");
                    respuesta = false;
                }
                return respuesta;
            }
            catch (Exception ex)
            {
                generarLog("enviarReporteMarca() SEGUNDO CATCH " + ex.ToString(), "ERROR");
                return false;
            }
        }


        private string crearHtmlMarcaConPlantillaCasino(string rutFormateadoTrabajador, string nombreCompleto, string rutEmpresaFormateada,
              string razonSocial, string sentido, string sucursal, string direccion, string ubicacionReloj, string datoReloj, string hash, string fechaMarcaFormateada,
              string incidencia, string codigoTicket)
        {
            string ruta = System.IO.Directory.GetCurrentDirectory() + @"\notificacionMarcaCasino.html";
            StringBuilder emailHtml = new StringBuilder(File.ReadAllText(ruta));
            emailHtml.Replace("{sentidoMarca}", sentido);
            emailHtml.Replace("{nombreTrabajador}", nombreCompleto + " (RUT: " + rutFormateadoTrabajador + ")");
            emailHtml.Replace("{razonSocial}", razonSocial.ToUpper());
            emailHtml.Replace("{rutEmpresa}", rutEmpresaFormateada);
            emailHtml.Replace("{sucursal}", sucursal.ToUpper());
            emailHtml.Replace("{direccion}", direccion.ToUpper());
            emailHtml.Replace("{ubicacionReloj}", ubicacionReloj.ToUpper());
            emailHtml.Replace("{datoReloj}", datoReloj.ToUpper());
            emailHtml.Replace("{checksum}", hash);
            emailHtml.Replace("{fechaMarca}", fechaMarcaFormateada);
            emailHtml.Replace("{rutFuncionario}", rutFormateadoTrabajador);
            emailHtml.Replace("{anho}", DateTime.Now.Year.ToString());
            emailHtml.Replace("{incidencia}", incidencia.ToUpper());
            emailHtml.Replace("{codigoTicket}", codigoTicket.ToUpper());

            if (string.IsNullOrWhiteSpace(incidencia))
            {
                emailHtml.Replace("{estiloDivIncidencia}", "display: none;");
            }
            else
            {
                emailHtml.Replace("{estiloDivIncidencia}", "background-image:url(cid:BackgroundImage);background-position:top left;background-repeat:repeat;background-color:transparent;");
            }


            return emailHtml.ToString();
        }





        #endregion









    }
}
