namespace iMicroFin.Models
{
    public class MemberGroup
    {
        public string GroupCode { get; set; } = string.Empty;
        public int GroupId { get; set; }
        public string GroupName { get; set; } = string.Empty;
        public string CenterCode { get; set; } = string.Empty;
        public string CenterName { get; set; } = string.Empty;
        public string LeaderName { get; set; } = string.Empty;
        public bool isLoanRunning { get; set; } 
      
    }
}