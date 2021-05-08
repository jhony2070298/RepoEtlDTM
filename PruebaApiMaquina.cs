using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace EtlDTM
{
    class PruebaApiMaquina
    {
        public void Main()
        {
            try
            {
                #region Variables
                string flatFileMachine = "C:\\TDM_Pruebas\\TDM_Prueba1.txt";
                //string flatFileDriver = Dts.Variables["flatFilePathDriver"].Value.ToString();
                //string urlDriver = Dts.Variables["urlDriver"].Value.ToString();
                string urlMachine = "https://api.artimo.com.co/historical/MachineScore";
                string message = "Error en la solicitud del servicio.Verifique que los parámetros de entrada estén en el formato correcto.";
                int contador = 0;
                bool flat = true;
                #endregion
                //Limpiamos los archivos planos            
                //File.Delete(flatFileDriver);
                File.Delete(flatFileMachine);
                //Obtenemos los parámetros de entrada para consumir el servicio REST Login
                #region parámetros de entrada para consumir el servicio        
                string urlToken = "https://api.artimo.com.co/tokens";
                string accept = "application/json";
                string contentType = "application/x-www-form-urlencoded";
                string username = "integraciones@tdm.com.co";
                string password = "@Lc@tr@Z32";
                string grantType = "password";
                string accept_Encoding = "gzip";
                //Obtenemos los parámetros de entrada para consumir los servicios REST Operación Conductor y Máquina
                DateTime starDate = new DateTime();
                DateTime endDate = new DateTime();
                DateTime today = DateTime.Today;// 08/04/2021 0:00:00}
                DateTime firstdaycurrentmonth = new DateTime(today.Year, today.Month, 1);//01/04/2021 0:00:00 primer dia mes actual
                DateTime firstDayLastMonth = firstdaycurrentmonth.AddMonths(-1);//01/03/2021 0:00:00 primer día del mes anterior
                DateTime lastDayLastMonth = firstdaycurrentmonth.AddSeconds(-1);//{31/03/2021 23:59:59} último día del mes anterior
                DateTime day10LastMonth = firstDayLastMonth.AddDays(10).AddSeconds(-1);//{10/03/2021 23:59:59} mitad de mes anterior
                DateTime day11LastMonth = firstDayLastMonth.AddDays(10);// {11/03/2021 00:00:00} Inicio segundo corte
                DateTime day20LastMonth = firstDayLastMonth.AddDays(20).AddSeconds(-1);//{20/03/2021 23:59:59} mitad de mes anterior
                DateTime day21LastMonth = firstDayLastMonth.AddDays(20);//{21/03/2021 00:00:00}
                #endregion
                //Api para el token
                Token tokenJson = new Token();
                var response = tokenJson.GetToken(urlToken, accept, contentType, username, password, grantType);
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    //obtenemos el token
                    Token deserializedJson = JsonConvert.DeserializeObject<Token>(response.Content);
                    string token = deserializedJson.access_token;
                    RankingOperacion driver_Machine = new RankingOperacion();
                   
                    // se consume la tercera Api(Ranking de operación de máquina)
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
                        var responseGetMachine = driver_Machine.ConsumeApiRest(token, urlMachine, accept, accept_Encoding, starDate, endDate);
                        if (responseGetMachine.StatusCode == HttpStatusCode.OK)
                        {
                            List<RankingOperacion> listJsonMachine = JsonConvert.DeserializeObject<List<RankingOperacion>>(responseGetMachine.Content);
                            driver_Machine.LoadFlatFile(flatFileMachine, listJsonMachine);
                            contador = contador + 1;
                        }
                        else if (responseGetMachine.StatusCode == HttpStatusCode.BadRequest)
                        {
                            var mes = message;

                            flat = false;
                        }
                        else if (responseGetMachine.StatusCode == 0)
                        {
                            var mes = responseGetMachine.ErrorMessage;

                            flat = false;
                        }
                        else
                        {

                            flat = false;
                        }
                        if (contador == 3)
                            flat = false;
                    }// fin del while
                     //}

                }
                else if (response.StatusCode == 0)
                {
                    var mesagge = "No se generó token." + response.ErrorMessage;
                }
                else
                {
                    var mesagge = "No se generó token. Usuario no Autorizado. Validar credenciales de acceso.";

                }
            }
            catch (Exception ex)
            {
                if (ex.ToString().Length > 400)
                {
                    var mesagge = ex.ToString().Replace("'", "").Replace("\"", "").Substring(0, 400);
                   
                }
                else
                {
                    var mesagge = ex.ToString().Replace("'", "").Replace("\"", "");
                    
                }
            }
          
        }
        class Token
        {
            public string access_token { get; set; }
            public string token_type { get; set; }
            public string expires_in { get; set; }
                                        
            public IRestResponse GetToken(string url, string accept, string contentType, string username, string password, string grantType)
            {
                var client = new RestClient(url);
                client.Timeout = -1;
                var request = new RestRequest(Method.POST);
                request.AddHeader("Accept", accept);
                request.AddHeader("Content-Type", contentType);
                request.AddParameter("username", username);
                request.AddParameter("password", password);
                request.AddParameter("grant_type", grantType);
                IRestResponse response = client.Execute(request);
                return response;
            }
        }
        class RankingOperacion
        {
            public DateTime RankingDate { get; set; }
            public string DriverIdentification { get; set; }
            public string DriverName { get; set; }
            public string DriverGroup { get; set; }
            public double? OverSpeedScore { get; set; }
            public int OverSpeedCount { get; set; }
            public double? MaxSpeed { get; set; }
            public double? OverRpmScore { get; set; }
            public int OverRpmCount { get; set; }
            public double? MaxRpm { get; set; }
            public double? ExessiveIdleScore { get; set; }
            public int ExcessiveIdleCount { get; set; }
            public double? ExcessiveIdleTotalSeconds { get; set; }
            public double? GreenBandScore { get; set; }
            public int GreenBandCount { get; set; }
            public double? GreenBandTotalSeconds { get; set; }
            public double? OverAccelerationScore { get; set; }
            public int OverAccelerationCount { get; set; }
            public double? MaxAcceleration { get; set; }
            public double? HarshBrakeScore { get; set; }
            public int HarshBrakeCount { get; set; }
            public double? MaxBrake { get; set; }
            public double? LoginScore { get; set; }
            public double? LoginCount { get; set; }
            public double? DelayedStartScore { get; set; }
            public int DelayedStartCount { get; set; }
            public double? DelayedStartTime { get; set; }
            public double? DelayedRouteDrivingScore { get; set; }
            public int DelayedRouteDrivingCount { get; set; }
            public double? DelayedRouteDrivingTime { get; set; }
            public double? TotalScore { get; set; }
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

            public IRestResponse ConsumeApiRest(string token, string url, string accept, string accept_Encoding, DateTime starDate, DateTime endDate)
            {
                var clientGet = new RestClient(url);
                clientGet.Timeout = -1;
                var requestGet = new RestRequest(Method.GET);
                string bearer = "bearer" + " " + token;
                requestGet.AddHeader("Authorization", bearer);
                requestGet.AddHeader("Accept", accept);
                requestGet.AddHeader("Accept-Encoding", accept_Encoding);
                //requestGet.AddParameter("startDate", starDate.ToString("s").Replace("T", " "));
                //requestGet.AddParameter("endDate", endDate.ToString("s").Replace("T", " "));
                requestGet.AddParameter("startDate", "2021-05-07 00:00:00");
                requestGet.AddParameter("endDate", "2021-05-07 23:59:59");
                IRestResponse responseGet = clientGet.Execute(requestGet);
                return responseGet;
            }
            public void LoadFlatFile(string flatFilePath, List<RankingOperacion> listJsonDriverMachine)
            {

                string placa = "CO_GDX195";
                using (StreamWriter file = File.AppendText(flatFilePath))
                {
                    if (flatFilePath == "C:\\TDM_Pruebas\\TDM_Prueba1.txt")
                    {
                        foreach (RankingOperacion dataApi in listJsonDriverMachine)
                        {
                            if (dataApi.MachineName == placa)
                            {
                                var liter = Convert.ToInt32(dataApi.Liters.ToString().Replace(",", "."));
                                var galones = liter / 3.7854;

                                var dataComplet = liter + ";" + galones;
                                file.WriteLine(dataComplet);
                            }
                            
                        }

                    }
                   
                }
            }
        }

    }
}
