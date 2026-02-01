//using esyasoft.mobility.CHRGUP.service.api.DTOs.Reservation;
//using esyasoft.mobility.CHRGUP.service.api.Interfaces;
//using esyasoft.mobility.CHRGUP.service.core.Metadata;
//using esyasoft.mobility.CHRGUP.service.persistence.Data;
//using esyasoft.mobility.CHRGUP.service.core.Models;

//using Microsoft.EntityFrameworkCore;


//namespace esyasoft.mobility.CHRGUP.service.api.Services
//{
//    public class ReservationService : IReservationService
//    {
//        private readonly AppDbContext _db;

//        public ReservationService(AppDbContext db)
//        {
//            _db = db;
//        }

//        public async Task<ReservationResponseDto> CreateAsync(CreateReservationDto dto)
//        {
//            if (!await _db.chargers.AnyAsync(c => c.Id == dto.ChargerId))
//                throw new Exception("Charger not found");

//            if (!await _db.drivers.AnyAsync(d => d.Id == dto.DriverId))
//                throw new Exception("Driver not found");

//            var overlapExists = await _db.Set<Reservation>()
//                .AnyAsync(r =>
//                    r.ChargerId == dto.ChargerId &&
//                    r.ConnectorId == dto.ConnectorId &&
//                    r.Status == ReservationStatus.Active.ToString() &&
//                    dto.StartTime < r.EndTime &&
//                    dto.EndTime > r.StartTime);

//            if (overlapExists)
//                throw new Exception("Reservation already exists for this time slot");

//            var reservation = new Reservation
//            {
//                ChargerId = dto.ChargerId,
//                DriverId = dto.DriverId,
//                ConnectorId = dto.ConnectorId,
//                StartTime = dto.StartTime,
//                EndTime = dto.EndTime,
//                CreatedBy = dto.CreatedBy, 
//                Status = ReservationStatus.Active.ToString()
//            };

//            _db.Add(reservation);
//            await _db.SaveChangesAsync();

//            return Map(reservation);
//        }

//        public async Task<ReservationResponseDto?> CancelAsync(string id, CancelReservationDto dto)
//        {
//            var reservation = await _db.Set<Reservation>().FindAsync(id);
//            if (reservation == null)
//                return null;

//            if (reservation.Status != ReservationStatus.Active.ToString())
//                throw new Exception("Reservation cannot be cancelled");

//            reservation.Status = ReservationStatus.Cancelled.ToString();
//            reservation.CancelledBy = dto.CancelledBy;

//            await _db.SaveChangesAsync();
//            return Map(reservation);
//        }

//        public async Task<List<ReservationResponseDto>> GetAllAsync()
//        {
//            return await _db.Set<Reservation>()
//                .OrderByDescending(r => r.CreatedAt)
//                .Select(r => new ReservationResponseDto
//                {
//                    Id = r.Id,
//                    ChargerId = r.ChargerId,
//                    DriverId = r.DriverId,
//                    ConnectorId = r.ConnectorId,
//                    Status = r.Status,
//                    StartTime = r.StartTime,
//                    EndTime = r.EndTime,
//                    CreatedAt = r.CreatedAt
//                })
//                .ToListAsync();
//        }

//        private static ReservationResponseDto Map(Reservation r)
//        {
//            return new ReservationResponseDto
//            {
//                Id = r.Id,
//                ChargerId = r.ChargerId,
//                DriverId = r.DriverId,
//                ConnectorId = r.ConnectorId,
//                Status = r.Status,
//                StartTime = r.StartTime,
//                EndTime = r.EndTime,
//                CreatedAt = r.CreatedAt
//            };
//        }
//    }
//}

using esyasoft.mobility.CHRGUP.service.api.DTOs.Reservation;
using esyasoft.mobility.CHRGUP.service.api.Interfaces;
using esyasoft.mobility.CHRGUP.service.core.Metadata;
using esyasoft.mobility.CHRGUP.service.persistence.Data;
using esyasoft.mobility.CHRGUP.service.core.Models;
using MongoDB.Driver;

namespace esyasoft.mobility.CHRGUP.service.api.Services
{
    public class ReservationService : IReservationService
    {
        private readonly MongoDbContext _db;

        public ReservationService(MongoDbContext db)
        {
            _db = db;
        }

        public async Task<ReservationResponseDto> CreateAsync(CreateReservationDto dto)
        {
            var chargerExists = await _db.Chargers
                .Find(c => c.Id == dto.ChargerId)
                .AnyAsync();

            if (!chargerExists)
                throw new Exception("Charger not found");

            var driverExists = await _db.Drivers
                .Find(d => d.Id == dto.DriverId)
                .AnyAsync();

            if (!driverExists)
                throw new Exception("Driver not found");

            var overlapExists = await _db.Reservations
                .Find(r =>
                    r.ChargerId == dto.ChargerId &&
                    r.ConnectorId == dto.ConnectorId &&
                    r.Status == ReservationStatus.Active.ToString() &&
                    dto.StartTime < r.EndTime &&
                    dto.EndTime > r.StartTime
                )
                .AnyAsync();

            if (overlapExists)
                throw new Exception("Reservation already exists for this time slot");

            var reservation = new Reservation
            {
                Id = Guid.NewGuid().ToString(),
                ChargerId = dto.ChargerId,
                DriverId = dto.DriverId,
                ConnectorId = dto.ConnectorId,
                StartTime = dto.StartTime,
                EndTime = dto.EndTime,
                CreatedBy = dto.CreatedBy,
                Status = ReservationStatus.Active.ToString(),
                CreatedAt = DateTime.UtcNow
            };

            await _db.Reservations.InsertOneAsync(reservation);

            return Map(reservation);
        }

        public async Task<ReservationResponseDto?> CancelAsync(string id, CancelReservationDto dto)
        {
            var reservation = await _db.Reservations
                .Find(r => r.Id == id)
                .FirstOrDefaultAsync();

            if (reservation == null)
                return null;

            if (reservation.Status != ReservationStatus.Active.ToString())
                throw new Exception("Reservation cannot be cancelled");

            var update = Builders<Reservation>.Update
                .Set(r => r.Status, ReservationStatus.Cancelled.ToString())
                .Set(r => r.CancelledBy, dto.CancelledBy);

            await _db.Reservations.UpdateOneAsync(
                r => r.Id == id,
                update
            );

            reservation.Status = ReservationStatus.Cancelled.ToString();
            reservation.CancelledBy = dto.CancelledBy;

            return Map(reservation);
        }

        public async Task<List<ReservationResponseDto>> GetAllAsync()
        {
            return await _db.Reservations
                .Find(_ => true)
                .SortByDescending(r => r.CreatedAt)
                .Project(r => new ReservationResponseDto
                {
                    Id = r.Id,
                    ChargerId = r.ChargerId,
                    DriverId = r.DriverId,
                    ConnectorId = r.ConnectorId,
                    Status = r.Status,
                    StartTime = r.StartTime,
                    EndTime = r.EndTime,
                    CreatedAt = r.CreatedAt
                })
                .ToListAsync();
        }

        private static ReservationResponseDto Map(Reservation r)
        {
            return new ReservationResponseDto
            {
                Id = r.Id,
                ChargerId = r.ChargerId,
                DriverId = r.DriverId,
                ConnectorId = r.ConnectorId,
                Status = r.Status,
                StartTime = r.StartTime,
                EndTime = r.EndTime,
                CreatedAt = r.CreatedAt
            };
        }
    }
}
