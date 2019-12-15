namespace Common.Models
{
    public class Camera
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int roverId { get; set; }
        public string FullName { get; set; }
        public int RoverId { get; set; }
        public Rover Rover { get; set; }
    }
}
