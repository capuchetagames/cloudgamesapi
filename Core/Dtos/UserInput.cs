namespace Core.Dtos;

public class UserInput : BaseUserDto
{ 
    public required PermissionType Permission  { get; set; }
}