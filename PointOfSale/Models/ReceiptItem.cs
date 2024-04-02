namespace PointOfSale.Models
{
    public class ReceiptItem
    {
        public int TransactionItemIdentifier { get; set; }
        public string Description { get; set; }
        public string UPC { get; set; }
        public decimal Price { get; set; }
        public int Quantity { get; set; }
        public bool SelectedItem { get; set; }
    }
}
