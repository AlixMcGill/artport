

public class FileStorageService
{
    public FileStorageService()
    {
    }

    private static readonly HashSet<string> AllowedExtensions = new()
    {".jpg", ".jpeg", ".png", ".webp"};

    private static readonly long MaxFileBytes = 10 * 1024 * 1024;

    private void Validate(IFormFile file)
    {
        if (file == null)
            throw new ArgumentException("File is required");
        
        if (file.Length == 0)
            throw new ArgumentException("File is empty");

        if (file.Length > MaxFileBytes)
            throw new ArgumentException("File size is too large.");
        
        var extension = Path.GetExtension(file.FileName)?.ToLowerInvariant();

        if (string.IsNullOrWhiteSpace(extension) || !AllowedExtensions.Contains(extension))
            throw new ArgumentException("Invalid file type.");
        
        if (!file.ContentType.StartsWith("image/"))
            throw new ArgumentException("Invalid content type");
    }

    // Creates a new file
    public async Task<string> Create(IFormFile file)
    {
        // check that file is valid
        Validate(file);

        // check to make sure the folder exsists
        var uploadsFolder = Path.Combine("wwwroot", "uploads");
        if (!Directory.Exists(uploadsFolder)) Directory.CreateDirectory(uploadsFolder);

        var extension = Path.GetExtension(file.FileName);
        var fileName = $"{Guid.NewGuid()}{extension}";
        var localPath = Path.Combine(uploadsFolder, fileName);

        using (var stream =  new FileStream(localPath, FileMode.Create))
        {
            await file.CopyToAsync(stream);
        }

        return $"/uploads/{fileName}";
    }

    // Creates a new file and deletes reference to old file
    public async Task<string> Update(IFormFile file, string deprecatedUrl)
    {
        // check that file is valid
        Validate(file);

        // check to make sure the folder exsists
        var uploadsFolder = Path.Combine("wwwroot", "uploads");
        if (!Directory.Exists(uploadsFolder)) Directory.CreateDirectory(uploadsFolder);

        if (!string.IsNullOrWhiteSpace(deprecatedUrl))
        {
            var staleUrl = deprecatedUrl.TrimStart('/').Replace('/', Path.DirectorySeparatorChar);
            var stalePath = Path.Combine("wwwroot", staleUrl);

            if (System.IO.File.Exists(stalePath))
            {
                try
                {
                    System.IO.File.Delete(stalePath);
                }
                catch
                {
                    // fail silently, we don't want upload to fail if delete can't happen
                    // Figure out how to log this instead
                }
            }
        }

        var extension = Path.GetExtension(file.FileName);
        var fileName = $"{Guid.NewGuid()}{extension}";
        var localPath = Path.Combine(uploadsFolder, fileName);

        using (var stream =  new FileStream(localPath, FileMode.Create))
        {
            await file.CopyToAsync(stream);
        }

        return $"/uploads/{fileName}";

    }
}