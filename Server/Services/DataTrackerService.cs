using System.Text.Json;
using Hypercube.Utilities.Dependencies;
using Server.ServiceRealisation;
using Shared;

namespace Server.Services;

[Service]
public sealed class DataTrackerService
{
    [Dependency] private readonly EventBus _eventBus = null!;
    
    private string _attachedFileName = string.Empty;
    private Data? _data;

    private void PushChangesInFile()
    {
        if (_attachedFileName == string.Empty)
            return;

        var source = JsonSerializer.Serialize(_data!, new JsonSerializerOptions() { WriteIndented = true });
        File.WriteAllText(_attachedFileName, source);
    }

    public Data Read()
    {
        if (_data == null)
            throw new Exception("No data loaded");
        
        return _data;
    }
    
    public void LoadFromFile(string filename)
    {
        var source = File.ReadAllText(filename);
        _data = JsonSerializer.Deserialize<Data>(source);
    }

    public void AttachToFile(string filename)
    {
        _attachedFileName = filename;
    }

    public void Mutate(Action<Data> action)
    {
        if (_data == null)
            throw new Exception("No data loaded");
        
        action(_data);
        PushChangesInFile();
    } 
}