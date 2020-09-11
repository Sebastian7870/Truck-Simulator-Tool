namespace Truck_Simulator_Tool__WPF_.TruckSimulatorTool.Json
{

    public class Rootobject_Telemetry
    {
        public Ets2 ets2 { get; set; }
        public Omsi omsi { get; set; }
    }

    public class Ets2
    {
        public Events events { get; set; }
        public Game game { get; set; }
        public Job job { get; set; }
        public Trailer[] trailers { get; set; }
        public Truck truck { get; set; }
    }

    public class Events
    {
        public Lastjobcancelled lastJobCancelled { get; set; }
        public Lastjobdelivered lastJobDelivered { get; set; }
        public Lastplayerfined lastPlayerFined { get; set; }
        public Lastplayerteleported lastPlayerTeleported { get; set; }
        public Lastplayertollgatepaid lastPlayerTollgatePaid { get; set; }
        public Lastplayeruseferry lastPlayerUseFerry { get; set; }
    }

    public class Lastjobcancelled
    {
        public int cancelPenalty { get; set; }
        public string guid { get; set; }
        public int timestamp { get; set; }
        public Truckplacement truckPlacement { get; set; }
    }

    public class Truckplacement
    {
        public float heading { get; set; }
        public float pitch { get; set; }
        public float roll { get; set; }
        public float x { get; set; }
        public float y { get; set; }
        public float z { get; set; }
    }

    public class Lastjobdelivered
    {
        public bool autoLoadUsed { get; set; }
        public bool autoParkUsed { get; set; }
        public float cargoDamage { get; set; }
        public int deliveryTime { get; set; }
        public float distanceKM { get; set; }
        public int earnedXP { get; set; }
        public string guid { get; set; }
        public int revenue { get; set; }
        public int timestamp { get; set; }
        public Truckplacement1 truckPlacement { get; set; }
    }

    public class Truckplacement1
    {
        public float heading { get; set; }
        public float pitch { get; set; }
        public float roll { get; set; }
        public float x { get; set; }
        public float y { get; set; }
        public float z { get; set; }
    }

    public class Lastplayerfined
    {
        public int fineAmount { get; set; }
        public string fineOffence { get; set; }
        public string guid { get; set; }
        public int timestamp { get; set; }
        public Truckplacement2 truckPlacement { get; set; }
    }

    public class Truckplacement2
    {
        public float heading { get; set; }
        public float pitch { get; set; }
        public float roll { get; set; }
        public float x { get; set; }
        public float y { get; set; }
        public float z { get; set; }
    }

    public class Lastplayerteleported
    {
        public float distance { get; set; }
        public string guid { get; set; }
        public Newplacement newPlacement { get; set; }
        public Oldplacement oldPlacement { get; set; }
        public int timestamp { get; set; }
    }

    public class Newplacement
    {
        public float heading { get; set; }
        public float pitch { get; set; }
        public float roll { get; set; }
        public float x { get; set; }
        public float y { get; set; }
        public float z { get; set; }
    }

    public class Oldplacement
    {
        public float heading { get; set; }
        public float pitch { get; set; }
        public float roll { get; set; }
        public float x { get; set; }
        public float y { get; set; }
        public float z { get; set; }
    }

    public class Lastplayertollgatepaid
    {
        public string guid { get; set; }
        public int payAmount { get; set; }
        public int timestamp { get; set; }
        public Truckplacement3 truckPlacement { get; set; }
    }

    public class Truckplacement3
    {
        public float heading { get; set; }
        public float pitch { get; set; }
        public float roll { get; set; }
        public float x { get; set; }
        public float y { get; set; }
        public float z { get; set; }
    }

    public class Lastplayeruseferry
    {
        public string guid { get; set; }
        public int payAmount { get; set; }
        public string sourceID { get; set; }
        public string sourceName { get; set; }
        public string targetID { get; set; }
        public string targetName { get; set; }
        public int timestamp { get; set; }
        public Truckplacement4 truckPlacement { get; set; }
    }

    public class Truckplacement4
    {
        public float heading { get; set; }
        public float pitch { get; set; }
        public float roll { get; set; }
        public float x { get; set; }
        public float y { get; set; }
        public float z { get; set; }
    }

    public class Game
    {
        public bool connected { get; set; }
        public bool dataAvailable { get; set; }
        public string documentsPath { get; set; }
        public string gameID { get; set; }
        public string gameName { get; set; }
        public string gamePath { get; set; }
        public int gameTime { get; set; }
        public string gameVersion { get; set; }
        public bool isMP { get; set; }
        public string lastProfile { get; set; }
        public int nextRestStopTime { get; set; }
        public int overlayState { get; set; }
        public bool paused { get; set; }
        public string[] profiles { get; set; }
        public int renderHeight { get; set; }
        public int renderWidth { get; set; }
        public string renderer { get; set; }
        public float scale { get; set; }
        public string telemetryPluginVersion { get; set; }
        public int telemetryVersion { get; set; }
    }

    public class Job
    {
        public Cargo cargo { get; set; }
        public int deadlineTime { get; set; }
        public string destinationCity { get; set; }
        public string destinationCityID { get; set; }
        public string destinationCompany { get; set; }
        public string destinationCompanyID { get; set; }
        public int income { get; set; }
        public int remainingTime { get; set; }
        public string sourceCity { get; set; }
        public string sourceCityID { get; set; }
        public string sourceCompany { get; set; }
        public string sourceCompanyID { get; set; }
    }

    public class Cargo
    {
        public string id { get; set; }
        public bool isSpecial { get; set; }
        public bool loaded { get; set; }
        public string market { get; set; }
        public string name { get; set; }
        public int plannedDistanceKM { get; set; }
        public float totalDamage { get; set; }
        public float totalMass { get; set; }
        public int unitCount { get; set; }
        public float unitMass { get; set; }
    }

    public class Truck
    {
        public Acceleration acceleration { get; set; }
        public float adblue { get; set; }
        public float adblueCapacity { get; set; }
        public float adblueWarningFactor { get; set; }
        public bool adblueWarningOn { get; set; }
        public float airPressure { get; set; }
        public bool airPressureEmergencyOn { get; set; }
        public float airPressureEmergencyValue { get; set; }
        public bool airPressureWarningOn { get; set; }
        public float airPressureWarningValue { get; set; }
        public float batteryVoltage { get; set; }
        public bool batteryVoltageWarningOn { get; set; }
        public float batteryVoltageWarningValue { get; set; }
        public bool blinkerLeftActive { get; set; }
        public bool blinkerLeftOn { get; set; }
        public bool blinkerRightActive { get; set; }
        public bool blinkerRightOn { get; set; }
        public float brakeTemperature { get; set; }
        public string brand { get; set; }
        public string brandID { get; set; }
        public Cabin cabin { get; set; }
        public bool cruiseControlOn { get; set; }
        public float cruiseControlSpeed { get; set; }
        public int displayedGear { get; set; }
        public bool electricOn { get; set; }
        public bool engineOn { get; set; }
        public float engineRpm { get; set; }
        public float engineRpmMax { get; set; }
        public int forwardGears { get; set; }
        public float fuel { get; set; }
        public float fuelAverageConsumption { get; set; }
        public float fuelCapacity { get; set; }
        public float fuelRange { get; set; }
        public float fuelWarningFactor { get; set; }
        public bool fuelWarningOn { get; set; }
        public float gameBrake { get; set; }
        public float gameClutch { get; set; }
        public float gameSteer { get; set; }
        public float gameThrottle { get; set; }
        public int gear { get; set; }
        public Head head { get; set; }
        public Hook hook { get; set; }
        public string id { get; set; }
        public string licensePlate { get; set; }
        public string licensePlateCountry { get; set; }
        public string licensePlateCountryID { get; set; }
        public bool lightsAuxFrontOn { get; set; }
        public int lightsAuxFrontValue { get; set; }
        public bool lightsAuxRoofOn { get; set; }
        public int lightsAuxRoofValue { get; set; }
        public bool lightsBeaconOn { get; set; }
        public bool lightsBeamHighOn { get; set; }
        public bool lightsBeamLowOn { get; set; }
        public bool lightsBrakeOn { get; set; }
        public bool lightsDashboardOn { get; set; }
        public float lightsDashboardValue { get; set; }
        public bool lightsParkingOn { get; set; }
        public bool lightsReverseOn { get; set; }
        public bool motorBrakeOn { get; set; }
        public string name { get; set; }
        public float navigationEstimatedDistance { get; set; }
        public int navigationEstimatedTime { get; set; }
        public float odometer { get; set; }
        public float oilPressure { get; set; }
        public bool oilPressureWarningOn { get; set; }
        public float oilPressureWarningValue { get; set; }
        public float oilTemperature { get; set; }
        public bool parkBrakeOn { get; set; }
        public Placement placement { get; set; }
        public int retarderBrake { get; set; }
        public int retarderStepCount { get; set; }
        public int reverseGears { get; set; }
        public int shifterSlot { get; set; }
        public string shifterType { get; set; }
        public float speed { get; set; }
        public float speedLimit { get; set; }
        public float userBrake { get; set; }
        public float userClutch { get; set; }
        public float userSteer { get; set; }
        public float userThrottle { get; set; }
        public float waterTemperature { get; set; }
        public bool waterTemperatureWarningOn { get; set; }
        public float waterTemperatureWarningValue { get; set; }
        public float wearCabin { get; set; }
        public float wearChassis { get; set; }
        public float wearEngine { get; set; }
        public float wearTransmission { get; set; }
        public float wearWheels { get; set; }
        public int wheelCount { get; set; }
        public bool wipersOn { get; set; }
    }

    public class Acceleration
    {
        public float x { get; set; }
        public float y { get; set; }
        public float z { get; set; }
    }

    public class Cabin
    {
        public float x { get; set; }
        public float y { get; set; }
        public float z { get; set; }
    }

    public class Head
    {
        public float x { get; set; }
        public float y { get; set; }
        public float z { get; set; }
    }

    public class Hook
    {
        public float x { get; set; }
        public float y { get; set; }
        public float z { get; set; }
    }

    public class Placement
    {
        public float heading { get; set; }
        public float pitch { get; set; }
        public float roll { get; set; }
        public float x { get; set; }
        public float y { get; set; }
        public float z { get; set; }
    }

    public class Trailer
    {
        public bool attached { get; set; }
        public string bodyType { get; set; }
        public string brand { get; set; }
        public string brandID { get; set; }
        public string cargoAccessoryID { get; set; }
        public float cargoDamage { get; set; }
        public string chainType { get; set; }
        public Hook1 hook { get; set; }
        public string id { get; set; }
        public string licensePlate { get; set; }
        public string licensePlateCountry { get; set; }
        public string licensePlateCountryID { get; set; }
        public string name { get; set; }
        public Placement1 placement { get; set; }
        public float wearChassis { get; set; }
        public float wearWheels { get; set; }
        public int wheelCount { get; set; }
    }

    public class Hook1
    {
        public float x { get; set; }
        public float y { get; set; }
        public float z { get; set; }
    }

    public class Placement1
    {
        public float heading { get; set; }
        public float pitch { get; set; }
        public float roll { get; set; }
        public float x { get; set; }
        public float y { get; set; }
        public float z { get; set; }
    }

    public class Omsi
    {
        public bool connected { get; set; }
    }
}
