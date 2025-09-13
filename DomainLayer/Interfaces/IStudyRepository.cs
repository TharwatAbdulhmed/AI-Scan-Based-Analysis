using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DomainLayer.models;

namespace DomainLayer.Interfaces
{
    public interface IStudyRepository
    {
        Task<IEnumerable<Study>> GetAllAsync();
        Task<Study> GetByIdAsync(int id);
        Task<Study> GetByNameAsync(string diseaseName);
        Task<Study> AddAsync(Study study);
        Task UpdateAsync(Study study);
        Task DeleteAsync(int id);
    }
}
