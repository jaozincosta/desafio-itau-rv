using System.ComponentModel.DataAnnotations.Schema;

namespace InvestimentosRendaVariavel.Models
{
    [Table("posicoes")]
    public class Posicao
    {
        public int Id { get; set; }

        [Column("usuario_id")]
        public int UsuarioId { get; set; }

        [Column("ativo_id")]
        public int AtivoId { get; set; }

        [Column("quantidade")]
        public int Quantidade { get; set; }

        [Column("preco_medio")]
        public decimal PrecoMedio { get; set; }

        [Column("pnl")]
        public decimal PnL { get; set; }

        public Usuario? Usuario { get; set; }
        public Ativo? Ativo { get; set; }
    }
}