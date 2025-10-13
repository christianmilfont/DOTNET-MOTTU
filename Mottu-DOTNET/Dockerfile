# Etapa 1: Build da aplicação
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copia e restaura dependências
COPY ["Mottu_DOTNET.csproj", "./"]
RUN dotnet restore "Mottu_DOTNET.csproj"

# Copia todo o código e compila em modo Release
COPY . .
RUN dotnet publish "Mottu_DOTNET.csproj" -c Release -o /app/publish

# Etapa 2: Imagem final para execução
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=build /app/publish .
EXPOSE 8080
ENTRYPOINT ["dotnet", "Mottu_DOTNET.dll"]
