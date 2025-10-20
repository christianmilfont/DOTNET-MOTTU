# Projeto da matéria de DOTNET – MOTTU
## Uma aplicação voltada a cadastrar clientes e motos, além disso alinhar as motos e clientes para os determinados pátios. Portando, criado para administrar e localizar as motos vindas pelos clientes (Front) e vindas por inadimplência (Back), junto a esse nosso projeto o IoT dando suporte com o uso do ESP-32 para ativar o alarme com seu próprio Dashboard! 

#### Nome dos integrantes:
- Christian Milfont rm555345
- Iago Victor rm558450


## Esse projeto mescla a entrega do CP4 juntamente a entrega da Sprint 3

- Aplicar os fundamentos de Clean Code, Domain-Driven Design (DDD) e Clean Architecture.

- Criar uma API escalável, legível e bem estruturada, refletindo práticas profissionais do mercado.

## Justificativa da Arquitetura
A solução foi estruturada seguindo os princípios da Clean Architecture e Domain-Driven Design (DDD), visando alta coesão, baixo acoplamento e facilidade de manutenção. A separação em camadas (Domínio, Aplicação, Infraestrutura e API) permite que regras de negócio fiquem isoladas de detalhes de implementação, como persistência e exposição via HTTP. O uso de DTOs garante segurança e clareza na comunicação entre camadas e com o cliente. O Entity Framework Core foi adotado para abstrair o acesso ao banco SQL Server Express, facilitando testes e evolução futura. A API expõe endpoints RESTful com boas práticas, incluindo paginação, status codes adequados e documentação automática via Swagger/OpenAPI, promovendo interoperabilidade e facilidade de uso para integradores.


### Para rodar a aplicação:
```
git clone https://github.com/christianmilfont/DOTNET-MOTTU.git

cd DOTNET-MOTTU

dotnet run
```

### Veja mais para baixo como rodar o banco de dados utilizado para essa aplicação .NET
_________________________________________________________________________________


Diagrama da Aplicação:

<img width="609" height="645" alt="image" src="https://github.com/user-attachments/assets/5fafb904-3ff7-468f-9a97-df9ab91a2a02" />













### Bibliotecas que utilizei:

- Pacotes para Infrastructure (EF Core + SQL Server)

SQL Server:

```  dotnet add src/Infrastructure package Microsoft.EntityFrameworkCore
dotnet add src/Infrastructure package Microsoft.EntityFrameworkCore.SqlServer
dotnet add src/Infrastructure package Microsoft.EntityFrameworkCore.Design
dotnet add src/Infrastructure package Microsoft.EntityFrameworkCore.Tools
dotnet add package AspNetCore.HealthChecks.UI
dotnet add package Microsoft.AspNetCore.Mvc.Versioning
dotnet add package Microsoft.AspNetCore.Authentication.JwtBearer
dotnet add package Microsoft.ML

```










- Infrastructure – EF Core + Repositórios + Migrations:
```
dotnet ef database update
```
<img width="289" height="56" alt="image" src="https://github.com/user-attachments/assets/8f5e9424-8e8b-42c6-a481-59d90cd78d8c" />

## Banco de dados (SQL Server Express)
- Para rodar o banco de dados, é necessário:
```
  sqllocaldb stop "MSSQLLocalDB"
  sqllocaldb delete "MSSQLLocalDB"
  sqllocaldb create "MSSQLLocalDB"
  sqllocaldb start "MSSQLLocalDB"

```
- Verificar se o banco está acessível:
```
sqllocaldb info "MSSQLLocalDB"
(localdb)\MSSQLLocalDB
```
- Se necessário, crie o banco de dados tambem!:
```
CREATE DATABASE mottu_challenge;
```

## Explicação detalhada da aplicação:

### Camada API

- Contém controllers (PatioController e MotoController) que recebem as requisições HTTP.

- Não contém regras de negócio.

- Responsável por orquestrar chamadas à camada Application e retornar DTOs ou erros.



