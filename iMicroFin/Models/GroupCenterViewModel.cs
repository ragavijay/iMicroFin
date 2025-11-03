using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace iMicroFin.Models
{
    public class GroupCenterViewModel
    {
        public string CenterCode { get; set; } = string.Empty;
        public int CenterId { get; set; } 
        public String CenterName { get; set; } = string.Empty;
        public int BranchId { get; set; }
    }
}