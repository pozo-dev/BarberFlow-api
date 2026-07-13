using BarberFlow.Domain.Entities;

namespace BarberFlow.Domain.Interfaces.Repositories
{
    public interface IOtpCodeRepository
    {
        void Add(OtpCode otp);
        Task<OtpCode?> GetByIdAsync(Guid otpId, CancellationToken cancellationToken);
        Task<OtpCode?> GetLastValidOtpAsync(Guid userId, CancellationToken cancellationToken);
        Task InvalidateAllAsync(Guid userId, CancellationToken cancellationToken);
    }
}
