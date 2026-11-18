using EvRent.Models;

namespace EvRent.Services
{
    public class RangeCalculatorService
    {
        private readonly Dictionary<string, double> _drivingMode = new()
        {
             { "eco", 1.15 }, { "normal", 1.0 }, { "sport", 0.80 }
        };

        private readonly Dictionary<string, double> _climate = new()
         {
             { "off", 1.0 }, { "ac", 0.92 }, { "heating", 0.88 }
         };

        private readonly Dictionary<string, double> _load = new()
         {
              { "light", 1.05 }, { "normal", 1.0 }, { "heavy", 0.90 }
         };

        private readonly Dictionary<string, double> _temperature = new()
         {
              { "cold", 0.75 }, { "cool", 0.90 }, { "mild", 1.0 }, { "hot", 0.93 }
         };

        public RangeResult Calculate(RangeRequest req)
        {
            double availableKwh = req.BatteryCapacityKwh * (req.BatteryLevelPercent / 100.0);
            double baseRange = (availableKwh / req.ConsumptionKwhPer100km) * 100;

            double factor = _drivingMode[req.DrivingMode]
                          * _climate[req.Climate]
                          * _load[req.Load]
                          * _temperature[req.Temperature];

            double finalRange = baseRange * factor;
            double safeRange = finalRange * 0.85;

            return new RangeResult
            {
                AvailableKwh = Math.Round(availableKwh, 1),
                BaseRangeKm = (int)baseRange,
                FinalRangeKm = (int)finalRange,
                SafeRangeKm = (int)safeRange
            };
        }


    }
}
