namespace iMicroFin.Models
{
    public class LoanRepaymentStatus
    {
        public string GroupCode { get; set; }
        public string GroupName { get; set; }
        public string LeaderName { get; set; }
        public int LoanAmount { get; set; }
        public DateTime LoanDate { get; set; }
        public DateTime StartingDate { get; set; }
        public DateTime EndingDate { get; set; }
        public int Tenure { get; set; }
        public int EWI { get; set; }
        public string EWIs { get; set; }
        public string CollectionDay { get; set; }
        public int MemberCount { get; set; }
        public string[] MemberName { get; set; }
        public string[] MemberCode { get; set; }
        public string[] LoanCode { get; set; }
        public int[] MemberEWI { get; set; }
        public DateTime[] ActualDate { get; set; }
        public int[,] Amount { get; set; }
        public int[] ColTotal { get; set; }
        public int[] RowTotal { get; set; }
        public int[] TotalAmount { get; set; }
        public int[] PendingAmount { get; set; }
        public int OverallTotalAmount { get; set; }
        public int OverallRecdAmount { get; set; }
        public int OverallPendingAmount { get; set; }
    }
}