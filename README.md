# Analytics System - Plataforma de Análise de Dados e Eventos em Tempo Real

[![.NET](https://img.shields.io/badge/.NET-8.0-purple.svg)](https://dotnet.microsoft.com/)
[![PostgreSQL](https://img.shields.io/badge/PostgreSQL-15+-blue.svg)](https://www.postgresql.org/)
[![Redis](https://img.shields.io/badge/Redis-7.0+-red.svg)](https://redis.io/)
[![RabbitMQ](https://img.shields.io/badge/RabbitMQ-3.12+-orange.svg)](https://www.rabbitmq.com/)
[![Status](https://img.shields.io/badge/Status-Production%20Ready-green.svg)]()

Plataforma empresarial de alta performance para coleta, processamento e análise de grandes volumes de eventos e métricas em tempo real. Arquitetura baseada em Clean Architecture com processamento assíncrono, garantindo escalabilidade horizontal e baixa latência.

---

## Navegação Rápida

- [Visão Geral](#visão-geral)
- [Arquitetura](#arquitetura)
- [Guia de Instalação](#guia-de-instalação)
- [Documentação da API](#documentação-da-api)
  - [Coleta de Eventos](#coleta-de-eventos)
  - [Analytics e Relatórios](#analytics-e-relatórios)
  - [Monitoramento em Tempo Real](#monitoramento-em-tempo-real)
  - [Sistema de Alertas](#sistema-de-alertas)
- [Modelos de Dados](#modelos-de-dados)
- [Configuração Avançada](#configuração-avançada)
- [Performance e Benchmarks](#performance-e-benchmarks)
- [Troubleshooting](#troubleshooting)

---

## Visão Geral

### Funcionalidades Principais

**Coleta e Processamento**
- Ingestão de eventos de alta performance (5.000 req/min por tenant)
- Processamento assíncrono com RabbitMQ
- Suporte a coleta em lote (até 100 eventos por requisição)
- Agregação automática de métricas por período

**Analytics e Relatórios**
- Métricas de tráfego e comportamento do usuário
- Análise de fontes de tráfego e dispositivos
- Distribuição geográfica
- Relatórios customizáveis por período

**Monitoramento em Tempo Real**
- Dashboard de métricas ao vivo
- WebSocket para atualizações em tempo real
- Detecção de anomalias e padrões suspeitos

**Sistema de Alertas**
- Alertas configuráveis baseados em métricas
- Notificações via webhook
- Histórico de alertas disparados

### Casos de Uso

| Cenário | Aplicação |
|---------|-----------|
| **E-commerce** | Tracking de vendas, comportamento de compra, análise de funil |
| **SaaS/Web Apps** | Métricas de usuário, engagement, feature usage |
| **IoT/Telemetria** | Processamento de dados de sensores, detecção de anomalias |
| **Logs Centralizados** | Agregação de logs, alertas de sistema, análise de performance |
| **Marketing Digital** | UTM tracking, análise de campanhas, ROI |

---

## Arquitetura

### Stack Tecnológico

| Componente | Tecnologia | Versão |
|------------|------------|--------|
| **API Framework** | ASP.NET Core | 8.0 |
| **Banco de Dados** | PostgreSQL | 15+ |
| **Cache** | Redis | 7.0+ |
| **Message Queue** | RabbitMQ | 3.12+ |
| **Logging** | Serilog | - |
| **Testes** | BenchmarkDotNet, k6 | - |

### Fluxo de Dados

```
Cliente → POST /events → Analytics API → RabbitMQ → Worker → PostgreSQL
                              ↓                              ↓
                         Cache Redis ←―――――――――――――――――――――――
                              ↓
                      GET /analytics/*
```

### Arquitetura Clean Architecture

O projeto segue os princípios da **Clean Architecture**, garantindo baixo acoplamento, alta coesão e testabilidade:

```
/
├── Analytics.API/             # Ponto de entrada HTTP, expõe os endpoints.
├── Analytics.Application/     # Casos de uso e orquestração (sem dependências externas).
├── Analytics.Domain/          # Entidades, regras de negócio e contratos.
├── Analytics.Infrastructure/  # Acesso a banco, Redis, RabbitMQ, serviços externos.
├── Analytics.Benchmarks/      # Benchmarks de performance com BenchmarkDotNet.
└── tests/
    └── load-tests/            # Scripts de carga (ex: simulações com k6).
```

**Princípios Aplicados:**
- **Inversão de Dependência**: Camadas externas dependem das internas
- **Separação de Responsabilidades**: Cada camada tem função específica
- **Testabilidade**: Dependências injetadas permitem mocks e testes isolados
- **Independência de Framework**: Regras de negócio não dependem de tecnologia específica

### Estratégias de Performance

| Estratégia | Implementação | Benefício |
|------------|---------------|-----------|
| **Cache-Aside** | Redis com TTL configurável | Redução de 80% no tempo de resposta |
| **CQRS** | Separação de leitura/escrita | Otimização independente |
| **Async Processing** | RabbitMQ workers | Desacoplamento e escalabilidade |
| **Indexação Estratégica** | Índices otimizados no PostgreSQL | Consultas sub-100ms |
| **Connection Pooling** | Pooling de conexões DB | Melhor utilização de recursos |

---

## Guia de Instalação

### Pré-requisitos

- [.NET 8 SDK](https://dotnet.microsoft.com/)
- [Docker + Docker Compose](https://www.docker.com/)
- Git

### Instalação Rápida

1. **Clone o repositório**
```bash
git clone https://github.com/mateusgomst/analytics-system.git
cd analytics-system
```

2. **Configure as variáveis de ambiente**
```bash
cp .env.example .env
# Edite o arquivo .env conforme necessário
```

3. **Inicie os serviços**
```bash
docker compose up -d --build
```

4. **Verifique o status**
```bash
docker compose ps
```

### Verificação dos Serviços

| Serviço | URL | Credenciais |
|---------|-----|-------------|
| **Analytics API** | http://localhost:8080 | - |
| **Swagger UI** | http://localhost:8080/swagger | - |
| **RabbitMQ Management** | http://localhost:15672 | admin / admin |
| **PostgreSQL** | localhost:5432 | Conforme .env |
| **Redis** | localhost:6379 | - |

---

## Documentação da API

### Autenticação

Todas as requisições devem incluir os headers de autenticação:

```http
Authorization: Bearer {api_key}
X-Tenant-ID: {tenant_id}
Content-Type: application/json
```

### Rate Limits

| Endpoint Type | Limite |
|---------------|--------|
| Coleta de Eventos | 5.000 req/min |
| Analytics/Relatórios | 1.000 req/min |
| Tempo Real | 2.000 req/min |

---

## Coleta de Eventos

### POST /api/v1/events

Coleta um único evento.

**Headers:**
```http
Authorization: Bearer {api_key}
X-Tenant-ID: {tenant_id}
Content-Type: application/json
```

**Request Body:**
```json
{
  "EventType": "page_view",
  "Timestamp": "2025-08-07T17:25:18Z",
  "UserId": "user_12345",
  "SessionId": "sess_abc123",
  "Payload": {
    "page": "/produto/smartphone",
    "device": "mobile",
    "referrer": "google",
    "country": "BR",
    "utm_source": "google",
    "utm_campaign": "promocao_verao"
  }
}
```

**Response (201):**
```json
{
  "event_id": "550e8400-e29b-41d4-a716-446655440000",
  "status": "accepted",
  "processed_at": "2025-08-07T17:25:19Z"
}
```

**Campos Obrigatórios:**
- `event_type`: Tipo do evento (page_view, click, purchase, etc.)
- `timestamp`: Data/hora do evento em ISO8601
- `user_id`: Identificador único do usuário

**Campos Opcionais:**
- `session_id`: ID da sessão do usuário
- `payload`: Dados adicionais do evento (JSON)

---

### POST /api/v1/events/batch

Coleta múltiplos eventos em uma única requisição (até 100 eventos).

**Request Body:**
```json
{
  "events": [
    {
      "event_type": "page_view",
      "timestamp": "2025-08-07T17:25:18Z",
      "user_id": "user_12345",
      "session_id": "sess_abc123",
      "payload": {
        "page": "/home",
        "device": "mobile"
      }
    },
    {
      "event_type": "click",
      "timestamp": "2025-08-07T17:25:25Z",
      "user_id": "user_12345",
      "session_id": "sess_abc123",
      "payload": {
        "element": "buy_button",
        "page": "/produto/smartphone"
      }
    }
  ]
}
```

**Response (201):**
```json
{
  "total_events": 2,
  "accepted": 2,
  "rejected": 0,
  "batch_id": "batch_550e8400",
  "processed_at": "2025-08-07T17:25:19Z"
}
```

---

## Analytics e Relatórios

### GET /api/v1/analytics/overview

Métricas gerais do dashboard principal.

**Parâmetros de Query:**
- `start_date` (obrigatório): Data de início (ISO8601)
- `end_date` (obrigatório): Data de fim (ISO8601)
- `device` (opcional): mobile, desktop, tablet
- `country` (opcional): Código do país (ISO 3166-1)

**Exemplo de Request:**
```http
GET /api/v1/analytics/overview?start_date=2025-08-01T00:00:00Z&end_date=2025-08-07T23:59:59Z&device=mobile
```

**Response (200):**
```json
{
  "periodo": {
    "start_date": "2025-08-01T00:00:00Z",
    "end_date": "2025-08-07T23:59:59Z"
  },
  "resumo": {
    "total_page_views": 145230,
    "usuarios_unicos": 28940,
    "sessoes_unicas": 42450,
    "media_paginas_por_sessao": 3.4,
    "duracao_media_sessao_minutos": 4.2,
    "taxa_rejeicao_percent": 42.5
  },
  "tendencias": {
    "crescimento_page_views_percent": 12.5,
    "crescimento_usuarios_percent": 8.3
  },
  "paginas_mais_visitadas": [
    {
      "page": "/produto/smartphone",
      "views": 12340,
      "usuarios_unicos": 8900,
      "tempo_medio_minutos": 2.1
    }
  ]
}
```

**Cache:** 5 minutos

---

### GET /api/v1/analytics/sources

Análise detalhada das fontes de tráfego.

**Parâmetros de Query:**
- `start_date` (obrigatório): Data de início
- `end_date` (obrigatório): Data de fim
- `source_type` (opcional): organic, direct, social, referral

**Response (200):**
```json
{
  "fontes_trafego": {
    "organico": {
      "sessoes": 18092,
      "percentual": 40.0,
      "taxa_rejeicao": 35.2
    },
    "direto": {
      "sessoes": 13569,
      "percentual": 30.0,
      "taxa_rejeicao": 28.1
    },
    "social": {
      "sessoes": 9046,
      "percentual": 20.0,
      "taxa_rejeicao": 52.3
    },
    "referencia": {
      "sessoes": 4523,
      "percentual": 10.0,
      "taxa_rejeicao": 45.7
    }
  },
  "principais_referenciadores": [
    {
      "dominio": "google.com",
      "sessoes": 15234,
      "taxa_conversao": 8.5
    },
    {
      "dominio": "facebook.com", 
      "sessoes": 7892,
      "taxa_conversao": 4.2
    }
  ]
}
```

**Cache:** 10 minutos

---

### GET /api/v1/analytics/devices

Distribuição por dispositivos e navegadores.

**Response (200):**
```json
{
  "dispositivos": {
    "mobile": {
      "sessoes": 27138,
      "percentual": 60.0,
      "duracao_media_sessao": 3.2,
      "taxa_rejeicao": 45.1
    },
    "desktop": {
      "sessoes": 15407,
      "percentual": 34.1,
      "duracao_media_sessao": 5.8,
      "taxa_rejeicao": 32.4
    },
    "tablet": {
      "sessoes": 2685,
      "percentual": 5.9,
      "duracao_media_sessao": 4.1,
      "taxa_rejeicao": 38.7
    }
  },
  "navegadores": [
    {
      "nome": "Chrome",
      "sessoes": 25467,
      "percentual": 56.4
    },
    {
      "nome": "Safari",
      "sessoes": 12890,
      "percentual": 28.5
    },
    {
      "nome": "Firefox",
      "sessoes": 4532,
      "percentual": 10.0
    }
  ],
  "sistemas_operacionais": [
    {
      "nome": "Android",
      "sessoes": 18234,
      "percentual": 40.3
    },
    {
      "nome": "iOS",
      "sessoes": 12456,
      "percentual": 27.6
    }
  ]
}
```

---

### GET /api/v1/analytics/geography

Distribuição geográfica do tráfego.

**Response (200):**
```json
{
  "paises": [
    {
      "codigo_pais": "BR",
      "nome_pais": "Brasil",
      "sessoes": 28456,
      "percentual": 63.0,
      "duracao_media_sessao": 4.5,
      "taxa_rejeicao": 38.2
    },
    {
      "codigo_pais": "US",
      "nome_pais": "Estados Unidos",
      "sessoes": 8934,
      "percentual": 19.8,
      "duracao_media_sessao": 5.1,
      "taxa_rejeicao": 35.7
    }
  ],
  "principais_cidades": [
    {
      "cidade": "São Paulo",
      "pais": "BR",
      "sessoes": 12456,
      "percentual": 27.6
    },
    {
      "cidade": "Rio de Janeiro",
      "pais": "BR", 
      "sessoes": 8934,
      "percentual": 19.8
    }
  ]
}
```

---

## Monitoramento em Tempo Real

### GET /api/v1/analytics/realtime

Métricas em tempo real dos últimos 30 minutos.

**Response (200):**
```json
{
  "estatisticas_atuais": {
    "usuarios_ativos": 1247,
    "eventos_por_minuto": 234,
    "paginas_mais_acessadas_agora": [
      {
        "page": "/produto/smartphone",
        "usuarios_ativos": 156
      },
      {
        "page": "/home",
        "usuarios_ativos": 98
      }
    ]
  },
  "fontes_trafego_agora": {
    "organico": 45.2,
    "direto": 28.1,
    "social": 18.7,
    "pago": 8.0
  },
  "alertas": [
    {
      "tipo": "pico_trafego",
      "mensagem": "Tráfego aumentou 150% nos últimos 10 minutos",
      "severidade": "warning",
      "timestamp": "2025-08-07T17:20:00Z"
    }
  ]
}
```

**Cache:** 30 segundos

---

### WebSocket /ws/realtime

Conexão WebSocket para atualizações em tempo real.

**Conexão:**
```javascript
const ws = new WebSocket('ws://localhost:8080/ws/realtime');
ws.onmessage = function(event) {
    const data = JSON.parse(event.data);
    // Processar dados em tempo real
};
```

**Mensagens recebidas a cada 30 segundos:**
```json
{
  "type": "realtime_update",
  "timestamp": "2025-08-07T17:25:30Z",
  "data": {
    "usuarios_ativos": 1247,
    "eventos_por_minuto": 234
  }
}
```

---

## Sistema de Alertas

### GET /api/v1/alerts/rules

Lista todas as regras de alerta configuradas.

**Response (200):**
```json
{
  "rules": [
    {
      "id": "550e8400-e29b-41d4-a716-446655440000",
      "nome": "Alerta Alto Tráfego",
      "metric_name": "eventos_por_minuto",
      "condicao": {
        "operador": "maior_que",
        "limite": 500
      },
      "notification_url": "https://hooks.slack.com/webhook-url",
      "ativo": true,
      "criado_em": "2025-08-01T10:00:00Z"
    }
  ]
}
```

---

### POST /api/v1/alerts/rules

Cria uma nova regra de alerta.

**Request Body:**
```json
{
  "nome": "Alerta Alto Tráfego",
  "metric_name": "eventos_por_minuto",
  "condicao": {
    "operador": "maior_que",
    "limite": 500
  },
  "notification_url": "https://hooks.slack.com/webhook-url",
  "ativo": true
}
```

**Response (201):**
```json
{
  "id": "550e8400-e29b-41d4-a716-446655440000",
  "status": "created",
  "message": "Regra de alerta criada com sucesso"
}
```

**Operadores disponíveis:**
- `maior_que`: >
- `menor_que`: <
- `igual_a`: =
- `diferente_de`: !=
- `maior_igual`: >=
- `menor_igual`: <=

---

### GET /api/v1/alerts/history

Histórico de alertas disparados.

**Parâmetros de Query:**
- `start_date` (opcional): Filtrar por data de início
- `severity` (opcional): info, warning, error
- `limit` (opcional): Número máximo de resultados (padrão: 50)

**Response (200):**
```json
{
  "alerts": [
    {
      "id": "alert_123",
      "rule_name": "Alerta Alto Tráfego",
      "message": "Tráfego ultrapassou 500 eventos/min: 678 eventos/min",
      "severity": "warning",
      "triggered_at": "2025-08-07T15:30:00Z",
      "status": "sent"
    }
  ],
  "total": 1,
  "has_more": false
}
```

---

## Modelos de Dados

### Event

Entidade principal para armazenamento de eventos.

```csharp
public class Event
{
    public Guid Id { get; set; }
    public string EventType { get; set; }     // page_view, click, purchase
    public DateTime Timestamp { get; set; }
    public string UserId { get; set; }
    public string SessionId { get; set; }     // ID da sessão
    public JsonDocument Payload { get; set; } // Dados adicionais
    public string TenantId { get; set; }      // Multi-tenancy
    public DateTime CreatedAt { get; set; }
}
```

### AggregatedMetric

Métricas pré-calculadas para otimização de consultas.

```csharp
public class AggregatedMetric
{
    public Guid Id { get; set; }
    public string MetricName { get; set; }    // daily_page_views, unique_users
    public DateTime Date { get; set; }        // Data da agregação
    public double Value { get; set; }         // Valor da métrica
    public JsonDocument Dimensions { get; set; } // device, country, source
    public string TenantId { get; set; }
    public DateTime UpdatedAt { get; set; }
}
```

### AlertRule

Configuração de regras de alerta.

```csharp
public class AlertRule
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string MetricName { get; set; }
    public string Condition { get; set; }     // Condição em JSON
    public string NotificationUrl { get; set; } // Webhook URL
    public bool IsActive { get; set; }
    public string TenantId { get; set; }
    public DateTime CreatedAt { get; set; }
}
```

---

## Configuração Avançada

### Variáveis de Ambiente

```bash
# Database
DATABASE_URL=postgresql://user:password@localhost:5432/analytics
DATABASE_POOL_SIZE=20

# Redis
REDIS_URL=redis://localhost:6379
REDIS_TTL_OVERVIEW=300
REDIS_TTL_SOURCES=600
REDIS_TTL_REALTIME=30

# RabbitMQ
RABBITMQ_URL=amqp://admin:admin@localhost:5672
RABBITMQ_QUEUE_EVENTS=analytics.events
RABBITMQ_PREFETCH_COUNT=100

# API
API_PORT=8080
API_KEY_HEADER=X-API-Key
TENANT_HEADER=X-Tenant-ID

# Rate Limiting
RATE_LIMIT_EVENTS=5000
RATE_LIMIT_ANALYTICS=1000
RATE_LIMIT_REALTIME=2000

# Workers
WORKER_BATCH_SIZE=100
WORKER_PROCESSING_DELAY=1000
AGGREGATION_INTERVAL=3600
```

### Estrutura de Cache (Redis)

| Chave | TTL | Descrição |
|-------|-----|-----------|
| `analytics:overview:{tenant}:{hash}` | 5 min | Métricas gerais |
| `analytics:sources:{tenant}:{hash}` | 10 min | Fontes de tráfego |
| `analytics:devices:{tenant}:{hash}` | 10 min | Análise de dispositivos |
| `analytics:geography:{tenant}:{hash}` | 15 min | Dados geográficos |
| `analytics:realtime:{tenant}` | 30 seg | Métricas em tempo real |

### Workers Background

| Worker | Frequência | Função |
|--------|------------|--------|
| **EventProcessorWorker** | Contínuo | Processa eventos da fila RabbitMQ |
| **MetricAggregationWorker** | A cada hora | Calcula métricas agregadas |
| **AlertProcessingWorker** | A cada minuto | Verifica condições de alerta |
| **CacheWarmupWorker** | A cada 5 min | Pré-aquece cache com consultas comuns |

---

## Performance e Benchmarks

### Metas de Performance

| Métrica | Target | Medição |
|---------|--------|---------|
| Ingestão de Eventos | < 50ms P95 | Tempo de resposta da API |
| Consultas Analytics | < 200ms P95 | Tempo de resposta com cache |
| Consultas Tempo Real | < 100ms P95 | Tempo de resposta WebSocket |
| Taxa de Acerto Cache | > 90% | Hit rate no Redis |
| Throughput | 5000 req/min | Por tenant |

### Executar Benchmarks

**Performance Benchmarks:**
```bash
cd Analytics.Benchmarks
dotnet run -c Release
```

**Testes de Carga (k6):**
```bash
# Teste de ingestão de eventos
k6 run tests/load-tests/event-ingestion.js

# Teste de consultas analytics
k6 run tests/load-tests/analytics-queries.js

# Teste de tempo real
k6 run tests/load-tests/realtime-queries.js
```

### Monitoramento Recomendado

**Métricas de Sistema:**
- CPU e Memória da aplicação
- Conexões ativas do PostgreSQL
- Uso de memória do Redis
- Tamanho da fila RabbitMQ

**Métricas de Negócio:**
- Eventos processados por minuto
- Tempo médio de processamento
- Taxa de erro da API
- Latência por endpoint

---

## Troubleshooting

### Problemas Comuns

**1. Alta latência na API**
- Verificar hit rate do cache Redis
- Analisar queries lentas no PostgreSQL
- Verificar conexões de banco disponíveis

**2. Eventos não sendo processados**
- Verificar status do RabbitMQ
- Conferir logs do EventProcessorWorker
- Validar formato dos eventos enviados

**3. Alertas não disparando**
- Verificar se AlertProcessingWorker está ativo
- Confirmar configuração das regras
- Testar webhook de notificação

**4. Cache ineficiente**
- Ajustar TTL das chaves Redis
- Verificar padrões de consulta
- Analisar distribuição de tenants

### Logs e Debugging

**Níveis de Log:**
- `Information`: Operações normais
- `Warning`: Situações que requerem atenção
- `Error`: Erros que impedem processamento
- `Debug`: Informações detalhadas para debug

**Localização dos Logs:**
```bash
# Logs da aplicação
docker logs analytics-api

# Logs dos workers
docker logs analytics-worker

# Logs do RabbitMQ
docker logs analytics-rabbitmq
```

### Comandos Úteis

```bash
# Status dos containers
docker compose ps

# Logs em tempo real
docker compose logs -f analytics-api

# Reiniciar serviços
docker compose restart analytics-api

# Limpar cache Redis
docker exec -it analytics-redis redis-cli FLUSHDB

# Verificar filas RabbitMQ
docker exec -it analytics-rabbitmq rabbitmqctl list_queues
```

---

## Contribuição

Contribuições são bem-vindas! Para contribuir:

1. Fork o projeto
2. Crie uma branch para sua feature
3. Commit suas mudanças
4. Abra um Pull Request

### Desenvolvimento Local

```bash
# Instalar dependências
dotnet restore

# Executar testes
dotnet test

# Executar aplicação
dotnet run --project Analytics.API
```

---

**Versão:** 1.0  
**Última Atualização:** 07/08/2025  
**Licença:** MIT
