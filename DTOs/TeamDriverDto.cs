namespace KeepTheApex.DTOs;

public class TeamDriverDto
{
    public string Id { get; set; }
    public string Name { get; set; }
}

public class DriverDto
{
    public string Id { get; set; }
    public string Name { get; set; }
    public string TeamId { get; set; }
}