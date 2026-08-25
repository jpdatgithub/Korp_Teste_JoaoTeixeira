# KorpERP

## Pré-requisitos

Antes de iniciar, instale:

- Docker
- .NET SDK
- Node.js
- npm
- Entity Framework Core CLI (`dotnet-ef`)

Para instalar o `dotnet-ef`, execute:

```powershell
dotnet tool install --global dotnet-ef
```

## Quickstart

Clone o repositório e entre na pasta do projeto:

```powershell
git clone <URL_DO_REPOSITORIO>
cd Korp_Teste_JoaoTeixeira
```

Inicie os bancos de dados e o RabbitMQ:

```powershell
docker compose up -d
```

Instale as dependências do frontend:

```powershell
npm install --prefix .\KorpERP-angular-front
```

Atualize os bancos de dados:

```powershell
.\db-update.ps1
```

Inicie as APIs e o frontend:

```powershell
.\run.ps1
```