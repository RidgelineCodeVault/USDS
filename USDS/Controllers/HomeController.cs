using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Diagnostics;
using System.Xml;
using USDS.Models;

namespace USDS.Controllers
{
    public class HomeController(ILogger<HomeController> logger) : Controller
    {
        private readonly ILogger<HomeController> _logger = logger;
        private readonly HttpClient _httpClient = new();

        // TODO: to run this use a local URL CHANGE THIS to that URL 
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
            
            XmlDocument doc = new XmlDocument();
            doc.Load(filePath);

            XmlNode titleNode = doc.DocumentElement?.SelectSingleNode("/CFRDOC/FMTR/TITLEPG/TITLENUM");
            model.Title = $@"{titleNode?.InnerText}";

            XmlNode subject = doc.DocumentElement.SelectSingleNode("/CFRDOC/FMTR/TITLEPG/SUBJECT");
            model.Subject = $@"{subject?.InnerText}";

            XmlNode revisedDate = doc.DocumentElement.SelectSingleNode("/CFRDOC/FMTR/TITLEPG/REVISED");
            model.RevisionDate = $@"{revisedDate?.InnerText}";

            XmlNode amDate = doc.DocumentElement.SelectSingleNode("/CFRDOC/AMDDATE");
            model.AmDate = $@"{amDate?.InnerText}";
            
            // Use an XSLT for a visual transform.  More time required... 
            model.XmlData = doc.InnerText;

            var wordCount = 0;
            XmlReader reader = XmlReader.Create(filePath);
            while (reader.Read())
            {
                // Could leverage this to parse document differently
                // Takes time to parse large files
                switch (reader.NodeType)
                {
                    //case XmlNodeType.Element:
                    //    //Console.Write("<{0}>", reader.Name);
                    //    break;
                    case XmlNodeType.Text:
                        var words = reader.Value.Split(" ");
                        wordCount += words.Length;
                        //Console.Write(reader.Value);
                        break;
                    //case XmlNodeType.EndElement:
                    //    //Console.Write("</{0}>", reader.Name);
                    //    break;
                }
            }
            model.WordCount = $@"Word Count: {wordCount}";
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
