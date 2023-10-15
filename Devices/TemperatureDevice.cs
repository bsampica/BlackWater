using BlackWater.Devices.Base;
using Splat;

namespace BlackWater.Devices;

public class TemperatureDevice : GenericPiDevice
{
    public TemperatureDevice(ILogger logger) : base(logger)
    {
    }
}