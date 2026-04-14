using System.ComponentModel.DataAnnotations;

namespace TestApp1.Contracts.Products;

public sealed class UpdateProductRequest
{
    [Required]
    [StringLength(120, MinimumLength = 2)]
    public string Name { get; set; } = string.Empty;

    [Range(typeof(decimal), "1", "999999999")]
    public decimal Price { get; set; }

    [Required]
    [StringLength(80, MinimumLength = 2)]
    public string Category { get; set; } = string.Empty;

    [Required]
    [StringLength(50, MinimumLength = 1)]
    public string Code { get; set; } = string.Empty;
}
