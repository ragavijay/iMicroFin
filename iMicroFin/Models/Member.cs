namespace iMicroFin.Models
{
    public class MemberInfo
    {
        public string MemberCode { get; set; } = string.Empty;
        public string MemberName { get; set; } = string.Empty;
    }
    public class Member
    {
        public string MemberCode { get; set; } = string.Empty;
        public int MemberId { get; set; }
        public string GroupCode { get; set; } = string.Empty;
        public string GroupName { get; set; } = string.Empty;
        public int BranchId { get; set; }
        public string CenterCode { get; set; } = string.Empty;

        public string CenterName { get; set; } = string.Empty;
        public string LeaderName { get; set; } = string.Empty;
        public EMemberType MemberType { get; set; } 
        public string MemberName { get; set; } = string.Empty;
        public EGender Gender { get; set; }
        public DateTime DOB { get; set; }
        public EMaritalStatus MaritalStatus { get; set; }
        public EReligion Religion { get; set; }
        public string FName { get; set; } = string.Empty; // Father's name
        public string HName { get; set; } = string.Empty; // Husband's name
        public EOccupation Occupation { get; set; }
        public EOccupationType OccupationType { get; set; }
        public string AddressLine1 { get; set; } = string.Empty;
        public string AddressLine2 { get; set; } = string.Empty;
        public string AddressLine3 { get; set; } = string.Empty;
        public string AddressLine4 { get; set; } = string.Empty;
        public string Taluk { get; set; } = string.Empty;
        public string Panchayat { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;
        public string Pincode { get; set; } = string.Empty;
        public int NoOfYears { get; set; }
        public EHouseType HouseType { get; set; }
        public EPropertyOwnership PropertyOwnership { get; set; }
        public string Phone { get; set; } = string.Empty;
        public string MemberAadharNumber { get; set; } = string.Empty;
        public string RMemberAadharNumber { get; set; } = string.Empty;
        public string PAN { get; set; } = string.Empty;
        public string RationCardNo { get; set; } = string.Empty;
        public string VoterIDNo { get; set; } = string.Empty;
        public string AccountNumber { get; set; } = string.Empty;
        public string RAccountNumber { get; set; } = string.Empty;
        public string IFSC { get; set; } = string.Empty;
        public string BankCustomerId { get; set; } = string.Empty;
        public string NomineeName { get; set; } = string.Empty;
        public ERelationship Relationship { get; set; }
        public string NomineeAadharNumber { get; set; } = string.Empty;
        public DateTime NomineeDOB { get; set; }
        public IFormFile? Photo { get; set; }
        public IFormFile? Aadhar { get; set; }
        public string CurrentLoanCode { get; set; } = string.Empty;
        public static int GetAge(DateTime dateOfBirth)
        {
            int age = 0;
            age = DateTime.Now.Year - dateOfBirth.Year;
            if (DateTime.Now.DayOfYear < dateOfBirth.DayOfYear)
                age = age - 1;

            return age;
        }
        public int GetMemberAge()
        {
            int age = 0;
            age = DateTime.Now.Year - DOB.Year;
            if (DateTime.Now.DayOfYear < DOB.DayOfYear)
                age = age - 1;

            return age;
        }
        public int GetNomineeAge()
        {
            int age = 0;
            age = DateTime.Now.Year - NomineeDOB.Year;
            if (DateTime.Now.DayOfYear < NomineeDOB.DayOfYear)
                age = age - 1;

            return age;
        }
        public string GetPhotoPath()
        {
            return @"http://fileuploads.amftn.in/Img/Member/" + MemberCode + ".jpg";
        }

        public static MemoryStream GetExportMembers(List<Member> members)
        {
            MemoryStream memory = new MemoryStream();
            StreamWriter stream = new StreamWriter(memory);
            foreach (Member member in members)
            {
                stream.WriteLine(String.Format("{0},{1},{2},{3},{4},{5},{6},{7},{8},{9},{10},{11},{12},{13},{14},{15},{16}",
                    member.MemberCode,
                    member.MemberName,
                    member.GetMemberAge() + "/" + member.DOB.ToString("dd-MM-yyyy"),
                    "M" + (int)member.MaritalStatus + 1,
                    member.Religion,
                    (member.Gender == EGender.Female ? "F" : "T"),
                    member.NomineeName,
                    member.GetNomineeAge() + "/" + member.NomineeDOB.ToString("dd-MM-yyyy"),
                    "K" + (int)member.Relationship+1,
                    member.Phone,
                    member.Aadhar,
                    member.VoterIDNo,
                    member.RationCardNo,
                    member.AddressLine1 + "," + member.AddressLine2 + "," + member.AddressLine3 + "," + member.AddressLine4 + "," + member.City + "," + member.Pincode,
                    member.AccountNumber,
                    member.IFSC,
                    member.Occupation)
                );
            }
            stream.Flush();
            memory.Position = 0;
            return memory;
        }
    }
    public class ViewMembersViewModel
    {
        public string GroupCode { get; set; } = string.Empty;
        public string GroupName { get; set; } = string.Empty;
        public string LeaderName { get; set; } = string.Empty;
        public string CenterCode { get; set; } = string.Empty;
        public string CenterName { get; set; } = string.Empty;
    }
}