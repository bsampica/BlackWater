using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Splat;

namespace BlackWater.Models.Base;

public abstract class SensorValue : ISensorModel
{
    public Guid SensorId { get; set; }
    public DateTime TimeOfValue { get; set; }
    private readonly ILogger _logger;

    protected SensorValue(ILogger logger)
    {
        _logger = logger;
    }
}