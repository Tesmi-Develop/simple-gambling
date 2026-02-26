namespace Server.Utility;

public static class SpriteValidator
{
    public static string? GetValidSpriteExtension(byte[] data)
    {
        if (data.Length < 12) 
            return null;
        
        if (data[0] == 0x89 && data[1] == 0x50 && data[2] == 0x4E && data[3] == 0x47)
        {
            return ".png";
        }
        
        if (data[0] == 0xFF && data[1] == 0xD8 && data[2] == 0xFF)
        {
            return ".jpg";
        }
        
        if (data[0] == 0x52 && data[1] == 0x49 && data[2] == 0x46 && data[3] == 0x46 && // RIFF
            data[8] == 0x57 && data[9] == 0x45 && data[10] == 0x42 && data[11] == 0x50)   // WEBP
        {
            return ".webp";
        }
        
        return null;
    }
}