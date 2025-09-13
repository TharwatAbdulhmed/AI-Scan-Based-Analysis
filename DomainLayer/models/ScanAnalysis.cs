using System.ComponentModel.DataAnnotations;

namespace DomainLayer.models
{
    public class ScanAnalysis
    {
        [Key]
        public int Id { get; set; }

        public string ImagePath { get; set; } // Optional
        public AnalysisType ModelType { get; set; }
        public string Result { get; set; } // Optional
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

        // Navigation property
        public AnalysisResult analysisResult { get; set; }
    }

}
