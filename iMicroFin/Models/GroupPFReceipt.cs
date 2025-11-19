namespace iMicroFin.Models
{
    public class GroupPFReceipt
    {
        public string GroupCode { get; set; }
        public int ReceiptId { get; set; }
        public DateTime ActualReceiptDate { get; set; }
        public string GroupName{ get; set; }
        public string LeaderName { get; set; }
        public int StatusCode { get; set; }
        public int ProcessingFee{ get; set; }
        public int Insurance{ get; set; }
        public int TotalFee { get; set; }
        public string UserId { get; set; }
    }
}