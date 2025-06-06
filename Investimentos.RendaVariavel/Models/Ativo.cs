using System.ComponentModel.DataAnnotations.Schema;

namespace InvestimentosRendaVariavel.Models
{
    [Table("ativos")]
    public class Ativo
    {
        public int Id { get; set; }

        [Column("codigo")]
        public string Codigo { get; set; } = string.Empty;

        [Column("nome")]
        public string Nome { get; set; } = string.Empty;
    }
}