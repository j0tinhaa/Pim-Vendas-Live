# 🎬 LiveStore — Sistema de Vendas por Live

Sistema web para gerenciamento de vendas realizadas durante lives no Instagram.
Desenvolvido em ASP.NET Core MVC com Entity Framework Core e SQL Server.

---

## 📋 Funcionalidades

### 🔐 Autenticação
- Login obrigatório para acesso ao sistema
- Usuário padrão: `admin` / `admin`
- Cookie Authentication (sem Identity)

### 🎬 Lives
- Iniciar nova live (encerra a ativa automaticamente)
- Tela de vendas em tempo real durante a live
- Encerrar live
- Histórico de todas as lives com faturamento e total de vendas

### 💰 Vendas
- Registro de vendas vinculadas à live ativa
- Busca de produto por código (AJAX) com preenchimento automático
- Cadastro automático de cliente e produto ao registrar venda
- Status: Reservado → Pago → Entregue / Cancelado
- Edição e exclusão de vendas

### 📦 Produtos
- Cadastro com código único (ex: ROSA-001)
- Nome, preço, tipo, cor e tamanho
- Soft delete (desativação sem remover histórico)
- Cadastro automático durante registro de venda

### 👥 Clientes
- Identificação via @ do Instagram
- Cadastro automático ao registrar venda
- Campos opcionais: nome e telefone

### 📊 Dashboard
- Total de vendas, clientes e lives
- Faturamento total e do dia
- Live com maior faturamento
- Últimas vendas registradas
- Banner de live ativa

---

## 🛠 Tecnologias

| Tecnologia | Uso |
|---|---|
| ASP.NET Core MVC (.NET 8/9) | Framework principal |
| C# | Linguagem |
| Entity Framework Core | ORM / acesso ao banco |
| SQL Server | Banco de dados |
| Razor Pages | Views / templates |
| Bootstrap 5 | Layout responsivo |
| Bootstrap Icons | Ícones |
| jQuery + DataTables | Tabelas dinâmicas |
| Cookie Authentication | Autenticação |

---

## 🗂 Estrutura do Projeto

```
LiveStore/
├── Controllers/
│   ├── HomeController.cs
│   ├── LoginController.cs
│   ├── LiveController.cs
│   ├── VendaController.cs
│   ├── ProdutoController.cs
│   └── ClienteController.cs
├── Models/
│   ├── ClienteModel.cs
│   ├── LiveModel.cs
│   ├── ProdutoModel.cs
│   └── VendaModel.cs
├── Data/
│   └── ApplicationDbContext.cs
├── Repositories/
│   ├── Interfaces/IRepositories.cs
│   └── Repositories.cs
├── Services/
│   ├── Interfaces/IServices.cs
│   └── Services.cs
├── Views/
│   ├── Home/Index.cshtml        ← Dashboard
│   ├── Login/Index.cshtml       ← Tela de login
│   ├── Live/
│   │   ├── Nova.cshtml
│   │   ├── Ativa.cshtml         ← Tela principal da live
│   │   ├── Index.cshtml         ← Histórico
│   │   └── Detalhes.cshtml
│   ├── Venda/
│   │   ├── Cadastrar.cshtml
│   │   └── Editar.cshtml
│   ├── Produto/
│   │   ├── Index.cshtml
│   │   ├── Cadastrar.cshtml
│   │   └── Editar.cshtml
│   ├── Cliente/
│   │   ├── Index.cshtml
│   │   └── Editar.cshtml
│   └── Shared/
│       └── _Layout.cshtml
├── wwwroot/
│   ├── css/site.css
│   └── js/site.js
├── Program.cs
└── appsettings.json
```

---

## ⚙️ Como Rodar o Projeto

### Pré-requisitos
- .NET 8 ou 9 SDK
- SQL Server (local ou remoto)
- Visual Studio 2022 ou VS Code

### 1. Clonar / copiar o projeto

Copie todos os arquivos para uma pasta de projeto ASP.NET Core MVC.

### 2. Configurar a connection string

Em `appsettings.json`, ajuste o servidor conforme seu ambiente:

```json
"ConnectionStrings": {
  "DefaultConnection": "server=SEU_SERVIDOR; database=LiveStore; trusted_connection=true; TrustServerCertificate=True;"
}
```

### 3. Aplicar as migrations

No **Package Manager Console** (Visual Studio):

```powershell
Add-Migration Inicial
Update-Database
```

Ou via terminal:

```bash
dotnet ef migrations add Inicial
dotnet ef database update
```

### 4. Executar

```bash
dotnet run
```

Acesse: `https://localhost:{porta}`

Login padrão: **admin** / **admin**

---

## 🔄 Fluxo de Lives e Vendas

```
[Nova Live]
    ↓
Informa nome e descrição
    ↓
Live é criada e ativada (live anterior é encerrada automaticamente)
    ↓
[Tela de Live Ativa]
    ↓
Vendedora anuncia peça → informa código na tela
    ↓
Sistema busca produto pelo código (AJAX)
    ↓
Se produto não existir → cadastrado automaticamente
Se cliente não existir → cadastrado automaticamente
    ↓
Venda registrada e vinculada à live
    ↓
[Encerrar Live]
    ↓
Live aparece no Histórico com faturamento total
```

---

## 📊 Modelo de Dados

```
LiveModel
  ├── Id, Nome, Descricao, Status, IniciadaEm, EncerradaEm
  └── → VendaModel[] (1:N)

VendaModel
  ├── Id, LiveId (FK), ClienteInstagram (FK), ProdutoId (FK)
  ├── CodigoProduto, NomeProduto (desnormalizado para histórico)
  ├── Valor, Status, Observacoes
  └── DataVenda, DataAtualizacao

ProdutoModel
  ├── Id, Codigo (único), Nome, Preco
  ├── Tipo, Cor, Tamanho, Ativo
  └── → VendaModel[] (1:N)

ClienteModel
  ├── InstagramUser (PK string), Nome?, Telefone?
  └── → VendaModel[] (1:N)
```

---

## 🔐 Autenticação

O sistema usa **Cookie Authentication** sem ASP.NET Identity.

- Credenciais configuradas diretamente no `LoginController`
- Para produção, recomenda-se migrar para ASP.NET Identity com banco de usuários

---

## 🚀 Melhorias Futuras Sugeridas

1. **Múltiplos usuários** — tabela de usuários com senhas hasheadas (bcrypt)
2. **Níveis de acesso** — admin, operador, visualizador
3. **ASP.NET Identity** — autenticação completa com recovery de senha
4. **Exportar relatório Excel** — ClosedXML ou EPPlus
5. **Integração com Instagram** — webhook para capturar comentários automaticamente
6. **App mobile** — versão PWA ou MAUI para usar durante a live
7. **Notificações em tempo real** — SignalR para atualizar a tela automaticamente
8. **Filtros avançados** — por status, data, produto, cliente
9. **Estoque** — controle de quantidade por produto
10. **Relatórios visuais** — gráficos de faturamento por período (Chart.js)
