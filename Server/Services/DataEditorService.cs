using System.Diagnostics.CodeAnalysis;
using Hypercube.Utilities.Dependencies;
using Server.Events;
using Server.Events.DataEvents;
using Server.ServiceRealisation;
using Server.Utility;
using Shared;

namespace Server.Services;

[Service]
public class DataEditorService
{
    [Dependency] private readonly EventBus _eventBus;
    [Dependency] private readonly DataTrackerService _dataTrackerService;
    
    public void AddSpinItem(SpinItem spinItem)
    {
        var imageBytes = Convert.FromBase64String(spinItem.Sprite);
        var format = SpriteValidator.GetValidSpriteExtension(imageBytes);
        if (format is null)
            throw new Exception("Invalid sprite format");
        
        var fileName = $"Sprites/{spinItem.Name}{format}";
        File.WriteAllBytes(fileName, imageBytes);
        spinItem.Sprite = fileName;
        
        _dataTrackerService.Mutate((data) =>
        {
            data.SpinItems.Add(spinItem);
        });
        
        _eventBus.Publish(new AddedSpinItem { Item = spinItem });
    }

    private bool TryFindSpinItem(string itemName, [MaybeNullWhen(false)] out SpinItem spinItem)
    {
        var items = _dataTrackerService.Read().SpinItems;

        foreach (var item in items)
        {
            if (item.Name != itemName) 
                continue;
            
            spinItem = item;
            return true;
        }

        spinItem = null;
        return false;
    }
    
    public void RemoveSpinItem(string itemName)
    {
        if (!TryFindSpinItem(itemName, out var spinItem))
            throw new Exception("SpinItem not found");
        
        _dataTrackerService.Mutate((data) =>
        {
            data.SpinItems.Remove(spinItem);
        });
        
        _eventBus.Publish(new RemovedSpinItem { ItemName = itemName });
    }
}