using System.ComponentModel.DataAnnotations;

namespace MicroFin.Models
{
    public class CashReceiptStatement
    {

        public int SNo { get; set; }
        public int ReceiptId { get; set; }
        public string MemberCode { get; set; }
        public string MemberName { get; set; }
        public string LoanCode { get; set; }
        public string Description { get; set; }
        public int Amount { get; set; }
        [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy}")]
        public DateTime ActualReceiptDate { get; set; }
    }
}