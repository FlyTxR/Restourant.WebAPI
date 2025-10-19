using Business.DTO;
using Business.Settings;
using DataAccess;
using DataAccess.Ext;
using DataAccess.Models;
using Microsoft.EntityFrameworkCore;

namespace Business.Services
{
    public class BookingService
    {
        private readonly SqlContext _context;

        private readonly RestaurantSettings _settings;
    

        public BookingService(SqlContext context, RestaurantSettings restaurantSettings)
        {
            _context = context;
            _settings = restaurantSettings;
        }

        public async Task<IEnumerable<BookingDto>> GetAllBookingsAsync(CancellationToken cancellationToken = default)
        {
            var bookings = await _context.Bookings
                .Include(b => b.BookingStatus)
                .OrderByDescending(b => b.CreatedAt)
                .ToListAsync(cancellationToken);

            return bookings.Select(MapToDto);
        }

        public async Task<IEnumerable<BookingDto>> GetBookingsByStatusAsync(int statusId, CancellationToken cancellationToken = default)
        {
            var bookings = await _context.Bookings
                .Include(b => b.BookingStatus)
                .Where(b => b.BookingStatusId == statusId)
                .OrderByDescending(b => b.CreatedAt)
                .ToListAsync(cancellationToken);

            return bookings.Select(MapToDto);
        }

        public async Task<BookingDto?> GetBookingByIdAsync(int id)
        {
            var booking = await _context.Bookings
                .Include(b => b.BookingStatus)
                .FirstOrDefaultAsync(b => b.Id == id);

            return booking != null ? MapToDto(booking) : null;
        }

        public async Task<IEnumerable<TimeSlotDTO>> GetAvailableTimeSlotsAsync(DateTime date, int numberOfGuests, CancellationToken cancellationToken = default)
        {
            var startOfDay = date.Date;
            var endOfDay = startOfDay.AddDays(1);

            var maxCapacity = _settings.GetInt("MaxCapacityPerSlot", 50);
            var lunchTimes = _settings.GetArray("LunchTimes");
            var dinnerTimes = _settings.GetArray("DinnerTimes");
            var allTimes = lunchTimes.Concat(dinnerTimes);

            var existingBookings = await _context.Bookings
                .Where(b => b.BookingDate >= startOfDay && b.BookingDate < endOfDay)
                .Where(b => b.BookingStatusId != 5 && b.BookingStatusId != 6)
                .ToListAsync(cancellationToken);

            var timeSlots = new List<TimeSlotDTO>();

            foreach (var time in allTimes)
            {
                var bookingsAtTime = existingBookings
                    .Where(b => b.BookingTime == time)
                    .ToList();

                var totalGuests = bookingsAtTime.Sum(b => b.NumberOfGuests);
                var spotsLeft = maxCapacity - totalGuests;
                var available = spotsLeft >= numberOfGuests;

                timeSlots.Add(new TimeSlotDTO
                {
                    Time = time,
                    Available = available,
                    SpotsLeft = spotsLeft > 0 ? spotsLeft : 0
                });
            }

            return timeSlots;
        }

        public async Task<IEnumerable<BookingDto>> GetBookingsByDateAsync(DateTime date, CancellationToken cancellationToken = default)
        {
            var startDate = date.Date;
            var endDate = startDate.AddDays(1);

            var bookings = await _context.Bookings
                .Include(b => b.BookingStatus)
                .Where(b => b.BookingDate >= startDate && b.BookingDate < endDate)
                .OrderBy(b => b.BookingDate)
                .ToListAsync(cancellationToken);

            return bookings.Select(MapToDto);
        }

        public async Task<BookingDto> CreateBookingAsync(CreateBookingDTO bookingDto)
        {
            var booking = new Booking
            {
                CustomerName = bookingDto.CustomerName,
                Email = bookingDto.Email,
                PhoneNumber = bookingDto.PhoneNumber,
                BookingDate = bookingDto.BookingDate,
                BookingTime = bookingDto.BookingTime,
                NumberOfGuests = bookingDto.NumberOfGuests,
                SpecialRequests = bookingDto.SpecialRequests,
                BookingStatusId = (int)Enums.BookingStatus.Pending,
                CreatedAt = DateTime.UtcNow,
            };

            _context.Bookings.Add(booking);
            await _context.SaveChangesAsync();

            booking = await _context.Bookings
                .Include(b => b.BookingStatus)
                .FirstAsync(b => b.Id == booking.Id);

            return MapToDto(booking);
        }

        public async Task<bool> UpdateBookingAsync(int id, UpdateBookingDTO bookingDto)
        {
            var booking = await _context.Bookings.FindAsync(id);
            if (booking == null) return false;

            booking.CustomerName = bookingDto.CustomerName;
            booking.Email = bookingDto.Email;
            booking.PhoneNumber = bookingDto.PhoneNumber;
            booking.BookingDate = bookingDto.BookingDate;
            booking.NumberOfGuests = bookingDto.NumberOfGuests;
            booking.SpecialRequests = bookingDto.SpecialRequests;
            booking.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteBookingAsync(int id)
        {
            var booking = await _context.Bookings.FindAsync(id);
            if (booking == null) return false;

            _context.Bookings.Remove(booking);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> UpdateBookingStatusAsync(int id, int statusId)
        {
            var booking = await _context.Bookings.FindAsync(id);
            if (booking == null) return false;

            booking.BookingStatusId = statusId;
            booking.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return true;
        }

        private BookingDto MapToDto(Booking booking)
        {
            return new BookingDto
            {
                Id = booking.Id,
                CustomerName = booking.CustomerName,
                Email = booking.Email,
                PhoneNumber = booking.PhoneNumber,
                BookingDate = booking.BookingDate,
                BookingTime = booking.BookingTime,
                NumberOfGuests = booking.NumberOfGuests,
                SpecialRequests = booking.SpecialRequests,
                BookingStatusId = booking.BookingStatusId,
                BookingStatusName = booking.BookingStatus?.Name,
                CreatedAt = booking.CreatedAt,
                UpdatedAt = booking.UpdatedAt
            };
        }

    }
}
