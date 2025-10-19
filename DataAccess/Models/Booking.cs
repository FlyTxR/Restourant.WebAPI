using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Identity.Client;

namespace DataAccess.Models
{
    internal class Booking
    {
        public int Id { get; set; }
        public string CustomerName { get; set; }
        public string CustomerEmail { get; set; }
        public string CustomerPhone { get; set; }

        public DateTime Date { get; set; }

        public string Time {  get; set; }

        public int Status { get; set; }
        public int NumberOfGuests { get; set; }
    }
}
