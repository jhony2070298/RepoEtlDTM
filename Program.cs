using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Services.Description;
using System.Data.SqlClient;

using System.Data;
using Nancy.Json;

namespace EtlDTM
{
    class Program
    {
       // List<DataApi> listJson = new List<DataApi>();
        static void Main(string[] args)
        {

            string lastServiceDate = "17/03/2021 2:10:47 pm";
            string dateFormatted = DateTime.Parse(lastServiceDate).ToString("O");

            //string errorMessage = "";
            //try
            //{
            //    int contador = 0;
            //    DataApi result = new DataApi();
            //    string dateFormatted = "";
            //    string ultimoRegistroArchivoPlano = "";
            //    bool flat = true;
            //    while (flat == true)
            //    {
            //        CultureInfo enUS = new CultureInfo("en-US");
            //        //string dateString;
            //        DateTime dateValue;// ponerle una sola h
            //        string lastServiceDate = "17/03/2021 2:10:47 p. m.".Replace(". ","").Replace(".","");
            //        if (DateTime.TryParseExact(lastServiceDate, "dd/MM/yyyy h:mm:ss tt", enUS,
            //                                DateTimeStyles.None, out dateValue))
            //            dateFormatted = DateTime.Parse(lastServiceDate).ToString("yyyy-MM-dd HH:mm:ss");


            //        else
            //            Console.WriteLine("'{0}' is not in an acceptable format.", lastServiceDate);

            //        DateTime fecha = DateTime.Now;
            //        if (result.latitude == 0 || result.latitude == null)
            //        {
            //           lastServiceDate = "17/03/2021 2:10:47 p. m.".Replace(". ", "");     
            //           dateFormatted = DateTime.Parse(lastServiceDate).ToString("yyyy-MM-dd HH:mm:ss");
            //            //dateFormatted = fecha.ToString("yyyy-MM-dd HH:mm:ss.fff");
            //        }
            //        else
            //        {
            //            dateFormatted = DateTime.Parse(ultimoRegistroArchivoPlano).ToString("yyyy-MM-dd HH:mm:ss.fff");
            //        }
            //        var client = new RestClient("https://api.artimo.com.co/tokens");
            //        client.Timeout = -1;
            //        var request = new RestRequest(Method.POST);
            //        request.AddHeader("Accept", "application/json");
            //        request.AddHeader("Content-Type", "application/x-www-form-urlencoded");
            //        request.AddParameter("username", "hector.jaramillo@tdm.com.co");
            //        request.AddParameter("password", "Correo2017");
            //        request.AddParameter("grant_type", "password");
            //        IRestResponse response = client.Execute(request);  
            //        TokenJson deserializedJson = JsonConvert.DeserializeObject<TokenJson>(response.Content);
            //        string token = deserializedJson.access_token;

            //        //DateTime fecha = DateTime.Now;
            //        //string dateFormatted = fecha.ToString("yyyy-MM-dd HH:mm:ss.fff");
            //        string fff = "2021-02-10 01:59:19.530";
            //        var clientGet = new RestClient("https://api.artimo.com.co/Historical/Events");           
            //        clientGet.Timeout = -1;
            //        var requestGet = new RestRequest(Method.GET);
            //        string bearer = "bearer" + " " + token;
            //        requestGet.AddHeader("Authorization", bearer);
            //        requestGet.AddHeader("Accept", "application/json");
            //        requestGet.AddHeader("Accept-Encoding", "gzip");
            //        requestGet.AddParameter("TimeStamp",fff);
            //        IRestResponse responseGet = clientGet.Execute(requestGet);

            //        List<DataApi> listJson = JsonConvert.DeserializeObject<List<DataApi>>(responseGet.Content);
            //        //string prueba = "Data Source=DESKTOP-FQTCBO1;User ID=sa;Initial Catalog=TDM;Persist Security Info=True;Auto Translate=False;";
            //        //string neo = prueba.Replace("Persist Security Info=True;Auto Translate=False;", "Password = 123456");


            //        //result = listJson.FindLast(delegate (DataApi dt) { return dt.timeStamp > fecha; });
            //        result = listJson.Last();
            //        ultimoRegistroArchivoPlano = result.timeStamp.ToString();
            //        if (responseGet.Content != null)
            //        {                    
            //            using (StreamWriter archivo = File.AppendText("C:\\productJson\\DataApiTDM.txt"))
            //            {
            //                foreach (DataApi dataApi in listJson)
            //                {
            //                    var dataComplet = dataApi.startDate + ";" + dataApi.endDate + ";" + dataApi.timeStamp + "." + dataApi.timeStamp.Millisecond +
            //                                        ";" + dataApi.duration + ";" + dataApi.@event + ";" + dataApi.latitude +
            //                                        ";" + dataApi.longitude + ";" + dataApi.driverName + ";" + dataApi.driverIdentification +
            //                                        ";" + dataApi.machineName + ";" + dataApi.value;                               
            //                    archivo.WriteLine(dataComplet);
            //                }                          
            //            }
            //            contador = contador+1;
            //        }
            //        else
            //        {
            //            errorMessage = "No se proporsionaron datos desde el Web Services";
            //            ValidationErrors validationErrors = new ValidationErrors();
            //            validationErrors.Errors(errorMessage);//Método para insertar en la base de datos el error, en la tabla logTransacciones
            //        }
            //        if (listJson.Count < 1000 || contador ==10)
            //        {
            //            flat = false;
            //        }
            //    }
            //}
            //catch (Exception ex)
            //{

            //    errorMessage = ex.ToString();
            //    ValidationErrors validationErrors = new ValidationErrors();
            //    validationErrors.Errors(errorMessage);
            //}           
            //Console.ReadKey();
        }        
    }  
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
        //public decimal latitude { get; set; }
        public double? longitude { get; set; }
        //public decimal longitude { get; set; }
   
