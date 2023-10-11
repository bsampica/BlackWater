using System;
using System.Threading;
using System.Threading.Tasks;
using MQTTnet;
using MQTTnet.Client;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using MQTTnet.Extensions.ManagedClient;

namespace BlackWater;

public class TelemetryConnection
{
    private IManagedMqttClient? mqttClient;
    private ManagedMqttClientOptions? mqttOptions;
    private string server = "";
    // private static string username = "";
    // private static string password = "";
    private string clientId = "";
    private string topicD2C = "";
    private string topicC2D = "";


    public TelemetryConnection()
    {
        Console.WriteLine("Constructing Telemetry Connection: ");
    }

    public async Task ConnectAsync(CancellationToken token = default)
    {
        Console.WriteLine("Entering ConnectAsync (Currently not Async)");
        var factory = new MqttFactory();
        //mqttClient = factory.CreateMqttClient();
        mqttClient = factory.CreateManagedMqttClient();

        server = "broker.hivemq.com";
        clientId = "raspberrypi-lpb1";

        // username = $"{server}/{clientId}/?api-version=2021-04-12";
        // password = "JJJ1tZKo/Z/x41qhSxSKnrbbzYIPCNc5ZAIoTNqTjBc=";
        topicD2C = $"devices/{clientId}/messages/events/";
        topicC2D = $"devices/{clientId}/messages/devicebound/#";

        // Console.WriteLine($"MQTT Server:{server} | Username:{username} | ClientID:{clientId}");
        Console.WriteLine($"Mqtt Client Connecting");

        var clientOptions = new MqttClientOptionsBuilder()
            .WithTcpServer(server, 8883)
            //.WithCredentials(username, password)
            .WithClientId(clientId)
            .WithTlsOptions(new MqttClientTlsOptions() { UseTls = true });

        var reconnectDelay = new TimeSpan(0, 0, 10);
        mqttOptions = new ManagedMqttClientOptionsBuilder()
                            .WithAutoReconnectDelay(reconnectDelay)
                            .WithClientOptions(clientOptions)
                            .Build();

        mqttClient.ConnectedAsync += ClientConnected;
        mqttClient.DisconnectedAsync += ClientDisconnected;
        mqttClient.ConnectingFailedAsync += ClientConnectFailed;
        mqttClient.ConnectionStateChangedAsync += ClientConnectionStateFailed;
        await mqttClient.StartAsync(mqttOptions);

    }

    private static Task ClientConnectionStateFailed(EventArgs args)
    {
        // Console.WriteLine(args);
        return Task.CompletedTask;
    }

    private static Task ClientConnectFailed(ConnectingFailedEventArgs args)
    {
        // Console.WriteLine(args);
        return Task.CompletedTask;
    }

    private static Task ClientDisconnected(MqttClientDisconnectedEventArgs args)
    {
        // Console.WriteLine(args);
        return Task.CompletedTask;
    }

    private static Task ClientConnected(MqttClientConnectedEventArgs args)
    {
        // Console.WriteLine(args);
        return Task.CompletedTask;
    }

    public async Task PublishLoopAsync()
    {
        while (true)
        {
            JObject payloadObject = new JObject();
            payloadObject.Add("Temperature", "22." + DateTime.UtcNow.Millisecond.ToString());
            payloadObject.Add("Humidity", ".84." + DateTime.UtcNow.Millisecond.ToString());

            string payload = JsonConvert.SerializeObject(payloadObject);
            Console.WriteLine($"Topic: {topicD2C} - Payload: {payload}");

            var message = new MqttApplicationMessageBuilder()
                .WithTopic(topicD2C)
                .WithPayload(payload)
                .WithQualityOfServiceLevel(MQTTnet.Protocol.MqttQualityOfServiceLevel.AtLeastOnce)
                .Build();

            Console.WriteLine("Publishing Message Async: Start");

            Console.WriteLine($"IsConnected: {mqttClient!.IsConnected}");
            await mqttClient.EnqueueAsync(message);
            // Thread.Sleep(3000);
            await Task.Delay(5000);
        }
    }
}