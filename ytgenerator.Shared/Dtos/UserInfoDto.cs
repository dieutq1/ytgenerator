
using ytgenerator.Shared;

namespace ytgenerator.Shared.Dtos;
public class UserInfo
{
    public Guid Id { get; set; }
    public string Email { get; set; }
    public SystemRole Role { get; set; }
    public string Name { get; set; }
    public string TokenString { get; set; }
    public bool IsTokenValid { get; set; }
    public string MapifyKey { get; set; }
    public string OpenaiKey { get; set; }
    public string GoogleDriveKey { get; set; }
}