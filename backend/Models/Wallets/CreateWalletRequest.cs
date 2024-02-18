namespace WebApi.Models.Wallets
{
    public class CreateWalletRequest
    {
        public int UserId { get; set; }
        public decimal Amount { get; set; }
    }
}
