using Splat;
using BlackWater.Models.Base;

namespace BlackWater.Models;

public class CurrentSensorValue : SensorValue
{
    public CurrentSensorValue(ILogger logger) : base(logger)
    {
    }
}