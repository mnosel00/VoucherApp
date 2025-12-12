using System.Collections.Generic;
using System.Threading.Tasks;
using VoucherApp.Core.Entities;

namespace VoucherApp.Core.Interfaces
{
    public interface IVoucherService
    {
        Task<Voucher> GetVoucherByCodeAsync(string code);
        Task<Voucher> CreateVoucherAsync(string description);
        Task UseVoucherAsync(int voucherId);
        Task<IEnumerable<Voucher>> GetAllVouchersAsync();
    }
}