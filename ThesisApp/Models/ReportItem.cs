namespace ThesisApp.Models
{
    public class ReportItem
    {
        public long ThesisId { get; set; }
        public long CuratorId { get; set; }
        public string CuratorFirstname { get; set; }
        public string CuratorLastname { get; set; }
        public List<ReportStudent> Students { get; set; }
        public string ThesisTitleKg { get; set; }
        public string ThesisTitleTr { get; set; }
    }
}
