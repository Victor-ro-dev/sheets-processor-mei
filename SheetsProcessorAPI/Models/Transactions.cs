using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SheetsProcessorApi.Models;

[Table("Transactions")]
public class Transactions
{
    [Key]
    public Guid Id {get; set;} = Guid.NewGuid();
    [Required]
    public DateTime Data {get; set;}
    [Required]
    [Column(TypeName = "decimal(18, 2)")]
    public decimal Valor {get; set;}
    [Required]
    [MaxLength(255)]
    public string? Descricao {get; set;}
    [Required]
    [MaxLength(100)]
    public required string Hash {get; init;}
    [MaxLength(50)]
    public string Categoria {get; set;} = "Não Categorizado";
    public bool UsoPessoal {get; set;} = false;

}