using System.ComponentModel.DataAnnotations;

namespace LabCw5.DTOs;

public class CreateRoomDto
{
    [Required]
    public string Name { get; set; }
    [Required]
    public string BuildingCode { get; set; }
    public int Floor { get; set; }
    [Range(1, int.MaxValue)]
    public int Capacity { get; set; }
    public bool hasProjector { get; set; }
    public bool IsActive { get; set; }
}