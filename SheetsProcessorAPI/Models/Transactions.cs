using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SheetsProcessorApi.Models;

[Table("Transactions")]
public class Transactions
{
    [Key]
    public Guid Id {get; set;} = Guid.NewGuid();
    [Required]
    public DateTime Date {get; set;}
    [Required]
    [Column(TypeName = "decimal(18, 2)")]
    public decimal Value {get; set;}
    [Required]
    [MaxLength(255)]
    public string Description {get; set;} = "Sem Descrição";
    [Required]
    [MaxLength(100)]
    public required string Hash {get; init;}
    [MaxLength(50)]
    public string Category {get; set;} = "Não Categorizado";
    public bool PersonalUse {get; set;} = false;

}