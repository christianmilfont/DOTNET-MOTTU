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

️ 

## Swagger:
- Crie primeiro o Pátio, logo após poderá com o ID do pátio criar uma moto e assim por diante, poderá ter a liberdade de verificar status e posição da moto!

- Mottu-DOTNET

Moto
```

PATCH /api/Moto/{placa}/status 

GET /api/Moto/{placa} 
```
Patio
```

POST /api/Patio 

GET /api/Patio/{id} 

POST /api/Patio/{patioId}/motos 

DELETE /api/Patio/{patioId}/motos/{placa}
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
