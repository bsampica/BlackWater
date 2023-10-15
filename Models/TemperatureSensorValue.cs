using Splat;
using BlackWater.Models.Base;

namespace BlackWater.Models;

public class TemperatureSensorValue : SensorValue
{
    public TemperatureSensorValue(ILogger logger) : base(logger)
    {
    }
}