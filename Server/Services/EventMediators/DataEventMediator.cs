using Hypercube.Utilities.Dependencies;
using Server.DTO;
using Server.ServiceRealisation;
using Server.Services.DTOHandlers;
using Server.Utility;
using Shared;
using Shared.NetworkEvents.DataModification;

namespace Server.Services.EventMediators;

[Service]
public class DataEventMediator : IInitializable
{
    [Dependency] private readonly EventBus _eventBus = null!;
    [Dependency] private readonly DataEditorService _dataEditorService = null!;

    private bool IsAdmin(Client client)
    {
        return client.UserState.IsAdmin;
    }
    
    public void Init()
    {
        _eventBus.Subscribe<AddSpinItem>(args =>
        {
            if (!IsAdmin(args.GetSender()))
                return;
            
            try
            {
                _dataEditorService.AddSpinItem(args.NewItem);
            }
            catch (Exception e)
            {
                Console.WriteLine($"Client to add spin item failed: {e.Message}");
            }
        });
        
        _eventBus.Subscribe<UpdateSpinItem>(args =>
        {
            if (!IsAdmin(args.GetSender()))
                return;
            
            try
            {
                _dataEditorService.UpdateSpinItem(args.PatchSpinItem);
            }
            catch (Exception e)
            {
                Console.WriteLine($"Client to update spin item failed: {e.Message}");
            }
        });
        
        _eventBus.Subscribe<RemoveSpinItem>(args =>
        {
            if (!IsAdmin(args.GetSender()))
                return;
            
            try
            {
                _dataEditorService.RemoveSpinItem(args.ItemName);
            }
            catch (Exception e)
            {
                Console.WriteLine($"Client to remove spin item failed: {e.Message}");
            }
        });
    }
}