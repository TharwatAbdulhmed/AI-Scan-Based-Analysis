using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DomainLayer.Interfaces;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Http;

namespace ServicesLayer
{
    public class ImageResolverService : IImageResolverService
    {
        public string ResolveImageUrl(Microsoft.AspNetCore.Http.HttpRequest request, string imagePath)
        {
            if (string.IsNullOrWhiteSpace(imagePath))
                return null;

            // Construct the full URL using the request's scheme, host, and image path
            var baseUrl = $"{request.Scheme}://{request.Host}";
            return $"{baseUrl.TrimEnd('/')}/images/{imagePath.TrimStart('/')}";
        }

      
    }
}
