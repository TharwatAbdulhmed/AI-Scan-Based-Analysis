using System.ComponentModel.DataAnnotations;

namespace DomainLayer.models
{
    public class Study
    {
        [Key]
        public int Id { get; set; }

        public string DiseaseName { get; set; } // Name of the disease
        public string Description { get; set; } // Description of the study
        public string ImgPath { get; set; } // Relative path to the image (e.g., "images/skin.jpg")
        public string TreatmentMethod { get; set; } // Treatment method

        // Foreign key for ModelType
        public int ModelTypeId { get; set; }

        // Navigation property
        public ModelType ModelType { get; set; }
    }

}