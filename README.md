# Brainz - Gestao de Usuarios e Eventos

Aplicacao fullstack para gerenciamento de usuarios e eventos de uma instituicao de ensino, com integracao ao Microsoft Graph para sincronizacao periodica de dados.

## Recursos Implementados

### Backend (.NET 8 Web API)
- **Clean Architecture** com 4 camadas: Domain, Application, Infrastructure, API
- **Entity Framework Core** com SQL Server e migrations
- **JWT Bearer Authentication** para protecao dos endpoints
- **Microsoft Graph Integration** para busca de usuarios e eventos
- **Hangfire** para sincronizacao periodica em background (usuarios a cada 1h, eventos a cada 2h)
- **Swagger/OpenAPI** para documentacao interativa da API
- **Middleware de excecoes** para tratamento global de erros
- **Padrao RESTful** com paginacao e busca

### Frontend (React + Vite + TypeScript)
- **Tailwind CSS** para estilizacao responsiva
- **React Router** para navegacao SPA com rotas protegidas
- **Axios** com interceptors para autenticacao automatica
- **Hooks customizados** para gerenciamento de estado e dados
- **Debounce na busca** para otimizacao de requisicoes
- **Layout responsivo** adaptado para mobile, tablet e desktop

### API Endpoints
| Metodo | Rota | Auth | Descricao |
|--------|------|------|-----------|
| POST | `/api/auth/login` | Nao | Autenticacao (retorna JWT) |
| GET | `/api/users` | Sim | Lista usuarios (paginado + busca) |
| GET | `/api/users/{id}` | Sim | Detalhe do usuario |
| GET | `/api/users/{id}/events` | Sim | Eventos do usuario (paginado) |
| GET | `/api/sync/logs` | Sim | Logs de sincronizacao |
| POST | `/api/sync/trigger?type=Full` | Sim | Dispara sincronizacao manual |

## Pre-requisitos

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- [Node.js 18+](https://nodejs.org/)
- [Docker Desktop](https://www.docker.com/products/docker-desktop/)

## Como Rodar o Projeto

### 1. Subir o banco de dados
```bash
docker-compose up -d
```

### 2. Rodar o backend
```bash
cd backend
dotnet run --project src/Brainz.Api
```
As migrations sao aplicadas automaticamente ao iniciar em modo Development.
O backend estara disponivel em `http://localhost:5109` com Swagger em `http://localhost:5109/swagger`.

### 3. Rodar o frontend
```bash
cd frontend
npm install
npm run dev
```
O frontend estara disponivel em `http://localhost:5173`.

### Login
- **Usuario:** admin
- **Senha:** YOUR_ADMIN_PASSWORD

## Como Rodar os Testes

```bash
cd backend
dotnet test
```

Os testes unitarios cobrem:
- **AuthService** - autenticacao com credenciais validas/invalidas
- **UserService** - listagem paginada e busca por ID
- **EventService** - listagem paginada de eventos por usuario
- **SyncService** - sincronizacao de usuarios e eventos, tratamento de erros

## Arquitetura

```
backend/
├── src/
│   ├── Brainz.Domain/          # Entidades, Interfaces, Enums
│   ├── Brainz.Application/     # Services, DTOs, Interfaces
│   ├── Brainz.Infrastructure/  # EF Core, Graph, Hangfire, JWT
│   └── Brainz.Api/             # Controllers, Middleware, Config
└── tests/
    └── Brainz.UnitTests/       # xUnit + Moq + FluentAssertions

frontend/
└── src/
    ├── api/          # Camada de comunicacao com a API
    ├── components/   # Componentes reutilizaveis (UI, Layout, Users, Events)
    ├── context/      # Contexto de autenticacao (React Context)
    ├── hooks/        # Hooks customizados (useAuth, useUsers, useEvents, useDebounce)
    ├── pages/        # Paginas da aplicacao
    ├── router/       # Configuracao de rotas e rotas protegidas
    ├── types/        # Tipos TypeScript
    └── utils/        # Utilitarios (token storage, formatacao de datas)
```

## Decisoes Tecnicas

- **Sincronizacao periodica com Hangfire**: Os dados do Microsoft Graph sao sincronizados para uma base interna, evitando chamadas diretas a API externa em cada requisicao e melhorando a performance para alto volume de usuarios.
- **DTOs filtrados**: Apenas dados essenciais sao enviados ao frontend, excluindo informacoes sensiveis como IDs do Microsoft Graph e emails de organizadores.
- **Paginacao em todos os endpoints**: Preparado para alto volume de dados com busca server-side e paginacao.
- **Upsert para usuarios, replace para eventos**: Usuarios sao atualizados incrementalmente; eventos em janela de tempo sao substituidos para garantir dados atualizados.

## Tecnologias

- .NET 8, Entity Framework Core, SQL Server, Hangfire, JWT, Swagger
- React, Vite, TypeScript, Tailwind CSS, Axios, React Router
- Docker, xUnit, Moq, FluentAssertions
