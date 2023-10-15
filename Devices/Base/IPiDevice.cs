using System;
using System.ComponentModel;

namespace BlackWater.Devices.Base;

public interface IPiDevice : INotifyPropertyChanged
{
    string DeviceId { get; set; }
    string DeviceName { get; set; }
    string DataTypeString { get; set; }
    Type DataType { get; set; }
    
}