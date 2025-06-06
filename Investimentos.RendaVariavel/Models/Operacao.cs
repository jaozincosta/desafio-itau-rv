using System.ComponentModel.DataAnnotations.Schema;

namespace InvestimentosRendaVariavel.Models
{
    [Table("operacoes")]
    public class Operacao
    {
        public int Id { get; set; }

        [Column("usuario_id")]
        public int UsuarioId { get; set; }

        [Column("ativo_id")]
        public int AtivoId { get; set; }

        [Column("quantidade")]
        public int Quantidade { get; set; }

        [Column("preco_unitario")]
        public decimal PrecoUnitario { get; set; }

        [Column("tipo_operacao")]
        public string TipoOperacao { get; set; } = "Compra";

        [Column("corretagem")]
        public decimal Corretagem { get; set; }

        [Column("data_hora")]
        public DateTime DataHora { get; set; }

        public Usuario? Usuario { get; set; }
        public Ativo? Ativo { get; set; }
    }
}