# 🛒 Sistema de Processamento de Pedidos e Eventos

Bem-vindo ao repositório do **Sistema de Processamento de Pedidos**, uma solução distribuída construída sob os princípios da **Arquitetura Limpa (Clean Architecture)** e **Orientação a Eventos (Event-Driven Architecture)**. 

Este projeto demonstra como criar microsserviços desacoplados e escaláveis utilizando o ecossistema .NET, mensageria com RabbitMQ e persistência isolada com PostgreSQL.

---

## 🏗️ Arquitetura e Tecnologias

A aplicação foi desenhada visando **alta coesão**, **baixo acoplamento** e **resiliência**.

### 🛠️ Tecnologias Utilizadas
* **C# / .NET 10**: Plataforma de desenvolvimento unificada.
* **PostgreSQL**: Banco de dados relacional.
* **RabbitMQ**: Message Broker para comunicação assíncrona.
* **MassTransit**: Framework abstrato poderoso para roteamento de mensagens / service bus no .NET.
* **Docker & Docker Compose**: Orquestração e containerização dos serviços para um ambiente de desenvolvimento e produção padronizado.
* **Swagger / OpenAPI**: Documentação interativa das APIs.

### 🧩 Padrões Aplicados
* **Clean Architecture**: Separação clara entre Domínio (Regras de Negócio Puras), Aplicação (Casos de Uso), Infraestrutura (Mensageria, Integrações Externas) e Data (EF Core).
* **CQRS (Command Query Responsibility Segregation)**: Divisão lógica no fluxo de operações de escrita e leitura (embora simplificado neste escopo focado em Commands).
* **Event-Driven Architecture**: Comunicação via pub/sub visando desacoplamento de contextos (Checkout não precisa esperar o Pagamento terminar para responder ao cliente).
* **Repository Pattern**: Abstração da camada de acesso a dados.

---

## 📦 Estrutura de Microsserviços

O ecossistema é formado atualmente por dois serviços autônomos:

1. **Checkout.API (`/src/Services/Checkout`)**
   - Responsável por receber a intenção de compra do cliente.
   - Valida os dados de entrada, registra o pedido em seu banco de dados isolado (`checkout_db`).
   - Dispara um evento de integração avisando que um pedido foi gerado.

2. **Payment.Worker (`/src/Services/Payment`)**
   - Serviço de background (Worker Service) que atua como "Consumidor".
   - Escuta eventos de pedidos criados de forma assíncrona.
   - Processa o pagamento simulado e armazena os dados em seu próprio banco (`payment_db`).

---

## 🔄 Entendendo o Fluxo da Aplicação

Como a mágica acontece nos bastidores quando uma compra é feita?
Aqui está o **Fluxo de Processamento de Pedido**:

1. **Requisição HTTP (API)**
   O cliente faz uma chamada `POST /api/checkout` na `Checkout.API` passando o CPF e o Valor do Carrinho.

2. **Caso de Uso (Application Layer)**
   O `CriarPedidoUseCase` entra em ação:
   - Instancia o Agregado/Entidade `Pedido` (com regras ricas de validação internamente no Domínio).
   - Utiliza as interfaces injetadas para interagir com persistência sem conhecer a tecnologia (EF Core).

3. **Isolamento de Banco e Transação**
   O repositório salva fisicamente o novo `Pedido` na tabela do banco de dados relacional exclusivo do Checkout (`checkout_db`).

4. **Publicação do Evento (Eventual Consistency)**
   Ainda dentro do caso de uso do Checkout, após garantir que o pedido foi salvo, um evento `PedidoCriadoEvent` é publicado no barramento (RabbitMQ, empacotado e abstraído pelo MassTransit).
   - *O Checkout.API retorna HTTP 200 (OK) para o cliente. A requisição HTTP finaliza rápida e com sucesso aqui. O cliente não precisa esperar a cobrança passar.*

5. **Consumo Assíncrono (Worker)**
   O serviço `Payment.Worker` (rodando de forma invisível) é notificado pelo RabbitMQ sobre o evento `PedidoCriadoEvent`.
   O componente `PedidoCriadoEventConsumer` é acionado pelo MassTransit automaticamente.

6. **Processamento do Pagamento**
   O Worker encaminha os dados para o `ProcessarPagamentoUseCase` dentro do seu contexto:
   - Uma entidade `Pagamento` é criada associada àquele `PedidoId`.
   - O repositório interno de pagamento salva na tabela do `payment_db`.

---

## 🚀 Como Executar o Projeto Localmente

Devido à containerização via Docker Compose, rodar todo o ecossistema é uma tarefa de apenas um comando:

### Pré-requisitos
- [Docker Engine & Docker Compose](https://www.docker.com/) instalados.

### Passos para inicializar

1. Faça um git clone ou abra a pasta raiz do projeto no seu terminal.
2. Execute o comando para baixar as imagens necessárias, compilar o código fonte e subir todos os serviços:
   ```bash
   docker compose up -d --build
   ```

3. **Verificando se está rodando:**
   Os seguintes containers deverão estar no ar:
   * `rabbitmq-broker`: O servidor de mensageria (RabbitMQ).
   * `postgres-db`: Banco de dados do Checkout.
   * `postgres-payment-db`: Banco de dados do Payment.
   * `checkout-api`: Nossa API Web.
   * `payment-worker`: O Worker Service consumindo as filas.

4. **Acessando a Aplicação:**
   Para interagir com a interface visual exposta do Swagger:
   👉 **http://localhost:8080/swagger**

5. **Visualizando o RabbitMQ (Opcional):**
   Você pode ver as filas funcionando e conectadas internamente via Management UI:
   👉 **http://localhost:15672**
   *(Login padrão: `guest` / Senha: `guest`)*

---

### 📝 Observações Finais
Este repositório visa ser um lab prático para escalabilidade, observabilidade e boas práticas utilizando Full Cycle Engineering e a transição para engenharia Cloud Native.
