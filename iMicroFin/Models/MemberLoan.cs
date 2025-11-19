namespace iMicroFin.Models
{
    public class MemberLoan
    {
        public Member? member { get; set; }
        public Loan? loan { get; set; }

        public static MemoryStream GetExportTransferReport(List<MemberLoan> memberLoans)
        {
            MemoryStream memory = new MemoryStream();
            StreamWriter stream = new StreamWriter(memory);
            foreach (MemberLoan memberLoan in memberLoans)
            {
                stream.WriteLine(String.Format("{0},{1},{2},{3}",
                    memberLoan.loan?.LoanAmount??0,
                    memberLoan.member?.IFSC??"",
                    memberLoan.member?.AccountNumber,"",
                    memberLoan.member?.MemberName??"")
                );
            }
            stream.Flush();
            memory.Position = 0;
            return memory;
        }
    }
}