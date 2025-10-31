using System;
using System.Collections.Generic;

namespace ASI.Basecode.Services.ServiceModels
{
    public class MemberReportModel
    {
        public int TotalMembers { get; set; }
        public int TotalAdmins { get; set; }
        public int ActiveBorrowers { get; set; } // Members with active borrowings
        public DateTime ReportGeneratedDate { get; set; }

        public List<UserModel> Members { get; set; }

        public MemberReportModel()
        {
            Members = new List<UserModel>();
            ReportGeneratedDate = DateTime.Now;
        }
    }
}