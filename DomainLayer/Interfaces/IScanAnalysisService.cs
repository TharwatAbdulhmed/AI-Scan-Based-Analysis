using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DomainLayer.models;
using Microsoft.AspNetCore.Http;

namespace DomainLayer.Interfaces
{

    public interface IScanAnalysisService
    {
        Task<ScanAnalysis> AnalyzeImageAsync(IFormFile file, string modelName);
        Task<ScanAnalysis> GetAnalysisResultAsync(int id);
        Task<IEnumerable<ScanAnalysis>> GetAllAsync(); // New method to get all records

        Task<bool> DeleteAsync(int id); // New method for deletion
        Task<ScanAnalysis> UpdateAsync(int id, ScanAnalysis updatedScanAnalysis); // New method for update



    }
}
