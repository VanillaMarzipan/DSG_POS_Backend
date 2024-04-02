namespace PointOfSale.Options
{
    public class ItemOptions
    {
        public ItemEndpoints Endpoints { get; set; }

        public class ItemEndpoints
        {
            public string NewItem { get; set; }
            public string DeleteItem { get; set; }
        }
    }
}