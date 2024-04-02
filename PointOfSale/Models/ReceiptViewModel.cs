using Response = DSG.POS.PosTransactionManager.Models.Response;

namespace PointOfSale.Models
{
    public class ReceiptViewModel
    {
        public Response.Transaction Transaction { get; }
        public bool ShowCompleteButton { get; }
        public bool ShowReceiptPrinted { get; }

        public ReceiptViewModel(Response.Transaction transaction, bool showCompleteButton, bool showReceiptPrinted)
        {
            Transaction = transaction;
            ShowCompleteButton = showCompleteButton;
            ShowReceiptPrinted = showReceiptPrinted;
        }
    }
}