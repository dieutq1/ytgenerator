using RestEase;
using ytgenerator.Shared.Requests;
using ytgenerator.Shared.Dtos;


namespace ytgenerator.Client.DataService
{
    public interface IDataServices
    {
        [Post("api/User/register")]
        Task<CudResponseDto> CreateUserAsync([Body] CreateUserRequest request);

        [Patch("api/User/change-password")]
        Task<CudResponseDto> ChangePasswordAsync([Body] ChangePasswordRequest request);

        [Post("api/User/login")]
        Task<LoginResponseDto> LoginAsync([Body] UserLoginRequest request);

        [Post("/api/Process/get-outlines")]
        Task<List<VideoOutline>> GetOutlines([Body] List<string> youtubeLinks);

        [Post("/api/Process/generate-content")]
        Task<List<GenerateContentDto>> GenerateContent([Body] List<VideoOutline> request);


        [Post("/api/Process/generate-docx")]
        Task<List<GenerateDocxDto>> GenerateContentDocx([Body] List<VideoOutline> request);

        [Post("/api/Process/generate-new-outline")]
        Task<List<VideoOutline>> GenerateNewOutline([Body] List<VideoOutline> request);

        [Post("/api/Process/generate-seo")]
        Task<List<GenerateDocxDto>> GenerateSeo([Body] List<VideoOutline> request);

        [Get("api/User/get-info")]
        Task<UserInfo> GetUserInfo();
    }
}