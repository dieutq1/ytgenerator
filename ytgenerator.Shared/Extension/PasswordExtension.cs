
namespace ytgenerator.Shared
{
    public static class PasswordExtension
    {
        public static string ToHashPassword(this string password)
        {
            return BCrypt.Net.BCrypt.HashPassword(password);
        }

        public static bool VerifyPassword(this string passwordHash, string password)
        {
            return BCrypt.Net.BCrypt.Verify(password, passwordHash);
        }
    }
}
