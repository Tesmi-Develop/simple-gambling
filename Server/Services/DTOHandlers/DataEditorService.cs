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
        
        _dataTrackerService.Mutate(data =>
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
        
        _dataTrackerService.Mutate(data =>
        {
            data.SpinItems.Remove(spinItem);
        });
        
        _eventBus.Publish(new RemovedSpinItem { ItemName = itemName });
    }

    public void UpdateSpinItem(PatchSpinItem patchSpinItem)
    {
        if (!TryFindSpinItem(patchSpinItem.OriginalName, out var originalSpinItem))
            throw new Exception("SpinItem not found");
        
        _dataTrackerService.Mutate(_ =>
        {
            var patchType = typeof(PatchSpinItem);
            var targetType = typeof(SpinItem);
    
            foreach (var patchProp in patchType.GetProperties())
            {
                var originalProp = targetType.GetProperty(patchProp.Name);
                if (originalProp is null)
                    continue;
                
                var patchValue = patchProp.GetValue(patchSpinItem);
                var originalValue = originalProp.GetValue(originalSpinItem);
                if (patchValue is null) 
                    continue;
            
                var attr = (PathCustomHandler?)Attribute.GetCustomAttribute(patchProp, typeof(PathCustomHandler));
                if (attr is not null)
                {
                    var method = patchType.GetMethod(attr.MethodName);
                    var newValue = method?.Invoke(patchSpinItem, [patchValue, originalValue]);
            
                    originalProp.SetValue(originalSpinItem, newValue);
                    return;
                }

                originalProp.SetValue(originalSpinItem, patchValue);
            } 
        });
        
        _eventBus.Publish(new UpdatedSpinItem { Item = originalSpinItem });
    }
}