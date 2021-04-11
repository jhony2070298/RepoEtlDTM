using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EtlDTM
{
    class CopiaTDMScriptTask
    {
        public void Main()
        {
            //try
            //{
            //    DataApi ultimoRegistroArchivoPlano = new DataApi();
            //    DateTime ultimoTimeStampArchivoPlano = new DateTime();

            //    DateTime lastServiceDate = new DateTime();
            //    //Limpiamos el DataApiTDM.txt
            //    string rutaArchivoPlano = Dts.Variables["rutaArchivoPlano"].Value.ToString(); ;
            //    File.Delete(rutaArchivoPlano);
            //    int contador = 0;
            //    bool flat = true;
            //    while (flat == true)
            //    {
            //        if (contador == 0)
            //        {  /* Si es la primera vez que consumimos el servicio o en cada ejecución de la etl
            //            Obtenemos la fecha y hora del último timeStamp enviado por el servicio, almacenado en la base de datos*/
            //            lastServiceDate = (DateTime)Dts.Variables["fechaUltimoServicio"].Value;
            //        }
            //        else
            //        {
            //            lastServiceDate = ultimoTimeStampArchivoPlano;
            //        }
            //        //Obtenemos los parámetros de entrada para consumir el servicio
            //        string accept = Dts.Variables["acceptParameterToken"].Value.ToString();
            //        string contentType = Dts.Variables["contentTypeParameterToken"].Value.ToString();
            //        string username = Dts.Variables["usernameParameterToken"].Value.ToString();
            //        string password = Dts.Variables["passwordParameterToken"].Value.ToString();
            //        string grantType = Dts.Variables["grantTypeParameterToken"].Value.ToString();

            //        // Consumimos primera API, obtenemos el token
            //        var client = new RestClient("https://api.artimo.com.co/tokens");
            //        client.Timeout = -1;
            //        var request = new RestRequest(Method.POST);
            //        request.AddHeader("Accept", accept);
            //        request.AddHeader("Content-Type", contentType);
            //        request.AddParameter("username", username);
            //        request.AddParameter("password", password);
            //        request.AddParameter("grant_type", grantType);

            //        IRestResponse response = client.Execute(request);

            //        if (response.StatusCode == HttpStatusCode.OK)
            //        {
            //            TokenJson deserializedJson = JsonConvert.DeserializeObject<TokenJson>(response.Content);
            //            //obtenemos el token
            //            string token = deserializedJson.access_token;

            //            //Llamado segunda API
            //            var clientGet = new RestClient("https://api.artimo.com.co/Historical/Events");
            //            clientGet.Timeout = -1;
            //            var requestGet = new RestRequest(Method.GET);
            //            string bearer = "bearer" + " " + token;
            //            requestGet.AddHeader("Authorization", bearer);
            //            requestGet.AddHeader("Accept", "application/json");
            //            requestGet.AddHeader("Accept-Encoding", "gzip");
            //            requestGet.AddParameter("TimeStamp", lastServiceDate.ToString("O"));
            //            IRestResponse responseGet = clientGet.Execute(requestGet);
            //            List<DataApi> listJson = JsonConvert.DeserializeObject<List<DataApi>>(responseGet.Content);
            //            ultimoRegistroArchivoPlano = listJson.Last();
            //            ultimoTimeStampArchivoPlano = ultimoRegistroArchivoPlano.timeStamp;

            //            if (responseGet.Content != "")
            //            {
            //                using (StreamWriter archivo = File.AppendText(rutaArchivoPlano))
            //                {
            //                    foreach (DataApi dataApi in listJson)
            //                    {
            //                        var dataComplet = dataApi.startDate.ToString("O") + ";" + dataApi.endDate.ToString("O") + ";" + dataApi.timeStamp.ToString("O") +
            //                                            ";" + dataApi.duration + ";" + dataApi.@event + ";" + dataApi.latitude +
            //                                            ";" + dataApi.longitude + ";" + dataApi.driverName + ";" + dataApi.driverIdentification +
            //                                            ";" + dataApi.machineName + ";" + dataApi.value;

            //                        archivo.WriteLine(dataComplet);
            //                    }
            //                }
            //                contador = contador + 1;
            //            }
            //            else
            //            {
            //                Dts.Variables["errorMessage"].Value = "No se proporsionaron datos desde el Web Services";
            //                flat = false;
            //            }
            //            if (listJson.Count < 1000 || contador == 10)
            //            {
            //                flat = false;
            //            }
            //        }
            //        else
            //        {
            //            Dts.Variables["errorMessage"].Value = "No se generó token. Usuario no Autorizado. Validar credenciales de acceso.";
            //            flat = false;
            //        }
            //    }
            //}
            //catch (Exception ex)
            //{
            //    if (ex.ToString().Length > 400)
            //    {
            //        Dts.Variables["errorMessage"].Value = ex.ToString().Replace("'", "").Replace("\"", "").Substring(0, 400);
            //    }
            //    else
            //    {
            //        Dts.Variables["errorMessage"].Value = ex.ToString().Replace("'", "").Replace("\"", "");
            //    }
            //}
            //Dts.Variables["StrSQL"].Value = "select  GETDATE() as FechaEjecucion,"
            //                                           + "0 as RegistroLeidos,"
            //                                          + "0 as RegistrosCargados,"
            //                                          + "'0' as Estado,'"
            //                                          + Dts.Variables["errorMessage"].Value.ToString() + "' as Mensaje";
        }

        class TokenJson
        {
            public string access_token { get; set; }
            public string token_type { get; set; }
            public string expires_in { get; set; }
        }

        public class DataApi
        {
            public DateTime startDate { get; set; }
            public DateTime endDate { get; set; }
            public DateTime timeStamp { get; set; }
            public int duration { get; set; }
            public int @event { get; set; }
            public double? latitude { get; set; }
            public double? longitude { get; set; }
            public object driverName { get; set; }
            public object driverIdentification { get; set; }
            public string machineName { get; set; }
            public string value { get; set; }
        }
    }
}
