using DomainLayer.Interfaces;
using DomainLayer.models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Talabat.API.Errors;

namespace AI_Scan_Analyses_Test.Controllers
{
    /// <summary>
    /// Controller for handling scan analysis operations.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class ScanController : ControllerBase
    {
        private readonly IScanAnalysisService _scanAnalysisService;
        private readonly ILogger<ScanController> _logger;

        public ScanController(IScanAnalysisService scanAnalysisService, ILogger<ScanController> logger)
        {
            _scanAnalysisService = scanAnalysisService;
            _logger = logger;
        }

        /// <summary>
        /// Analyzes an uploaded image using the specified model.
        /// </summary>
        /// <param name="file">The image file to analyze.</param>
        /// <param name="modelName">The name of the model to use for analysis.</param>
        /// <returns>The analysis result.</returns>
        [HttpPost("analyze")]
        public async Task<IActionResult> Analyze(IFormFile file, [FromForm] string modelName)
        {
            if (file == null || file.Length == 0)
                return BadRequest(new { Message = "No file uploaded." });

            if (!Enum.TryParse(modelName, true, out AnalysisType modelType))
                return BadRequest(new { Message = "Invalid model name." });

            try
            {
                var result = await _scanAnalysisService.AnalyzeImageAsync(file, modelName);
                return Ok(new { Message = "Analysis successful.", Data = result });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error during image analysis: {ex.Message}");
                return StatusCode(500, new { Message = "An error occurred during image analysis.", Error = ex.Message });
            }
        }

        /// <summary>
        /// Retrieves the analysis result for a specific scan by ID.
        /// </summary>
        /// <param name="id">The ID of the scan analysis.</param>
        /// <returns>The analysis result.</returns>
        //[Authorize]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetAnalysisResult(int id)
        {
            try
            {
                var result = await _scanAnalysisService.GetAnalysisResultAsync(id);
                return result != null ? Ok(new { Message = "Analysis retrieved successfully.", Data = result }) : NotFound(new ApiResponse(404));
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error retrieving analysis result: {ex.Message}");
                return StatusCode(500, new { Message = "An error occurred while retrieving the analysis result.", Error = ex.Message });
            }
        }

        /// <summary>
        /// Retrieves all scan analyses with optional pagination.
        /// </summary>
        /// <param name="page">The page number (default: 1).</param>
        /// <param name="pageSize">The number of records per page (default: 10).</param>
        /// <returns>A paginated list of scan analyses.</returns>
        //[Authorize(Policy = "AdminOnly")]
        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            try
            {
                var records = await _scanAnalysisService.GetAllAsync();
                var paginatedRecords = records.Skip((page - 1) * pageSize).Take(pageSize);

                return Ok(new
                {
                    Message = "Scan analyses retrieved successfully.",
                    TotalRecords = records.Count(),
                    Page = page,
                    PageSize = pageSize,
                    Records = paginatedRecords
                });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error retrieving all scan analyses: {ex.Message}");
                return StatusCode(500, new { Message = "An error occurred while retrieving scan analyses.", Error = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAnalysis(int id)
        {
            try
            {
                var isDeleted = await _scanAnalysisService.DeleteAsync(id);
                if (!isDeleted)
                    return NotFound(new { Message = "ScanAnalysis not found." });

                return Ok(new { Message = "ScanAnalysis deleted successfully." });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error deleting ScanAnalysis: {ex.Message}");
                return StatusCode(500, new { Message = "An error occurred while deleting the ScanAnalysis.", Error = ex.Message });
            }
        }

        /// <summary>
        /// Updates a scan analysis by ID.
        /// </summary>
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateAnalysis(int id, [FromBody] ScanAnalysis updatedScanAnalysis)
        {
            try
            {
                var result = await _scanAnalysisService.UpdateAsync(id, updatedScanAnalysis);
                return Ok(new { Message = "ScanAnalysis updated successfully.", Data = result });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { Message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error updating ScanAnalysis: {ex.Message}");
                return StatusCode(500, new { Message = "An error occurred while updating the ScanAnalysis.", Error = ex.Message });
            }
        }
    }
}