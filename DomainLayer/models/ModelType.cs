using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DomainLayer.models
{
    
        public class ModelType
        {
            [Key]
            public int Id { get; set; }
            public string ModelName { get; set; } // e.g., "Skin", "Chest", "Brain"
        }
    
}
