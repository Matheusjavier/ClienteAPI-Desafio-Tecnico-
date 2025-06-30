# ClienteAPI

![.NET](https://img.shields.io/badge/.NET-512BD4?style=for-the-badge&logo=dotnet&logoColor=white)
![C#](https://img.shields.io/badge/C%23-239120?style=for-the-badge&logo=c-sharp&logoColor=white)
![ASP.NET Core](https://img.shields.io/badge/ASP.NET_Core-512BD4?style=for-the-badge&logo=asp.net&logoColor=white)
![SQL Server](https://img.shields.io/badge/SQL_Server-CC2927?style=for-the-badge&logo=microsoft-sql-server&logoColor=white)
![Entity Framework Core](https://img.shields.io/badge/Entity_Framework_Core-512BD4?style=for-the-badge&logo=dot-net&logoColor=white)
![JWT](https://img.shields.io/badge/JWT-000000?style=for-the-badge&logo=json-web-tokens&logoColor=black)
![Swagger](https://img.shields.io/badge/Swagger-85EA2D?style=for-the-badge&logo=swagger&logoColor=black)

## Descrição

Esta é a `ClienteAPI`, uma API RESTful desenvolvida com ASP.NET Core para gerenciar clientes e seus logradouros. Ela inclui autenticação e autorização via JWT e ASP.NET Core Identity, usando Entity Framework Core com SQL Server.

## Funcionalidades

* **Clientes:** CRUD completo (Criar, Ler, Atualizar, Deletar).
* **Logradouros:** CRUD completo, associados a clientes.
* **Autenticação JWT:** Registro e login de usuários.
* **Autorização:** Endpoints protegidos por token.
* **Banco de Dados:** Entity Framework Core com SQL Server.
* **Stored Procedure:** Exemplo de busca de clientes por nome parcial.
* **Documentação:** Swagger UI para testar a API.
* **CORS:** Configurado para permitir requisições de qualquer origem.

## Arquitetura

O projeto é estruturado em camadas para melhor organização e separação de responsabilidades:

* **`ClienteAPI.Domain`**: Entidades de negócio (`Cliente`, `Logradouro`).
* **`ClienteAPI.Identity`**: Extensão do ASP.NET Core Identity (`ApplicationUser`).
* **`ClienteAPI.Data`**: `DbContexts` e `Repositories` para acesso ao banco de dados.
* **`ClienteAPI.Services`**: Lógica de negócio da aplicação.
* **`ClienteAPI.Web`**: Controllers da API e DTOs (Models).

## Tecnologias

* .NET 8+
* C#
* ASP.NET Core
* Entity Framework Core
* SQL Server (LocalDB)
* ASP.NET Core Identity
* JWT
* Swagger/Swashbuckle

## Pré-requisitos

* [.NET SDK 8.0](https://dotnet.microsoft.com/download/dotnet/8.0) ou superior
* [SQL Server LocalDB](https://docs.microsoft.com/en-us/sql/tools/sql-server-management-studio/download-sql-server-management-studio-ssms?view=sql-server-ver16#download-ssms)
* [Visual Studio](https://visualstudio.microsoft.com/vs/) ou [Visual Studio Code](https://code.visualstudio.com/)

## Configuração do Ambiente

1.  **Clone o Repositório:**
    ```bash
    git clone SEU_LINK_DO_REPOSITORIO_GITHUB
    cd ClienteAPI
    ```

2.  **Configurar `appsettings.json`:**
    Abra `ClienteAPI.Web/appsettings.json`. Ajuste suas Connection Strings para o SQL Server e configure a seção JWT:

    ```json
    "ConnectionStrings": {
      "DefaultConnection": "Server=(localdb)\\MSSQLLocalDB;Database=ClienteAPIDb;Trusted_Connection=True;MultipleActiveResultSets=true",
      "IdentityConnection": "Server=(localdb)\\MSSQLLocalDB;Database=ClienteAPIIdentityDb;Trusted_Connection=True;MultipleActiveResultSets=true"
    },
    "Jwt": {
      "Issuer": "https://localhost:7122/", 
      "Audience": "https://localhost:7122/", 
      "Secret": "SUA_CHAVE_SECRETA_JWT_AQUI_MINIMO_DE_16_CARACTERES_MAS_RECOMENDADO_MAIS_LONGO_E_COMPLEXO",
      "ExpiresInDays": 4
    }
    ```
    * **JWT `Secret`**: Use um gerador para criar uma chave base64 forte (ex: uma string de 32 bytes convertida para Base64).
    * **JWT `Issuer` e `Audience`**: Devem corresponder à URL/porta da sua aplicação (verifique `Properties/launchSettings.json`).

3.  **Aplicar Migrações do Banco de Dados:**
    Execute no terminal, nos respectivos diretórios dos projetos:

    ```bash
    # Para o banco de dados de domínio
    cd ClienteAPI.Data
    dotnet ef database update

    # Para o banco de dados de Identity
    cd ../ClienteAPI.Data.IdentityContexts
    dotnet ef database update
    ```
    * **Se for a primeira vez**, crie as migrações antes de atualizar:
        ```bash
        # No ClienteAPI.Data
        dotnet ef migrations add InitialDomainMigration

        # No ClienteAPI.Data.IdentityContexts
        dotnet ef migrations add InitialIdentityMigration
        ```

4.  **Criar Stored Procedure (Opcional):**
    Para o endpoint `GET /api/clientes/search-by-sp`, execute este script no seu `ClienteAPIDb`:

    ```sql
    CREATE PROCEDURE GetClientesByName
        @NomeParcial NVARCHAR(MAX) = NULL
    AS
    BEGIN
        SET NOCOUNT ON;
        SELECT Id, Nome, Email, Logotipo
        FROM Clientes
        WHERE @NomeParcial IS NULL OR Nome LIKE '%' + @NomeParcial + '%';
    END;
    GO
    ```

## Como Executar

1.  **Via Terminal:**
    ```bash
    cd ClienteAPI.Web
    dotnet run
    ```
    A API iniciará (geralmente em `https://localhost:7122/`).

2.  **Via Visual Studio:**
    Abra a solução (`.sln`) e execute o projeto `ClienteAPI.Web` (F5).

## Testando a API (Swagger UI)

Acesse `https://localhost:7122/swagger` no seu navegador (ajuste a porta se necessário).

**Fluxo de Teste:**

1.  **Registrar:** Use `POST /api/Auth/register` para criar um usuário.
2.  **Login:** Use `POST /api/Auth/login` com as credenciais do usuário recém-criado. **Copie o `Token` da resposta.**
3.  **Autorizar no Swagger:** Clique no botão "Authorize" (verde) no topo do Swagger UI. No campo "Value", digite `SEU_TOKEN_COPIADO` . Clique em "Authorize".
4.  **Testar Endpoints Protegidos:** Agora você pode testar todos os endpoints de `Clientes` e `Logradouros`.

## Estrutura do Banco de Dados (Resumo)

### `ClienteAPIDb` (Domínio)

* `Clientes`: `Id`, `Nome`, `Email`, `Logotipo`
* `Logradouros`: `Id`, `NomeLogradouro`, `Numero`, `Complemento`, `Bairro`, `Cidade`, `Estado`, `CEP`, `ClienteId` (FK)

### `ClienteAPIIdentityDb` (Identity)

* Tabelas padrão do ASP.NET Core Identity (ex: `AspNetUsers`, `AspNetRoles`). `AspNetUsers` terá uma coluna `Nome` adicional.

## Contribuições

Sinta-se à vontade para abrir issues ou pull requests.

## Licença

Este projeto está licenciado sob a [MIT License](LICENSE).
