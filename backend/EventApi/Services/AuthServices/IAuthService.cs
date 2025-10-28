using EventApi.Dtos.AuthDtos;

namespace EventApi.Services.AuthServices
{
    public interface IAuthService
    {
        Task<RegisterResponseDto> RegisterAsync(RegisterDto dto);
        Task<LoginResponseDto> LoginAsync(LoginDto dto);
    }
}