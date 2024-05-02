# User Microservice API

Esta API permite a criação, atualização, busca e exclusão de usuários. Além disso, fornece autenticação JWT para proteger os endpoints da API.

## Funcionalidades

- **Cadastro de Usuário**: Permite o cadastro de novos usuários na plataforma.
- **Login de Usuário**: Permite que usuários registrados façam login na plataforma para acessar recursos protegidos.
- **Atualização de Usuário**: Usuários registrados podem atualizar suas informações na plataforma.
- **Exclusão de Usuário**: Usuários registrados podem excluir suas contas na plataforma.
- **Buscar Usuário por ID**: Permite a busca de um usuário específico por meio do seu ID.
- **Buscar Todos os Usuários**: Retorna uma lista de todos os usuários cadastrados na plataforma.

## Autenticação

A API utiliza JSON Web Tokens (JWT) para autenticar usuários. Todos os endpoints, exceto os de cadastro de usuário e login, exigem um token de autenticação válido.

Para obter um token de autenticação, os usuários devem fazer login com suas credenciais (nome de usuário e senha) e o token será gerado e fornecido em resposta.

## Endpoints

- `POST /api/auth/login`: Realiza o login de um usuário e retorna um token JWT.
- `POST /api/users`: Cria um novo usuário.
- `GET /api/users`: Retorna uma lista de todos os usuários.
- `GET /api/users/{id}`: Retorna um usuário específico com base no ID fornecido.
- `PUT /api/users/{id}`: Atualiza as informações de um usuário existente.
- `DELETE /api/users/{id}`: Exclui um usuário existente.

## Requisitos

- .NET Core 3.1 ou superior
- MySQL Server
- Docker

## Configuração do Ambiente

1. Clone este repositório em sua máquina local.
2. Configure a conexão com o banco de dados MySQL no arquivo `appsettings.json`.
3. Execute o comando `docker-compose up` para iniciar o servidor MySQL.
4. Execute o projeto utilizando o comando `dotnet run`.
5. Acesse a API em `https://localhost:5001/swagger/index.html`.

## Exemplo de Requisição

### POST /api/auth/login

#### Corpo da Requisição
```json
{
    "username": "example_user",
    "password": "example_password"
    "Email": "example@example.com.br"
}
