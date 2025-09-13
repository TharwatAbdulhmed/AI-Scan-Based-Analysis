using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DomainLayer.models
{
    #region edit code to decode
    //public class AnalysisResult
    //{
    //    public Guid Id { get; set; } = Guid.NewGuid();
    //    public string Prediction { get; set; }  // Stores array as CSV
    //    public float Confidence { get; set; }
    //} 
    #endregion

    public class AnalysisResult
    {
        [Key]
        public int Id { get; set; }

        public string Prediction { get; set; } // Optional
        public float Confidence { get; set; }  // Required

        // Foreign key for the relationship with ScanAnalysis
        public int ScanAnalysisId { get; set; }
    }

}
