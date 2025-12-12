namespace VoucherApp.Core.Entities
{
    public class Voucher
    {
        public int Id { get; set; }
        public int RewardTemplateId { get; set; }
        public virtual RewardTemplate RewardTemplate { get; set; } = null!;
        public Guid QrCodeContent { get; set; } 
        public required string ShortCode { get; set; } 
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public bool IsRedeemed { get; set; }
        public DateTime? RedeemedAt { get; set; }
    }
}