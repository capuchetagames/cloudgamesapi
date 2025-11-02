# Cloud Games API

API RESTful para gerenciamento de uma plataforma de jogos em nuvem, desenvolvida com ASP.NET Core 8.0 seguindo os princípios de Clean Architecture.

## 📋 Sobre o Projeto

A Cloud Games API é uma aplicação backend completa para gerenciar usuários, jogos e pedidos em uma plataforma de jogos em nuvem. O projeto implementa autenticação JWT, autorização baseada em roles, validação de dados e cache em memória para otimização de performance.

## 🏗️ Arquitetura

O projeto segue os princípios de **Clean Architecture**, organizado em três camadas principais:

### 📂 Estrutura do Projeto

```
cloudgamesapi/
├── CloudGamesApi/          # Camada de Apresentação (API)
│   ├── Controllers/        # Controladores da API
│   ├── Configs/           # Configurações (Swagger, etc)
│   ├── Middlewares/       # Middlewares customizados
│   ├── Service/           # Serviços e validadores
│   └── Program.cs         # Ponto de entrada da aplicação
├── Core/                  # Camada de Domínio
│   ├── Entity/           # Entidades do domínio
│   ├── Dtos/             # Data Transfer Objects
│   ├── Repository/       # Interfaces de repositórios
│   └── Models/           # Interfaces de serviços
├── Infrastructure/        # Camada de Infraestrutura
│   ├── Repository/       # Implementação dos repositórios
│   └── Migrations/       # Migrações do Entity Framework
└── CloudGamesApi.Tests/  # Testes unitários
```

## 🚀 Tecnologias Utilizadas

### Framework e Linguagem
- **.NET 8.0** - Framework principal
- **C#** - Linguagem de programação
- **ASP.NET Core Web API** - Framework para APIs RESTful

### Banco de Dados
- **Entity Framework Core 9.0** - ORM
- **MySQL/MariaDB** - Banco de dados relacional
- **Pomelo.EntityFrameworkCore.MySql** - Provider MySQL para EF Core

### Segurança e Autenticação
- **JWT (JSON Web Tokens)** - Autenticação stateless
- **ASP.NET Core Identity (PasswordHasher)** - Hash seguro de senhas
- **Microsoft.AspNetCore.Authentication.JwtBearer** - Middleware de autenticação JWT

### Validação e Documentação
- **FluentValidation** - Validação de dados
- **Swagger/OpenAPI** - Documentação interativa da API
- **ReDoc** - Documentação alternativa da API

### Cache e Performance
- **IMemoryCache** - Cache em memória para otimização

## 📋 Pré-requisitos

Antes de executar o projeto, certifique-se de ter instalado:

