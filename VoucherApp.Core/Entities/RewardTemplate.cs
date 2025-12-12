namespace VoucherApp.Core.Entities
{
    public enum RewardCategory { DiscountPln, DiscountPercent, Item, Time, Empty }
    public enum BasketType { Weak = 1, Strong = 2 }

    public class RewardTemplate
    {
        public int Id { get; set; }
        public required string Name { get; set; } 
        public BasketType BasketType { get; set; } 
        public RewardCategory Category { get; set; }
        public decimal Value { get; set; } 
        public int BatchQuantity { get; set; }
    }
}