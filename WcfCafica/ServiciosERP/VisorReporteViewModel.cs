using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using WcfCafica.Contexts;
using WcfCafica.Contexts.Reportes;

namespace WcfCafica.ServiciosERP
{
    public class VisorReporteViewModel
    {
        //  public dynamic Data { get; set; }
        //private ObservableCollection<Property> _Properties;

        enum Days { Sun = 1, Mon = 2, Tue = 3, Wed = 4, Thu = 5, Fri = 6, Sat = 10 };
        private string requestUrl = System.Diagnostics.Debugger.IsAttached == true ? "http://10.10.1.72:8082/jasperserver/" : "http://10.10.1.170:8080/jasperserver/";
        private string token;
        private string _UrlReporte;
        private List<reportParameter> _LstParametros;

        private string _Formato;



        public string UrlReporte
        {
            get
            {
                return _UrlReporte;
            }

            set
            {
                _UrlReporte = value;
            }
        }

        public List<reportParameter> LstParametros
        {
            get
            {
                return _LstParametros;
            }

            set
            {
                _LstParametros = value;
            }
        }

        public string Formato
        {
            get
            {
                return _Formato;
            }

            set
            {
                _Formato = value;
            }
        }

        public VisorReporteViewModel(string urlreporte, List<reportParameter> parametros, string formato)
        {
            LstParametros = new List<reportParameter>();
            UrlReporte = urlreporte;
            LstParametros = parametros;
            Formato = formato;
            //GenerarReporte();
        }
        public VisorReporteViewModel(string urlreporte, reportParameter parametro, string formato)
        {
            LstParametros = new List<reportParameter>();
            UrlReporte = urlreporte;
            LstParametros.Add(parametro);
            Formato = formato;
            //GenerarReporte();
        }
        public VisorReporteViewModel(string urlreporte, string formato)
        {
            LstParametros = new List<reportParameter>();
            UrlReporte = urlreporte;
            Formato = formato;
            //GenerarReporte();
        }


        public string GenerarReporte()
        {
            try
            {
                ValidarToken Token = new ValidarToken();


                string BdEmpresa = Token.getKey("bd", "TokenEmpresa");
                string PasswordReportes = Token.getKey("password_reportes", "TokenEmpresa");
                string RazoSocialEmpresa= Token.getKey("razonsocial", "TokenEmpresa");
                string UsuarioFirmado = Token.getKey("user","Token");

                //Paso 1
                //Adquire un idetificado con jaspersoft
                token = null;
                token = (string)Peticion("POST", "j_username=" + BdEmpresa + "&j_password=" + PasswordReportes, "application/x-www-form-urlencoded", "", typeof(string), "rest/login");
                var Extension = "pdf";

                //Genera caracterizticas del reporte
                ReporteModel rpt = new ReporteModel();
                rpt.reportUnitUri = UrlReporte;
                rpt.async = false;
                rpt.freshData = false;
                rpt.saveDataSnapshot = false;
                rpt.outputFormat = Extension;
                rpt.interactive = true;
                rpt.ignorePagination = false;

                //    reportParameter parametro = new reportParameter();

                //Agrega el parametro usuario y empresa
                reportParameter ParametroEmpresa = new reportParameter();
                ParametroEmpresa.name = "ParametroEmpresa";
                ParametroEmpresa.value.Add(RazoSocialEmpresa);
                LstParametros.Add(ParametroEmpresa);

                reportParameter ParametroUsuario = new reportParameter();
                ParametroUsuario.name = "ParametroUsuario";
                ParametroUsuario.value.Add(UsuarioFirmado);
                LstParametros.Add(ParametroUsuario);

                //Agrega los parametros al reporte
                reportParameters parametros = new reportParameters();
                parametros.reportParameter = LstParametros;

                rpt.parameters = parametros;

                //Paso 2
                //Mandas la orden para que genere el reporte y retorne el identificador del mismo
                ResponseReportModel RespuestaReporte = new ResponseReportModel();
                JsonSerializerSettings microsoftDateFormatSettings = new JsonSerializerSettings { DateFormatHandling = DateFormatHandling.MicrosoftDateFormat };
                RespuestaReporte = (ResponseReportModel)Peticion("POST", JsonConvert.SerializeObject(rpt, microsoftDateFormatSettings), "application/json", "application/json", typeof(ResponseReportModel), "rest_v2/reportExecutions");

                if (RespuestaReporte.exports[0].status != "ready")
                    throw new Exception(RespuestaReporte.exports[0].status);

                //Paso 3
                //Una vez con el id generado mandas a descargar el reporte
                //Nota se utiliza la clase WebCliente por que la clase HttpWebRequest solo es compatible con texto
                string url = requestUrl + "rest_v2/reportExecutions/" + RespuestaReporte.requestId + "/exports/" + RespuestaReporte.exports[0].id + "/outputResource";

                //string rutaTemporal = Path.GetDirectoryName(Path.GetTempPath()) + "\\Erp-" + RespuestaReporte.exports[0].id + "." + Extension;
                string rutaTemporal = System.Web.HttpContext.Current.Server.MapPath("/"+ RespuestaReporte.exports[0].id + "." + Extension);
        

                WebClient Cliente = new WebClient();
                Cliente.Headers.Add("Cookie", token);
                Cliente.DownloadFile(url, rutaTemporal);

                return RespuestaReporte.exports[0].id + "." + Extension;

            }
            catch (Exception)
            {
                throw;
            }
        }


        public object Peticion(string Method, string Request, string ContentType, string Accept, Type ResponseType, string URL)
        {

            object result;
            try
            {

                HttpWebRequest httpWebRequest = WebRequest.Create(requestUrl + URL) as HttpWebRequest;
                httpWebRequest.Method = Method;
                httpWebRequest.ContentType = ContentType;
                httpWebRequest.Accept = Accept;

                if (!String.IsNullOrEmpty(token))
                    httpWebRequest.Headers.Add("Cookie", token);

                if (Method == "POST")
                {
                    byte[] bytes = Encoding.UTF8.GetBytes(Request);
                    Stream requestStream = httpWebRequest.GetRequestStream();
                    requestStream.Write(bytes, 0, bytes.Length);
                    requestStream.Close();
                }
                using (HttpWebResponse httpWebResponse = httpWebRequest.GetResponse() as HttpWebResponse)
                {
                    if (httpWebResponse.StatusCode != HttpStatusCode.OK)
                    {
                        throw new Exception(string.Format("Server error (HTTP {0}: {1}).", httpWebResponse.StatusCode, httpWebResponse.StatusDescription));
                    }

                    Stream responseStream = httpWebResponse.GetResponseStream();
                    StreamReader streamReader = new StreamReader(responseStream);

                    string text2 = streamReader.ReadToEnd();

                    object obj;
                    if (String.IsNullOrEmpty(token))
                        obj = httpWebResponse.Headers["Set-Cookie"];
                    else
                        obj = JsonConvert.DeserializeObject(text2, ResponseType);

                    result = obj;
                }
            }
            catch (Exception)
            {
                throw;
            }
            return result;
        }
    }
}