using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using Server.Utility;
using Shared;

namespace Server.DTO;

public sealed class Client
{
    private static Dictionary<string, Type>  _types = [];
    
    public event Action<Packet> OnDataReceived;
    
    private readonly NetworkStream _stream;
    private readonly TcpClient _socket;
    public IUserState UserState { get; }

    static Client()
    {
        foreach (var (type, data) in ReflectionHelper.GetAllTypes<NetworkEventAttribute>())
        {
            _types[type.Name] = type;
        }
    }

    public Client(TcpClient socket)
    {
        var myGuid = Guid.NewGuid();
        
        _socket = socket;
        _stream = socket.GetStream();
        UserState = new UserState(myGuid.ToString());
    }

    // TODO: Снести этот код в сервис
    public async Task Send<T>(T data)
    {
        var json = JsonSerializer.Serialize(data);
        var packet = Encoding.UTF8.GetBytes(json);
        var length = BitConverter.GetBytes(packet.Length);
        
        await _stream.WriteAsync(length);
        await _stream.WriteAsync(packet);
        await _stream.FlushAsync();
    }
    
    public (Packet, Type) Receive(byte[] message)
    {
        var json = Encoding.UTF8.GetString(message);
        var packetObject = JsonSerializer.Deserialize<Packet>(json);
        if (packetObject == null)
            throw new Exception("Packet object is null");

        var jsonData = (JsonElement)packetObject.Data;
        
        if (!_types.TryGetValue(packetObject.EventName, out var eventType))
            throw new Exception("Unknown packet type");
        
        var arg = jsonData.Deserialize(eventType);
        if (arg is null)
            throw new Exception("Unknown packet type");
        
        packetObject.Data = arg;
        OnDataReceived?.Invoke(packetObject);
        
        return (packetObject, eventType);
    }
}