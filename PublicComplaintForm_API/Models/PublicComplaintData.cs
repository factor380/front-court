namespace PublicComplaintForm_API.Models
{
    public class PublicComplaintData
    {
        public string CourtName { get; set; } = string.Empty;
        public int ReportYear { get; set; }
        public int ReportMonth { get; set; }
        public int CurrentMonthTotal { get; set; }
        public int? PreviousMonthTotal { get; set; }
        public int? SameMonthLastYearTotal { get; set; }
    }
}