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
cd .\KorpERP-angular-front\
npm install
```

Atualize os bancos de dados:

```powershell
.\db-update.ps1
```

Inicie as APIs e o frontend:

```powershell
.\run.ps1
```

## Detalhamento técnico da solução

### Ciclos de vida do Angular

ngOnInit, para carregar as listas inicialmente e takeUntilDestroyed para pararmos de observar o componente quando ele for destruido.

### RxJS

Sim, para cuidar da comunicação assíncrona entre o back e o front end, como é o caso de "imprimir" uma nota, que fica observando aquela nota vendo se ela processou corretamente.

### Bibliotecas do frontend

- **Angular Material:** para componentes visuais.
- **Angular CDK:** para usar BreakpointObserver e Breakpoints.
- E bibliotecas nativas do **Angular**

### Frameworks e tecnologias do backend

- **ASP.NET Core:** construção e exposição das APIs REST.
- **Entity Framework Core:** persistência de dados, mapeamento das entidades, consultas e migrações.
- **MassTransit:** abstração para publicação e consumo de eventos entre os serviços.
- **PostgreSQL:** banco de dados relacional utilizado pelas APIs.
- **RabbitMQ:** broker responsável pelo transporte assíncrono das mensagens.

### Tratamento de erros e exceções no backend

Os serviços validam as operações e lançam exceções específicas quando necessário, como `KeyNotFoundException` para recursos inexistentes. Os controllers tratam essas exceções e devolvem os códigos HTTP correspondentes ao cliente. Também há tratamento de falhas do Entity Framework Core, incluindo conflitos de concorrência e violações de unicidade; nesses casos, as transações são revertidas para preservar a consistência dos dados.

### Uso de LINQ

O LINQ foi utilizado extensivamente nas consultas ao banco de dados e no processamento de coleções. Ele é empregado em filtros com `Where`, ordenações com `OrderBy`, seleção e remoção de duplicidades com `Select` e `Distinct`, carregamento de relacionamentos com `Include`, materialização dos resultados e mapeamento das entidades para DTOs por meio da seleção apenas das propriedades necessárias.