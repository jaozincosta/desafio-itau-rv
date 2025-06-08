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
* [x] Worker Service simulando consumo Kafka
* [x] Circuit Breaker e fallback com Polly
* [x] Estratégia de escalabilidade documentada

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
├── Investimentos.RendaVariavel.Tests/     # Testes unitários com xUnit
│   └── CalculoPrecoMedioServiceTests.cs   # Casos de teste para preço médio
│
├── Investimentos.RendaVariavel.Worker/    # Worker .NET simulando consumo Kafka
│   ├── Program.cs                          # Host configurado para rodar Worker
│   ├── Worker.cs                           # Simulação com retry e idempotência
│   └── appsettings.json
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

O Worker foi testado com sucesso, salvando cotações válidas e ignorando duplicadas.

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

- **Kubernetes (HPA)**: define réplicas automáticas com base no uso de CPU/memória. Exemplo:
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
- **Azure App Service**: permite escalonamento por métricas via portal (ex: CPU > 70%).
- **AWS ECS com Fargate**: escalonamento automático por CPU, memória ou tamanho da fila (mensagens pendentes).

O serviço deve ser **stateless** para permitir múltiplas instâncias paralelas, compartilhando a base de dados e recursos externos (como Kafka ou cache distribuído).

### Balanceamento de Carga: Round-robin vs Latência

- **Round-robin**:
  - Requisições são distribuídas de forma sequencial entre instâncias.
  - Simples, eficaz em cenários com cargas uniformes.
- **Baseado em latência**:
  - Envia a requisição para a instância com menor tempo de resposta.
  - Ideal quando há variação de carga ou performance entre instâncias.

> **Recomendação**: utilizar **balanceamento por latência** em produção com tráfego elevado, para otimizar resposta e eficiência de uso de recursos.

---

## Próximos passos

* [ ] API RESTful com OpenAPI (Swagger) documentando endpoints
