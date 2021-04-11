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
using System.Net.Http;
using System.Net.Http.Headers;

namespace EtlDTM
{
    class Program
    {
        static void Main(string[] args) 
        {
            
            string errorMessage = "";
            try
            {
                var today = DateTime.Today;// 08/04/2021 0:00:00}
                DateTime starDate = new DateTime();
                DateTime endDate = new DateTime();
                DateTime firstdaycurrentmonth = new DateTime(today.Year, today.Month, 1);//01/04/2021 0:00:00 primer dia mes actual
                DateTime firstDayLastMonth = firstdaycurrentmonth.AddMonths(-1);//01/03/2021 0:00:00 primer día del mes anterior
                DateTime lastDayLastMonth = firstdaycurrentmonth.AddSeconds(-1);//{31/03/2021 23:59:59} último día del mes anterior
                DateTime day10LastMonth = firstDayLastMonth.AddDays(10).AddSeconds(-1);//{10/03/2021 23:59:59} mitad de mes anterior
                DateTime day11LastMonth = firstDayLastMonth.AddDays(10);// {11/03/2021 00:00:00} Inicio segundo corte
                DateTime day20LastMonth = firstDayLastMonth.AddDays(20).AddSeconds(-1);//{20/03/2021 23:59:59} mitad de mes anterior
                DateTime day21LastMonth = firstDayLastMonth.AddDays(20);//{21/03/2021 00:00:00}
                #region Variables
                string flatFileMachine = "C:\\TDM_DatosApi\\DataApiTDMMachine.txt";
                string flatFileDriver = "C:\\TDM_DatosApi\\DataApiTDMDriver.txt";
                string flatFileEvent = "C:\\TDM_DatosApi\\DataApiTDMEventData.txt";
                string urlEvent = "https://api.artimo.com.co/Historical/Events";
                string urlDriver = "https://api.artimo.com.co/historical/driverscore";
                string urlMachine = "https://api.artimo.com.co/historical/MachineScore";
                string lastServiceDate = "17/03/2021 2:10:47 pm";
                string timeStamp = DateTime.Parse(lastServiceDate).ToString("O");
                int contador = 0;
                string ultimoRegistroArchivoPlano = "";
                bool flat = true;
                string bearer = string.Empty;
                DataApi result = new DataApi();
                #endregion
                File.Delete(flatFileEvent);
                File.Delete(flatFileDriver);
                File.Delete(flatFileMachine);

                //Obtenemos el token
                TokenJson tokenJson = new TokenJson();
                string token = tokenJson.GetToken();
                //consumimos primera Api  
                while (flat == true)
                {
                    if (result.latitude == 0 || result.latitude == null)
                    {
                        lastServiceDate = "17/03/2021 2:10:47 p. m.".Replace(". ", "");
                        timeStamp = DateTime.Parse(lastServiceDate).ToString("yyyy-MM-dd HH:mm:ss");
                    }
                    else
                    {
                        timeStamp = DateTime.Parse(ultimoRegistroArchivoPlano).ToString("yyyy-MM-dd HH:mm:ss.fff");
                    }
                    var responseGetEvent = result.apiEvent(token, urlEvent, timeStamp, firstDayLastMonth, lastDayLastMonth);
                    List<DataApi> listJson = JsonConvert.DeserializeObject<List<DataApi>>(responseGetEvent.Content);
                    result = listJson.Last();
                    ultimoRegistroArchivoPlano = result.timeStamp.ToString();
                    if (responseGetEvent.Content != null)
                    {
                        result.FlatFile(flatFileEvent, null, listJson);
                        contador = contador + 1;
                    }
                    if (listJson.Count < 1000 || contador == 2)
                    {
                        flat = false;
                    }
                }// fin del while
                // se consume segunda Api
                starDate = firstDayLastMonth;
                endDate = lastDayLastMonth;
                var responseGetDriver = result.apiEvent(token,urlDriver,null,starDate,endDate);  
                if (responseGetDriver.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    List<Driver_Machine> listJsonDriver = JsonConvert.DeserializeObject<List<Driver_Machine>>(responseGetDriver.Content);
                    result.FlatFile(flatFileDriver, listJsonDriver, null);      
                }
                else if (responseGetDriver.StatusCode == System.Net.HttpStatusCode.BadRequest)
                {
                    errorMessage = "Error en la solicitud del servicio 'Conductor'";
                    ValidationErrors validationErrors = new ValidationErrors();
                    validationErrors.Errors(errorMessage);//Método para insertar en la base de datos el error, en la tabla logTransacciones
                }
                else
                {
                    errorMessage = "No se proporsionaron datos desde el Web Services";
                    ValidationErrors validationErrors = new ValidationErrors();
                    validationErrors.Errors(errorMessage);//Método para insertar en la base de datos el error, en la tabla logTransacciones
                }
                // se consume tercera Api
                contador = 0;
                flat = true;
                while (flat == true)
                {
                    if (contador == 0)
                    {
                         starDate = firstDayLastMonth;
                         endDate = day10LastMonth;
                    }
                    else if (contador == 1)
                    {
                        starDate = day11LastMonth;
                        endDate = day20LastMonth;
                    }
                    else
                    {
                        starDate = day21LastMonth;
                        endDate = lastDayLastMonth;
                    }
                    var responseGetMachine = result.apiEvent(token, urlMachine,null, starDate, endDate);
                    if (responseGetMachine.StatusCode == System.Net.HttpStatusCode.OK)
                    {
                        List<Driver_Machine> listJsonMachine = JsonConvert.DeserializeObject<List<Driver_Machine>>(responseGetMachine.Content);
                        result.FlatFile(flatFileMachine, listJsonMachine, null);
                        contador = contador + 1;
                    }
                    else if (responseGetDriver.StatusCode == System.Net.HttpStatusCode.BadRequest)
                    {
                        errorMessage = "Error en la solicitud del servicio 'Máquina'";
                        ValidationErrors validationErrors = new ValidationErrors();
                        validationErrors.Errors(errorMessage);//Método para insertar en la base de datos el error, en la tabla logTransacciones
                    }
                    if (contador == 3)
                        flat = false;
                } // fin del while
            } 
            catch (Exception ex)
            {
                errorMessage = ex.ToString();
                ValidationErrors validationErrors = new ValidationErrors();
                validationErrors.Errors(errorMessage);
            }
           
        } 
    }  
}
   
    class TokenJson
    {
        public string access_token { get; set; }
        public string token_type { get; set; }
        public string expires_in { get; set; }
        public string GetToken()
        {
            var client = new RestClient("https://api.artimo.com.co/tokens");
            client.Timeout = -1;
            var request = new RestRequest(Method.POST);
            request.AddHeader("Accept", "application/json");
            request.AddHeader("Content-Type", "application/x-www-form-urlencoded");
            request.AddParameter("username", "integraciones@tdm.com.co");
            request.AddParameter("password", "@Lc@tr@Z32");
            request.AddParameter("grant_type", "password");
            IRestResponse response = client.Execute(request);
            TokenJson deserializedJson = JsonConvert.DeserializeObject<TokenJson>(response.Content);
            string token = deserializedJson.access_token;
            return token;
        }
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

        public IRestResponse apiEvent(string token,string url,string timeStamp,DateTime starDate, DateTime endDate)
        {
            IRestResponse responseGet = null;

            var clientGet = new RestClient(url);
            clientGet.Timeout = -1;
            var requestGet = new RestRequest(Method.GET);
            string bearer = "bearer" + " " + token;
            requestGet.AddHeader("Authorization", bearer);
            requestGet.AddHeader("Accept", "application/json");
            requestGet.AddHeader("Accept-Encoding", "gzip");
            if (url == "https://api.artimo.com.co/Historical/Events")
            {
                requestGet.AddParameter("TimeStamp", timeStamp);
            }         
            else
            {
                requestGet.AddParameter("startDate", starDate.ToString("s").Replace("T", " "));
                //requestGet.AddParameter("endDate", endDate.ToString("s").Replace("T", " "));  
                requestGet.AddParameter("endDate", endDate.ToString("s"));  
            }
            responseGet = clientGet.Execute(requestGet);
            return responseGet;
        }
        public void FlatFile(string flatFilePath, List<Driver_Machine> listJsonDriverMachine,List<DataApi> listJson)
        {
            using (StreamWriter archivo = File.AppendText(flatFilePath))
            {
                if (flatFilePath == "C:\\TDM_DatosApi\\DataApiTDMMachine.txt")
                {
                    foreach (Driver_Machine dataApi in listJsonDriverMachine)
                    {
                        var dataComplet = dataApi.RankingDate               + ";" + dataApi.MachineName                  + ";" + dataApi.MachineGroup              + ";" + dataApi.OverSpeedScore +
                                    ";" + dataApi.OverSpeedCount            + ";" + dataApi.OverSpeedTotalSeconds        + ";" + dataApi.MaxSpeed                  + ";" + dataApi.SpeedTime +
                                    ";" + dataApi.OverRpmScore              + ";" + dataApi.OverSpeedCount               + ";" + dataApi.OverRpmTotalSeconds       + ";" + dataApi.MaxRpm           + ";" + dataApi.RpmTime +
                                    ";" + dataApi.ExessiveIdleScore         + ";" + dataApi.ExcessiveIdleCount           + ";" + dataApi.ExcessiveIdleTotalSeconds + ";" + dataApi.GreenBandScore +
                                    ";" + dataApi.GreenBandCount            + ";" + dataApi.GreenBandTotalSeconds        + ";" + dataApi.GreenBandTime             + ";" + dataApi.OverAccelerationScore +
                                    ";" + dataApi.OverAccelerationCount     + ";" + dataApi.OverAccelerationTotalSeconds + ";" + dataApi.MaxAcceleration           + ";" + dataApi.HarshBrakeScore +
                                    ";" + dataApi.HarshBrakeCount           + ";" + dataApi.GreenBandTotalSeconds        + ";" + dataApi.MaxBrake                  + ";" + dataApi.LoginScore +
                                    ";" + dataApi.LoginCount                + ";" + dataApi.DelayedStartScore            + ";" + dataApi.DelayedStartCount         + ";" + dataApi.DelayedStartTime +
                                    ";" + dataApi.DelayedRouteDrivingScore  + ";" + dataApi.DelayedRouteDrivingCount     + ";" + dataApi.DelayedRouteDrivingTime   + ";" + dataApi.Distance +
                                    ";" + dataApi.Odometer                  + ";" + dataApi.Liters                       + ";" + dataApi.DrivingTime               + ";" + dataApi.EngineTime +
                                    ";" + dataApi.EngineHours               + ";" + dataApi.ExidleTime                   + ";" + dataApi.IdleEnterCount           + ";" + dataApi.NidleTime +
                                    ";" + dataApi.StandingTime              + ";" + dataApi.StartLatitude                + ";" + dataApi.StartLongitude           + ";" + dataApi.EndLatitude +
                                    ";" + dataApi.EndLongitude              + ";" + dataApi.TotalScore                   + ";" + dataApi.TotalTime;
                        archivo.WriteLine(dataComplet);
                    }
                }
                else if (flatFilePath == "C:\\TDM_DatosApi\\DataApiTDMDriver.txt")
                {
                    foreach (Driver_Machine dataApi in listJsonDriverMachine)
                    {
                        var dataComplet = dataApi.RankingDate                        + ";" + dataApi.DriverIdentification    + ";" + dataApi.DriverName +
                                            ";" + dataApi.DriverGroup                + ";" + dataApi.OverSpeedScore          + ";" + dataApi.OverSpeedCount +   ";" + dataApi.MaxSpeed +
                                            ";" + dataApi.OverRpmScore               + ";" + dataApi.OverRpmCount            + ";" + dataApi.MaxRpm +
                                            ";" + dataApi.ExessiveIdleScore          + ";" + dataApi.ExcessiveIdleCount      + ";" + dataApi.ExcessiveIdleTotalSeconds +
                                            ";" + dataApi.GreenBandScore             + ";" + dataApi.GreenBandCount          + ";" + dataApi.GreenBandTotalSeconds +
                                            ";" + dataApi.OverAccelerationScore      + ";" + dataApi.OverAccelerationCount   + ";" + dataApi.MaxAcceleration +
                                            ";" + dataApi.HarshBrakeScore            + ";" + dataApi.HarshBrakeCount         + ";" + dataApi.MaxBrake +
                                            ";" + dataApi.LoginScore                 + ";" + dataApi.LoginCount              + ";" + dataApi.DelayedStartScore +
                                            ";" + dataApi.DelayedStartCount          + ";" + dataApi.DelayedStartTime        + ";" + dataApi.DelayedRouteDrivingScore +
                                            ";" + dataApi.DelayedRouteDrivingCount   + ";" + dataApi.DelayedRouteDrivingTime + ";" + dataApi.TotalScore;
                        archivo.WriteLine(dataComplet);
                    }
                }
                else
                {        
                    foreach (DataApi dataApi in listJson)
                    {
                        var dataComplet =         dataApi.startDate   + ";" + dataApi.endDate    + ";" + dataApi.timeStamp +
                                            ";" + dataApi.duration    + ";" + dataApi.@event     + ";" + dataApi.latitude +
                                            ";" + dataApi.longitude   + ";" + dataApi.driverName + ";" + dataApi.driverIdentification +
                                            ";" + dataApi.machineName + ";" + dataApi.value;
                        archivo.WriteLine(dataComplet);
                    }
                }
               
            }
        }
    }
    
