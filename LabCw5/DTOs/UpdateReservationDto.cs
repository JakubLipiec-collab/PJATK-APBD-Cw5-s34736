using System.ComponentModel.DataAnnotations;

namespace LabCw5.DTOs;

public class UpdateReservationDto
{
    [Required]
    public int RoomId { get; set; }
    [Required]
    public string OrganizerName { get; set; } = string.Empty;
    [Required]
    public string Topic { get; set; } = string.Empty;
    public DateOnly Date { get; set; }
    public TimeOnly StartTime { get; set; }
    public TimeOnly EndTime { get; set; }
    [RegularExpression("planned|confirmed|cancelled")]
    public string Status { get; set; } = string.Empty;
}