### Camada Application

- Contém PatioService: a camada de orquestração da lógica de negócio.

- Funciona com DTOs (PatioDto, MotoDto) para não expor diretamente as entidades do domínio.

- Valida regras como:
```
Verificar se o pátio existe.

Garantir que a posição é válida (enum Posicao.TipoPosicao).

Orquestrar a criação, atualização, remoção e consulta de motos e pátios.

Se conecta aos repositórios da camada Infrastructure para persistência.
```
### Camada Domain

- Contém as entidades de negócio:

- Patio: agregado raiz que possui uma coleção de Moto.
  
- Cliente: feito especialmente para os motoristas serem anexados as motos (LOGIN e FORM para o mobile)

- Moto: possui Placa, Status e Posicao.

- Contém os Value Objects:

- Placa: encapsula regras de validação da placa.

- Posicao: enum ou objeto com regras de posição (Front/Back).

- Contém regras de negócio que pertencem às entidades, como AlterarStatus, AdicionarMoto, etc.

### Camada Infrastructure

- Contém repositórios EF Core (MotoRepository, PatioRepository) que implementam interfaces da camada Application.

- Contém AppDbContext:

- Configura relacionamento entre Patio e Moto.

- Configura VOs (OwnsOne) para Placa e Posicao.

- Responsável por persistência e acesso ao banco.

- Banco de dados (SQL Server)
```
Tabelas:

Patios: armazena dados do pátio.

Motos: armazena motos vinculadas a pátios.

Cliente: pode ou não ter uma moto anexada a ele.

Regras de integridade e relacionamentos definidas pelo EF Core via migrations.
```


### Fluxo de operação

- Cliente faz uma requisição HTTP → Ex.: POST /api/patio/{id}/motos.

- Controller recebe a requisição e chama PatioService.AdicionarMotoAsync.

#### PatioService:

- Valida se o pátio existe.

- Cria Moto com Placa e Posicao.

- Chama repositórios para salvar Moto e atualizar Patio.

- Repositórios (MotoRepository e PatioRepository) usam AppDbContext para persistir dados no SQL Server.

- Resultado é mapeado para MotoDto e retornado pelo controller para o cliente.
---
  
## Endpoint de Health Check

### Como implementar:

**Instale o pacote:**
```
dotnet add package AspNetCore.HealthChecks.UI
```

**Em Program.cs:**
```
builder.Services.AddHealthChecks();

var app = builder.Build();

app.MapHealthChecks("/health");

app.Run();
```

Teste:
Acesse no navegador ou via curl:
```
GET https://localhost:5001/health
```

---

## Versionamento da API 
### Objetivo:

Permitir múltiplas versões da API coexistirem.

**Como implementar:**

Instale o pacote:
```
dotnet add package Microsoft.AspNetCore.Mvc.Versioning
```
O controller que foi adicionado o versionamento foi exatamente os EndPoints do MotoController

**Para testar**:
```
POST /api/v1/moto/adicionar

GET /api/v1/moto/ABC1234

PUT /api/v1/moto/atualizar/ABC1234

DELETE /api/v1/moto/remover/{patioId}/ABC1234

GET /api/v1/moto/listar

GET /api/v1/moto/total
```

---

# JWT (mais completo e seguro)

**Adicione o pacote:**
```
dotnet add package Microsoft.AspNetCore.Authentication.JwtBearer
```
## Autenticação com JWT

Implementação da geração de tokens JWT em uma classe de serviço (JwtService), que cria tokens assinados com uma chave secreta segura.

- Configuração do middleware de autenticação JWT no Program.cs, definindo parâmetros como emissor (Issuer), audiência (Audience), e validação do token.

- Assegura que apenas usuários autenticados possam acessar endpoints protegidos.

- Exemplo de uso do token para autenticar o usuário e retornar informações básicas junto com o token no login.

### Configuração do Swagger para documentação e testes