- [.NET SDK 8.0](https://dotnet.microsoft.com/download/dotnet/8.0) ou superior
- [MySQL](https://www.mysql.com/downloads/) ou [MariaDB](https://mariadb.org/download/)
- IDE recomendada: [Visual Studio 2022](https://visualstudio.microsoft.com/), [Visual Studio Code](https://code.visualstudio.com/) ou [JetBrains Rider](https://www.jetbrains.com/rider/)

## ⚙️ Configuração

### 1. Clone o Repositório

```bash
git clone https://github.com/fiapcloudgames/cloudgamesapi.git
cd cloudgamesapi
```

### 2. Configure o Banco de Dados

Edite o arquivo `CloudGamesApi/appsettings.json` e configure a connection string do MySQL:

```json
{
  "ConnectionStrings": {
    "ConnectionString": "server=localhost;database=cloudgames;user=seu_usuario;password=sua_senha"
  }
}
```

### 3. Configure o JWT

No arquivo `CloudGamesApi/appsettings.Development.json`, configure as chaves JWT:

```json
{
  "Jwt": {
    "Key": "sua_chave_secreta_aqui_minimo_32_caracteres",
    "Issuer": "CloudGamesApi"
  }
}
```

> ⚠️ **IMPORTANTE**: Em produção, nunca exponha a chave JWT no código. Use variáveis de ambiente ou serviços de gerenciamento de segredos.

### 4. Crie o Banco de Dados

Execute as migrações do Entity Framework:

```bash
dotnet ef database update --project Infrastructure --startup-project CloudGamesApi
```

Ou, se preferir, restaure e aplique as migrações:

```bash
dotnet restore
dotnet build
dotnet ef database update --project Infrastructure --startup-project CloudGamesApi
```

## 🏃 Executando o Projeto

### Modo Desenvolvimento

```bash
cd CloudGamesApi
dotnet run
```

A API estará disponível em:
- **HTTPS**: `https://localhost:7000` (ou porta configurada)
- **HTTP**: `http://localhost:5000` (ou porta configurada)

### Acessar a Documentação

Após iniciar a aplicação em modo desenvolvimento:

- **Swagger UI**: `https://localhost:7000/swagger`
- **ReDoc**: `https://localhost:7000/api-docs` (se configurado)

## 📚 Entidades do Domínio

### User (Usuário)
```csharp
- Id: int
- Name: string
- Email: string
- Password: string (hash)
- Permission: PermissionType (Admin, User)
- Orders: ICollection<Order>
```

### Game (Jogo)
```csharp
- Id: int
- Name: string
- Category: string
- Active: bool
- Price: decimal
- Orders: ICollection<Order>
```

### Order (Pedido)
```csharp
- Id: int
- UserId: int
- GameId: int
- User: User
- Game: Game
```

## 🔐 Autenticação e Autorização

A API utiliza **JWT (JSON Web Tokens)** para autenticação. 

### Tipos de Permissão

- **Admin**: Acesso completo a todos os endpoints
- **User**: Acesso limitado a operações de leitura e suas próprias informações

### Fluxo de Autenticação

1. Registrar um novo usuário: `POST /new-user`
2. Fazer login: `POST /Auth` (retorna um token JWT)
3. Incluir o token no header das requisições: `Authorization: Bearer {seu_token}`

## 📡 Endpoints da API

### Authentication

| Método | Endpoint | Descrição | Autenticação |
|--------|----------|-----------|--------------|
| POST | `/Auth` | Login e geração de token JWT | Pública |

### Users

| Método | Endpoint | Descrição | Autenticação |
|--------|----------|-----------|--------------|
| GET | `/User` | Lista todos os usuários | Admin |
| GET | `/User/{id}` | Busca usuário por ID | Admin |
| GET | `/User/{id}/games` | Lista jogos do usuário | Autenticado |
| POST | `/new-user` | Registra novo usuário (role User) | Pública |
| POST | `/User` | Cria usuário com role específica | Admin |
| PUT | `/User` | Atualiza usuário | Admin |
| DELETE | `/User/{id}` | Deleta usuário | Admin |

### Games

| Método | Endpoint | Descrição | Autenticação |
|--------|----------|-----------|--------------|
| GET | `/Games` | Lista todos os jogos (com cache) | Autenticado |
| GET | `/Games/{id}` | Busca jogo por ID (com cache) | Autenticado |
| POST | `/Games` | Cria novo jogo | Admin |
| PUT | `/Games` | Atualiza jogo | Admin |
| DELETE | `/Games/{id}` | Deleta jogo | Admin |

### Exemplo de Requisição

#### Registrar Novo Usuário
```bash
curl -X POST https://localhost:7000/new-user \
  -H "Content-Type: application/json" \
  -d '{
    "name": "João Silva",
    "email": "joao@example.com",
    "password": "senha123"
  }'
```

#### Login
```bash
curl -X POST https://localhost:7000/Auth \
  -H "Content-Type: application/json" \
  -d '{
    "name": "João Silva",
    "password": "senha123"
  }'
```

#### Listar Jogos (com autenticação)
```bash
curl -X GET https://localhost:7000/Games \
  -H "Authorization: Bearer {seu_token_jwt}"
```

## 🧪 Testes

Para executar os testes unitários:

```bash
dotnet test
```

Ou para executar com cobertura:

```bash
dotnet test /p:CollectCoverage=true
```

## 🔧 Funcionalidades Principais

### ✅ Cache em Memória
- A listagem de jogos utiliza cache para melhorar a performance
- Cache individual por jogo para buscas otimizadas

### ✅ Validação de Dados
- Implementação de FluentValidation para validação robusta
- Validadores customizados para Login e User

### ✅ Logging
- Sistema de logs com correlation ID para rastreamento de requisições
- BaseLogger customizado para logs estruturados

### ✅ Middleware Customizado
- LogMiddleware para interceptar e logar todas as requisições

### ✅ Segurança
- Senhas armazenadas com hash seguro (PasswordHasher)
- Tokens JWT com expiração de 30 minutos
- Autorização baseada em políticas (Policy-based)
- Proteção contra ataques comuns

## 📝 Configurações Adicionais

### CORS (Cross-Origin Resource Sharing)

Para habilitar CORS, adicione no `Program.cs`:

```csharp
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll",
        builder => builder
            .AllowAnyOrigin()
            .AllowAnyMethod()
            .AllowAnyHeader());
});

app.UseCors("AllowAll");
```

### Variáveis de Ambiente

Recomenda-se usar variáveis de ambiente para configurações sensíveis:

```bash
export ConnectionStrings__ConnectionString="server=localhost;database=cloudgames;user=root;password=senha"
export Jwt__Key="sua_chave_secreta"
export Jwt__Issuer="CloudGamesApi"
```

## 🚀 Deploy

### Build para Produção

```bash
dotnet publish -c Release -o ./publish
```

### Docker (Exemplo)

Crie um `Dockerfile`:

```dockerfile
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["CloudGamesApi/CloudGamesApi.csproj", "CloudGamesApi/"]
COPY ["Core/Core.csproj", "Core/"]
COPY ["Infrastructure/Infrastructure.csproj", "Infrastructure/"]
RUN dotnet restore "CloudGamesApi/CloudGamesApi.csproj"
COPY . .
WORKDIR "/src/CloudGamesApi"
RUN dotnet build "CloudGamesApi.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "CloudGamesApi.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "CloudGamesApi.dll"]
```

## 🤝 Contribuindo

1. Faça um Fork do projeto
2. Crie uma branch para sua feature (`git checkout -b feature/MinhaFeature`)
3. Commit suas mudanças (`git commit -m 'Adiciona MinhaFeature'`)
4. Push para a branch (`git push origin feature/MinhaFeature`)
5. Abra um Pull Request

## 📄 Licença

Este projeto é parte do curso FIAP Cloud Games.

## 👥 Autores

Desenvolvido pela equipe FIAP Cloud Games.

## 📞 Suporte

Para suporte e dúvidas:
- Abra uma issue no GitHub
- Entre em contato através do repositório

---

**Desenvolvido com ❤️ usando .NET 8.0 e Clean Architecture**