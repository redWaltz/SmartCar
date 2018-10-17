using SmartCar.Services;
using Newtonsoft.Json;
using System.Web.Http;
using System.Collections.Generic;
using System;

namespace SmartCar.Controllers
{
    public class VehiclesController : ApiController
    {
        GMConnectionService gmConnectionSrv;
        GMConnectionService gmConnectionsService => gmConnectionSrv ?? (gmConnectionSrv = new GMConnectionService());

        /// <summary>
        /// Returns general vehicle info
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        public string Get(int id)
        {
            var response = gmConnectionsService.GetVehicleInfo(id);
            return JsonConvert.SerializeObject(response);
        }


        /// <summary>
        /// Returns the status of the doors
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        public string Doors(int id)
        {
            var response = gmConnectionsService.GetDoorSecurity(id);
            return JsonConvert.SerializeObject(response);
        }

        /// <summary>
        /// Returns the percentage of fuel remaining
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        public string Fuel(int id)
        {
            var response = gmConnectionsService.GetFuelAndBatteryLevels(id, true);
            return JsonConvert.SerializeObject(new Dictionary<string, decimal?> { { "percent", response } });
        }

        /// <summary>
        /// Returns the percentage of battery remaining
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        public string Battery(int id)
        {
            var response = gmConnectionsService.GetFuelAndBatteryLevels(id, false);
            return JsonConvert.SerializeObject(new Dictionary<string, decimal?> { { "percent", response } });
        }

        /// <summary>
        /// Starts or stops the engine
        /// </summary>
        /// <param name="id"></param>
        /// <param name="command"></param>
        /// <returns></returns>
        [HttpPost]
        public string Engine(int id, string command)
        {
            var action = string.Empty;
            if(command.Equals("start", StringComparison.InvariantCultureIgnoreCase))
            {
                action = "START_VEHICLE";
            }
            else if(command.Equals("stop", StringComparison.InvariantCultureIgnoreCase))
            {
                action = "STOP_VEHICLE";
            }
            else
            {
                return JsonConvert.SerializeObject(new Dictionary<string, string> { { "status", "error" } });
            }

            var response = gmConnectionsService.VehicleStartStop(id, action);

            if(response.Equals("EXECUTED", StringComparison.InvariantCultureIgnoreCase))
            {
                return JsonConvert.SerializeObject(new Dictionary<string, string> { { "status", "success" } });
            }

            return JsonConvert.SerializeObject(new Dictionary<string, string> { { "status", "error" } });
        }

    }
}