        public object driverName { get; set; }
        //public string driverName { get; set; }
        public object driverIdentification { get; set; }
        //public string driverIdentification { get; set; }
        public string machineName { get; set; }
        public string value { get; set; }
    }
    public class ValidationErrors
    {
        public void Errors(string error)
        {
            DateTime fechaEjecucion = DateTime.Now;
            SqlConnection conexion = new SqlConnection("Data Source=.;Initial Catalog = TDM;Trusted_Connection=true;");
            conexion.Open();
            string cadena = "INSERT INTO EventosLogisticosLog(FechaEjecucion, RegistroLeidos, RegistrosCargados, Estado, Mensaje)" +
                " VALUES(@fechaEjecucion,@registrosLeidos, @registrosCargados, @estado , @errorMessage)";
            SqlCommand comando = new SqlCommand(cadena, conexion);
            comando.Parameters.AddWithValue("@fechaEjecucion", fechaEjecucion);
            comando.Parameters.AddWithValue("@registrosLeidos", 0);
            comando.Parameters.AddWithValue("@registrosCargados", 0);
            comando.Parameters.AddWithValue("@estado", "0");
            comando.Parameters.AddWithValue("@errorMessage", error);
            comando.ExecuteNonQuery();
            conexion.Close();
        }
    }
//using (StreamWriter outputFile = new StreamWriter("C:\\productJson\\DataApiTDM.txt"))
//{
//    foreach (DataApi dataApi in listJson)
//    {
//        var dataComplet = dataApi.startDate + ";" + dataApi.endDate + ";" + dataApi.timeStamp +
//                          ";" + dataApi.duration + ";" + dataApi.@event + ";" + dataApi.latitude +
//                          ";" + dataApi.latitude + ";" + dataApi.driverName + ";" + dataApi.driverIdentification +
//                          ";" + dataApi.machineName + ";" + dataApi.value;
//        outputFile.WriteLine(dataComplet);
//    }
//}



