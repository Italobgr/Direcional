# Direcional API

API desenvolvida em **.NET 9** para gestão de clientes, apartamentos, vendas e reservas.

Inclui autenticação via **JWT**, documentação com **Swagger** e **testes de integração** utilizando SQLite.

---

## 🚀 Tecnologias

- .NET 9
- Entity Framework Core
- SQL Server Express
- JWT Bearer Authentication
- FluentValidation
- Swagger / Swashbuckle
- xUnit + WebApplicationFactory (Testes de Integração)

---

## 🛠️ Configuração do Ambiente

### 1. Clonar o repositório

```bash
git clone [https://github.com/Italobgr/Direcional.git](https://github.com/Italobgr/Direcional.git)
cd direcional-api
2. Subir o ambiente com Docker


docker compose up -d --build

Depois rodar o comando para atualizar migrations:

dotnet ef database update 

Isso irá iniciar:

A API em http://localhost:8080

Uma instância do SQL Server em um contêiner

3. Acessar a Documentação (Swagger)
A documentação da API estará disponível em:
http://localhost:8080/swagger


🔑 Autenticação
A API utiliza autenticação via JWT Bearer Token. Para acessar os endpoints protegidos, você precisa primeiro obter um token.

Exemplo de Login
Bash

curl -X POST http://localhost:8080/api/auth/login \
-H "Content-Type: application/json" \
-d '{
  "username": "seu_usuario",
  "password": "sua_senha"
}'
Resposta esperada:

JSON

{
  "token": "eyJhbGciOiJIUzI1NiIsInR..."
}
Use o token recebido no cabeçalho Authorization das suas requisições ou no campo Authorize do Swagger:

Authorization: Bearer {seu_token}
Endpoints Principais
Clientes
Bash

# Listar todos os clientes
curl -X GET http://localhost:8080/api/clientes -H "Authorization: Bearer {token}"

# Criar um novo cliente
curl -X POST http://localhost:8080/api/clientes \
-H "Authorization: Bearer {token}" \
-H "Content-Type: application/json" \
-d '{ "nome": "Novo Cliente", "email": "cliente@exemplo.com" }'

# Atualizar um cliente existente
curl -X PUT http://localhost:8080/api/clientes/1 \
-H "Authorization: Bearer {token}" \
-H "Content-Type: application/json" \
-d '{ "id": 1, "nome": "Cliente Atualizado", "email": "cliente.atualizado@exemplo.com" }'

# Remover um cliente
curl -X DELETE http://localhost:8080/api/clientes/1 -H "Authorization: Bearer {token}"
Apartamentos
Bash

# Listar todos os apartamentos
curl -X GET http://localhost:8080/api/apartamentos -H "Authorization: Bearer {token}"
Vendas
Bash

# Criar uma nova venda
curl -X POST http://localhost:8080/api/vendas \
-H "Authorization: Bearer {token}" \
-H "Content-Type: application/json" \
-d '{ "clienteId": 1, "apartamentoId": 101, "valor": 500000.00 }'
Reservas
Bash

# Criar uma nova reserva
curl -X POST http://localhost:8080/api/reservas \
-H "Authorization: Bearer {token}" \
-H "Content-Type: application/json" \
-d '{ "clienteId": 2, "apartamentoId": 102, "dataReserva": "2024-10-20T10:00:00Z" }'
✅ Testes
Os testes de integração são executados com SQLite in-memory, garantindo que não haja dependência de um banco de dados externo como o SQL Server.

Como executar os testes:
Bash

dotnet test
📂 Estrutura do Projeto
src/
├── Direcional.Api/         # Projeto principal da API
│   ├── Controllers/        # Endpoints REST
│   ├── Infra/              # DbContext, Migrations
│   ├── Services/           # Serviços (ex: Geração de JWT)
│   └── Program.cs          # Configuração da aplicação
│
└── Direcional.Tests/       # Projeto de testes
    └── Integration/        # Testes de integração com WebApplicationFactory

