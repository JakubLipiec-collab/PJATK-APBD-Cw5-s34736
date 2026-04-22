using Microsoft.AspNetCore.Mvc;
using LabCw5.Models;
using LabCw5.DTOs;

namespace LabCw5.Controllers;

[ApiController]
[Route("api/[controller]")]
public class RoomsController : ControllerBase
{
    private static List<Room> _rooms =
    [
        new Room
        {
            Id = 1,
            Name = "Lab 104",
            BuildingCode = "B",
            Floor = 1,
            Capacity = 24,
            hasProjector = true,
            IsActive = true,
        },
        new Room
        {
            Id = 2,
            Name = "Lab 111",
            BuildingCode = "B",
            Floor = 1,
            Capacity = 12,
            hasProjector = false,
            IsActive = true,
        },
        new Room
        {
            Id = 3,
            Name = "Lab 207",
            BuildingCode = "A",
            Floor = 2,
            Capacity = 16,
            hasProjector = true,
            IsActive = true,
        }
    ];
    
    [HttpGet]
    public IActionResult GetAll([FromQuery] int? minCapacity, [FromQuery] bool? hasProjector, [FromQuery] bool activeOnly = false)
    {
        if (minCapacity is not null || hasProjector is not null || activeOnly != false)
        {
            var query = _rooms.AsQueryable();
            if (minCapacity is not null)
            {
                query = query.Where(e => e.Capacity >= minCapacity.Value);
            }

            if (hasProjector is not null)
            {
                query = query.Where(e => e.hasProjector == hasProjector.Value);
            }

            if (activeOnly)
            {
                query = query.Where(e => e.IsActive);
            }
            var rooms = query.Select(e=> new RoomDto{
                Id = e.Id,
                Name = e.Name,
                BuildingCode = e.BuildingCode,
                Floor = e.Floor,
                Capacity = e.Capacity,
                hasProjector = e.hasProjector,
                IsActive = e.IsActive
            }).ToList();
            return Ok(rooms);
        }
        var roomsDto = new List<RoomDto>();
        foreach (var room in _rooms)
        {
            roomsDto.Add(new RoomDto
                {
                    Id = room.Id,
                    Name = room.Name,
                    BuildingCode = room.BuildingCode,
                    Floor = room.Floor,
                    Capacity = room.Capacity,
                    hasProjector = room.hasProjector,
                    IsActive = room.IsActive
                }
            );
        }
        return Ok(roomsDto);
    }

    [HttpGet("{id:int}")]
    public IActionResult GetById(int id)
    {
        var room = _rooms.FirstOrDefault(x => x.Id == id);
        if (room is null)
        {
            return NotFound($"Room with id {id} not found");
        }
        return Ok(new RoomDto
        {
            Id = room.Id,
            Name = room.Name,
            BuildingCode = room.BuildingCode,
            Floor = room.Floor,
            Capacity = room.Capacity,
            hasProjector = room.hasProjector,
            IsActive = room.IsActive
        });
    }

    [HttpGet("building/{buildingCode}")]
    public IActionResult GetByBuildingCode(string buildingCode)
    {
        var room = _rooms.Where(x => x.BuildingCode == buildingCode).ToList();
        if (room.Count == 0)
        {
            return NotFound($"Rooms in building with code {buildingCode} not found");
        }
        var roomsDto = room.Select(item => new RoomDto
            {
                Id = item.Id,
                Name = item.Name,
                BuildingCode = item.BuildingCode,
                Floor = item.Floor,
                Capacity = item.Capacity,
                hasProjector = item.hasProjector,
                IsActive = item.IsActive
            }).ToList();
        return Ok(roomsDto);
    }

    // [HttpGet]
    // public IActionResult GetFiltered([FromQuery] int? minCapacity, [FromQuery] bool? hasProjector, [FromQuery] bool activeOnly = false)
    // {
    //     var rooms = _rooms.Where(e =>
    //         e.Capacity > minCapacity &&
    //         e.hasProjector == hasProjector &&
    //         (!activeOnly || e.IsActive)
    //     ).Select(e => new RoomDto
    //     {
    //         Id = e.Id,
    //         Name = e.Name,
    //         BuildingCode = e.BuildingCode,
    //         Floor = e.Floor,
    //         Capacity = e.Capacity,
    //         hasProjector = e.hasProjector,
    //         IsActive = e.IsActive
    //     }).ToList();
    //     return Ok(rooms);
    // }

    [HttpPost]
    public IActionResult Post(CreateRoomDto roomDto)
    {
        var newRoom = new Room
        {
            Id = _rooms.Count > 0 ? _rooms.Last().Id + 1:1,
            Name = roomDto.Name,
            BuildingCode = roomDto.BuildingCode,
            Floor = roomDto.Floor,
            Capacity = roomDto.Capacity,
            hasProjector = roomDto.hasProjector,
            IsActive = roomDto.IsActive
        };
        _rooms.Add(newRoom);
        return CreatedAtAction(nameof(GetById), new { id = newRoom.Id }, newRoom);
    }

    [HttpPut("{id:int}")]
    public IActionResult Update(int id, UpdateRoomDto roomDto)
    {
        var updatedRoom = _rooms.FirstOrDefault(x => x.Id == id);
        if (updatedRoom is null)
        {
            return NotFound($"Room with id  {id} not found");
        }

        updatedRoom.Name = roomDto.Name;
        updatedRoom.BuildingCode = roomDto.BuildingCode;
        updatedRoom.Floor = roomDto.Floor;
        updatedRoom.Capacity = roomDto.Capacity;
        updatedRoom.hasProjector = roomDto.hasProjector;
        updatedRoom.IsActive = roomDto.IsActive;
        
        return NoContent();
    }

    [HttpDelete("{id:int}")]
    public IActionResult Delete(int id)
    {
        var updatedRoom = _rooms.FirstOrDefault(x => x.Id == id);
        if (updatedRoom is null)
        {
            return NotFound($"Room with id  {id} not found");
        }
        _rooms.Remove(updatedRoom);
        return NoContent();
    }
}