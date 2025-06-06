# Desafio Técnico - Renda Variável (Itaú)

Este repositório contém a solução para o desafio técnico proposto pelo Itaú Unibanco.

---

## ✅ Status

**Etapas concluídas:**

* [x] Estrutura do projeto
* [x] Modelagem do banco e classes de domínio
* [x] Lógica de posição do cliente
* [x] Cálculo de preço médio ponderado
* [x] Testes unitários com xUnit
* [x] Teste mutante manual

---

## 1. Estrutura do Projeto

O repositório foi organizado da seguinte forma:

```
├── Investimentos.RendaVariavel/           # Projeto principal (.NET 8)
│   ├── Models/                             # Entidades mapeadas com [Table] e [Column]
│   ├── DbContext/                          # EF Core com MySQL
│   ├── Services/                           # Regras de negócio
│   └── Program.cs                          # Execução via console
│
├── Investimentos.RendaVariavel.Tests/     # Projeto de testes unitários com xUnit
│   └── CalculoPrecoMedioServiceTests.cs   # Casos de teste para preço médio
```

---

## 2. Modelagem

Modelagem relacional com as seguintes tabelas:

* `usuarios (id, nome, email, percentual_corretagem)`
* `ativos (id, codigo, nome)`
* `operacoes (usuario_id, ativo_id, quantidade, preco_unitario, tipo_operacao, corretagem, data_hora)`
* `posicoes (usuario_id, ativo_id, quantidade, preco_medio, pnl)`
* `cotacoes (ativo_id, preco_unitario, data_hora)`

Todas as entidades foram mapeadas em C# com `DataAnnotations` (`[Table]`, `[Column]`), e o `DbContext` (`InvestimentoContext`) configurado com Pomelo MySQL Provider.

---

## 3. Lógica de posição do cliente

Classe: `InvestimentoService`

Implementações:

* Total investido por ativo
* Total de corretagem por cliente
* Posição por papel (quantidade, preço médio, PnL)
* PnL global do cliente

---

## 4. Cálculo de Preço Médio

Classe: `CalculoPrecoMedioService`

```csharp
public decimal CalcularPrecoMedio(List<(int quantidade, decimal precoUnitario)> compras)
```

* Valida entradas nulas ou inválidas
* Retorna o preço médio ponderado

---

## 5. Testes Unitários

Projeto: `Investimentos.RendaVariavel.Tests`

```bash
dotnet test
```

Coberturas:

* Cálculo correto com dados reais
* Falha com lista vazia
* Falha com quantidade 0

---

## 6. Teste Mutante

Foi alterado o método `CalcularPrecoMedio` para simular um erro lógico:

```diff
- return totalInvestido / totalQuantidade;
+ return totalInvestido / (totalQuantidade + 1);
```

Resultado esperado: o teste `DeveCalcularPrecoMedioCorretamente` falhou, confirmando a efetividade do teste e a cobertura da lógica.

---


