using System.ComponentModel.DataAnnotations;
namespace iMicroFin.Models
{
    public class GroupInstalmentReceiptInfo
    {
        
        public int ReceiptId { get; set; }
        public string MemberCode { get; set; }
        public string MemberName { get; set; }
        public string LoanCode { get; set; }
        public int NoOfInstalments { get; set; }
        public int Ewi { get; set; }
        public int TotalDue { get; set; }
        [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy}")]
        public DateTime NextDueDate { get; set; }
    }
}