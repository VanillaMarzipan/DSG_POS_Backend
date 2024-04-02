namespace PointOfSale.Options
{
    public class ProductLookupOptions
    {
        public ProductLookupEndpoints Endpoints { get; set; }

        public class ProductLookupEndpoints
        {
            public string UpcLookup { get; set; }
        }
    }
}