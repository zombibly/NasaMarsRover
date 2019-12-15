using Newtonsoft.Json;
using System;

namespace Business.Models
{
    public class Photo
    {
        public int Id { get; set; }
        public int Sol { get; set; }
        [JsonProperty(PropertyName="img_src")]
        public string ImgSrc { get; set; }
        [JsonProperty(PropertyName="earth_date")]
        public DateTime EarthDate { get; set; }
        public int CameraId { get; set; }
        public Camera Camera { get; set; }
    }
}
