using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Splat;

namespace BlackWater.Devices.Base;

public class GenericPiDevice : IPiDevice
{
    private string _deviceId;
    private string _deviceName;
    private string _dataTypeString;
    private Type _dataType;

    private ILogger _logger;

    public event PropertyChangedEventHandler? PropertyChanged;

    public string DeviceId
    {
        get => _deviceId;
        set => SetField(ref _deviceId, value);
    }

    public string DeviceName
    {
        get => _deviceName;
        set => SetField(ref _deviceName, value);
    }

    public string DataTypeString
    {
        get => _dataTypeString;
        set => SetField(ref _dataTypeString, value);
    }

    public Type DataType
    {
        get => _dataType;
        set => SetField(ref _dataType, value);
    }

    public GenericPiDevice(ILogger logger)
    {
        _logger = logger;
    }

    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    protected bool SetField<T>(ref T field, T value, [CallerMemberName] string? propertyName = null)
    {
        if (EqualityComparer<T>.Default.Equals(field, value)) return false;
        field = value;
        OnPropertyChanged(propertyName);
        return true;
    }
}