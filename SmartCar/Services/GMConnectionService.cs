using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SmartCar.Models;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace SmartCar.Services
{
    public class GMConnectionService
    {
        private HttpClient client;
        private string GM_URL = "http://gmapi.azurewebsites.net/";

        public GMConnectionService()
        {
            client = new HttpClient();
        }


        public VehicleInfo GetVehicleInfo(int id)
        {
            var relativeUrl = "getVehicleInfoService";
            var response = Task.Run(() => QueryGMServiceAsync(id, relativeUrl)).Result;

            var obj = JsonConvert.DeserializeObject<JObject>(response);
            var values = obj.Last.First;

            var vehicleInfo = new VehicleInfo
            {
                vin = values["vin"]["value"].ToString(),
                color = values["color"]["value"].ToString(),
                driveTrain = values["driveTrain"]["value"].ToString(),
                doorCount = values["fourDoorSedan"]["value"].ToString() == "True" ? 4 : 2
            };

            return vehicleInfo;
        }

        public List<DoorStatus> GetDoorSecurity(int id)
        {
            var relativeUrl = "getSecurityStatusService";
            var response = Task.Run(() => QueryGMServiceAsync(id, relativeUrl)).Result;

            var obj = JsonConvert.DeserializeObject<JObject>(response);
            var values = obj.Last.First;

            var doors = new List<DoorStatus>();
            foreach (var token in values["doors"]["values"])
            {
                doors.Add(new DoorStatus
                {
                    location = token["location"]["value"].ToString(),
                    locked = token["locked"]["value"].ToString() == "locked"
                });
            }

            return doors;
        }

        public decimal? GetFuelAndBatteryLevels(int id, bool forFuel = true)
        {
            var relativeUrl = "getEnergyService";
            var response = Task.Run(() => QueryGMServiceAsync(id, relativeUrl)).Result;

            var obj = JsonConvert.DeserializeObject<JObject>(response);
            var values = obj.Last.First;

            decimal percent;

            if (forFuel)
            {
                if (decimal.TryParse(values["tankLevel"]["value"].ToString(), out percent))
                {
                    return percent;
                }
            }
            else
            {
                if (decimal.TryParse(values["batteryLevel"]["value"].ToString(), out percent))
                {
                    ;
                    return percent;
                }
            }
            return null;
        }

        public string VehicleStartStop(int id, string action)
        {
            var relativeUrl = "actionEngineService";
            var content = new StringContent($"{{\"id\": \"{id}\", \"command\": \"{action}\", \"responseType\": \"JSON\"}}", Encoding.UTF8, "application/json");

            var response = Task.Run(() => QueryGMServiceAsync(id, relativeUrl, content)).Result;

            var obj = JsonConvert.DeserializeObject<JObject>(response);
            var values = obj.Last.First;

            var status = values["status"].ToString();

            return status;
        }

        private async Task<string> QueryGMServiceAsync(int id, string relativeUrl, StringContent stringContent = null)
        {
            var url = $"{GM_URL}{relativeUrl}";
            var content = stringContent ?? new StringContent($"{{\"id\": \"{id}\", \"responseType\": \"JSON\"}}", Encoding.UTF8, "application/json");

            var response = await client.PostAsync(url, content);
            var responseString = await response.Content.ReadAsStringAsync();
            return responseString;
        }


    }
}