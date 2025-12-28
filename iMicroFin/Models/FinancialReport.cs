namespace iMicroFin.Models
{
    public class FinancialReport
    {
        public string FYCode { get; set; }
        public string FYDisplay { get; set; }
        public int TotalLoansCount { get; set; }
        public decimal TotalLoansDisbursed { get; set; }
        public decimal TotalEWIReceived { get; set; }
        public decimal FutureEWIExpected { get; set; }
        public decimal BadLoanPendingAmount { get; set; }
        public decimal BadLoanDiscountProvided { get; set; }
        public decimal PreClosureDiscountProvided { get; set; }
        public decimal ActualInterestIncome { get; set; }
        public decimal AnticipatedInterestIncome { get; set; }
        public decimal TotalInterestIncome { get; set; }
        public decimal NetProfit { get; set; }
    }
}