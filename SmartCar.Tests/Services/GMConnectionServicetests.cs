using Microsoft.VisualStudio.TestTools.UnitTesting;
using SmartCar.Services;
using System;
using System.Linq;

namespace SmartCar.Tests.Services
{
    [TestClass]
    public class GMConnectionServiceTests
    {
        private GMConnectionService gmConnectionSrv;
        private GMConnectionService GmConnectionsService => gmConnectionSrv ?? (gmConnectionSrv = new GMConnectionService());

        [TestMethod]
        public void GetVehicleInfoTest()
        {
            var result = GmConnectionsService.GetVehicleInfo(1234);
            Assert.IsNotNull(result);
            Assert.AreEqual(result.vin, "123123412412");
            Assert.AreEqual(result.color, "Metallic Silver");
            Assert.AreEqual(result.doorCount, 4);
            Assert.AreEqual(result.driveTrain, "v8");
        }

        [TestMethod]
        public void GetDoorSecurityTest()
        {
            var results = GmConnectionsService.GetDoorSecurity(1234);
            Assert.IsNotNull(results);
            var frontLeft = results.FirstOrDefault(x => x.location.Equals("frontLeft", StringComparison.InvariantCultureIgnoreCase));
            Assert.AreEqual(frontLeft.locked, false);
        }


        [TestMethod]
        public void GetFuelAndBatteryLevelsTest()
        {
            var fuelResult = GmConnectionsService.GetFuelAndBatteryLevels(1234, true);
            Assert.IsNotNull(fuelResult);
            var batteryResult = GmConnectionsService.GetFuelAndBatteryLevels(1234, false);
            Assert.IsNull(batteryResult);
        }

        [TestMethod]
        public void VehicleStartStopTest()
        {
            var startResult = GmConnectionsService.VehicleStartStop(1234, "START_VEHICLE");
            Assert.IsNotNull(startResult);
            var stopResult = GmConnectionsService.VehicleStartStop(1234, "STOP_VEHICLE");
            Assert.IsNotNull(stopResult);
        }
    }
}
