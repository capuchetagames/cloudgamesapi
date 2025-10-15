namespace Core.Dtos;

public class UserDto
{
    public required string Name  { get; set; }
    public required string Email  { get; set; }
    public required string Password  { get; set; }
    public required string Permission  { get; set; }
}