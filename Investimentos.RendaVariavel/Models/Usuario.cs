using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace InvestimentosRendaVariavel.Models
{
    [Table("usuarios")]
    public class Usuario
    {
        public int Id { get; set; }
        public string Nome { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;

        [Column("percentual_corretagem")]
        public decimal PercentualCorretagem { get; set; }

        public ICollection<Operacao>? Operacoes { get; set; }
        public ICollection<Posicao>? Posicoes { get; set; }
    }
}