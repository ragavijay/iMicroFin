namespace iMicroFin.Models
{
    public class EWIDue
    {
        public string LoanCode { get; set; }
        public int BranchId { get; set; }
        public string MemberCode { get; set; }
        public string MemberName{ get; set; }
        public string Phone { get; set; }
        public int NoOfInstalments{ get; set; }
        public int Ewi{ get; set; }
        public int PendingAmount { get; set; }
        public DateTime DueDate { get; set; }
    }
}