Configuração do Swagger UI para exibir a documentação da API, incluindo suporte para autenticação JWT.

- Permite que usuários insiram o token JWT no Swagger UI usando o botão Authorize, facilitando testes dos endpoints protegidos.

- Mantém a documentação atualizada automaticamente conforme o código evolui.

## Outras configurações importantes

Arquivos e códigos principais

- appsettings.json: Contém configurações sensíveis, como a connection string do banco e as chaves do JWT.

- Program.cs: Configuração geral da aplicação — serviços, middlewares, autenticação, Swagger, CORS, etc.

- JwtService.cs: Serviço responsável por gerar tokens JWT válidos, assinados e com dados do usuário.

- Controllers: Endpoints da API, como o LoginController que retorna o token após autenticação. ( [Authorize] )

## Como usar

- Configure a connection string no appsettings.json para apontar para seu banco SQL Server.

- Ajuste a chave secreta (Jwt:Key) para uma string forte e segura (mínimo 256 bits).

- Execute as migrations do EF Core para criar o banco, caso necessário.

- Rode a aplicação (dotnet run).

- Acesse o Swagger UI em http://localhost:5093/swagger/index.html

- Use o endpoint de login para gerar seu token JWT. (Lembre-se que para adequação ao sistema o ideal é criar um Cliente antes, e depois logar)

- No Swagger, clique em Authorize e insira o token no formato:
  
```
Bearer xxxxxx
```

Teste os endpoints protegidos com o token autenticado.

---


## Usar ML.NET em um Endpoint (25 pts)
### Objetivo:

Criar um modelo de machine learning simples (ex: previsão de preço, classificação).

**Instale ML.NET:**
```
dotnet add package Microsoft.ML
```

---


## Swagger:
- Crie primeiro o Pátio, logo após poderá com o ID do pátio criar uma moto e assim por diante, poderá ter a liberdade de verificar status e posição da moto!

## Mottu-DOTNET
/swagger/v1/swagger.json

**Cliente**

```
GET
/api/Cliente

POST
/api/Cliente

GET
/api/Cliente/{id}

PUT
/api/Cliente/{id}

DELETE
/api/Cliente/{id}
```
---

**Login**
```
POST
/api/Login
```
**Moto**

```
POST
/api/Moto/adicionar

GET
/api/Moto/{placa}

PUT
/api/Moto/atualizar/{placa}

DELETE
/api/Moto/remover/{patioId}/{placa}

GET
/api/Moto/listar

GET
/api/Moto/total
```

---

**Patio**
```
POST
/api/Patio

GET
/api/Patio

GET
/api/Patio/{id}

DELETE
/api/Patio/{id}

POST
/api/Patio/{patioId}/motos

DELETE
/api/Patio/{patioId}/motos/{placa}
```

## Para funcionar a minha conexão com o banco de dados eu tive de criar no meu dispositivo utilizando o SQLCMD no Prompt de Comando (antigo comentario) (lembrete)
```bash
sqlcmd -S DESKTOP-L9QAGFT\SQLEXPRESS -E
```
-S DESKTOP-L9QAGFT\SQLEXPRESS → seu servidor/instância.
-E → usa autenticação do Windows (Trusted Connection).

```
Dentro do sqlcmd, rode:

CREATE DATABASE MottuDb;
GO
```

```
Verificar se o banco foi criado
SELECT name FROM sys.databases;
GO
```
```
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=DESKTOP-L9QAGFT\\SQLEXPRESS;Database=MottuDb;Trusted_Connection=True;TrustServerCertificate=True;"
  }
}

C:\Users\labsfiap>sqllocaldb i
MSSQLLocalDB

C:\Users\labsfiap>sqlcmd -S (localdb)\MSSQLLocalDB -E
1> CREATE DATABASE MottuDb;
2> GO
```
<img width="805" height="494" alt="image" src="https://github.com/user-attachments/assets/7d8f12c5-a6b5-476e-9398-5202590cf0bd" />
