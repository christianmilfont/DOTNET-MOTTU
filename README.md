# Projeto da matéria de DOTNET – MOTTU



## Esse projeto mescla a entrega do CP4 juntamente a entrega da Sprint 3



- Aplicar os fundamentos de Clean Code, Domain-Driven Design (DDD) e Clean Architecture.

- Criar uma API escalável, legível e bem estruturada, refletindo práticas profissionais do mercado.



_________________________________________________________________________________



Diagrama da Aplicação:


<img width="586" height="652" alt="image" src="https://github.com/user-attachments/assets/564bfd92-d45d-462d-aaa5-1fd376fefcf8" />













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
dotnet ef migrations add Inicial
dotnet ef database update
```
<img width="289" height="56" alt="image" src="https://github.com/user-attachments/assets/8f5e9424-8e8b-42c6-a481-59d90cd78d8c" />




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


## Para funcionar a minha conexão com o banco de dados eu tive de criar no meu dispositivo utilizando o SQLCMD no Prompt de Comando
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
```
<img width="805" height="494" alt="image" src="https://github.com/user-attachments/assets/7d8f12c5-a6b5-476e-9398-5202590cf0bd" />
