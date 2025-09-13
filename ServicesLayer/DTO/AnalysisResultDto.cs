using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServicesLayer.DTO
{
    public class AnalysisResultDto
    {
        public List<float> Prediction { get; set; }
        public float Confidence { get; set; }
    }
}
