namespace iMicroFin.Models
{
    public class FinancialYear
    {
        public string FYCode { get; set; }  // e.g., "2023-24"
        public string FYDisplay { get; set; } // e.g., "FY 2023-24"
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
    }
}