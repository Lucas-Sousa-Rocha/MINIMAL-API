# 🚗🔑 Minimal API - Sistema de Administradores e Veículos

## 📄 Descrição
Esta é uma **Minimal API** desenvolvida em **.NET 9** para gerenciamento de administradores e veículos, com autenticação baseada em **JWT**, uso de **Entity Framework Core** com SQL Server e documentação via **Swagger**. O projeto inclui exemplos de resposta nos endpoints e testes automatizados.

Principais funcionalidades:
- 🔐 Login e gerenciamento de administradores
- 🚘 Cadastro, listagem, atualização e remoção de veículos
- 🛡️ Autorização baseada em roles (`ADMIN`, `USER`)
- 👤 Criação automática de um usuário master (`admin@mail.com`) no banco de dados na inicialização
- 📑 Documentação interativa via Swagger
- 🧪 Testes unitários (Xunit + Moq)

---

## 🛠️ Tecnologias e Dependências

- 🟦 .NET 9 (Minimal API)
- 💻 C#
- 🗄️ Entity Framework Core
- 🛢️ SQL Server
- 🔑 JWT (JSON Web Token) para autenticação
- 🧂 BCrypt para hash de senhas
- 📚 Swagger (OpenAPI)
- 🧪 Xunit e Moq para testes

### 📦 Dependências NuGet

```xml
<PackageReference Include="BCrypt.Net-Next" Version="4.0.3" />
<PackageReference Include="Microsoft.AspNetCore.Authentication.JwtBearer" Version="9.0.9" />
<PackageReference Include="Microsoft.AspNetCore.OpenApi" Version="9.0.9" />
<PackageReference Include="Microsoft.EntityFrameworkCore" Version="9.0.9" />
<PackageReference Include="Microsoft.EntityFrameworkCore.Design" Version="9.0.9">
  <PrivateAssets>all</PrivateAssets>
  <IncludeAssets>runtime; build; native; contentfiles; analyzers; buildtransitive</IncludeAssets>
</PackageReference>
<PackageReference Include="Microsoft.EntityFrameworkCore.SqlServer" Version="9.0.9" />
<PackageReference Include="Microsoft.EntityFrameworkCore.Tools" Version="9.0.9">
  <PrivateAssets>all</PrivateAssets>
  <IncludeAssets>runtime; build; native; contentfiles; analyzers; buildtransitive</IncludeAssets>
</PackageReference>
<PackageReference Include="Swashbuckle.AspNetCore" Version="9.0.6" />
<PackageReference Include="Swashbuckle.AspNetCore.Swagger" Version="9.0.6" />
```

---

## 🚦 Requisitos
- 📦 .NET 9 SDK estável
- 🛢️ SQL Server
- 🖥️ Visual Studio 2022+ ou VS Code
- 🧪 Postman ou similar para testar endpoints

---

## ⚙️ Configuração do Projeto

### 1️⃣ Clonar o repositório
```bash
git clone <URL_DO_REPOSITORIO>
cd MINIMAL-API
```

### 2️⃣ Configurar `appsettings.json`
Defina sua string de conexão e as configurações do JWT:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=MinimalAPI;Trusted_Connection=True;"
  },
  "Jwt": {
    "Key": "SUA_CHAVE_SECRETA_MUITO_FORTE",
    "Issuer": "SeuSistema",
    "Audience": "SeuSistemaUsers",
    "ExpireMinutes": 60
  }
}
```

### 3️⃣ Criar o banco de dados
Execute o EF Core Migrations para criar o banco automaticamente:

```bash
dotnet ef database update
```

---

## ▶️ Executando a API

### 💻 Via CLI
```bash
dotnet run
```

### 🛠️ Via Executável (após publicação)
```bash
dotnet MINIMAL-API.dll
```

### 🌐 Acessando o Swagger
No ambiente de desenvolvimento, acesse:

```
http://localhost:<PORTA>/swagger
```

---

## 🔍 Endpoints

### 👨‍💼 Administradores

| Método | Endpoint                   | Descrição                          | Autorização |
|--------|----------------------------|------------------------------------|-------------|
| POST   | /administradores/login     | Login e geração de token JWT       | Anônimo     |
| POST   | /administradores           | Criar novo administrador           | ADMIN       |
| GET    | /administradores/{id}      | Buscar administrador por ID        | ADMIN       |
| GET    | /administradores           | Listar administradores (paginação) | ADMIN       |

### 🚗 Veículos

| Método | Endpoint              | Descrição                      | Autorização   |
|--------|-----------------------|--------------------------------|---------------|
| POST   | /veiculos             | Criar veículo                  | USER          |
| GET    | /veiculos             | Listar veículos (paginação)    | USER ou ADMIN |
| GET    | /veiculos/{id}        | Buscar veículo por ID          | USER ou ADMIN |
| PUT    | /veiculos/{id}        | Atualizar veículo              | USER          |
| DELETE | /veiculos/{id}        | Deletar veículo                | USER          |

---

## 🔑 Autenticação JWT

Para acessar endpoints protegidos, envie o token JWT no header:

```
Authorization: Bearer <TOKEN>
```

Tokens expiram conforme o parâmetro `ExpireMinutes` no `appsettings.json`.

---

## 👤 Usuário Master

Na inicialização da aplicação será criado automaticamente:

- **Email:** admin@mail.com
- **Senha:** Admin
- **Perfil:** ADMIN

---

## 🧪 Testes Automatizados

O projeto contém testes automatizados usando **Xunit** e **Moq** para entidades, lógica de domínio e mocks de repositórios.

### ▶️ Executando os testes

```bash
dotnet test
```

### Exemplos de testes incluídos

- 🧩 Mock de repositório e verificação de métodos com Moq
- 📦 Testes de entidades (get/set)
- 🛡️ Testes de regras de negócio (perfil ADMIN/USER)
- 🚦 Teste de manipulação de listas mockadas

Os testes estão na pasta `MINIMAL_API_TESTE` e utilizam as seguintes bibliotecas:
- [xUnit](https://xunit.net/)
- [Moq](https://github.com/moq)

---

## 📝 Observações

- 🔒 As senhas são armazenadas com BCrypt
- 📅 Datas de veículos devem ser enviadas no formato `YYYY-MM-DD`
- ✅ A aplicação já contém validações básicas de duplicidade e formatos incorretos

---

## 📜 Licença

Este projeto está licenciado sob a licença MIT.  
Consulte o arquivo [LICENSE](LICENSE) para mais informações.