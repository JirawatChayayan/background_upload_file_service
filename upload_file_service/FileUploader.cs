using RestSharp;
using System;
using System.IO;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Windows.Media.Protection.PlayReady;

public class FileUploader
{
    public static async Task UploadFilesAsync2(string serverPath,string[] filePaths)
    {
        var client = new RestClient(serverPath);
        var request = new RestRequest("uploadfiles", Method.Post);

        foreach (var filePath in filePaths)
        {
            request.AddFile("files", filePath);
        }

        try
        {
            // Send the request to the FastAPI server
            var response = await client.ExecuteAsync(request);

            if (response.IsSuccessful)
            {
                Console.WriteLine("Files uploaded successfully!");
                Console.WriteLine($"Server Response: {response.Content}");
            }
            else
            {
                Console.WriteLine($"Failed to upload files. Status Code: {response.StatusCode}");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"An error occurred: {ex.Message}");
        }
    }

    public static async Task UploadFilesAsync(string serverPath, string[] filePaths)
    {
        using var form = new MultipartFormDataContent();
        HttpClient client = new HttpClient();
        foreach (var filePath in filePaths)
        {
            var fileContent = new StreamContent(File.OpenRead(filePath));
            fileContent.Headers.ContentType = MediaTypeHeaderValue.Parse("image/jpeg");

            // Add file content to the form data
            form.Add(fileContent, "files", Path.GetFileName(filePath));
        }

        // Send the HTTP POST request
        using var response = await client.PostAsync(serverPath + "/uploadfile", form);

        // Ensure the request was successful
        response.EnsureSuccessStatusCode();
    }
}