public class Driver_Machine
{
    public DateTime RankingDate { get; set; }

    public string DriverIdentification { get; set; }
    public string DriverName { get; set; }
    public string DriverGroup { get; set; }
    public double OverSpeedScore { get; set; }
    public int OverSpeedCount { get; set; }
    public double MaxSpeed { get; set; }
    public double OverRpmScore { get; set; }
    public int OverRpmCount { get; set; }
    public double MaxRpm { get; set; }
    public double ExessiveIdleScore { get; set; }
    public int ExcessiveIdleCount { get; set; }
    public double ExcessiveIdleTotalSeconds { get; set; }
    public double GreenBandScore { get; set; }
    public int GreenBandCount { get; set; }
    public double GreenBandTotalSeconds { get; set; }
    public double OverAccelerationScore { get; set; }
    public int OverAccelerationCount { get; set; }
    public double MaxAcceleration { get; set; }
    public double HarshBrakeScore { get; set; }


    public int HarshBrakeCount { get; set; }
    public double MaxBrake { get; set; }
    public double LoginScore { get; set; }
    public double LoginCount { get; set; }
    public double DelayedStartScore { get; set; }
    public int DelayedStartCount { get; set; }
    public double DelayedStartTime { get; set; }
    public double DelayedRouteDrivingScore { get; set; }
    public int DelayedRouteDrivingCount { get; set; }
    public double DelayedRouteDrivingTime { get; set; }

