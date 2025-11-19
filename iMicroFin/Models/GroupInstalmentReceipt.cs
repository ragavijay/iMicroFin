namespace iMicroFin.Models
{
    public class GroupInstalmentReceipt
    {
        public string GroupCode { get; set; }
        public int ReceiptId { get; set; }
        public DateTime ActualReceiptDate { get; set; }
        public string GroupName { get; set; }
        public string LeaderName { get; set; }
        public int StatusCode { get; set; }
        public int NoOfInstalments{ get; set; }
        public int Ewi{ get; set; }
        public int TotalDue { get; set; }
        public string UserId { get; set; }
       
    }
}