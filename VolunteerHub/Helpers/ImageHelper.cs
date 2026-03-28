using System;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI.WebControls;

namespace VolunteerHub.Helpers
{
    public static class ImageHelper
    {
        private const long MaxBytes = 2 * 1024 * 1024; // 2 MB

        // Magic bytes for supported image formats
        private static readonly byte[][] _signatures = {
            new byte[] { 0xFF, 0xD8, 0xFF },          // JPEG
            new byte[] { 0x89, 0x50, 0x4E, 0x47 },    // PNG
            new byte[] { 0x47, 0x49, 0x46 }            // GIF
        };

        private static readonly string[] _extensions = { ".jpg", ".jpeg", ".png", ".gif" };

        /// <summary>
        /// Overload for pages that use the FileUpload Web control.
        /// Delegates to SaveUpload after extracting the underlying HttpPostedFile.
        /// </summary>
        public static string ValidateAndSave(FileUpload fu, HttpServerUtility server, string subfolder = "ProfileImages")
        {
            if (fu == null || !fu.HasFile) return null;
            return SaveUpload(fu.PostedFile, subfolder, server);
        }

        /// <summary>
        /// Validates and saves an uploaded image from an HttpPostedFile.
        /// Returns the virtual path (e.g. ~/Uploads/ProfileImages/abc123.jpg) or null if no file.
        /// Throws InvalidOperationException if the file fails size, extension, or magic-byte validation.
        /// Falls back to HttpContext.Current.Server when server is not supplied.
        /// </summary>
        public static string SaveUpload(HttpPostedFile file, string subfolder, HttpServerUtility server = null)
        {
            if (file == null || file.ContentLength == 0) return null;
            if (server == null) server = HttpContext.Current.Server;

            if (file.ContentLength > MaxBytes)
                throw new InvalidOperationException("Image must be smaller than 2 MB.");

            string ext = Path.GetExtension(file.FileName).ToLower();
            if (!_extensions.Contains(ext))
                throw new InvalidOperationException("Only JPG, PNG, and GIF images are allowed.");

            // Magic-byte check: read the first 8 bytes to verify the actual binary format.
            // File extension alone can be faked; magic bytes are embedded in the binary and harder to spoof.
            byte[] header = new byte[8];
            using (var s = file.InputStream)
            {
                s.Read(header, 0, 8);
                s.Seek(0, SeekOrigin.Begin); // reset stream so SaveAs() can re-read from the beginning
            }
            bool valid = _signatures.Any(sig => sig.SequenceEqual(header.Take(sig.Length).ToArray()));
            if (!valid)
                throw new InvalidOperationException("Invalid image format.");

            string folder = server.MapPath($"~/Uploads/{subfolder}/");
            if (!Directory.Exists(folder)) Directory.CreateDirectory(folder);

            // Random GUID filename: prevents collisions and keeps the original filename
            // (which may contain personal info or path traversal characters) off disk.
            string fileName = Guid.NewGuid().ToString("N") + ext;
            file.SaveAs(Path.Combine(folder, fileName));

            return $"~/Uploads/{subfolder}/{fileName}";
        }
    }
}
