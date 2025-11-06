using Microsoft.AspNetCore.Mvc;
using System.Security.Cryptography;

namespace USDS.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DataController : ControllerBase
    {
        // GET: api/Data - default
        [HttpGet]
        public Dictionary<string,string> Get()
        {
            string folderPath = @$"{Directory.GetCurrentDirectory()}\Data\CFR-2025\"; 
            string[] folders = Directory.GetDirectories(folderPath);
            var directories = Directory.EnumerateDirectories(folderPath);
            
            var result = new Dictionary<string,string>();
            
            foreach (string directory in directories)
            {
                var path = new DirectoryInfo(directory);
                var name = path.Name;
                result.Add(path.ToString(), name);
            }
            return result;
        }
        
        // GET api/Data/{id}
        [HttpGet("{id}")]
        public Dictionary<string, string> Get(string id)
        {
            var folderName = @$"title-{id}";
            string folderPath = @$"{Directory.GetCurrentDirectory()}\Data\CFR-2025\{folderName}"; 
            string[] files = Directory.GetFiles(folderPath);
            
            var result = new Dictionary<string, string>();

            foreach (string file in files)
            {
                var path = new FileInfo(file);
                var name = path.Name;
                result.Add(path.ToString(), name);
            }
            return result;
        }
        
        [HttpGet("{subFolder}/{fileName}")]
        public string Get(string subFolder, string fileName)
        {
            string filePath = @$"{Directory.GetCurrentDirectory()}\Data\CFR-2025\{subFolder}\{fileName}";
            
            var fileData = new FileInfo(filePath);

            var sha256Checksum = GetSha256Checksum(filePath); // add privileges
            //Console.WriteLine($"SHA256: {sha256Checksum}");// Log Value

            var md5Checksum = GetMd5Checksum(filePath);
            //Console.WriteLine($"MD5: {md5Checksum}");// Log Value
            
            if (!fileData.Exists) return md5Checksum;

            string contents = System.IO.File.ReadAllText(filePath);

            return filePath;
        }
        
        // POST api/<ValuesController>
        [HttpPost]
        public void Post([FromBody] string value, string id, string fileName)
        {
            
        }

        // PUT api/<ValuesController>/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
            
        }

        // DELETE api/<ValuesController>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
           
        }


        private static string GetMd5Checksum(string file)
        {
            using FileStream stream = System.IO.File.OpenRead(file);
            MD5 sha = MD5.Create();
            byte[] checksum = sha.ComputeHash(stream);
            return BitConverter.ToString(checksum).Replace("-", String.Empty);
        }

        private static string GetSha256Checksum(string filePath)
        {
            using var sha256 = SHA256.Create();
            using var stream = System.IO.File.OpenRead(filePath);
            var hash = sha256.ComputeHash(stream);
            return BitConverter.ToString(hash).Replace("-", "").ToLowerInvariant();
        }
    }
}
