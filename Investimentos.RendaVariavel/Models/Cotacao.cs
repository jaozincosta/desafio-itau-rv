using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace InvestimentosRendaVariavel.Models
{
    [Table("cotacoes")]
    public class Cotacao
    {
        public int Id { get; set; }

        [Column("ativo_id")]
        public int AtivoId { get; set; }

        [Column("preco_unitario")]
        public decimal PrecoUnitario { get; set; }

        [Column("data_hora")]
        public DateTime DataHora { get; set; }

        public Ativo? Ativo { get; set; }
    }
}