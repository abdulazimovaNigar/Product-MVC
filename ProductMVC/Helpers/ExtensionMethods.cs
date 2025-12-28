namespace ProductMVC.Helpers;

public static class ExtensionMethods
{
    public static bool CheckType(this IFormFile file, string type)
    {
        return file.ContentType.Contains(type);
    }

    public static bool CheckSize(this IFormFile file, int size)
    {
        return file.Length < size * 1024 * 1024;
    }

    public static string SaveFile(this IFormFile file, string path)
    {
        string uniqueName = Guid.NewGuid().ToString() + file.FileName;
       
        string mainImagePath = Path.Combine(path, uniqueName);

        using FileStream mainImageStream = new FileStream(mainImagePath, FileMode.Create);

        file.CopyTo(mainImageStream);
        return uniqueName;
    }
}
