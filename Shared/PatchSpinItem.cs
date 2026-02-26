using System;
using System.IO;

namespace Shared
{
    [Serializable]
    public class PatchSpinItem
    {
        public string OriginalName { get; set; } = null!;
        
        public string? DisplayName { get; set; } = null;
        [PathCustomHandler(nameof(UpdateSprite))]
        public string? Sprite { get; set; } = null;
        public int? Weight { get; set; } = null;

        public string UpdateSprite(string newSprite, string oldSprite)
        {
            var imageBytes = Convert.FromBase64String(newSprite);
            var format = SpriteValidator.GetValidSpriteExtension(imageBytes);
            if (format is null)
                throw new Exception("Invalid sprite format");
            
            var fileName = $"Sprites/{OriginalName}{format}";
            File.WriteAllBytes(fileName, imageBytes);

            return fileName;
        }
    }
}