namespace PointOfSale.Options
{
    public class TransactionManagerOptions
    {
        public string BaseUrl { get; set; }

        public TransactionOptions TransactionOptions { get; set; }
        public ItemOptions ItemOptions { get; set; }
        public TenderOptions TenderOptions { get; set; }
    }
}
