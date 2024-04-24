using System;
using uPLibrary.Networking.M2Mqtt;
using uPLibrary.Networking.M2Mqtt.Messages;

public class MqttClientService
{
    private MqttClient _client;

    public MqttClientService(string brokerHostName)
    {
        // Erstelle den MQTT Client
        _client = new MqttClient(brokerHostName);

        // Registriere ein Event Handler für eingehende Nachrichten
        _client.MqttMsgPublishReceived += Client_MqttMsgPublishReceived;

        // Verbindungs-Callback-Handler
        _client.ConnectionClosed += Client_ConnectionClosed;
    }

    private void Client_ConnectionClosed(object sender, EventArgs e)
    {
        Console.WriteLine("MQTT connection closed.");
    }

    private void Client_MqttMsgPublishReceived(object sender, MqttMsgPublishEventArgs e)
    {
        string receivedMessage = System.Text.Encoding.UTF8.GetString(e.Message);
        Console.WriteLine($"Message received on topic {e.Topic}: {receivedMessage}");
    }

    public void Connect(string clientId)
    {
        _client.Connect(clientId);
    }

    public void Subscribe(string[] topics, byte[] qosLevels)
    {
        _client.Subscribe(topics, qosLevels);
    }

    public void Publish(string topic, string message)
    {
        byte[] messageBytes = System.Text.Encoding.UTF8.GetBytes(message);
        _client.Publish(topic, messageBytes, MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE, retain: false);
    }

    public void Disconnect()
    {
        _client.Disconnect();
    }
}
