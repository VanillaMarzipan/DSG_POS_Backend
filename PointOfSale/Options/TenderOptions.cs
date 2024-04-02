namespace PointOfSale.Options
{
    public class TenderOptions
    {
        public TenderEndpoints Endpoints { get; set; }

        public class TenderEndpoints
        {
            public string NewTender { get; set; }
        }
    }
}