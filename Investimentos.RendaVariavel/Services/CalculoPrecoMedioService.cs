namespace InvestimentosRendaVariavel.Services
{
    public class CalculoPrecoMedioService
    {
        public decimal CalcularPrecoMedio(List<(int quantidade, decimal precoUnitario)> compras)
        {
            if (compras == null || compras.Count == 0)
                throw new ArgumentException("A lista de compras não pode estar vazia.");

            decimal totalInvestido = 0;
            int totalQuantidade = 0;

            foreach (var (quantidade, preco) in compras)
            {
                if (quantidade <= 0 || preco < 0)
                    throw new ArgumentException("Quantidade e preço devem ser válidos.");

                totalInvestido += quantidade * preco;
                totalQuantidade += quantidade;
            }

            if (totalQuantidade == 0)
                throw new DivideByZeroException("Quantidade total não pode ser zero.");

            return totalInvestido / totalQuantidade;
        }
    }
}
