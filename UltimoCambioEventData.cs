using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace EtlDTM
{
    class UltimoCambioEventData
    {
        //public void Main()
        //{
        //    try
        //    {
        //        #region Variables
        //        DataApi result = new DataApi();
        //        DataApi ultimoRegistroArchivoPlano = new DataApi();
        //        DateTime ultimoTimeStampArchivoPlano = new DateTime();
        //        DateTime timeStamp = new DateTime();
        //        string flatFileEvent = Dts.Variables["flatFilePathEventData"].Value.ToString();
        //        string urlEvent = Dts.Variables["urlEventData"].Value.ToString();
        //        int contador = 0;
        //        bool flat = true;
        //        #endregion
        //        //Limpiamos los archivos planos    
        //        File.Delete(flatFileEvent);

        //        #region parámetros de entrada para consumir el servicio
        //        //Obtenemos los parámetros de entrada para consumir el servicio REST Login
        //        string accept = Dts.Variables["acceptParameter"].Value.ToString();
        //        string contentType = Dts.Variables["contentTypeParameterToken"].Value.ToString();
        //        string username = Dts.Variables["usernameParameterToken"].Value.ToString();
        //        string password = Dts.Variables["passwordParameterToken"].Value.ToString();
        //        string grantType = Dts.Variables["grantTypeParameterToken"].Value.ToString();
        //        string accept_Encoding = Dts.Variables["accept_EncodingParameter"].Value.ToString();
        //        #endregion
        //        // Consumimos primera API, obtenemos el token    
        //        TokenJson tokenJson = new TokenJson();
        //        var response = tokenJson.GetToken(accept, contentType, username, password, grantType);
        //        if (response.StatusCode == HttpStatusCode.OK)
        //        {
        //            //obtenemos el token
        //            TokenJson deserializedJson = JsonConvert.DeserializeObject<TokenJson>(response.Content);
        //            string token = deserializedJson.access_token;
        //            while (flat == true)
        //            {
        //                if (contador == 0)
        //                {  /* Si es la primera vez que consumimos el servicio o en cada ejecución de la etl
        //                    Obtenemos la fecha y hora del último timeStamp enviado por el servicio, almacenado en la base de datos*/
        //                    timeStamp = (DateTime)Dts.Variables["fechaUltimoServicio"].Value;
        //                }
        //                else
        //                {
        //                    timeStamp = ultimoTimeStampArchivoPlano;
        //                }
        //                //Llamado segunda API 
        //                var responseGetEvent = result.apiEvent(token, urlEvent, timeStamp, accept, accept_Encoding);
        //                List<DataApi> listJson = JsonConvert.DeserializeObject<List<DataApi>>(responseGetEvent.Content);
        //                ultimoRegistroArchivoPlano = listJson.Last();
        //                ultimoTimeStampArchivoPlano = ultimoRegistroArchivoPlano.timeStamp;

        //                if (responseGetEvent.StatusCode == HttpStatusCode.OK)
        //                {
        //                    using (StreamWriter archivo = File.AppendText(flatFileEvent))
        //                    {
        //                        foreach (DataApi dataApi in listJson)
        //                        {
        //                            var dataComplet = dataApi.startDate + ";" + dataApi.endDate + ";" + dataApi.timeStamp +
        //                                                ";" + dataApi.duration + ";" + dataApi.@event + ";" + dataApi.latitude +
        //                                                ";" + dataApi.longitude + ";" + dataApi.driverName + ";" + dataApi.driverIdentification +
        //                                                ";" + dataApi.machineName + ";" + dataApi.value;
        //                            archivo.WriteLine(dataComplet);
        //                        }
        //                    }
        //                    contador = contador + 1;
        //                }
        //                else
        //                {
        //                    Dts.Variables["errorMessage"].Value = "No se proporsionaron datos desde el Web Services EventData";
        //                    flat = false;
        //                }
        //                if (listJson.Count < 1000 || contador == 2)
        //                {
        //                    flat = false;
        //                }
        //            }// fin del while               
        //        }
        //        else
        //        {
        //            Dts.Variables["errorMessage"].Value = "No se generó token. Usuario no Autorizado. Validar credenciales de acceso.";
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        if (ex.ToString().Length > 400)
        //        {
        //            Dts.Variables["errorMessage"].Value = ex.ToString().Replace("'", "").Replace("\"", "").Substring(0, 400);
        //        }
        //        else
        //        {
        //            Dts.Variables["errorMessage"].Value = ex.ToString().Replace("'", "").Replace("\"", "");
        //        }
        //    }
        //    Dts.Variables["StrSQL"].Value = "select  GETDATE() as FechaEjecucion,"
        //                                               + "0 as RegistroLeidos,"
        //                                              + "0 as RegistrosCargados,"
        //                                              + "'0' as Estado,'"
        //                                              + Dts.Variables["errorMessage"].Value.ToString() + "' as Mensaje";
        //}
        //class TokenJson
        //{
        //    public string access_token { get; set; }
        //    public string token_type { get; set; }
        //    public string expires_in { get; set; }

        //    public IRestResponse GetToken(string accept, string contentType, string username, string password, string grantType)
        //    {
        //        var client = new RestClient("https://api.artimo.com.co/tokens");
        //        client.Timeout = -1;
        //        var request = new RestRequest(Method.POST);
        //        request.AddHeader("Accept", accept);
        //        request.AddHeader("Content-Type", contentType);
        //        request.AddParameter("username", username);
        //        request.AddParameter("password", password);
        //        request.AddParameter("grant_type", grantType);
        //        IRestResponse response = client.Execute(request);
        //        return response;
        //    }
        //}

        //public class DataApi
        //{
        //    public DateTime startDate { get; set; }
        //    public DateTime endDate { get; set; }
        //    public DateTime timeStamp { get; set; }
        //    public int duration { get; set; }
        //    public int @event { get; set; }
        //    public double? latitude { get; set; }
        //    public double? longitude { get; set; }
        //    public object driverName { get; set; }
        //    public object driverIdentification { get; set; }
        //    public string machineName { get; set; }
        //    public string value { get; set; }

        //    public IRestResponse apiEvent(string token, string url, DateTime timeStamp, string accept,
        //                                  string accept_Encoding)
        //    {
        //        var clientGet = new RestClient(url);
        //        clientGet.Timeout = -1;
        //        var requestGet = new RestRequest(Method.GET);
        //        string bearer = "bearer" + " " + token;
        //        requestGet.AddHeader("Authorization", bearer);
        //        requestGet.AddHeader("Accept", accept);
        //        requestGet.AddHeader("Accept-Encoding", accept_Encoding);
        //        requestGet.AddParameter("TimeStamp", timeStamp.ToString("O"));
        //        IRestResponse responseGet = clientGet.Execute(requestGet);
        //        return responseGet;
        //    }
        //    #region ScriptResults declaration
        //    /// <summary>
        //    /// This enum provides a convenient shorthand within the scope of this class for setting the
        //    /// result of the script.
        //    /// 
        //    /// This code was generated automatically.
        //    /// </summary>
        //    enum ScriptResults
        //    {
        //        Success = Microsoft.SqlServer.Dts.Runtime.DTSExecResult.Success,
        //        Failure = Microsoft.SqlServer.Dts.Runtime.DTSExecResult.Failure
        //    };
        //    #endregion

        //}
    }
}
