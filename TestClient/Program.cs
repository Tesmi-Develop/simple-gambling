using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using Shared;
using Shared.Events;
using Shared.Events.Gambling;

// 1. Подключение
using TcpClient client = new TcpClient("127.0.0.1", 8000);
using NetworkStream stream = client.GetStream();
Console.WriteLine("[CLIENT] Подключено к серверу.");

// Запускаем чтение ответов в отдельном потоке, чтобы не блокировать отправку
_ = Task.Run(() => ReceiveMessagesAsync(stream));

// Бесконечный цикл, чтобы клиент не закрывался и мог отправлять новые данные
while (true)
{
    Console.WriteLine("Нажмите Enter, чтобы отправить еще один ивент (или 'exit' для выхода)...");
    var input = Console.ReadLine();
    if (input == "exit") break;

    await SendPacketAsync(stream, new Packet { EventName = nameof(SpinRequested), Data = new SpinRequested()});
}

#region Network Logic

// Метод для отправки пакетов (инкапсулируем логику с префиксом длины)
async Task SendPacketAsync(NetworkStream stream, Packet packet)
{
    string json = JsonSerializer.Serialize(packet);
    byte[] messageBytes = Encoding.UTF8.GetBytes(json);
    byte[] lengthPrefix = BitConverter.GetBytes(messageBytes.Length);

    await stream.WriteAsync(lengthPrefix, 0, lengthPrefix.Length);
    await stream.WriteAsync(messageBytes, 0, messageBytes.Length);
    
    Console.WriteLine($"[CLIENT] Отправлено: {packet.EventName} ({messageBytes.Length} байт)");
}

// Метод для постоянного чтения ответов от сервера
async Task ReceiveMessagesAsync(NetworkStream stream)
{
    try
    {
        while (true)
        {
            // 1. Читаем длину (4 байта)
            byte[] lengthBuffer = new byte[4];
            int read = await stream.ReadAsync(lengthBuffer, 0, 4);
            if (read == 0) break; // Сервер закрыл соединение

            int messageLength = BitConverter.ToInt32(lengthBuffer, 0);

            // 2. Читаем тело сообщения
            byte[] messageBytes = new byte[messageLength];
            int totalRead = 0;
            while (totalRead < messageLength)
            {
                int currentRead = await stream.ReadAsync(messageBytes, totalRead, messageLength - totalRead);
                if (currentRead == 0) return;
                totalRead += currentRead;
            }

            // 3. Десериализация
            string json = Encoding.UTF8.GetString(messageBytes);
            var response = JsonSerializer.Deserialize<Packet>(json);
            
            Console.WriteLine($"[CLIENT] Получен ответ от сервера: {json}");
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine($"[CLIENT] Ошибка при чтении: {ex.Message}");
    }
    finally
    {
        Console.WriteLine("[CLIENT] Соединение с сервером потеряно.");
    }
}

#endregion