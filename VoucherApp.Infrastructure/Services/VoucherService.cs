using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
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
            // Sprawdzamy, czy voucher o danym kodzie już istnieje
            var existingVoucher = await _context.Vouchers.FirstOrDefaultAsync(v => v.Code == code);
            if (existingVoucher != null)
            {
                // Zwracamy null, jeśli kod nie jest unikalny
                return null;
            }

            var voucher = new Voucher
            {
                Code = code,
                Description = description,
                IsUsed = false
            };

            _context.Vouchers.Add(voucher);
            await _context.SaveChangesAsync();
            return voucher;
        }

        public async Task<IEnumerable<Voucher>> GetAllVouchersAsync()
        {
            // Pobieramy wszystkie vouchery, sortując od najnowszych
            return await _context.Vouchers.OrderByDescending(v => v.Id).ToListAsync();
        }

        public async Task<Voucher> GetVoucherByCodeAsync(string code)
        {
            return await _context.Vouchers.FirstOrDefaultAsync(v => v.Code == code);
        }

        public async Task UseVoucherAsync(int voucherId)
        {
            var voucher = await _context.Vouchers.FindAsync(voucherId);
            if (voucher != null && !voucher.IsUsed)
            {
                voucher.IsUsed = true;
                await _context.SaveChangesAsync();
            }
        }
    }
}