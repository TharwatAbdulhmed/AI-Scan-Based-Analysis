
namespace ServicesLayer.DTO
{
    public class ScanAnalysisDto
    {
        public string ImagePath { get; set; }
        public string ModelType { get; set; }
        public string Result { get; set; }
        public AnalysisResultDto analysisResult { get; set; }
    }

   
}
