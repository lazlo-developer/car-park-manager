using CarParkManager.Application.Models;
using CarParkManager.Domain;

namespace CarParkManager.Application.Services
{
    public interface IParkingService
    {
        Task<AllocateResponse?> AllocateAsync(AllocateRequest request);
        Task<StatusResponse> GetStatusAsync();
        Task<ExitResponse?> ExitAsync(ExitRequest request);
    }
}
