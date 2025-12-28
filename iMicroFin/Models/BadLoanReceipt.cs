using System.ComponentModel.DataAnnotations;

namespace iMicroFin.Models
{
    public class BadLoanReceipt
    {
        public int ReceiptId { get; set; }
        public DateTime ActualReceiptDate { get; set; }
        public string LoanCode { get; set; }
        public string LoanStatus { get; set; }
        public string MemberCode { get; set; }
        public string MemberName { get; set; }
        public int LoanAmount { get; set; }
        public int TotalDue { get; set; }
        public int AmountPaid { get; set; }
        public int PendingAmount { get; set; }
        public int PaymentAmount { get; set; }
        public int SettlementDiscount { get; set; }
        public int RemainingBalance { get; set; }
        public bool IsFullSettlement { get; set; }
        public string UserId { get; set; }
    }
}