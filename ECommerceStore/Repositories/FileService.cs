namespace ECommerceStore.Repositories
{
    public interface IFileService
    {
        Task<string> SaveFile(IFormFile imageFile, string[] allowedExtensions);
        void DeleteFile(string fileName);

    }

    public class FileService : IFileService
    {

        private readonly IWebHostEnvironment _environment;

        public FileService(IWebHostEnvironment environment)
        {
            _environment = environment;
        }
        public async Task<string> SaveFile(IFormFile imageFile, string[] allowedExtensions)
        {
            var contentPath = _environment.WebRootPath;
            var path = Path.Combine(contentPath, "images");

            var ext = Path.GetExtension(imageFile.FileName);

            if (!allowedExtensions.Contains(ext.ToLower()))
            {
                throw new InvalidOperationException("Недопустимый формат файла");
            }

            if (imageFile.Length > 5 * 1024 * 1024)
            {
                throw new InvalidOperationException("Файл слишком большой");
            }

            var fileName = $"{Guid.NewGuid()}{ext}";
            var fileNameWithPath = Path.Combine(path, fileName);

            using var stream = new FileStream(fileNameWithPath, FileMode.Create);
            await imageFile.CopyToAsync(stream);

            return $"/images/{fileName}";
        }


        public void DeleteFile(string fileName)
        {
            if (string.IsNullOrEmpty(fileName)) return;

            var contentPath = _environment.WebRootPath;
            var path = Path.Combine(contentPath, "images", Path.GetFileName(fileName));

            if (File.Exists(path))
            {
                File.Delete(path);
            }

        }

        
    }
}