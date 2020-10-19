using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Net;
using System.Net.Http;
using System.IO;
using System.Globalization;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;

namespace TestCoreAngular.Controllers
{
    public class Photo
    {
        public string id {get; set;}
        public string img_src {get; set;}
        public string earth_date {get; set;}        
    }

    [ApiController]
    [Route("[controller]")]
    public class PhotoController : ControllerBase
    {
        private readonly ILogger<PhotoController> _logger;
        public PhotoController(ILogger<PhotoController> logger)
        {
            _logger = logger;
        }

        [HttpGet]
        public IEnumerable<Photo> Get()
        {
            List<Photo> photos = new List<Photo>();
            string filepath = @"wwwroot\files\dates.txt";

            try
            {
                var fileStream = new FileStream(filepath, FileMode.Open, FileAccess.Read);
                using (var streamReader = new StreamReader(fileStream, Encoding.UTF8))
                {
                    string dateString;
                    CultureInfo provider = CultureInfo.InvariantCulture;
                    string[] formats = new string[] { "MM/dd/y", "MM/dd/yy", "MMMM d, yyyy", "MMMM dd, yyyy", "MMM-dd-yyyy", "MM/dd/yyyy", "MM-dd-yyyy", "MM.dd.yyyy" };
                                        
                    while ((dateString = streamReader.ReadLine()) != null)
                    {
                        DateTime dateValue;                    
                                            
                        if (DateTime.TryParseExact(dateString.Trim(), formats, provider, DateTimeStyles.None, out dateValue))
                        {
                            using (var httpClient = new HttpClient())
                            {
                                string url = string.Format("https://api.nasa.gov/mars-photos/api/v1/rovers/curiosity/photos?earth_date={0}&api_key=DEMO_KEY", dateValue.ToString("yyyy-MM-dd"));
                                var response = httpClient.GetStringAsync(new Uri(url)).Result;                            
                                JObject jObject = JObject.Parse(response);                            

                                var colPhoto =  from p in jObject["photos"]
                                                select new Photo 
                                                { 
                                                    id= (string)p["id"], 
                                                    img_src = (string)p["img_src"], 
                                                    earth_date = (string)p["earth_date"]
                                                };
                                if(colPhoto != null)
                                {                                
                                    photos.AddRange(colPhoto.ToList());
                                }                        

                            }

                        }                   
                        
                    }                
                }                
            }
            catch (Exception)
            {   
                //time being if any exception thrown made the list count to zero
                photos = new List<Photo>();
            }
            
            return photos;
        }
    }
}
