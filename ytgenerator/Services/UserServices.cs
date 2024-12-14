using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using ytgenerator.Constant;
using ytgenerator.Data;
using ytgenerator.Data.Entities;
using ytgenerator.Shared;
using ytgenerator.Shared.Dtos;
using ytgenerator.Shared.Requests;

namespace ytgenerator.Services
{
    public interface IUserServices
    {
        Task<CudResponseDto> CreateUserAsync(CreateUserRequest request);
        Task<CudResponseDto> ChangePasswordAsync(ChangePasswordRequest request);
        Task<LoginResponseDto> Login(UserLoginRequest request);

        Task<UserInfo> GetUserInfoByToken(string token);
    }

    public class UserServices : IUserServices
    {
        private readonly ApplicationDbContext _context;
        private readonly string _jwtKey;
        private readonly string _jwtIssuer;
        private readonly string _jwtAudience;

        public UserServices(ApplicationDbContext context, IConfiguration config)
        {
            _context = context;
            _jwtKey = config["Jwt:Key"];
            _jwtIssuer = config["Jwt:Issuer"];
            _jwtAudience = config["Jwt:Audience"];
        }

        public async Task<CudResponseDto> ChangePasswordAsync(ChangePasswordRequest request)
        {
            var user = await _context.Users.SingleOrDefaultAsync(u => u.Email == request.Email);
            if (user == null)
            {
                return new CudResponseDto { Message = ResponseMessageConstants.UserNotFound, IsSucceeded = false };
            }

            if (!user.PassWordHash.VerifyPassword(request.OldPassword))
            {
                return new CudResponseDto { Message = ResponseMessageConstants.OldPasswordIsIncorrect, IsSucceeded = false };
            }

            var hashedPassword = request.NewPassword.ToHashPassword();
            user.PassWordHash = hashedPassword;

            await _context.SaveChangesAsync();

            return new CudResponseDto { Message = ResponseMessageConstants.PasswordChangedSuccessfully, IsSucceeded = true };
        }

        public async Task<CudResponseDto> CreateUserAsync(CreateUserRequest request)
        {
            var existingUser = await _context.Users.SingleOrDefaultAsync(u => u.Email == request.Email);
            if (existingUser != null)
            {
                return new CudResponseDto { Message = ResponseMessageConstants.UserAlreadyExists, IsSucceeded = false };
            }

            var hashedPassword = request.Password.ToHashPassword();
            var newUser = new User
            {
                Email = request.Email,
                PassWordHash = hashedPassword,
                Role = request.Role,
                Name = request.Name
            };

            _context.Users.Add(newUser);
            await _context.SaveChangesAsync();

            return new CudResponseDto { Message = ResponseMessageConstants.UserRegisteredSuccessfully, Id = newUser.Id, IsSucceeded = true };
        }

        public async Task<UserInfo> GetUserInfoByToken(string token)
        {
            // decode token
            var tokenHandler = new JwtSecurityTokenHandler();
            var decodedToken = tokenHandler.ReadJwtToken(token);

            var emailClaim = decodedToken.Claims.FirstOrDefault(c => c.Type == "email")?.Value;

            var user = await _context.Users.Include(u => u.AccessTokens)
                                     .Include(x => x.Setting)
                                     .FirstOrDefaultAsync(u => u.Email == emailClaim);

            return new UserInfo
            {
                Email = user.Email,
                Id = user.Id,
                Name = user.Name,
                Role = user.Role,
                OpenaiKey = user.Setting?.OpenApiKey,
            };
        }

        public async Task<LoginResponseDto> Login(UserLoginRequest request)
        {
            var user = await _context.Users.SingleOrDefaultAsync(u => u.Email == request.Email);

            if (user == null)
            {
                return new LoginResponseDto { Message = ResponseMessageConstants.UserNotFound, IsSucceeded = false };
            }

            if (!user.PassWordHash.VerifyPassword(request.Password))
            {
                return new LoginResponseDto { Message = ResponseMessageConstants.PasswordNotCorrect, IsSucceeded = false };
            }

            var token = GenerateJwtToken(user);
            // add token to token table

            _context.AccessTokens.Add(new AccessToken
            {
                Value = token,
                UserId = user.Id
            });

            await _context.SaveChangesAsync();

            return new LoginResponseDto
            {
                Message = ResponseMessageConstants.UserLoggedInSuccessfully,
                IsSucceeded = true,
                Token = token,
                User = new UserInfo
                {
                    Id = user.Id,
                    Email = user.Email,
                    Name = user.Name,
                    Role = user.Role
                }
            };
        }

        private string GenerateJwtToken(User user)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_jwtKey);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(
                [
                    new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                    new Claim(ClaimTypes.Email, user.Email),
                    new Claim(ClaimTypes.Role, user.Role.ToString()),
                    new Claim(ClaimTypes.Name, user.Name),
                ]),
                Expires = DateTime.UtcNow.AddHours(4),
                Issuer = _jwtIssuer,
                Audience = _jwtAudience,
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }


    }
}
