namespace iMicroFin.Models
{
    public class CumulativeReport
    {
        public string GroupCode { get; set; }
        public string GroupName { get; set; }
        public string LeaderName{ get; set; }
        public string EwiDay { get; set; }
        public int TotalEwi{ get; set; }
        public int TotalMembers { get; set; }
        public int Ewi{ get; set; }
        public int Tenure { get; set; }
        public int TotalEwiReceived { get; set; }
        public int TotalEwiPending { get; set; }
        public int TotalEwiPerWeek { get; set; }
        public int EwiPaid { get; set; }
        public int EwiPending { get; set; }
        public void Setup()
        {
            TotalEwiPending = TotalEwi - TotalEwiReceived;
            TotalEwiPerWeek = Ewi * TotalMembers;
            EwiPaid = TotalEwiReceived / TotalEwiPerWeek;
            EwiPending = Tenure - EwiPaid;
        }
    }
    
}