    public double TotalScore { get; set; }

    /*****************/
    public string MachineName { get; set; }
    public string MachineGroup { get; set; }
    public double? OverSpeedTotalSeconds { get; set; }
    public double? SpeedTime { get; set; }
    public double? OverRpmTotalSeconds { get; set; }
    public double? RpmTime { get; set; }
    public double? GreenBandTime { get; set; }
    public double? OverAccelerationTotalSeconds { get; set; }
    public double? HarshBrakeTotalSeconds { get; set; }


    public double? Distance { get; set; }
    public double? Odometer { get; set; }
    public double? Liters { get; set; }
    public double? DrivingTime { get; set; }
    public double? EngineTime { get; set; }
    public double? EngineHours { get; set; }
    public double? ExidleTime { get; set; }
    public int? IdleEnterCount { get; set; }
    public double? NidleTime { get; set; }
    public double? StandingTime { get; set; }


    public double? StartLatitude { get; set; }
    public double? StartLongitude { get; set; }
    public double? EndLatitude { get; set; }
    public double? EndLongitude { get; set; }
    public double? TotalTime { get; set; }
}

public class Machine
{
    //public DateTime RankingDate { get; set; }
    //public string MachineName { get; set; }
    //public string MachineGroup { get; set; }
    //public double? OverSpeedScore { get; set; }
    //public int? OverSpeedCount { get; set; }
    //public double? OverSpeedTotalSeconds { get; set; }
    //public double? MaxSpeed { get; set; }
    //public double? SpeedTime { get; set; }
    //public double? OverRpmScore { get; set; }
    //public int? OverRpmCount { get; set; }


