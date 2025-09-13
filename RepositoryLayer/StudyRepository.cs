using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DomainLayer.Interfaces;
using DomainLayer.models;
using Microsoft.EntityFrameworkCore;
using RepositoryLayer.Data;

namespace RepositoryLayer
{
    public class StudyRepository : IStudyRepository
    {
        private readonly ApplicationDbContext _context;

        public StudyRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Study>> GetAllAsync()
        {
            return await _context.Studies.Include(s => s.ModelType).ToListAsync();
        }

        public async Task<Study> GetByIdAsync(int id)
        {
            return await _context.Studies.Include(s => s.ModelType).FirstOrDefaultAsync(s => s.Id == id);
        }

        public async Task<Study> GetByNameAsync(string diseaseName)
        {
            return await _context.Studies.Include(s => s.ModelType).FirstOrDefaultAsync(s => s.DiseaseName.ToLower() == diseaseName.ToLower());
        }

        public async Task<Study> AddAsync(Study study)
        {
            _context.Studies.Add(study);
            await _context.SaveChangesAsync();
            return study;
        }

        public async Task UpdateAsync(Study study)
        {
            _context.Studies.Update(study);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var study = await _context.Studies.FindAsync(id);
            if (study != null)
            {
                _context.Studies.Remove(study);
                await _context.SaveChangesAsync();
            }
        }
    }
}
