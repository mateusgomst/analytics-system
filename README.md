
📊 Analytics-System: Plataforma de Análise de Dados, Métricas e Eventos em Tempo Real
🎯 Sobre o Projeto
Analytics-System é uma plataforma de alta performance para coleta, processamento, agregação e análise de grandes volumes de eventos e métricas em tempo real. A arquitetura baseada em Clean Architecture e processamento assíncrono garante alta testabilidade, escalabilidade horizontal e baixa latência.

Projetado para cenários onde o tempo de resposta é crítico, o sistema permite desde análises simples até consultas avançadas, alertas dinâmicos e relatórios em tempo real.

💡 Casos de Uso e Aplicações
A flexibilidade do sistema permite que ele seja aplicado em diferentes contextos:

📈 Monitoramento de Eventos em Tempo Real: Coleta eventos de usuários (cliques, acessos, compras) e alimenta dashboards analíticos e relatórios.

📡 Análise de Dados de IoT: Processa telemetria de sensores e dispositivos, detectando falhas, anomalias e tendências.

🪵 Centralização de Logs: Agrega e processa logs em segundo plano, permitindo consultas rápidas, filtros e alertas sobre a saúde do sistema.

🔔 Geração de Alertas Automatizados: Detecta padrões anormais ou métricas fora do esperado e dispara alertas configuráveis (ex: picos de acesso, quedas de venda, falhas de serviço).

📨 Plataforma de Notificações Personalizadas: Orquestra notificações baseadas em eventos e regras, rastreando métricas de entrega e engajamento.

📊 Dashboards de BI via API: Exemplo: Um e-commerce envia eventos de vendas para a API → Os dados são agregados → O dashboard consome GET /relatorio/vendas?periodo=mensal com resposta em milissegundos.

📌 Recursos e Funcionalidades
✅ Coleta de eventos via HTTP (REST)

✅ Processamento assíncrono e desacoplado com RabbitMQ

✅ Agregação de métricas (por hora, por dia, por usuário, etc.)

✅ Geração e visualização de relatórios rápidos via endpoints

✅ Suporte a alertas dinâmicos e configuráveis

✅ Detecção de anormalidades e padrões suspeitos

✅ Integração com Redis para cache

✅ Benchmark de performance e carga (BenchmarkDotNet + k6)

⚡ Estratégias de Performance
Estratégia	Descrição
Cache-Aside com Redis	Respostas de consultas frequentes são armazenadas em cache para minimizar acesso ao banco.
Processamento Assíncrono	Eventos são enviados para o RabbitMQ e processados em segundo plano por workers.
CQRS e Projeções	Leitura e escrita são desacopladas. Consultas usam projeções otimizadas.
Regras de Alertas Customizadas	Avaliação periódica de métricas e geração automática de alertas.
Indexação Estratégica	Uso eficiente de índices e filtros no PostgreSQL para consultas de alta performance.

Exportar para as Planilhas
🚀 Guia de Execução
Pré-requisitos
.NET 8 SDK

Docker + Docker Compose

Git

(Opcional) k6 para testes de carga

1. Clone o Repositório
Bash

git clone https://github.com/mateusgomst/analytics-system.git
cd analytics-system
2. Configure o Ambiente
Copie o arquivo de exemplo para criar seu arquivo de ambiente local e altere senhas e variáveis no .env conforme necessário.

Bash

cp .env.example .env
3. Inicie com Docker Compose
Bash

docker compose up -d --build
4. Verifique os Serviços
Serviço	URL	Credenciais
API (Swagger)	http://localhost:8080/swagger	N/A
RabbitMQ Management	http://localhost:15672	admin / admin
PostgreSQL	localhost:5432	conforme .env
Redis	localhost:6379	N/A

Exportar para as Planilhas
5. Executando Benchmarks e Testes
Benchmark de Performance:

Bash

dotnet run -c Release --project Analytics.Benchmarks
Testes de Carga (k6):

Bash

k6 run tests/load-tests/relatorio-vendas-mensais.js
6. Encerrando os Serviços
Para parar os contêineres:

Bash

docker compose down
Para parar e remover os volumes (dados):

Bash

docker compose down -v
🧠 Tecnologias e Arquitetura
Tema	Tecnologias / Práticas
Linguagem e Framework	.NET 8
Banco de Dados	PostgreSQL
Mensageria	RabbitMQ
Cache	Redis
Testes de Performance	BenchmarkDotNet, k6
Monitoramento	Serilog, Application Insights ou Seq
Arquitetura	Clean Architecture, CQRS, Programação Assíncrona

Exportar para as Planilhas
🗂️ Estrutura do Projeto
O projeto segue os princípios da Clean Architecture, com uma clara separação de responsabilidades para garantir a manutenção e a escalabilidade.

/
├── Analytics.API/             # Ponto de entrada HTTP, expõe os endpoints.
├── Analytics.Application/     # Casos de uso e orquestração (sem dependências externas).
├── Analytics.Domain/          # Entidades, regras de negócio e contratos.
├── Analytics.Infrastructure/  # Acesso a banco, Redis, RabbitMQ, serviços externos.
├── Analytics.Benchmarks/      # Benchmarks de performance com BenchmarkDotNet.
└── tests/
    └── load-tests/            # Scripts de carga (ex: simulações com k6).
📣 Futuros Melhoramentos
Interface Web para criação de regras de alerta

Suporte a WebSockets para alertas em tempo real

Clusterização para suportar múltiplas instâncias da API

Exportação de relatórios em CSV/Excel

Integrações com ferramentas externas de BI (ex: Power BI, Grafana)

🤝 Contribuições
Sinta-se à vontade para abrir issues, pull requests ou propor ideias! 💡
