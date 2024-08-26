using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Persistence.Models;

[Table("TableSpecification")]
public class TableSpecification
{
    [Key]
    [Required]
    public Guid TableId { get; set; }
    public int TableNumber { get; set; }
    public int ChairNumber { get; set; }
    [Required]
    public string TablePic { get; set; }
    public string? TableType { get; set; }
}