namespace KasperskyNew.Models
{
    public class ReviewRule
    {
        public string RuleName { get; set; }
        public List<string> IncludedPaths { get; set; } = new List<string>();
        public List<string> ExcludedPaths { get; set; } = new List<string>();
        public List<string> Reviewers { get; set; } = new List<string>();
    }
}
