[![Repository](https://img.shields.io/badge/GitHub-analytics--system-blue?logo=github)](https://github.com/mateusgomst/analytics-system)

# 📊 Plataforma de Análise de Dados e Consultas Avançadas (BI Simplificado)

-----

## 🎯 O Que É Este Projeto?

Este projeto visa desenvolver uma **API de alta performance** projetada para receber, processar e servir grandes volumes de dados (como vendas, acessos e registros) com foco em **consultas rápidas, filtragem e cruzamento de informações**. O objetivo principal é simular um sistema de Business Intelligence (BI) simplificado, onde a eficiência e a baixa latência são críticas.

### 💡 Casos de Uso Práticos

A API é ideal para cenários onde você precisa:

- **Coletar eventos em tempo real** (vendas, cliques, acessos, transações)
- **Processar e agregar dados** de forma assíncrona em segundo plano
- **Servir consultas analíticas rápidas** como relatórios de vendas mensais, rankings de produtos, métricas de usuários
- **Cruzar informações** entre diferentes datasets para insights de negócio

**Exemplo:** Uma loja envia eventos de venda para a API → Sistema processa e organiza os dados → Dashboard consulta `GET /relatorio/vendas?periodo=mensal` e obtém resposta em milissegundos.

### ⚡ Estratégias de Performance

Para alcançar essa performance, a API emprega estratégias avançadas como:

- **Cache-aside:** Otimizando o acesso a dados frequentemente solicitados.
- **Leitura otimizada:** Utilizando projeções e índices de banco de dados para buscar apenas o necessário.
- **Pré-processamento e indexação:** Preparando e agregando dados em segundo plano para respostas quase instantâneas.

-----

## 🚀 Guia de Execução

Siga os passos abaixo para configurar e rodar a aplicação em seu ambiente local.

### Pré-requisitos

Antes de começar, garanta que você tenha as seguintes ferramentas instaladas:

* [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0) - **Necessário** para adicionar dependências e rodar benchmarks
* [Docker](https://www.docker.com/products/docker-desktop/) e [Docker Compose](https://docs.docker.com/compose/install/)
* [Git](https://git-scm.com/downloads/)
* (Opcional, para testes de carga) [k6](https://k6.io/docs/getting-started/installation/)

### 1. Clone o Repositório

```bash
git clone https://github.com/mateusgomst/analytics-system.git
cd analytics-system
```

### 2. Configure o Ambiente

O projeto utiliza um arquivo `.env` para gerenciar as variáveis de ambiente. Você pode começar copiando o arquivo de exemplo.

```bash
# Copie o arquivo de exemplo para criar seu arquivo de ambiente local
cp .env.example .env
```

**Importante:** Abra o arquivo `.env` recém-criado e **altere as senhas** (`POSTGRES_PASSWORD` e `RABBITMQ_PASS`) para valores de sua preferência.

### 3. Suba a Aplicação com Docker Compose

Com o Docker em execução, use o comando abaixo para construir as imagens e iniciar todos os serviços (API, Banco de Dados, Redis e RabbitMQ).

```bash
# O comando --build garante que a imagem da sua API será reconstruída caso haja mudanças no código.
docker compose up -d --build
```

### 4. Verifique se Tudo Está Rodando

Após a execução do comando, os serviços estarão disponíveis nos seguintes endereços:

| Serviço | URL de Acesso | Credenciais (padrão do .env.example) |
|:---|:---|:---|
| **API (Swagger)** | `http://localhost:8080/swagger` | N/A |
| **RabbitMQ Management** | `http://localhost:15672` | `user: admin` / `pass: admin` |
| **PostgreSQL** | `localhost:5432` | Use um cliente de DB (DBeaver, DataGrip). |
| **Redis** | `localhost:6379` | Use um cliente de Redis (RedisInsight). |

### 5. Adicionando Dependências

Para adicionar novas dependências (pacotes NuGet), você precisa fazer localmente e depois rebuildar:

```bash
# Exemplo: adicionar uma dependência no projeto Domain
dotnet add Analytics.Domain package FluentValidation

# Ou adicionar referência entre projetos
dotnet add Analytics.API reference Analytics.Application
dotnet add Analytics.Application reference Analytics.Domain
dotnet add Analytics.Infrastructure reference Analytics.Domain
dotnet add Analytics.API reference Analytics.Infrastructure

# Depois rebuildar os contêineres
docker compose up -d --build
```

### 6. Executando Benchmarks de Performance

Para rodar os benchmarks de performance:

```bash
dotnet run -c Release --project Analytics.Benchmarks
```

O BenchmarkDotNet irá compilar e executar os cenários de teste, exibindo uma tabela detalhada com os resultados de performance no final.

### 7. Executando Testes de Carga (k6)

Para validar a performance da API sob estresse, você pode usar os scripts do k6 localizados na pasta `tests/load-tests`.

```bash
# Navegue até a pasta de testes de carga (se existir)
cd tests/load-tests

# Execute o teste para o endpoint de relatório
k6 run relatorio-vendas-mensais.js
```

### 8. Parando a Aplicação

Para parar todos os contêineres, utilize o comando:

```bash
docker compose down
```

Se desejar remover também os volumes (e apagar os dados do banco), adicione a flag `-v`:

```bash
docker compose down -v
```

-----

## 🧠 Tecnologias e Conceitos Aplicados

| Tema | Tecnologias / Práticas |
|:---|:---|
| **Cache-aside** | **Redis** com TTL (Time-To-Live), **IMemoryCache** ou **IDistributedCache**. |
| **Pré-processamento** | **Background Services** (.NET) para gerar relatórios e dados agregados. |
| **Queries Otimizadas** | **Projeções com LINQ**, **índices de banco de dados**, princípios de **CQRS**. |
| **Logs e Métricas** | **Serilog** para logging, **Application Insights** ou **Seq** para monitoramento. |
| **Benchmark e Tuning** | **BenchmarkDotNet** e **MiniProfiler**. |
| **Particionamento e Paginação** | Estratégias para lidar com grandes tabelas em bancos de dados. |
| **Carga Simulada** | **k6** ou **JMeter** para simular tráfego e validar o *throughput*. |

-----

## 📦 Estrutura do Projeto

```
/
├── Analytics.API/              # Ponto de entrada HTTP, expõe os endpoints.
├── Analytics.Application/      # Casos de uso e lógica de orquestração.
├── Analytics.Domain/           # O coração do sistema: entidades e regras de negócio.
├── Analytics.Infrastructure/   # Implementações de serviços externos e acesso a dados.
├── Analytics.Benchmarks/       # Testes de performance com BenchmarkDotNet.
└── tests/
    └── load-tests/             # Scripts para testes de carga com k6.
```