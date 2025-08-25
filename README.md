# Direcional API

API desenvolvida em **.NET 9** para gestão de **clientes**, **apartamentos**, **reservas** e **vendas**, com **autenticação JWT**, **Entity Framework Core** e **SQL Server Express**.  
Documentação via **Swagger** e testes (mínimos) com **xUnit**.

> Nota: quando baixei o projeto pela primeira vez, precisei **rodar/atualizar as migrations** antes de subir tudo. Já deixei isso documentado abaixo.

---

## 📑 Sumário

- [Como rodar com Docker](#-como-rodar-com-docker)
- [Estrutura de pastas (resumo)](#-estrutura-de-pastas-resumo)
- [Variáveis de ambiente](#-variáveis-de-ambiente)
- [Banco de dados: estrutura das tabelas](#-banco-de-dados-estrutura-das-tabelas)
- [Geração e uso do token JWT](#-geração-e-uso-do-token-jwt)
- [Exemplos de requisições](#-exemplos-de-requisições)
- [Testes (opcional)](#-testes-opcional)
- [Dicas de solução de problemas](#-dicas-de-solução-de-problemas)
- [Decisões técnicas & considerações](#-decisões-técnicas--considerações)
- [Licença](#-licença)

---

## 🚀 Como rodar com Docker

Pré-requisitos:
- Docker + Docker Compose
- Porta `8080` livre para a API
- Porta `1433` livre para o SQL Server 

### Passo a passo

1. **Clonar o repositório**
```bash
git clone https://github.com/Italobgr/Direcional.git
cd Direcional

```
(Somente na 1ª vez) Rodar migrations
Se você acabou de clonar, aplique as migrations antes de subir os containers.

Localmente (requer SDK .NET instalado):


```
dotnet tool restore
dotnet restore
dotnet build
dotnet ef database update --project src/Direcional/Direcional.csproj

```
Subir tudo

```
docker compose up -d --build
```

API: http://localhost:8080

Swagger: http://localhost:8080/swagger

SQL Server: localhost,1433

Estrutura de pastas (resumo)

```
root
 ├─ docker/
 │   ├─ api.Dockerfile
 │   └─ sqlserver.env
 ├─ src/
 │   └─ Direcional/
 │       ├─ Controllers/
 │       ├─ Domain/
 │       ├─ Infrastructure/
 │       ├─ Application/
 │       ├─ Program.cs
 │       └─ appsettings.json
 ├─ tests/
 │   └─ Direcional.Tests/
 └─ docker-compose.yml
 ariáveis de ambiente
```
Exemplo no docker-compose.yml:
```

services:
  db:
    image: mcr.microsoft.com/mssql/server:2022-latest
    environment:
      - ACCEPT_EULA=Y
      - SA_PASSWORD=Your_strong_Password123!
    ports:
      - "1433:1433"

  api:
    build:
      context: .
      dockerfile: docker/api.Dockerfile
    environment:
      - ASPNETCORE_URLS=http://+:8080
      - ConnectionStrings__Default=Server=db;Database=DirecionalDb;User=sa;Password=Your_strong_Password123!;TrustServerCertificate=True
      - Jwt__Issuer=Direcional
      - Jwt__Audience=Direcional.Client
      - Jwt__Key=dev-secret-please-change
      - Jwt__ExpiresMinutes=120
    depends_on:
      - db
    ports:
      - "8080:8080"
```
Banco de dados: estrutura das tabelas
clientes
```
Id (PK, GUID)

Nome

Cpf (único)

Email

Telefone

CriadoEm
```
apartamentos

```
Id (PK, GUID)

Bloco

Numero

AreaM2

Quartos

Valor

Status → Disponivel, Reservado, Vendido

CriadoEm
```
reservas
```
Id (PK, GUID)

ClienteId (FK → clientes.Id)

ApartamentoId (FK → apartamentos.Id)

ValorSinal

DataReserva

Status → Ativa, Cancelada, Convertida
```
vendas
```
Id (PK, GUID)

ClienteId (FK → clientes.Id)

ApartamentoId (FK → apartamentos.Id)

ValorEntrada

ValorTotal

DataVenda

OrigemReservaId (FK opcional → reservas.Id)
```
Geração e uso do token JWT
Login

```
POST /api/auth/login
Content-Type: application/json

{
  "username": "corretor",
  "password": "123456"
}
```
Resposta
```

{
  "accessToken": "<jwt-aqui>",
  "expiresIn": 7200,
  "tokenType": "Bearer"
}
```
Usar em chamadas


Authorization: Bearer <jwt-aqui>
Exemplos de requisições

BASE=http://localhost:8080
TOKEN="<pergar o tk jwt na rota>"
Criar cliente
Campos: nome, email, telefone

```
curl -X POST "$BASE/api/clientes" \
 -H "Authorization: Bearer $TOKEN" -H "Content-Type: application/json" \
 -d '{
  "nome":"Maria Souza",
  "email":"maria@exemplo.com",
  "telefone":"31999990000"
}'
```
Criar apartamento
Campos: endereco, numeroQuartos, valor, disponivel
(disponivel pode ser omitido; default = true)

```
curl -X POST "$BASE/api/apartamentos" \
 -H "Authorization: Bearer $TOKEN" -H "Content-Type: application/json" \
 -d '{
  "endereco":"Rua das Palmeiras, 123 - Bloco A, ap 302",
  "numeroQuartos":2,
  "valor":380000.00,
  "disponivel":true
}'
```
Criar reserva
Campos: idCliente, idApartamento, dataReserva, validade
(ISO 8601; ex.: 2025-08-25T12:00:00Z)


CLIENTE_ID=1
APTO_ID=1
```
curl -X POST "$BASE/api/reservas" \
 -H "Authorization: Bearer $TOKEN" -H "Content-Type: application/json" \
 -d "{
  \"idCliente\": $CLIENTE_ID,
  \"idApartamento\": $APTO_ID,
  \"dataReserva\": \"2025-08-25T12:00:00Z\",
  \"validade\": \"2025-09-01T00:00:00Z\"
}"
```
Efetivar venda (CONFIRMAR RESERVA)
Fluxo oficial: confirmar a reserva → cria a venda.
Endpoint: POST /api/reservas/{id}/confirmar
Body: apenas um número JSON (valor final).


RESERVA_ID=1

# importante: o body é um número JSON (sem aspas)
```
curl -X POST "$BASE/api/reservas/$RESERVA_ID/confirmar" \
 -H "Authorization: Bearer $TOKEN" -H "Content-Type: application/json" \
 --data '345000.00'
```

Criar venda DIRETA (sem reserva)
Só funciona se o apartamento estiver disponível=true (não reservado).
Campos: idCliente, idApartamento, dataVenda, valorFinal

```
curl -X POST "$BASE/api/vendas" \
 -H "Authorization: Bearer $TOKEN" -H "Content-Type: application/json" \
 -d "{
  \"idCliente\": $CLIENTE_ID,
  \"idApartamento\": $APTO_ID,
  \"dataVenda\": \"2025-08-25T12:00:00Z\",
  \"valorFinal\": 380000.00
}"
```
Testes 
Para rodar testes:
```
dotnet test
```
🛠 Dicas de solução de problemas
PendingModelChangesWarning:

dotnet ef migrations add <nome>
dotnet ef database update
API sobe antes do banco:
```
docker compose restart api
```
Senha do SA inválida: use senha forte.

Swagger não aparece: verifique app.UseSwagger() no Program.cs.

## Decisões técnicas & considerações
SQL Server Express: alinhado ao stack Microsoft.

JWT: autenticação stateless, fácil de validar.

FluentValidation: regras mais legíveis.

Docker Compose: sobe banco e API juntos.

Migrations: tive que rodar manualmente na primeira execução → deixei documentado.

Camadas: Domain, Application, Infrastructure, API. Evitei over-engineering.

Regras de negócio:

Reserva → bloqueia apartamento (Reservado).

Venda → converte reserva e marca apartamento como Vendido.
