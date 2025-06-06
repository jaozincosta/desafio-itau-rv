using Xunit;
using System;
using System.Collections.Generic;
using InvestimentosRendaVariavel.Services;

namespace InvestimentosRendaVariavel.Tests
{
    public class CalculoPrecoMedioServiceTests
    {
        private readonly CalculoPrecoMedioService _service = new();

        [Fact]
        public void DeveCalcularPrecoMedioCorretamente()
        {
            var compras = new List<(int, decimal)>
            {
                (100, 10.00m),
                (50, 12.00m),
                (150, 9.50m)
            };

            var resultado = _service.CalcularPrecoMedio(compras);
            Assert.Equal(10.08m, Math.Round(resultado, 2));
        }

        [Fact]
        public void DeveLancarExcecao_ListaVazia()
        {
            var compras = new List<(int, decimal)>();
            Assert.Throws<ArgumentException>(() => _service.CalcularPrecoMedio(compras));
        }

        [Fact]
        public void DeveLancarExcecao_QuantidadeZero()
        {
            var compras = new List<(int, decimal)>
            {
                (0, 10.00m)
            };
            Assert.Throws<ArgumentException>(() => _service.CalcularPrecoMedio(compras));
        }
    }
}
