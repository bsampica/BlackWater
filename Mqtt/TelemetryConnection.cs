using System;
using System.Threading;
using System.Threading.Tasks;
using MQTTnet;
using MQTTnet.Client;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using MQTTnet.Extensions.ManagedClient;
using MQTTnet.Protocol;

namespace BlackWater.Mqtt;

public class TelemetryConnection
{
    private IManagedMqttClient? _mqttClient;
    private ManagedMqttClientOptions? _mqttOptions;
    private string _server = "";
    // private static string username = "";
    // private static string password = "";
    private string _clientId = "";
    private string _topicD2C = "";
    // private string topicC2D = "";


    public TelemetryConnection()
    {
        Console.WriteLine("Constructing Telemetry Connection: ");
    }

    public async Task ConnectAsync(CancellationToken token = default)
    {
        var factory = new MqttFactory();
        //mqttClient = factory.CreateMqttClient();
        _mqttClient = factory.CreateManagedMqttClient();

        _server = "broker.hivemq.com";
        _clientId = "raspberrypi-lpb1";

        // username = $"";
        // password = "";
        _topicD2C = $"devices/{_clientId}/messages/events/";
        //topicC2D = $"devices/{_clientId}/messages/devicebound/#";

        Console.WriteLine($"Mqtt Client Connecting");

        var clientOptions = new MqttClientOptionsBuilder()
            .WithTcpServer(_server, 8883)
            //.WithCredentials(username, password)
            .WithClientId(_clientId)
            .WithTlsOptions(new MqttClientTlsOptions() { UseTls = true });

        var reconnectDelay = new TimeSpan(0, 0, 10);
        _mqttOptions = new ManagedMqttClientOptionsBuilder()
                            .WithAutoReconnectDelay(reconnectDelay)
                            .WithClientOptions(clientOptions)
                            .Build();

        _mqttClient.ConnectedAsync += ClientConnected;
        _mqttClient.DisconnectedAsync += ClientDisconnectedAsync;
        _mqttClient.ConnectingFailedAsync += ClientConnectingFailedAsync;
        _mqttClient.ConnectionStateChangedAsync += ClientConnectionStateChangedAsync;
        _mqttClient.ApplicationMessageReceivedAsync += ClientApplicationMessageReceived;
        _mqttClient.ApplicationMessageProcessedAsync += ClientApplicationMessageProcessedAsync;
        _mqttClient.ApplicationMessageSkippedAsync += args =>
        {
            Console.WriteLine("Application Message Skipped: {0} ", args);
            return Task.CompletedTask;
        };

        await _mqttClient.StartAsync(_mqttOptions);

    }

    private Task ClientApplicationMessageProcessedAsync(ApplicationMessageProcessedEventArgs args)
    {
        Console.WriteLine("Application Message Processed: {0}", args);
        return Task.CompletedTask;
    }

    private Task ClientApplicationMessageReceived(MqttApplicationMessageReceivedEventArgs args)
    {
        Console.WriteLine("Application Message Received: {0}", args);
        return Task.CompletedTask;
    }

    private Task ClientConnectionStateChangedAsync(EventArgs args)
    {
        Console.WriteLine($"Client Connection State Changed: {args}");
        // Console.WriteLine(args);
        return Task.CompletedTask;
    }

    private Task ClientConnectingFailedAsync(ConnectingFailedEventArgs args)
    {
        Console.WriteLine($"Client Connecting Failed {args}");
        // Console.WriteLine(args);
        return Task.CompletedTask;
    }

    private Task ClientDisconnectedAsync(MqttClientDisconnectedEventArgs args)
    {
        Console.WriteLine($"Client Disconnected: {args}");
        return Task.CompletedTask;
    }

    private Task ClientConnected(MqttClientConnectedEventArgs args)
    {
        Console.WriteLine($"Client Connected:  {args}");
        return Task.CompletedTask;
    }

    public Task SubscribeAsync()
    {
        return _mqttClient.SubscribeAsync($"devices/{_clientId}/messages/events/#", MqttQualityOfServiceLevel.ExactlyOnce);
    }

    public async Task PublishLoopAsync()
    {
        while (true)
        {
            JObject payloadObject = new()
            {
                { "Temperature", "22." + DateTime.UtcNow.Millisecond.ToString() },
                { "Humidity", ".84." + DateTime.UtcNow.Millisecond.ToString() },
                { "ReadingDateTime", DateTime.UtcNow}
            };

            string payload = JsonConvert.SerializeObject(payloadObject);
            Console.WriteLine($"Topic: {_topicD2C} - Payload: {payload}");

            var message = new MqttApplicationMessageBuilder()
                .WithTopic(_topicD2C)
                .WithPayload(payload)
                .WithQualityOfServiceLevel(MQTTnet.Protocol.MqttQualityOfServiceLevel.AtLeastOnce)
                .Build();

            Console.WriteLine("Publishing Message Async: Start");

            Console.WriteLine($"IsConnected: {_mqttClient!.IsConnected}");
            await _mqttClient.EnqueueAsync(message);
            // Thread.Sleep(3000);
            await Task.Delay(5000);
        }
    }
}