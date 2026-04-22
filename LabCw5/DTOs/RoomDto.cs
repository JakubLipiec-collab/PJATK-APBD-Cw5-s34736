namespace LabCw5.DTOs;


public class RoomDto
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string BuildingCode { get; set; }
    public int Floor { get; set; }
    public int Capacity { get; set; }
    public bool hasProjector { get; set; }
    public bool IsActive { get; set; }
}