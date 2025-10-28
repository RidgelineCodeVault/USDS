using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Diagnostics;
using System.Net.Http;
using System.Reflection;
using System.Reflection.Metadata;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using System.Xml;
using USDS.Models;

namespace USDS.Controllers
{
    public class HomeController(ILogger<HomeController> logger) : Controller
    {
        private readonly ILogger<HomeController> _logger = logger;
        private readonly HttpClient _httpClient = new();
        private readonly string _url = "http://localhost:5268/";



        public async Task<IActionResult> Index(string? id, string? fileName)
        {
            var model = new IndexViewModel
            {
                Folders = new Dictionary<string, string>(),
                Files = new Dictionary<string, string>()
            };

            var directoryResponse = await _httpClient.GetAsync($"{_url}api/data");

            if (!directoryResponse.IsSuccessStatusCode) return View(model);

            var directories = directoryResponse.Content.ReadAsStringAsync().Result;
            model.Folders = JsonConvert.DeserializeObject<Dictionary<string, string>>(directories);

            if (id != null)
            {
                var i = id.LastIndexOf('-');
                var folderId = id.Substring(i + 1);

                var filesResponse = await _httpClient.GetAsync($@"{_url}api/data/{folderId}");
                var files = filesResponse.Content.ReadAsStringAsync().Result;
                model.Files = JsonConvert.DeserializeObject<Dictionary<string, string>>(files);
            }

            if (fileName != null)
            {
                var fileDataResponse = await _httpClient.GetAsync($@"{_url}api/data/{id}/{fileName}");
                var filePath = fileDataResponse.Content.ReadAsStringAsync().Result;

                ParseXmlFile(filePath, ref model);
            }

            return View(model);
        }

        public void ParseXmlFile(string filePath, ref IndexViewModel model)
        {
            string contents = System.IO.File.ReadAllText(filePath);
            // Fill model values
            var wordCount = 0;

            XmlReader reader = XmlReader.Create(filePath);

            while (reader.Read())
            {
                switch (reader.NodeType)
                {
                    case XmlNodeType.Element:
                        if (reader.Name.ToString() == "REVISED")
                        {
                            model.RevisionDate = reader.Value;
                            break;
                        }


                        //Console.Write("<{0}>", reader.Name);
                        break;
                    case XmlNodeType.Text:
                        var words = reader.Value.Split(" ");
                        wordCount += words.Length;

                        if (reader.Name.ToString() == "SUBJECT" && !string.IsNullOrEmpty(reader.Value))
                        {
                            model.Subject = reader.Value;
                            break;
                        }
                        //Console.Write(reader.Value);
                        break;
                    case XmlNodeType.EndElement:
                        //Console.Write("</{0}>", reader.Name);
                        break;
                }
            }

            model.WordCount = wordCount;
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }



    }
}
