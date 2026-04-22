using Microsoft.AspNetCore.Mvc;
using LabCw5.Models;
using LabCw5.DTOs;

namespace LabCw5.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ReservationController : ControllerBase
{
    public static List<Reservation> Reservations =
    [
        new Reservation
        {
            Id = 1,
            RoomId = 1,
            OrganizerName = "Jan Kowalski",
            Topic = "NET Core Basics",
            Date = new DateOnly(2026, 5, 10), 
            StartTime = new TimeOnly(8, 0), 
            EndTime = new TimeOnly(10, 0), 
            Status = "confirmed"
        },
        new Reservation { Id = 2, 
            RoomId = 1, 
            OrganizerName = "Anna Nowak", 
            Topic = "Advanced SQL",
            Date = new DateOnly(2026, 5, 10), 
            StartTime = new TimeOnly(11, 0), 
            EndTime = new TimeOnly(13, 0), 
            Status = "planned" },
        new Reservation { Id = 3, 
            RoomId = 2, 
            OrganizerName = "Marek Sowa", 
            Topic = "Scrum Meeting", 
            Date = new DateOnly(2026, 5, 11), 
            StartTime = new TimeOnly(9, 0), 
            EndTime = new TimeOnly(10, 0),
            Status = "confirmed" },
        new Reservation { Id = 4, 
            RoomId = 3, 
            OrganizerName = "Ewa Bąk", 
            Topic = "Recruitment", 
            Date = new DateOnly(2026, 5, 12), 
            StartTime = new TimeOnly(14, 0), 
            EndTime = new TimeOnly(15, 30), 
            Status = "planned" }
    ];
    
    [HttpGet]
    public IActionResult GetAll([FromQuery] DateOnly? date, [FromQuery] string? status, [FromQuery] int? roomId)
    {
        var query = Reservations.AsQueryable();
        if (date.HasValue)
        {
            query = query.Where(r => r.Date == date.Value);
        }
        if (!string.IsNullOrWhiteSpace(status))
        {
            query = query.Where(r => r.Status == status);
        }
        if (roomId.HasValue)
        {
            query = query.Where(r => r.RoomId == roomId.Value);
        }
        var reservations = query.Select(e => new ReservationDto
        {
            Id = e.Id,
            RoomId = e.RoomId,
            OrganizerName = e.OrganizerName,
            Topic = e.Topic,
            Date = e.Date,
            StartTime = e.StartTime,
            EndTime = e.EndTime,
            Status = e.Status
        }).ToList();

        return Ok(reservations);
    }
    
    [HttpGet("{id:int}")]
    public IActionResult GetById(int id)
    {
        var reservation = Reservations.FirstOrDefault(x => x.Id == id);
        if (reservation == null)
        {
            return NotFound($"Reservation {id} not found");
        }

        return Ok(new ReservationDto
        {
            Id = reservation.Id,
            RoomId = reservation.RoomId,
            OrganizerName = reservation.OrganizerName,
            Topic = reservation.Topic,
            Date = reservation.Date,
            StartTime = reservation.StartTime,
            EndTime = reservation.EndTime,
            Status = reservation.Status
        });
    }
    
    [HttpPost]
    public IActionResult Post(CreateReservationDto reservationDto)
    {
        if (reservationDto.EndTime <= reservationDto.StartTime)
            return BadRequest("EndTime must be later than StartTime.");
        
        var room = RoomsController.Rooms.FirstOrDefault(e => e.Id == reservationDto.RoomId);
        if (room == null) return NotFound("Room does not exist.");
        if (!room.IsActive) return BadRequest("Room is inactive.");
        
        var conflict = Reservations.Any(e => 
            e.RoomId == reservationDto.RoomId && 
            e.Date == reservationDto.Date && 
            e.Status != "cancelled" &&
            e.EndTime > reservationDto.StartTime &&
            e.StartTime < reservationDto.EndTime);

        if (conflict) return Conflict("Conflict");

        var newReservation = new Reservation
        {
            Id = Reservations.Count >0 ? Reservations.Max(e => e.Id) + 1 : 1,
            RoomId = reservationDto.RoomId,
            OrganizerName = reservationDto.OrganizerName,
            Topic = reservationDto.Topic,
            Date = reservationDto.Date,
            StartTime = reservationDto.StartTime,
            EndTime = reservationDto.EndTime,
            Status = "planned"
        };

        Reservations.Add(newReservation);
        
        var responseDto = new ReservationDto 
        { 
            Id = newReservation.Id, 
            RoomId = newReservation.RoomId, 
            OrganizerName = newReservation.OrganizerName,
            Topic = newReservation.Topic,
            Date = newReservation.Date,
            StartTime = newReservation.StartTime,
            EndTime = newReservation.EndTime,
            Status = newReservation.Status
        };

        return CreatedAtAction(nameof(GetById), new { id = newReservation.Id }, responseDto);
    }

    [HttpPut("{id:int}")]
    public IActionResult Update(int id, UpdateReservationDto reservationDto)
    {
        var reservation = Reservations.FirstOrDefault(r => r.Id == id);
        if (reservation == null)
        {
            return NotFound();
        }

        if (reservationDto.EndTime <= reservationDto.StartTime)
        {
            return BadRequest("EndTime must be later than StartTime.");
        }
        
        var conflict = Reservations.Any(e => 
            e.Id != id &&
            e.RoomId == reservationDto.RoomId && 
            e.Date == reservationDto.Date && 
            reservationDto.Status != "cancelled" &&
            reservationDto.StartTime < e.EndTime && reservationDto.EndTime > e.StartTime);

        if (conflict) return Conflict("Conflict.");
        
        reservation.RoomId = reservationDto.RoomId;
        reservation.OrganizerName = reservationDto.OrganizerName;
        reservation.Topic = reservationDto.Topic;
        reservation.Date = reservationDto.Date;
        reservation.StartTime = reservationDto.StartTime;
        reservation.EndTime = reservationDto.EndTime;
        reservation.Status = reservationDto.Status;

        return NoContent();
    }

    [HttpDelete("{id:int}")]
    public IActionResult Delete(int id)
    {
        var reservation = Reservations.FirstOrDefault(e => e.Id == id);
        if (reservation == null)
        {
            return NotFound();
        }
        Reservations.Remove(reservation);
        return NoContent();
    }
}