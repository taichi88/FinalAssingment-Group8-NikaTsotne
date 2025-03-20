using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankingSystem.Application.DTO.Response.ReportResponses
{
    public class UserStatisticsResponse
    {
        public string ReportType { get; set; } = "user_statistics";
        public int ThisYear { get; set; }
        public int LastYear { get; set; }
        public int Last30Days { get; set; }
    }
}