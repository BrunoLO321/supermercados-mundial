SUPERMERCADOS MUNDIAL - GUIA DE EXECUCAO
========================================

1. REQUISITOS

Instalar no computador:

- .NET SDK 8.0
- Node.js compativel com o Angular do projeto
- Git
- PowerShell

Versoes utilizadas no desenvolvimento:
- .NET SDK: 8.0.426
- Node.js: v24.22.0

Para verificar as instalacoes, execute:

dotnet --version
node --version
npm.cmd --version
git --version

2. BAIXAR O PROJETO

Abra o PowerShell e execute:

git clone https://github.com/BrunoLO321/supermercados-mundial.git

Entre na pasta do projeto:

cd supermercados-mundial

3. CONFIGURAR O BACKEND

Na raiz do projeto, configure as variaveis de ambiente:

$env:Auth__Username = "admin"
$env:Auth__Password = "DEFINA_SUA_SENHA_LOCAL"

Gere uma chave JWT para o ambiente local:

$env:Jwt__Key = [Convert]::ToHexString([Security.Cryptography.RandomNumberGenerator]::GetBytes(32))

Configure o emissor e a audiencia:

$env:Jwt__Issuer = "Mundial.Api"
$env:Jwt__Audience = "Mundial.Client"

Sugestão:
$env:Auth__Username = "admin"
$env:Auth__Password = "Mundial@Teste2026!"
$env:Jwt__Key = "A7F3C91D6E2B8450D4A8F17C93E65B20C8D1F4A769B2E5031A6C8F45D7E029BC"
$env:Jwt__Issuer = "Mundial.Api"
$env:Jwt__Audience = "Mundial.Client"

Restaure as dependencias:

dotnet restore

Compile o backend:

dotnet build

Inicie a API:

dotnet run --project src\Mundial.Api

Mantenha esse terminal aberto.

A API utiliza a porta HTTP 5072 no ambiente de desenvolvimento atual. Confira o endereco informado no terminal.

IMPORTANTE: as variaveis de ambiente sao validas na sessao atual do PowerShell. Se reiniciar o terminal, sera necessario configura-las novamente. Guarde a chave JWT gerada para reutiliza-la durante essa sessao.

4. CONFIGURAR O FRONTEND

Abra um segundo terminal do PowerShell.

Entre na pasta do frontend:

cd supermercados-mundial\frontend

Instale as dependencias:

npm.cmd install

Inicie o Angular:

npx.cmd ng serve

Mantenha esse terminal aberto.

5. ACESSAR O SISTEMA

Abra o navegador e acesse:

http://localhost:4200

Utilize o usuario e a senha configurados para o ambiente local.

A API utilizada pelo frontend esta configurada para:

http://localhost:5072

Se a API estiver em outra porta, sera necessario ajustar a configuracao de URL no frontend.

6. BANCO DE DADOS

O sistema utiliza SQLite.

O arquivo do banco de dados local nao e enviado pelo Git. Em uma instalacao nova, verifique o procedimento de inicializacao do banco e as migrations existentes antes de utilizar a aplicacao.

Nao exclua o banco de dados de um ambiente que contenha informacoes importantes.

7. PROBLEMAS COMUNS

Erro ao executar npm:
Utilize npm.cmd em vez de npm no PowerShell.

Erro 401 no login ou nas consultas:
Verifique as credenciais, a chave JWT, o emissor, a audiencia e se a API esta em execucao.

Erro de conexao com a API:
Confirme se o backend esta ativo na porta configurada.

Erro ao compilar:
Execute dotnet restore na raiz e npm.cmd install na pasta frontend.

8. OBSERVACOES

- Execute o backend e o frontend em terminais separados.
- Nao compartilhe senhas nem chaves JWT.
- As configuracoes de desenvolvimento podem precisar de ajustes em outro computador.
- O projeto esta em desenvolvimento e algumas funcionalidades ainda estao sendo implementadas.

Repositorio:
https://github.com/BrunoLO321/supermercados-mundial

Supermercados Mundial
