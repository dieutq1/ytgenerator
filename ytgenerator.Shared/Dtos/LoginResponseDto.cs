namespace ytgenerator.Shared.Dtos
{
    public class LoginResponseDto
    {
        public string Token { get; set; }
        public bool IsSucceeded { get; set; }
        public string Message { get; set; }
        public UserInfo User { get; set; }
    }
}
