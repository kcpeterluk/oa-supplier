using System.ComponentModel.DataAnnotations;

namespace OA.Supplier.WebApp.Components.Pages.Suppliers;

public class SupplierEditorViewModel
{
  [Required]
  [StringLength(450)]
  public required string Name { get; set; }

  [Required]
  [StringLength(450)]
  public required string Address { get; set; }
}