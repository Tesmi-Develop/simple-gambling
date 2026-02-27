using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using Server.Utility;
using Shared;

namespace Server.DTO;

public sealed class Client
{
    public event Action<Packet> OnDataReceived;
    
    private readonly NetworkStream _stream;
    private readonly TcpClient _socket;
    public IUserState UserState { get; }

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
}