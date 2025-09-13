using DomainLayer.Interfaces;
using DomainLayer.models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ServicesLayer;
using Talabat.API.Errors;

namespace AI_Scan_Analyses_Test.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StudiesController : ControllerBase
    {
        private readonly IStudyRepository _studyRepository;
        private readonly IImageResolverService _imageResolverService;

        public StudiesController(IStudyRepository studyRepository, IImageResolverService imageResolverService)
        {
            _studyRepository = studyRepository;
            _imageResolverService = imageResolverService;
        }
        //[Authorize]
        // GET: api/studies
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var studies = await _studyRepository.GetAllAsync();
            // Resolve image URLs
            // Resolve image URLs
            foreach (var study in studies)
            {
                study.ImgPath = _imageResolverService.ResolveImageUrl(Request, study.ImgPath);
            }
            return Ok(studies);
        }

        // GET: api/studies/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var study = await _studyRepository.GetByIdAsync(id);
            if (study == null)
                return NotFound(new ApiResponse(404));
            // Resolve image URL
            study.ImgPath = _imageResolverService.ResolveImageUrl(Request, study.ImgPath);

            return Ok(study);
        }

        // GET: api/studies/name/{diseaseName}
        [HttpGet("name/{diseaseName}")]
        public async Task<IActionResult> GetByName(string diseaseName)
        {
            var study = await _studyRepository.GetByNameAsync(diseaseName);
            if (study == null)
                return NotFound(new ApiResponse(404));
            // Resolve image URL
            study.ImgPath = _imageResolverService.ResolveImageUrl(Request, study.ImgPath);

            return Ok(study);
        }

        // POST: api/studies
        [HttpPost]
        public async Task<IActionResult> Add([FromBody] Study study)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var addedStudy = await _studyRepository.AddAsync(study);
            return CreatedAtAction(nameof(GetById), new { id = addedStudy.Id }, addedStudy);
        }
        [Authorize]
        // PUT: api/studies/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] Study study)
        {
            if (id != study.Id)
                return BadRequest(new ApiResponse(400));

            await _studyRepository.UpdateAsync(study);
            return NoContent();
        }
        [Authorize]
        // DELETE: api/studies/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            await _studyRepository.DeleteAsync(id);
            return NoContent();
        }
    }
}
