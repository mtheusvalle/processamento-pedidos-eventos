# Roadmap de Evolução: Processamento de Pedidos Orientado a Eventos

Este documento lista as melhorias arquiteturais e de negócio planejadas para o projeto, organizadas por etapas de implementação.

## Tarefa 1: Retorno do Pagamento e Atualização de Status (Coreografia / Saga Pattern)
**Objetivo:** O serviço de Checkout deve ser notificado sobre o resultado do pagamento para atualizar o status do pedido no banco de dados.
- [x] Adicionar status de pedido (ex: `AguardandoPagamento`, `Pago`, `Cancelado`) na entidade `Pedido` do Checkout.
- [x] Criar os eventos `PagamentoAprovadoEvent` e `PagamentoRecusadoEvent` no projeto `ProcessamentoPedidos.Core`.
- [x] O serviço de Pagamento (`Payment.Worker`) deve simular a aprovação/recusa e publicar o evento correspondente no RabbitMQ.
- [x] O serviço de Checkout deve consumir os eventos de pagamento e atualizar o status do pedido no banco de dados.

## Tarefa 2: Novo Microsserviço: Estoque (Inventory Service)
**Objetivo:** Garantir a reserva de produtos e aplicar transações compensatórias em caso de falha.
- [x] Criar o serviço `Inventory.API` e/ou `Inventory.Worker`.
- [x] Consumir evento para tentar reservar o estoque dos itens do pedido.
- [x] Em caso de falta de estoque, publicar `EstoqueRecusadoEvent`, acionando o estorno no serviço de Pagamento e cancelamento no Checkout.

## Tarefa 3: Novo Microsserviço: Notificação (Notification Worker)
**Objetivo:** Centralizar a comunicação com o cliente baseada nos eventos do sistema.
- [ ] Criar o serviço `Notification.Worker`.
- [ ] Escutar `PedidoCriadoEvent` (E-mail: "Recebemos seu pedido").
- [ ] Escutar `PagamentoAprovadoEvent` (E-mail: "Pagamento Aprovado").
- [ ] Simular o envio com logs formatados no console.

## Tarefa 4: Idempotência e Tratamento de Falhas (Resiliência)
**Objetivo:** Garantir que o processamento seja tolerante a falhas e não processe eventos duplicados.
- [ ] Implementar Idempotência usando o ID da mensagem no consumidor de pagamento (evitar cobrança dupla).
- [ ] Configurar políticas de Retry no MassTransit para lidar com instabilidades temporárias.
- [ ] Testar cenários de erro enviando mensagens para a Dead Letter Queue (DLQ).

## Tarefa 5: API Gateway (YARP)
**Objetivo:** Criar um ponto único de entrada (Single Point of Entry) para as APIs do projeto.
- [ ] Criar o projeto de API Gateway utilizando YARP (Yet Another Reverse Proxy).
- [ ] Configurar roteamento para as APIs de Checkout, Pagamento e Estoque.

## Tarefa 6: Observabilidade Distribuída (OpenTelemetry)
**Objetivo:** Monitoramento e rastreamento de requisições que atravessam múltiplos serviços e filas.
- [ ] Configurar OpenTelemetry no projeto.
- [ ] Exportar logs e traces para Jaeger ou Seq.
- [ ] Garantir que o `CorrelationId` seja propagado nas mensagens do MassTransit.