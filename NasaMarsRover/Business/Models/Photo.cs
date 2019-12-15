using System;

namespace Common.Models
{
    public class Photo
    {
        public int Id { get; set; }
        public int Sol { get; set; }
        public string ImgSrc { get; set; }
        public DateTime EarthDate { get; set; }
        public int CameraId { get; set; }
        public Camera Camera { get; set; }
    }
}
