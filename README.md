# Desafio Técnico - Itaú Unibanco

Este projeto foi desenvolvido como desafio técnico proposto pelo Itaú Unibanco. Ele simula uma aplicação de controle de investimentos, contemplando funcionalidades de cálculo de posições e preço médio, consulta de cotações e exposição de APIs RESTful.

O sistema é composto por:

Console App: exibe o resumo de investimentos diretamente via terminal.

Worker Service: simula consumo de fila Kafka para cotações, com suporte a retry, idempotência, fallback e circuit breaker.

API RESTful: expõe endpoints de consulta de posição, corretagens, ranking e cotações, com documentação Swagger em OpenAPI 3.0.

---

## ✅ Status

**Etapas concluídas:**

* [x] Estrutura do projeto
* [x] Modelagem do banco e classes de domínio
* [x] Lógica de posição do cliente
* [x] Cálculo de preço médio ponderado
* [x] Testes unitários com xUnit
* [x] Teste mutante manual
* [x] Worker Service simulando consumo Kafka
* [x] Circuit Breaker e fallback com Polly
* [x] Estratégia de escalabilidade documentada
* [x] API RESTful com OpenAPI (Swagger) documentada

---

## 1. Estrutura do Projeto

O repositório foi organizado da seguinte forma:

```
├── Investimentos.RendaVariavel/               # Projeto principal (.NET 8)
│   ├── Models/                             # Entidades EF Core com DataAnnotations
│   ├── DbContext/                          # InvestimentoContext.cs
│   ├── Services/                           # Regras de negócio (Cálculo, Resiliência)
│   └── Program.cs
│
├── Investimentos.RendaVariavel.Tests/         # Testes unitários com xUnit
│   └── CalculoPrecoMedioServiceTests.cs
│
├── Investimentos.RendaVariavel.Worker/        # Simulação de Kafka com Worker Service
│   ├── Program.cs
│   ├── Worker.cs
│   └── appsettings.json
│
└── Investimentos.RendaVariavel.API/           # API RESTful com Swagger
    ├── Controllers/
    │   ├── CotacoesController.cs
    │   └── InvestimentosController.cs
    ├── Program.cs
    └── openapi.yaml                        # Documentação OpenAPI 3.0
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

## 7. Worker Service - Kafka Simulado

Foi implementado um Worker .NET (`Investimentos.RendaVariavel.Worker`) que simula o consumo de mensagens Kafka para cotações. As mensagens são processadas como se fossem recebidas por uma fila externa, e gravadas no banco de dados.

### Mensagem simulada:

```json
{
  "AtivoId": 1,
  "PrecoUnitario": 11.25,
  "DataHora": "2025-06-06T12:00:00"
}
```

### Estratégias aplicadas:

* ✅ Retry: em caso de falha no banco (ex: FK, rede)
* ✅ Idempotência: evita salvar cotações duplicadas (mesmo Ativo + horário)
* ✅ Logs detalhados com status de cada tentativa

---

## 8. Engenharia do Caos

Foi implementado suporte a resiliência no serviço de cotações externas com:

* ✅ Retry: até 3 tentativas com Polly
* ✅ Circuit Breaker: corta chamadas após falhas consecutivas
* ✅ Fallback: valor nulo e log de erro são aplicados quando a API externa falha
* ✅ Observabilidade: logs detalhados de cada tentativa, abertura do circuito e fallback

Simulações com URL inválida e indisponibilidade confirmaram a robustez do serviço.

---

## 9. Escalabilidade e Performance

### Auto-scaling horizontal

Para garantir resiliência e atender o aumento de demanda (ex: 1 milhão de operações/dia), é recomendado aplicar escalabilidade horizontal (auto-scaling) no serviço:

* **Kubernetes (HPA)**: define réplicas automáticas com base no uso de CPU/memória. Exemplo:

```yaml
apiVersion: autoscaling/v2
kind: HorizontalPodAutoscaler
spec:
  minReplicas: 2
  maxReplicas: 10
  metrics:
    - type: Resource
      resource:
        name: cpu
        target:
          type: Utilization
          averageUtilization: 70
```

* **Azure App Service**: permite escalonamento por métricas via portal (ex: CPU > 70%).
* **AWS ECS com Fargate**: escalonamento automático por CPU, memória ou tamanho da fila (mensagens pendentes).

O serviço deve ser **stateless** para permitir múltiplas instâncias paralelas, compartilhando a base de dados e recursos externos (como Kafka ou cache distribuído).

### Balanceamento de Carga: Round-robin vs Latência

* **Round-robin**:

  * Requisições são distribuídas de forma sequencial entre instâncias.
  * Simples, eficaz em cenários com cargas uniformes.
* **Baseado em latência**:

  * Envia a requisição para a instância com menor tempo de resposta.
  * Ideal quando há variação de carga ou performance entre instâncias.

> **Recomendação**: utilizar **balanceamento por latência** em produção com tráfego elevado, para otimizar resposta e eficiência de uso de recursos.

---

## 10. API RESTful e Documentação OpenAPI

### Endpoints implementados:

| Método | Rota                                           | Descrição                             |
| ------ | ---------------------------------------------- | ------------------------------------- |
| GET    | `/api/Cotacoes/{codigoAtivo}`                  | Última cotação de um ativo            |
| GET    | `/api/Investimentos/preco-medio`               | Preço médio de um ativo por usuário   |
| GET    | `/api/Investimentos/posicao/{usuarioId}`       | Posição de um usuário                 |
| GET    | `/api/Investimentos/corretagens/{usuarioId}`   | Total de corretagem de um usuário     |
| GET    | `/api/Investimentos/ranking/top10-posicoes`    | Top 10 usuários por volume de posição |
| GET    | `/api/Investimentos/ranking/top10-corretagens` | Top 10 usuários por corretagem paga   |
| GET    | `/api/Investimentos/corretagem`                | Corretagem total da corretora         |

### Documentação Swagger

* Acessível em `http://localhost:5030/swagger`
* Formato: OpenAPI 3.0
* Arquivo: `openapi.yaml`

---

## Como Rodar

```bash
# Restaurar pacotes
dotnet restore

# Rodar testes
dotnet test

# Rodar API com Swagger
dotnet run --project ./Investimentos.RendaVariavel.API
```

---
