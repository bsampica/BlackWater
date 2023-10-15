using System;
using System.ComponentModel;

namespace BlackWater.Models.Base;

public interface ISensorModel
{
    Guid SensorId { get; set; }
    DateTime TimeOfValue { get; set; }
}