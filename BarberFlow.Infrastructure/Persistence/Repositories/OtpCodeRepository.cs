using BarberFlow.Domain.Entities;
using BarberFlow.Domain.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;

namespace BarberFlow.Infrastructure.Persistence.Repositories
{
    public class OtpCodeRepository : IOtpCodeRepository
    {
        private readonly BarberFlowDbContext _context;

        public OtpCodeRepository(BarberFlowDbContext context)
        {
            _context = context;
        }

        public void Add(OtpCode otp)
        {
            _context.OtpCodes.Add(otp);
        }

        public async Task<OtpCode?> GetByIdAsync(Guid otpId, CancellationToken cancellationToken)
        {
            return await _context.OtpCodes
                            .FirstOrDefaultAsync(
                                x => x.Id == otpId,
                                cancellationToken);
        }

        public async Task<OtpCode?> GetLastValidOtpAsync(Guid userId, CancellationToken cancellationToken)
        {
            return await _context.OtpCodes
                            .Where(x => x.UserId == userId && !x.IsUsed)
                            .OrderByDescending(x => x.CreatedAt)
                            .FirstOrDefaultAsync(cancellationToken);
        }

        public async Task InvalidateAllAsync(Guid userId, CancellationToken cancellationToken)
        {
            //await _context.OtpCodes
            //                .Where(x => x.UserId == userId && !x.IsUsed)
            //                .ExecuteUpdateAsync(setter => setter.SetProperty(o => o.IsUsed, true));

            var otpCodes = await _context.OtpCodes
                                  .Where(x => x.UserId == userId && !x.IsUsed)
                                  .ToListAsync(cancellationToken);

            foreach (var optCode in otpCodes)
            {
                optCode.MarkAsUsed();
            }
        }
    }
}
