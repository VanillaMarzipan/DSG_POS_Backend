namespace PointOfSale.Options
{
    public class TransactionOptions
    {
        public TransactionEndpoints Endpoints { get; set; }

        public class TransactionEndpoints
        {
            public string NewTransaction { get; set; }
            public string ActiveTransaction { get; set; }
            public string FinalizeTransaction { get; set; }
        }
    }
}
