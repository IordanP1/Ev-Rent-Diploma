namespace EvRent.Models
{
    public class RangeRequest
    {
        public double BatteryCapacityKwh { get; set; }
        public int BatteryLevelPercent { get; set; }
        public double ConsumptionKwhPer100km { get; set; }
        public string DrivingMode { get; set; } = "normal";
        public string Climate { get; set; } = "off";
        public string Load { get; set; } = "normal";
        public string Temperature { get; set; } = "mild";

    }
}
