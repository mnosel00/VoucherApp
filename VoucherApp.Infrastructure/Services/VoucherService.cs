using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VoucherApp.Core.Entities;
using VoucherApp.Core.Interfaces;
using VoucherApp.Infrastructure.Data;

namespace VoucherApp.Infrastructure.Services
{
    public class VoucherService : IVoucherService
    {
        private readonly AppDbContext _context;

        public VoucherService(AppDbContext context)
        {
            _context = context;
        }

        private async Task<string> GenerateUniqueShortCodeAsync()
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            var random = new Random();
            string code;
            do
            {
                code = new string(Enumerable.Repeat(chars, 8)
                    .Select(s => s[random.Next(s.Length)]).ToArray());
            } while (await _context.Vouchers.AnyAsync(v => v.ShortCode == code));

            return code;
        }

        public async Task<Voucher> CreateVoucherAsync(string description)
        {
            var code = await GenerateUniqueShortCodeAsync();

            var rewardTemplate = await _context.RewardTemplates.FirstOrDefaultAsync(rt => rt.Name == description);
            if (rewardTemplate == null)
            {
                rewardTemplate = new RewardTemplate { Name = description };
                _context.RewardTemplates.Add(rewardTemplate);
            }

            var voucher = new Voucher
            {
                ShortCode = code,
                RewardTemplate = rewardTemplate,
                QrCodeContent = Guid.NewGuid(),
                IsRedeemed = false
            };

            _context.Vouchers.Add(voucher);
            await _context.SaveChangesAsync();
            return voucher;
        }

        public async Task<IEnumerable<Voucher>> CreateMultipleVouchersAsync(string description, int count)
        {
            var rewardTemplate = await _context.RewardTemplates.FirstOrDefaultAsync(rt => rt.Name == description);
            if (rewardTemplate == null)
            {
                rewardTemplate = new RewardTemplate { Name = description };
                _context.RewardTemplates.Add(rewardTemplate);
            }

            var newVouchers = new List<Voucher>();
            for (int i = 0; i < count; i++)
            {
                var code = await GenerateUniqueShortCodeAsync();
                var voucher = new Voucher
                {
                    ShortCode = code,
                    RewardTemplate = rewardTemplate,
                    QrCodeContent = Guid.NewGuid(),
                    IsRedeemed = false
                };
                newVouchers.Add(voucher);
            }

            _context.Vouchers.AddRange(newVouchers);
            await _context.SaveChangesAsync();
            return newVouchers;
        }

        public async Task<IEnumerable<Voucher>> GetAllVouchersAsync()
        {
            return await _context.Vouchers.Include(v => v.RewardTemplate).OrderByDescending(v => v.Id).ToListAsync();
        }

        public async Task<Voucher> GetVoucherByCodeAsync(string code)
        {
            return await _context.Vouchers.FirstOrDefaultAsync(v => v.ShortCode == code);
        }

        public async Task<Voucher> GetVoucherWithTemplateByCodeAsync(string code)
        {
            return await _context.Vouchers
                .Include(v => v.RewardTemplate)
                .FirstOrDefaultAsync(v => v.ShortCode == code);
        }

        public async Task UseVoucherAsync(int voucherId)
        {
            var voucher = await _context.Vouchers.FindAsync(voucherId);
            if (voucher != null && !voucher.IsRedeemed)
            {
                voucher.IsRedeemed = true;
                voucher.RedeemedAt = DateTime.UtcNow;
                await _context.SaveChangesAsync();
            }
        }
    }
}