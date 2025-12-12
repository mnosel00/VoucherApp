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

        public async Task<Voucher> CreateVoucherAsync(string code, string description)
        {
            var existingVoucher = await _context.Vouchers.FirstOrDefaultAsync(v => v.ShortCode == code);
            if (existingVoucher != null)
            {
                return null;
            }

            // Zakładam, że istnieje domyślny RewardTemplate. W przyszłości można to rozbudować.
            var defaultRewardTemplate = await _context.RewardTemplates.FirstOrDefaultAsync();
            if (defaultRewardTemplate == null)
            {
                // Jeśli nie ma szablonu, nie można utworzyć vouchera.
                // Można tu rzucić wyjątek lub zwrócić null.
                return null;
            }

            var voucher = new Voucher
            {
                ShortCode = code,
                RewardTemplateId = defaultRewardTemplate.Id,
                QrCodeContent = Guid.NewGuid(), // Generujemy unikalny identyfikator dla kodu QR
                IsRedeemed = false,
                CreatedAt = DateTime.UtcNow
            };

            _context.Vouchers.Add(voucher);
            await _context.SaveChangesAsync();
            return voucher;
        }

        public async Task<IEnumerable<Voucher>> GetAllVouchersAsync()
        {
            // Dołączamy RewardTemplate, aby mieć dostęp do opisu
            return await _context.Vouchers
                .Include(v => v.RewardTemplate)
                .OrderByDescending(v => v.Id)
                .ToListAsync();
        }

        public async Task<Voucher> GetVoucherByCodeAsync(string code)
        {
            return await _context.Vouchers
                .Include(v => v.RewardTemplate)
                .FirstOrDefaultAsync(v => v.ShortCode == code);
        }

        public async Task UseVoucherAsync(int voucherId)
        {
            var voucher = await _context.Vouchers.FindAsync(voucherId);
            // Używamy 'IsRedeemed' zamiast 'IsUsed'
            if (voucher != null && !voucher.IsRedeemed)
            {
                voucher.IsRedeemed = true;
                voucher.RedeemedAt = DateTime.UtcNow; // Ustawiamy datę wykorzystania
                await _context.SaveChangesAsync();
            }
        }
    }
}