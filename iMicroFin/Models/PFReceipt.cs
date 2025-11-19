namespace iMicroFin.Models
{
    public class PFReceipt
    {
        public int ReceiptId { get; set; }
        public DateTime ActualReceiptDate { get; set; }
        public string LoanCode { get; set; }
        public String LoanStatus { get; set; }
        public string MemberCode{ get; set; }
        public string MemberName{ get; set; }
        public int ProcessingFee{ get; set; }
        public int Insurance{ get; set; }
        public int TotalFee { get; set; }
        public string UserId { get; set; }
    }
}