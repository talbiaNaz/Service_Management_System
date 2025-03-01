namespace SMS_Project.DTOs
{
    public class BookingDTO
    {
        public int Id { get; set; }
        public string UserName { get; set; }  // User's name, just an example
        public string ServiceName { get; set; }  // Service's name, just an example
        public DateTime BookingDate { get; set; }
        public string Status { get; set; }
    }

}
