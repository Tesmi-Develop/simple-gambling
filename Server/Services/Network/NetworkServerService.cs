using System.Net;
using System.Net.Sockets;
using Server.ServiceRealisation;

namespace Server.Services.Network;

[Service]
public sealed class NetworkServerService : IStartable
{
    public event Action<TcpClient, byte[]>? OnReceive;
    public event Action<TcpClient>? OnConnect;
    public event Action<TcpClient>? OnDisconnect;
    private TcpListener _listener = null!;

    public void Setup(IPAddress ip, int port)
    {
        _listener = new TcpListener(ip, port);
    }

    private async Task<bool> ReadExactlyAsync(NetworkStream stream, byte[] buffer)
    {
        var totalRead = 0;
        while (totalRead < buffer.Length)
        {
            var read = await stream.ReadAsync(buffer.AsMemory(totalRead, buffer.Length - totalRead));
            if (read == 0) return false; 
        
            totalRead += read;
        }
        
        return true;
    }
    
    private async Task ListenClient(TcpClient client)
    {
        using (client)
        {
            await using var stream = client.GetStream();

            while (true)
            {
                try
                {
                    var lengthBuffer = new byte[4];
                    if (!await ReadExactlyAsync(stream, lengthBuffer)) 
                        break;
                    
                    var messageLength = BitConverter.ToInt32(lengthBuffer, 0);
                    var messageBuffer = new byte[messageLength];
                    if (!await ReadExactlyAsync(stream, messageBuffer)) 
                        break;
                    
                    try
                    {
                        OnReceive?.Invoke(client, messageBuffer);
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine($"Got exception while handling message client: {e.Message}");
                    }
                }
                catch (IOException e)
                {
                    break;
                }    
            }
            OnDisconnect?.Invoke(client);
        }
    }

    private async Task ListenClients()
    {
        while (true)
        {
            var client = await _listener.AcceptTcpClientAsync();
            OnConnect?.Invoke(client);
            _ = ListenClient(client);
        }
    }
    
    public Task Start()
    {
        if (_listener is null)
            throw new Exception("Network server listener not initialized");
        
        _listener.Start();
        _ = ListenClients();
        
        return Task.CompletedTask;
    }
}