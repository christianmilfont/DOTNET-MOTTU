#!/usr/bin/env bash
set -euo pipefail

# ==========================
# Parâmetros da Conexão MySQL
# ==========================
DbHost="${DbHost:-48.216.181.214}"
DbPort="${DbPort:-3306}"
DbName="${DbName:-Mottu-Dotnet}"
DbUser="${DbUser:-root}"
DbPass="${DbPass:-Senha123!}"

# =======================
# Validação do cliente MySQL
# =======================
if ! command -v mysql >/dev/null 2>&1; then
  echo "ERRO: cliente 'mysql' não encontrado. Instale-o antes de executar este script."
  exit 1
fi

# =======================
# Criar banco de dados se não existir
# =======================
echo "📦 Criando banco de dados '$DbName' se não existir..."
mysql --protocol=TCP -h"$DbHost" -P"$DbPort" -u"$DbUser" -p"$DbPass" \
  -e "CREATE DATABASE IF NOT EXISTS \`$DbName\`;"

# =======================
# Criar tabelas baseadas no DbContext
# =======================
echo "🧱 Criando tabelas do domínio Mottu_DOTNET..."

mysql --protocol=TCP -h"$DbHost" -P"$DbPort" -u"$DbUser" -p"$DbPass" "$DbName" <<'SQL'

-- ========================
-- Tabela: Patios
-- ========================
CREATE TABLE IF NOT EXISTS Patios (
  Id CHAR(36) NOT NULL PRIMARY KEY,
  Nome VARCHAR(255) NOT NULL
);

-- ========================
-- Tabela: Clientes
-- ========================
CREATE TABLE IF NOT EXISTS Clientes (
  Id CHAR(36) NOT NULL PRIMARY KEY,
  Nome VARCHAR(255) NOT NULL,
  Telefone VARCHAR(50) NOT NULL,
  Email VARCHAR(255) NOT NULL,
  Endereco VARCHAR(255) NOT NULL
);

-- ========================
-- Tabela: Motos
-- ========================
CREATE TABLE IF NOT EXISTS Motos (
  Id CHAR(36) NOT NULL PRIMARY KEY,
  Placa VARCHAR(20) NOT NULL,
  Status VARCHAR(50) NOT NULL,
  Posicao ENUM('Front','Back') NOT NULL,
  ClienteId CHAR(36) NULL,
  PatioId CHAR(36) NOT NULL,
  CONSTRAINT FK_Motos_Clientes
    FOREIGN KEY (ClienteId)
    REFERENCES Clientes(Id)
    ON DELETE SET NULL,
  CONSTRAINT FK_Motos_Patios
    FOREIGN KEY (PatioId)
    REFERENCES Patios(Id)
    ON DELETE CASCADE
);

-- Índices
CREATE INDEX IF NOT EXISTS IX_Motos_ClienteId ON Motos (ClienteId);
CREATE INDEX IF NOT EXISTS IX_Motos_PatioId ON Motos (PatioId);

SQL

echo "✅ Banco e tabelas do domínio criados com sucesso."

# =======================
# Azure WebApp (mantido)
# =======================
rg="rg-api-dotnet"
location=${LOCATION:-"eastus"}
plan="planApiDotnet"
app=${NOME_WEBAPP:-"mottu-dotnet"}
runtime="dotnet:8"
sku="F1"

echo "📁 Criando Grupo de Recursos se não existir..."
az group create --name "$rg" --location "$location" 1>/dev/null

echo "⚙️ Criando Plano de Serviço se não existir..."
az appservice plan create --name "$plan" --resource-group "$rg" --location "$location" --sku "$sku" 1>/dev/null

echo "🚀 Criando Serviço de Aplicativo se não existir..."
az webapp create --resource-group "$rg" --plan "$plan" --runtime "$runtime" --name "$app" 1>/dev/null

# =======================
# Configuração de Logs
# =======================
app_logging="$(az webapp log show -g "$rg" -n "$app" --query 'applicationLogs.fileSystem.level' -o tsv 2>/dev/null || true)"
ws_logging="$(az webapp log show -g "$rg" -n "$app" --query 'httpLogs.fileSystem.enabled' -o tsv 2>/dev/null || true)"
det_errors="$(az webapp log show -g "$rg" -n "$app" --query 'detailedErrorMessages.enabled' -o tsv 2>/dev/null || true)"
failed_req="$(az webapp log show -g "$rg" -n "$app" --query 'failedRequestsTracing.enabled' -o tsv 2>/dev/null || true)"

if [ "$app_logging" != "Information" ] || [ "$ws_logging" != "true" ] || [ "$det_errors" != "true" ] || [ "$failed_req" != "true" ]; then
  echo "🪵 Habilitando Logs do Serviço de Aplicativo..."
  az webapp log config \
    --resource-group "$rg" \
    --name "$app" \
    --application-logging filesystem \
    --web-server-logging filesystem \
    --level information \
    --detailed-error-messages true \
    --failed-request-tracing true 1>/dev/null
else
  echo "ℹ️ Logs já configurados."
fi

echo "✅ Azure WebApp configurado e pronto!"