    //public double? OverRpmTotalSeconds { get; set; }
    //public double? MaxRpm { get; set; }
    //public double? RpmTime { get; set; }
    //public double? ExessiveIdleScore { get; set; }
    //public int? ExcessiveIdleCount { get; set; }
    //public double? ExcessiveIdleTotalSeconds { get; set; }
    //public double? GreenBandScore { get; set; }
    //public int? GreenBandCount { get; set; }
    //public double? GreenBandTotalSeconds { get; set; }
    //public double? GreenBandTime { get; set; }


    //public double? OverAccelerationScore { get; set; }
    //public int? OverAccelerationCount { get; set; }
    //public double? OverAccelerationTotalSeconds { get; set; }
    //public double? MaxAcceleration { get; set; }
    //public double? HarshBrakeScore { get; set; }
    //public int? HarshBrakeCount { get; set; }
    //public double? HarshBrakeTotalSeconds { get; set; }
    //public double? MaxBrake { get; set; }
    //public double? LoginScore { get; set; }
    //public int? LoginCount { get; set; }


    //public double? DelayedStartScore { get; set; }
    //public int? DelayedStartCount { get; set; }
    //public double? DelayedStartTime { get; set; }
    //public double? DelayedRouteDrivingScore { get; set; }
    //public int? DelayedRouteDrivingCount { get; set; }
    //public double? DelayedRouteDrivingTime { get; set; }
    //public double? Distance { get; set; }
    //public double? Odometer { get; set; }
    //public double? Liters { get; set; }
    //public double? DrivingTime { get; set; }


    public double? EngineTime { get; set; }
    public double? EngineHours { get; set; }
    public double? ExidleTime { get; set; }
    public int? IdleEnterCount { get; set; }
    public double? NidleTime { get; set; }
    public double? StandingTime { get; set; }
    public double? StartLatitude { get; set; }
    public double? StartLongitude { get; set; }
    public double? EndLatitude { get; set; }
    public double? EndLongitude { get; set; }
    public double? TotalScore { get; set; }
    public double? TotalTime { get; set; }

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



