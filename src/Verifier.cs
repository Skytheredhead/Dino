using System;
using System.Net.Http;
using System.Threading.Tasks;
using HtmlAgilityPack;
using System.IO;

namespace CodeFetchCSharp
{
    class Program
    {
        static readonly HttpClient client = new HttpClient();
        
        static async Task Main(string[] args)
        {
            string websiteUrl = "https://justice151.github.io/OTK-DyKnow-Bypass/";
            var codes = await FetchCodesFromWebsite(websiteUrl);

            while (true)
            {
                Console.WriteLine("Enter the code: ");
                string userCode = Console.ReadLine();

                if (!string.IsNullOrEmpty(userCode) && ValidateCode(userCode, codes))
                {
                    Console.WriteLine("Valid code!");
                    await DownloadBlockerExe("https://github.com/Justice151/OTK-DyKnow-Bypass/raw/main/Blocker.exe");
                    break; // Exit the loop if the code is valid
                }
                else
                {
                    Console.WriteLine("Invalid code. Please try again.");
                }
            }
        }

        static async Task<string[]> FetchCodesFromWebsite(string url)
        {
            var response = await client.GetAsync(url);
            response.EnsureSuccessStatusCode();
            var htmlContent = await response.Content.ReadAsStringAsync();

            HtmlDocument doc = new HtmlDocument();
            doc.LoadHtml(htmlContent);

            var codeElement = doc.DocumentNode.SelectSingleNode("//p");
            if (codeElement != null)
            {
                string codesText = codeElement.InnerText.Trim();
                return codesText.Split();
            }

            return new string[] { };
        }

        static bool ValidateCode(string code, string[] validCodes)
        {
            foreach (var validCode in validCodes)
            {
                if (code == validCode)
                {
                    return true;
                }
            }

            return false;
        }

        static async Task DownloadBlockerExe(string fileUrl)
        {
            string fileName = "blocker.exe";
            string filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, fileName);

            var response = await client.GetAsync(fileUrl);
            response.EnsureSuccessStatusCode();

            using (var fs = new FileStream(filePath, FileMode.Create))
            {
                await response.Content.CopyToAsync(fs);
                Console.WriteLine("Dino Downloaded!"); // Confirmation message
            }
        }
    }
}
