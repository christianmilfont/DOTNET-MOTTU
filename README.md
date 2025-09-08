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
