// Dtos/CreateProductDto.cs
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;   // <— burayı ekleyin!

namespace MerhabaDunyaApi.Dtos
{
    public class CreateProductDto
    {
        [FromForm]
        public string Title { get; set; }

        [FromForm]
        public string Description { get; set; }

        [FromForm]
        public string Category { get; set; }

        [FromForm]
        public int SellerId { get; set; }

        [FromForm]
        public string SellerName { get; set; }

        [FromForm]
        public decimal Price { get; set; }

        [FromForm]
        public double Weight { get; set; }

        [FromForm]
        public IFormFile Image { get; set; }
    }
}
