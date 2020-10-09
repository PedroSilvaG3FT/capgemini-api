# CapgeminiAPI

- Projeto gerado com a versão 3.1.402 do .Net
- Entity Framework Core
- EPPlus para manipulação do arquivo .xlsx

# Iniciar Projeto 

- Clone o repositorio em sua maquina local.
- dotnet restore.
- dotnet run / f5.

# Camadas do Projeto
`Controller`: Onde ocorre o primeiro contato do Client com a API a partir de seus respectivos EndPoints.
`Service`: Camada que irá tratar da regra da requisição podendo incluir ou retornar valores do Banco de Dados.
`Model`: Armazena os modelos de entidades e o context.