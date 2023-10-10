using System;
using System.Threading;
using System.Threading.Tasks;
using DynamicData;
using MQTTnet;
using MQTTnet.Client;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace BlackWater;

public class TelemetryConnection
{
    private static IMqttClient? mqttClient;
    private static MqttClientOptions? mqttOptions;
    private static string server = "";
    private static string username = "";
    private static string password = "";
    private static string clientId = "";
    private static string topicD2C = "";
    private static string topicC2D = "";


    public TelemetryConnection()
    {
        var factory = new MqttFactory();
        mqttClient = factory.CreateMqttClient();

        server = "razzel-dazzel.azure-devices.net";
        clientId = "lpb-1";

        username = $"{server}/{clientId}/api-version=2018-06-30";
        topicD2C = $"devices/{clientId}/messages/events/";
        topicC2D = $"devices/{clientId}/messages/devicebound/#";

        Console.WriteLine($"MQTT Server:{server} Username:{username} ClientID:{clientId}");

        mqttOptions = new MqttClientOptionsBuilder()
                        .WithTcpServer(server, 8883)
                        .WithCredentials(username, "JJJ1tZKo/Z/x41qhSxSKnrbbzYIPCNc5ZAIoTNqTjBc=")
                        .WithClientId(clientId)
                        .WithTlsOptions(new MqttClientTlsOptions() { UseTls = true })
                        .Build();

        mqttClient.ApplicationMessageReceivedAsync += Client_HandleReceiveMessage;
        mqttClient.ConnectedAsync += Client_ConnectedAsync;
        mqttClient.DisconnectedAsync += Client_DisconnectedAsync;
        mqttClient.ConnectingAsync += ClientConnectingAsync;
    }

    public static async Task ConnectAsync(CancellationToken token = default)
    {
        if (mqttClient is not null)
            await mqttClient.ConnectAsync(mqttOptions, token);
    }

    public static async Task SubscribeAsync(CancellationToken token = default)
    {
        if (mqttClient is not null)
            await mqttClient.SubscribeAsync(topicC2D, MQTTnet.Protocol.MqttQualityOfServiceLevel.ExactlyOnce, token);
    }

    public static async Task PublishLoopAsync()
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

            if (mqttClient is not null)
                await mqttClient.PublishAsync(message);

            Console.WriteLine("Publishing Message Async: Finish");
            await Task.Delay(3000);
        }
    }

    private async Task ClientConnectingAsync(MqttClientConnectingEventArgs args)
    {
        Console.WriteLine(args);
    }

    private async Task Client_DisconnectedAsync(MqttClientDisconnectedEventArgs args)
    {
        Console.WriteLine(args);
    }

    private async Task Client_ConnectedAsync(MqttClientConnectedEventArgs args)
    {
        Console.WriteLine(args);
    }

    private async Task Client_HandleReceiveMessage(MqttApplicationMessageReceivedEventArgs args)
    {
        Console.WriteLine(args);
    }
}
