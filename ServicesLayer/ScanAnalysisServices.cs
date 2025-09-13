using System.Net.Http.Headers;
using System.Net.Http;
using DomainLayer.models;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System.Globalization;
using Microsoft.Extensions.Logging;

using DomainLayer.Interfaces;
using ServicesLayer.DTO;

namespace ServicesLayer
{
    public class ScanAnalysisService : IScanAnalysisService
    {
        private readonly IScanAnalysisRepository _repository;
        private readonly HttpClient _httpClient;
        private readonly ILogger<ScanAnalysisService> _logger;

        public ScanAnalysisService(IScanAnalysisRepository repository, IHttpClientFactory httpClientFactory, ILogger<ScanAnalysisService> logger)
        {
            _repository = repository;
            _httpClient = httpClientFactory.CreateClient("FlaskApiClient"); // Use named HttpClient
            _logger = logger;
        }

        public async Task<ScanAnalysis> AnalyzeImageAsync(IFormFile file, string modelName)
        {
            // Validate input parameters
            if (file == null || file.Length == 0)
                throw new ArgumentException("No file uploaded.");
            if (string.IsNullOrWhiteSpace(modelName))
                throw new ArgumentException("Model name is required.");

            try
            {
                // Prepare the multipart form data for the Flask API
                var content = new MultipartFormDataContent();
                var fileContent = new StreamContent(file.OpenReadStream());
                fileContent.Headers.ContentType = MediaTypeHeaderValue.Parse("image/jpeg");
                content.Add(fileContent, "file", file.FileName);
                content.Add(new StringContent(modelName), "model_name");

                // Send the request to the Flask server
                var response = await _httpClient.PostAsync("/analyze", content);

                // Handle non-successful responses
                if (!response.IsSuccessStatusCode)
                {
                    var errorResponse = await response.Content.ReadAsStringAsync();
                    _logger.LogError($"AI service error: {errorResponse}");
                    throw new Exception($"AI service error: {errorResponse}");
                }

                // Deserialize the response from Flask
                var responseContent = await response.Content.ReadAsStringAsync();
                var dto = JsonConvert.DeserializeObject<ScanAnalysisDto>(responseContent);

                // Log the deserialized DTO for debugging
                _logger.LogInformation($"DTO Received: {JsonConvert.SerializeObject(dto)}");

                // Validate the deserialized DTO
                if (dto == null)
                    throw new InvalidOperationException("Flask service returned an invalid or empty response.");
                if (dto.analysisResult == null)
                    throw new InvalidOperationException("Flask service did not return analysis results.");

                // Map the DTO to the ScanAnalysis entity
                var scanAnalysis = new ScanAnalysis
                {
                    ImagePath = dto.ImagePath ?? "Unknown",
                    ModelType = Enum.TryParse(dto.ModelType, true, out AnalysisType modelTypeEnum)
                        ? modelTypeEnum
                        : throw new InvalidOperationException("Invalid ModelType received from Flask."),
                    Result = dto.Result ?? "Unknown",
                    analysisResult = new AnalysisResult
                    {
                        Prediction = dto.analysisResult.Prediction != null
                            ? string.Join(",", dto.analysisResult.Prediction.Select(p => p.ToString(CultureInfo.InvariantCulture)))
                            : "Unknown",
                        Confidence = dto.analysisResult.Confidence,
                    }
                };

                // Save the analysis result to the database
                return await _repository.AddScanAnalysisAsync(scanAnalysis);
            }
            catch (Exception ex)
            {
                // Log the error and rethrow
                _logger.LogError($"Error during image analysis: {ex.Message}");
                throw;
            }
        }

        public Task<ScanAnalysis> GetAnalysisResultAsync(int id)
        {
            return _repository.GetScanAnalysisByIdAsync(id);
        }

        public async Task<IEnumerable<ScanAnalysis>> GetAllAsync()
        {
            try
            {
                return await _repository.GetAllScanAnalysesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error retrieving all scan analyses: {ex.Message}");
                throw;
            }
        }

        public async Task<bool> DeleteAsync(int id)
        {
            try
            {
                var isDeleted = await _repository.DeleteAsync(id);
                if (!isDeleted)
                {
                    _logger.LogWarning($"ScanAnalysis with ID {id} not found.");
                    return false; // Record not found
                }

                return true; // Deletion successful
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error deleting ScanAnalysis with ID {id}: {ex.Message}");
                throw; // Rethrow the exception after logging
            }
        }

        public async Task<ScanAnalysis> UpdateAsync(int id, ScanAnalysis updatedScanAnalysis)
        {
            try
            {
                var result = await _repository.UpdateAsync(id, updatedScanAnalysis);
                return result; // Return the updated record
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error updating ScanAnalysis with ID {id}: {ex.Message}");
                throw; // Rethrow the exception after logging
            }
        }
    }
}