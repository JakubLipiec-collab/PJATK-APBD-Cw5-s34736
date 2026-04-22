using System.ComponentModel.DataAnnotations;

namespace LabCw5.DTOs;

public class UpdateRoomDto
{
    [MaxLength(10), Required]
    public string Name { get; set; }
    [MaxLength(1), Required]
    public string BuildingCode { get; set; }
    [Required]
    public int Floor { get; set; }
    [Required]
    public int Capacity { get; set; }
    [Required]
    public bool hasProjector { get; set; }
    [Required]
    public bool IsActive { get; set; }
}