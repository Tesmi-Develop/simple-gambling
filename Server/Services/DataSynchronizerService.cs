using Hypercube.Utilities.Dependencies;
using Server.Events;
using Server.Events.DataEvents;
using Server.ServiceRealisation;
using Server.Utility;
using Shared;
using Shared.Events;
using Shared.Events.DataEvents;

namespace Server.Services;

[Service]
public sealed class DataSynchronizerService : IInitializable
{
    [Dependency] private readonly EventBus _eventBus = null!;
    [Dependency] private readonly NetworkBroadcaster _networkBroadcaster = null!;
    [Dependency] private readonly DataTrackerService _dataTrackerService = null!;

    private static string ConvertImageToBase64(string filePath)
    {
        if (!File.Exists(filePath)) return string.Empty;
        
        var imageArray = File.ReadAllBytes(filePath);
        return Convert.ToBase64String(imageArray);
    }
    
    private Data PrepareForClientData(Data serverData)
    {
        var clientData = ReflectionHelper.Clone(serverData);
        clientData.SpinItems = [];

        foreach (var item in serverData.SpinItems)
        {
            var newItem = ReflectionHelper.Clone(item);
            newItem.Sprite = ConvertImageToBase64(item.Sprite);
            clientData.SpinItems.Add(newItem);
        }

        return clientData;
    }
    
    private SpinItem PrepareForClientSpinItem(SpinItem spinItem)
    {
        var newItem = ReflectionHelper.Clone(spinItem);
        newItem.Sprite = ConvertImageToBase64(spinItem.Sprite);

        return newItem;
    }
    
    public void Init()
    {
        _eventBus.Subscribe<ClientConnected>((args) =>
        {
            _networkBroadcaster.SendEvent(args.Client, new InitialDataSync
            {
                CurrentData = PrepareForClientData(_dataTrackerService.Read())
            });
        });
        
        _eventBus.Subscribe<AddedSpinItem>((args) =>
        {
            _networkBroadcaster.SendEvent(new AddedSpinItemNetwork()
            {
                SpinItem = PrepareForClientSpinItem(args.Item)
            });
        });
        
        _eventBus.Subscribe<RemovedSpinItem>((args) =>
        {
            _networkBroadcaster.SendEvent(new RemovedSpinItemNetwork()
            {
                ItemName = args.ItemName
            });
        });
    }
}