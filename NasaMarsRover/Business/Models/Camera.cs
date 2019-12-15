namespace Business.Models
{
    public class Camera
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string FullName { get; set; }
        public int RoverId { get; set; }
        public Rover Rover { get; set; }
    }
}
