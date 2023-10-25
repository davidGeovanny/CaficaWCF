using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;
using System.Web.Script.Services;
using WcfCafica.Contexts.Empresa;

namespace WcfCafica.ServiciosERP.Inventarios
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the interface name "IWSProductoTerminado" in both code and config file together.
    [ServiceContract]
    public interface IWSComponentes : IWSBaseLike<Componentes>
    {
        [OperationContract]
        [WebInvoke(UriTemplate = "getAllOfType",
       BodyStyle = WebMessageBodyStyle.WrappedRequest,
       ResponseFormat = WebMessageFormat.Json,
       RequestFormat = WebMessageFormat.Json,
       Method = "POST")]
        List<Componentes> getAllOfType(int type);

        [OperationContract]
        [WebInvoke(UriTemplate = "getComponentesFormula",
        BodyStyle = WebMessageBodyStyle.WrappedRequest,
        ResponseFormat = WebMessageFormat.Json,
        RequestFormat = WebMessageFormat.Json,
        Method = "POST")]
        List<Componentes> getComponentesFormula(Componentes componente);
        

        [OperationContract]
        [WebInvoke(UriTemplate = "getComponentesAlmacen",
        BodyStyle = WebMessageBodyStyle.WrappedRequest,
        ResponseFormat = WebMessageFormat.Json,
        RequestFormat = WebMessageFormat.Json,
        Method = "POST")]
        List<Componentes> getComponentesAlmacen(long id);

        [OperationContract]
        [WebInvoke(UriTemplate = "getComponentesAlmacenConImagen",
        BodyStyle = WebMessageBodyStyle.WrappedRequest,
        ResponseFormat = WebMessageFormat.Json,
        RequestFormat = WebMessageFormat.Json,
        Method = "POST")]
        List<Componentes> getComponentesAlmacenConImagen(long id);

        [OperationContract]
        [WebInvoke(UriTemplate = "getComponentesAlmacenConImagenImpuestos",
        BodyStyle = WebMessageBodyStyle.WrappedRequest,
        ResponseFormat = WebMessageFormat.Json,
        RequestFormat = WebMessageFormat.Json,
        Method = "POST")]
        List<Componentes> getComponentesAlmacenConImagenImpuestos(long id);

        [OperationContract]
        [WebInvoke(UriTemplate = "getHistorialFormula",
         BodyStyle = WebMessageBodyStyle.WrappedRequest,
         ResponseFormat = WebMessageFormat.Json,
         RequestFormat = WebMessageFormat.Json,
         Method = "POST")]
        List<ComponentesFormula> getHistorialFormula(long id);

        [OperationContract]
        [WebInvoke(UriTemplate = "getHistorialFormulaDetalles",
         BodyStyle = WebMessageBodyStyle.WrappedRequest,
         ResponseFormat = WebMessageFormat.Json,
         RequestFormat = WebMessageFormat.Json,
         Method = "POST")]
        List<ComponentesFormulaDetalles> getHistorialFormulaDetalles(long id);

        [OperationContract]
        [WebInvoke(UriTemplate = "getNumerosSeries",
        BodyStyle = WebMessageBodyStyle.WrappedRequest,
        ResponseFormat = WebMessageFormat.Json,
        RequestFormat = WebMessageFormat.Json,
        Method = "POST")]
        List<LotesSeries> getNumerosSeries(long id);

        [OperationContract]
        [WebInvoke(UriTemplate = "getLotes",
        BodyStyle = WebMessageBodyStyle.WrappedRequest,
        ResponseFormat = WebMessageFormat.Json,
        RequestFormat = WebMessageFormat.Json,
        Method = "POST")]
        List<LotesSeries> getLotes(long id);

        [OperationContract]
        [WebInvoke(UriTemplate = "getComponentesAlmacenConExistencia",
        BodyStyle = WebMessageBodyStyle.WrappedRequest,
        ResponseFormat = WebMessageFormat.Json,
        RequestFormat = WebMessageFormat.Json,
        Method = "POST")]
        List<Componentes> getComponentesAlmacenConExistencia(long id);

        [OperationContract]
        [WebInvoke(UriTemplate = "getComponentesAlmacenConExistenciaeImagen",
        BodyStyle = WebMessageBodyStyle.WrappedRequest,
        ResponseFormat = WebMessageFormat.Json,
        RequestFormat = WebMessageFormat.Json,
        Method = "POST")]
        List<Componentes> getComponentesAlmacenConExistenciaeImagen(long id);

        [OperationContract]
        [WebInvoke(UriTemplate = "getComponentesResguardo",
       BodyStyle = WebMessageBodyStyle.WrappedRequest,
       ResponseFormat = WebMessageFormat.Json,
       RequestFormat = WebMessageFormat.Json,
       Method = "POST")]
        List<Componentes> getComponentesResguardo();

        [OperationContract]
        [WebInvoke(UriTemplate = "getComponenteCodigodeBarra",
        BodyStyle = WebMessageBodyStyle.WrappedRequest,
        ResponseFormat = WebMessageFormat.Json,
        RequestFormat = WebMessageFormat.Json,
        Method = "POST")]
        ComponentesCodigosBarras getComponenteCodigodeBarra(string codigo);

        [OperationContract]
        [WebInvoke(UriTemplate = "getComponentesXTipoConImagen",
       BodyStyle = WebMessageBodyStyle.WrappedRequest,
       ResponseFormat = WebMessageFormat.Json,
       RequestFormat = WebMessageFormat.Json,
       Method = "POST")]
        List<Componentes> getComponentesXTipoConImagen(long id);

    }
}
