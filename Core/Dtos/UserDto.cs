namespace Core.Dtos;

public class UserDto : BaseUserDto
{ 
    public required PermissionType Permission  { get; set; }
}