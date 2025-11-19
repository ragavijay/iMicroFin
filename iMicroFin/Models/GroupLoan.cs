namespace iMicroFin.Models
{
    public class GroupLoan
    {
        public int GroupId { get; set; }
        public string GroupCode { get; set; }
        public string GroupName { get; set; }
        public string CenterName { get; set; }
        public string LeaderName { get; set; }
        public int BranchId{ get; set; }
        public string LoanPurpose{ get; set; }
        public int LoanAmount { get; set; }
        public float ProcessingFeeRate{ get; set; }
        public int ProcessingFee{ get; set; }
        public float InsuranceRate{ get; set; }
        public int Insurance{ get; set; }
        public int Tenure{ get; set; }
        public float InterestRate{ get; set; }
        public float RepaymentAmount { get; set; }
        public int Ewi{ get; set; }
        public string LoanStatus{ get; set; }
        public DateTime LoanDate{ get; set; }
        public DateTime LoanDisposalDate { get; set; }
        public string StatusRemarks { get; set; }
    }
}