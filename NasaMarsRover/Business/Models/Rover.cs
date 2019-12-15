using System;

namespace Common.Models
{
    public class Rover
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public DateTime LandingDate { get; set; }
        public DateTime LaunchDate { get; set; }
        public string Status { get; set; }
        public int MaxSol { get; set; }
        public DateTime MaxDate { get; set; }
        public int TotalPhotos { get; set; }
    }
}
