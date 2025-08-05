

# 📊 Analytics-System: Plataforma de Análise de Dados e Consultas Avançadas

[](https://github.com/mateusgomst/analytics-system)

## 🎯 Sobre o Projeto

Este projeto é uma **API de alta performance** desenvolvida para coletar, processar e servir grandes volumes de dados de forma eficiente. Construída com os princípios de **Clean Architecture**, o projeto garante alta testabilidade, separação de responsabilidades e fácil manutenção, tornando-o ideal para cenários onde a velocidade e a baixa latência são cruciais.

### 💡 Casos de Uso e Aplicações Práticas

A arquitetura desta API é versátil e pode ser aplicada em diversos cenários que exigem processamento e análise de dados escaláveis:

  - **Monitoramento de Eventos em Tempo Real:** Coleta eventos de usuários (cliques, acessos, compras) para alimentar dashboards analíticos, criar alertas dinâmicos e gerar relatórios de comportamento.
  - **Análise de Dados de IoT:** Processa telemetria de sensores e dispositivos, permitindo a criação de relatórios de métricas, detecção de anomalias e dashboards de monitoramento.
  - **Sistema de Logs Centralizados:** Atua como um agregador de logs, processando-os em segundo plano para consultas rápidas, filtros avançados e alertas automáticos sobre a saúde das aplicações.
  - **Plataforma de Notificações Personalizadas:** Orquestra o envio de notificações (push, email) com base em eventos e regras de negócio, fornecendo relatórios de entrega e engajamento em tempo real.

**Exemplo:** Um e-commerce envia eventos de vendas para a API → O sistema processa os dados de forma assíncrona → Um dashboard de BI consulta `GET /relatorio/vendas?periodo=mensal` e recebe a resposta em milissegundos.

### ⚡ Estratégias de Performance

Para garantir a performance necessária, o projeto utiliza estratégias avançadas:

  - **Cache-Aside:** Otimiza o acesso a dados frequentes com **Redis**, reduzindo a carga no banco de dados e acelerando as consultas.
  - **Processamento Assíncrono:** Utiliza o **RabbitMQ** e Background Services para processar grandes volumes de dados em segundo plano, como agregações e indexação, garantindo que a API responda rapidamente.
  - **Queries Otimizadas:** Aproveita **projeções e índices de banco de dados** para buscar apenas o necessário, além de aplicar princípios de **CQRS** (Command Query Responsibility Segregation).

-----

## 🚀 Guia de Execução

Siga os passos abaixo para configurar e rodar a aplicação localmente.

### Pré-requisitos

Certifique-se de que as seguintes ferramentas estão instaladas:

  * **[.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)**
  * **[Docker](https://www.docker.com/products/docker-desktop/)** e **[Docker Compose](https://docs.docker.com/compose/install/)**
  * **[Git](https://git-scm.com/downloads/)**
  * **(Opcional)** **[k6](https://k6.io/docs/getting-started/installation/)** para testes de carga.

### 1\. Clone o Repositório

```bash
git clone https://github.com/mateusgomst/analytics-system.git
cd analytics-system
```

### 2\. Configure o Ambiente

Copie o arquivo de exemplo para criar seu arquivo de ambiente local e **altere as senhas** (`POSTGRES_PASSWORD` e `RABBITMQ_PASS`) para valores de sua preferência.

```bash
cp .env.example .env
```

### 3\. Inicie a Aplicação com Docker Compose

Este comando construirá as imagens e iniciará todos os serviços necessários (API, Banco de Dados, Redis e RabbitMQ).

```bash
docker compose up -d --build
```

### 4\. Verifique os Serviços

Após a execução, os serviços estarão disponíveis nos seguintes endereços:

| Serviço | URL de Acesso | Credenciais (padrão do `.env.example`) |
|:---|:---|:---|
| **API (Swagger)** | `http://localhost:8080/swagger` | N/A |
| **RabbitMQ Management** | `http://localhost:15672` | `user: admin` / `pass: admin` |
| **PostgreSQL** | `localhost:5432` | N/A |
| **Redis** | `localhost:6379` | N/A |

### 5\. Executando Benchmarks e Testes

  * **Benchmarks de Performance:**
    ```bash
    dotnet run -c Release --project Analytics.Benchmarks
    ```
  * **Testes de Carga (k6):**
    ```bash
    # Na pasta do projeto:
    k6 run tests/load-tests/relatorio-vendas-mensais.js
    ```

### 6\. Parando a Aplicação

Para parar todos os contêineres:

```bash
docker compose down
```

Para remover os contêineres e os volumes (incluindo os dados do banco):

```bash
docker compose down -v
```

-----

## 🧠 Tecnologias e Arquitetura

| Tema | Tecnologias / Práticas |
|:---|:---|
| **Linguagem e Framework** | **.NET 8** |
| **Banco de Dados** | **PostgreSQL** |
| **Mensageria** | **RabbitMQ** |
| **Cache** | **Redis** |
| **Testes de Performance** | **BenchmarkDotNet**, **k6** |
| **Monitoramento** | **Serilog**, **Application Insights** ou **Seq** |
| **Arquitetura** | **Clean Architecture**, **CQRS**, **Programação Assíncrona** |

-----

## 📦 Estrutura do Projeto

O projeto segue os princípios da **Clean Architecture**, com uma clara separação de responsabilidades para garantir a manutenção e a escalabilidade.

```
/
├── Analytics.API/             # Ponto de entrada HTTP, expõe os endpoints.
├── Analytics.Application/     # Camada de orquestração e casos de uso, livre de dependências externas.
├── Analytics.Domain/          # O coração do sistema: entidades e regras de negócio (independente de outras camadas).
├── Analytics.Infrastructure/  # Implementações de serviços externos e acesso a dados (banco, cache, etc.).
├── Analytics.Benchmarks/      # Testes de performance com BenchmarkDotNet.
└── tests/
    └── load-tests/          # Scripts para testes de carga com k6.
```
