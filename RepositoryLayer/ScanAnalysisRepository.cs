using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DomainLayer.models;
using Microsoft.EntityFrameworkCore;
using RepositoryLayer.Data;
using Microsoft.Extensions.Logging;
using DomainLayer.Interfaces;

namespace RepositoryLayer
{
    public class ScanAnalysisRepository : IScanAnalysisRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<ScanAnalysisRepository> _logger;

        public ScanAnalysisRepository(ApplicationDbContext context, ILogger<ScanAnalysisRepository> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<ScanAnalysis> AddScanAnalysisAsync(ScanAnalysis scanAnalysis)
        {
            try
            {
                if (scanAnalysis == null)
                {
                    throw new ArgumentNullException(nameof(scanAnalysis), "ScanAnalysis cannot be null.");
                }

                // Add the entity to the database
                await _context.ScanAnalyses.AddAsync(scanAnalysis);
                await _context.SaveChangesAsync();

                // Log success
                _logger.LogInformation($"ScanAnalysis added successfully with ID: {scanAnalysis.Id}");
                return scanAnalysis;
            }
            catch (DbUpdateException ex)
            {
                // Log database update errors
                _logger.LogError($"DbUpdateException while adding ScanAnalysis: {ex.InnerException?.Message}");
                throw; // Rethrow the exception after logging
            }
            catch (Exception ex)
            {
                // Log general exceptions
                _logger.LogError($"General Exception while adding ScanAnalysis: {ex.Message}");
                throw;
            }
        }

        public async Task<IEnumerable<ScanAnalysis>> GetAllScanAnalysesAsync()
        {
            try
            {
                // Retrieve all scan analyses with their related AnalysisResult
                return await _context.ScanAnalyses
                    .Include(sa => sa.analysisResult) // Eagerly load related AnalysisResult
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                // Log errors
                _logger.LogError($"Error retrieving all ScanAnalyses: {ex.Message}");
                throw;
            }
        }

        public async Task<ScanAnalysis> GetScanAnalysisByIdAsync(int id)
        {
            try
            {
                if (id <= 0)
                {
                    throw new ArgumentException("Invalid ID provided.", nameof(id));
                }

                // Retrieve the scan analysis by ID, including its related AnalysisResult
                return await _context.ScanAnalyses
                    .Include(sa => sa.analysisResult) // Eagerly load related AnalysisResult
                    .FirstOrDefaultAsync(sa => sa.Id == id);
            }
            catch (Exception ex)
            {
                // Log errors
                _logger.LogError($"Error retrieving ScanAnalysis by ID ({id}): {ex.Message}");
                throw;
            }
        }

        public async Task<bool> DeleteAsync(int id)
        {
            try
            {
                var scanAnalysis = await _context.ScanAnalyses.FindAsync(id);
                if (scanAnalysis == null)
                {
                    _logger.LogWarning($"ScanAnalysis with ID {id} not found.");
                    return false; // Record not found
                }

                _context.ScanAnalyses.Remove(scanAnalysis);
                await _context.SaveChangesAsync();

                _logger.LogInformation($"ScanAnalysis with ID {id} deleted successfully.");
                return true; // Deletion successful
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error deleting ScanAnalysis with ID {id}: {ex.Message}");
                throw;
            }
        }

        public async Task<ScanAnalysis> UpdateAsync(int id, ScanAnalysis updatedScanAnalysis)
        {
            try
            {
                var existingScanAnalysis = await _context.ScanAnalyses
                    .Include(sa => sa.analysisResult) // Include related AnalysisResult
                    .FirstOrDefaultAsync(sa => sa.Id == id);

                if (existingScanAnalysis == null)
                {
                    _logger.LogWarning($"ScanAnalysis with ID {id} not found.");
                    throw new KeyNotFoundException($"ScanAnalysis with ID {id} not found.");
                }

                // Update the existing record's properties
                existingScanAnalysis.ImagePath = updatedScanAnalysis.ImagePath ?? existingScanAnalysis.ImagePath;
                existingScanAnalysis.ModelType = updatedScanAnalysis.ModelType;
                existingScanAnalysis.Result = updatedScanAnalysis.Result ?? existingScanAnalysis.Result;

                // Update the analysis result if provided
                if (updatedScanAnalysis.analysisResult != null)
                {
                    existingScanAnalysis.analysisResult.Prediction = updatedScanAnalysis.analysisResult.Prediction;
                    existingScanAnalysis.analysisResult.Confidence = updatedScanAnalysis.analysisResult.Confidence;
                }

                await _context.SaveChangesAsync();

                _logger.LogInformation($"ScanAnalysis with ID {id} updated successfully.");
                return existingScanAnalysis; // Return the updated record
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error updating ScanAnalysis with ID {id}: {ex.Message}");
                throw;
            }
        }
    }
}