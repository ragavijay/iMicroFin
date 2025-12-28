using System.ComponentModel;

namespace iMicroFin.Models
{
    public class CumulativeReport
    {
        [DisplayName("Group Code")]
        public string GroupCode { get; set; }

        [DisplayName("Group Name")]
        public string GroupName { get; set; }

        [DisplayName("Leader")]
        public string LeaderName { get; set; }

        [DisplayName("Due Day")]
        public string EwiDay { get; set; }

        [DisplayName("Total Members")]
        public int TotalMembers { get; set; }

        [DisplayName("Avg Loan Amount")]
        public int AverageLoanAmount { get; set; }

        [DisplayName("Tenure (Weeks)")]
        public int Tenure { get; set; }

        [DisplayName("Weeks Completed")]
        public int WeeksCompleted { get; set; }

        [DisplayName("Avg EWI Received")]
        public int AverageEWIReceived { get; set; }

        [DisplayName("Avg EWI/Member")]
        public int Ewi { get; set; }

        [DisplayName("Total EWI Receivable")]
        public int TotalEwi { get; set; }

        [DisplayName("Total EWI Received")]
        public int TotalEwiReceived { get; set; }

        // Calculated properties
        [DisplayName("Total EWI Pending")]
        public int TotalEwiPending { get; set; }

        [DisplayName("Total Weekly EWI")]
        public int TotalEwiPerWeek { get; set; }

        [DisplayName("Weeks Outstanding")]
        public int WeeksOutstanding { get; set; }

        [DisplayName("Collection %")]
        public decimal CollectionPercentage { get; set; }

        public void Setup()
        {
            // Calculate pending amount
            TotalEwiPending = TotalEwi - TotalEwiReceived;

            // Calculate total weekly EWI (EWI per member * total members)
            TotalEwiPerWeek = Ewi * TotalMembers;

            // Calculate weeks outstanding (remaining weeks)
            WeeksOutstanding = Tenure - WeeksCompleted;

            // Calculate collection percentage
            CollectionPercentage = TotalEwi > 0
                ? (decimal)TotalEwiReceived / TotalEwi * 100
                : 0;
        }
    }
}