using System.ComponentModel.DataAnnotations;

namespace iMicroFin.Models
{
    public class PreClosureReceipt
    {
        public int ReceiptId { get; set; }
        public DateTime ActualReceiptDate { get; set; }
        public string LoanCode { get; set; }
        public string LoanStatus { get; set; }
        public string MemberCode { get; set; }
        public string MemberName { get; set; }
        public int LoanAmount { get; set; }
        public decimal InterestRate { get; set; }
        public int Tenure { get; set; }
        public DateTime LoanDisposalDate { get; set; }
        public int TotalPendingInstalments { get; set; }
        public int Ewi { get; set; }
        public int TotalDue { get; set; }
        public int SuggestedDiscount { get; set; }
        public int InterestSavings { get; set; }
        public int DelayPenalty { get; set; }
        public int AdvanceBenefit { get; set; }  // NEW FIELD
        public int PreClosureDiscount { get; set; }
        public int NetAmount { get; set; }
        public string UserId { get; set; }
    }
}