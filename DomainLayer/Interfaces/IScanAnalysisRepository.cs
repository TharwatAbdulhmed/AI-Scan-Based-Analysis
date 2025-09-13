using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using DomainLayer.models;

namespace DomainLayer.Interfaces
{
    public interface IScanAnalysisRepository
    {
        Task<ScanAnalysis> AddScanAnalysisAsync(ScanAnalysis scanAnalysis);
        Task<ScanAnalysis> GetScanAnalysisByIdAsync(int id);

        Task<IEnumerable<ScanAnalysis>> GetAllScanAnalysesAsync(); // New method to get all records

        Task<bool> DeleteAsync(int id); // New method for deletion
        Task<ScanAnalysis> UpdateAsync(int id, ScanAnalysis updatedScanAnalysis); // New method for update


    }


}
