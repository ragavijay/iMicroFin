namespace iMicroFin.Models
{
    public class GroupPFReceiptInfo
    {
        
        public int ReceiptId { get; set; }
        public string MemberCode { get; set; }
        public string MemberName { get; set; }
        public string LoanCode { get; set; }
        public int ProcessingFee{ get; set; }
        public int Insurance{ get; set; }
        public int TotalFee { get; set; }
        
    }
}