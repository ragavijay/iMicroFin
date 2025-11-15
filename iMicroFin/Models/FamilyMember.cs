namespace iMicroFin.Models
{
    public class FamilyMember
    {
        public string MemberCode { get; set; } = string.Empty;
        public int SNo { get; set; }
        public string FamilyMemberName { get; set; } = string.Empty;
        public string Relationship { get; set; } = string.Empty;
        public EOccupationType OccupationType { get; set; }
        public int MonthlyIncome { get; set; }  
        public string Qualification { get; set; } = string.Empty;
    }
}