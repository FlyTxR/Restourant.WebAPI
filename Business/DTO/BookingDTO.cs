using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.DTO
{
    public class BookingDto
    {
        public int Id { get; set; }
        public string CustomerName { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public DateTime BookingDate { get; set; }
        public string BookingTime { get; set; }
        public int NumberOfGuests { get; set; }
        public string SpecialRequests { get; set; }
        public int BookingStatusId { get; set; }
        public string BookingStatusName { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }


    public class CreateBookingDTO
    {
        public string CustomerName { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public DateTime BookingDate { get; set; }
        public string BookingTime { get; set; }
        public int NumberOfGuests { get; set; }
        public string SpecialRequests { get; set; }
    }

    public class UpdateBookingDTO
    {
        public string CustomerName { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public DateTime BookingDate { get; set; }
        public string BookingTime { get; set; }
        public int NumberOfGuests { get; set; }
        public string SpecialRequests { get; set; }
    }

    public class TimeSlotDTO
    {
        public string Time { get; set; }
        public bool Available { get; set; }
        public int SpotsLeft { get; set; }
    }


}
