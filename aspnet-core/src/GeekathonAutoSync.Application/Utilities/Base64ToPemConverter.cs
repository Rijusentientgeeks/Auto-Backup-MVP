using Abp.UI;
using System;
using System.IO;
using System.Threading.Tasks;

namespace GeekathonAutoSync.Utilities
{
    public class Base64ToPemConverter
    {
        // Method to convert Base64 string to .pem file
        public async Task<string> ConvertBase64ToPemFile(string base64String, int tenantId)
        {
            try
            {
                // Get the path to the wwwroot directory
                var wwwRootPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");

                // Combine the wwwroot path with the folder name
                var folderPath = Path.Combine(wwwRootPath, "PrivateKey");

                // Create the folder if it does not exist
                if (!Directory.Exists(folderPath))
                {
                    Directory.CreateDirectory(folderPath);
                }
                var subfolderPath = Path.Combine(folderPath, tenantId.ToString());
                if (!Directory.Exists(subfolderPath))
                {
                    Directory.CreateDirectory(subfolderPath);
                }

                // Step 1: Decode the Base64 string to byte array
                byte[] fileBytes = Convert.FromBase64String(base64String);
                var fileNameWithFolderPath = Path.Combine(subfolderPath, Guid.NewGuid().ToString() + ".pem");
                // Step 2: Write the byte array to a file
                File.WriteAllBytes(fileNameWithFolderPath, fileBytes);
                return fileNameWithFolderPath;
            }
            catch (Exception ex)
            {
                throw new UserFriendlyException("An error occurred: " + ex.Message);
            }
        }
    }
}
