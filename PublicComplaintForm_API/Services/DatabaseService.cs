using PublicComplaintForm_API.Models;
using Microsoft.Data.SqlClient;
using System.Linq;
using Dapper;
using Microsoft.SqlServer.Server;
using log4net;

namespace PublicComplaintForm_API.Services
{
    public class DatabaseService
    {
        private readonly string _connectionString = string.Empty;
        private readonly string _surveyConnectionString = string.Empty;

        private readonly ILog _logger;

        public DatabaseService(ILog logger)
        {
            _logger = logger;
        }

        public DatabaseService(string connectionString, string surveyConnectionString, ILog logger)
        {
            _connectionString = connectionString ?? string.Empty;
            _surveyConnectionString = surveyConnectionString ?? string.Empty;
            _logger = logger;
        }

        public async Task<Guid> GetCourtId(string courtName)
        {
            return Guid.Empty;
        }

        public async Task<Guid> GetCityId(string cityName)
        {
            return Guid.Empty;
        }

        public async Task<Guid> DoesContactExist(string IdNumber)
        {
            return Guid.Empty;
        }

        public async Task<List<Court>> FetchCourtList()
        {
            return new List<Court>();
        }

        public async Task<Guid> InsertContact(PublicComplaintData formData)
        {
            return Guid.Empty;
        }

        public async Task<bool> InsertComplaint(
            PublicComplaintData formData,
            Guid contactId,
            Guid inquiryId,
            bool receivedFiles)
        {
            return false;
        }

        public async Task<bool> CanSubmitSurvey(SurveyData surveyData)
        {
            return false;
        }

        public async Task SubmitSurvey(SurveyData surveyData)
        {
            return;
        }

        public async Task SubmitForm(
            PublicComplaintData formData,
            List<string> files,
            Guid inquiryId,
            bool receivedFiles)
        {
            return;
        }

        public async Task<List<PublicComplaintData>> GetMonthlyReport(bool useDummyData = true)
        {
            if (useDummyData)
            {
                return new List<PublicComplaintData>
                {
                    new PublicComplaintData { CourtName = "בית משפט השלום ירושלים", ReportYear = 2024, ReportMonth = 1, CurrentMonthTotal = 150, PreviousMonthTotal = 130, SameMonthLastYearTotal = 140 },
                    new PublicComplaintData { CourtName = "בית משפט מחוזי תל אביב", ReportYear = 2024, ReportMonth = 1, CurrentMonthTotal = 85, PreviousMonthTotal = 90, SameMonthLastYearTotal = 70 }
             };
         }

          using (var connection = new SqlConnection(_connectionString))
           {
              string sql = @"
              WITH MonthlyAggregated AS (
                SELECT ct.CourtName, YEAR(c.CreatedDate) AS ReportYear, MONTH(c.CreatedDate) AS ReportMonth, COUNT(c.ComplaintId) AS TotalInquiries
                FROM Complaints c
                JOIN Courts ct ON c.CourtId = ct.CourtId
                GROUP BY ct.CourtName, YEAR(c.CreatedDate), MONTH(c.CreatedDate)
            )
            SELECT CourtName, ReportYear, ReportMonth, TotalInquiries AS CurrentMonthTotal,
            LAG(TotalInquiries, 1) OVER (PARTITION BY CourtName ORDER BY ReportYear, ReportMonth) AS PreviousMonthTotal,
            LAG(TotalInquiries, 12) OVER (PARTITION BY CourtName ORDER BY ReportYear, ReportMonth) AS SameMonthLastYearTotal
            FROM MonthlyAggregated
            ORDER BY ReportYear DESC, ReportMonth DESC, CourtName;";

             var result = await connection.QueryAsync<PublicComplaintData>(sql);
            return result.ToList();
         }
        }


        
    }
}
