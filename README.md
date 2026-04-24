<div align="center">

<img src="TelasImagens/TELA LOGIN.png" alt="LiveStore Logo" width="100%"/>

# 🛍️ LiveStore — Sistema de Gestão de Lives de Vendas

**Plataforma completa para gerenciar vendas em lives do Instagram em tempo real.**  
Registre vendas, acompanhe clientes, controle produtos e envie relatórios personalizados via WhatsApp — tudo em um só lugar.

![ASP.NET Core](https://img.shields.io/badge/ASP.NET%20Core-8.0-512BD4?style=for-the-badge&logo=dotnet)
![SQL Server](https://img.shields.io/badge/SQL%20Server-CC2927?style=for-the-badge&logo=microsoftsqlserver&logoColor=white)
![Bootstrap](https://img.shields.io/badge/Bootstrap-5-7952B3?style=for-the-badge&logo=bootstrap)
![Entity Framework](https://img.shields.io/badge/Entity%20Framework-Core-512BD4?style=for-the-badge&logo=dotnet)

</div>

---

## ✨ Funcionalidades

| Módulo | Descrição |
|--------|-----------|
| 🔐 **Login** | Autenticação segura com cookie session |
| 📊 **Dashboard** | Visão geral de faturamento, vendas e gastos do mês |
| 📡 **Lives** | Iniciar, acompanhar em tempo real e encerrar lives |
| 🛒 **Vendas** | Registro rápido de vendas com código de produto |
| 👥 **Clientes** | Cadastro e edição de clientes por @ do Instagram |
| 📦 **Produtos** | Catálogo de produtos com código único, cor e tamanho |
| 💸 **Gastos** | Controle de gastos mensais da operação |
| 📄 **Relatórios** | Geração de PDF e envio por WhatsApp para cada cliente |

---

## 🖥️ Telas do Sistema

### 🔑 Login
<img src="TelasImagens/TELA LOGIN.png" alt="Tela de Login" width="100%"/>

---

### 📊 Dashboard
<img src="TelasImagens/TELA DASHBOARD.png" alt="Dashboard" width="100%"/>

---

### 📡 Histórico de Lives
<img src="TelasImagens/TELA HISTORICO-LIVES.png" alt="Histórico de Lives" width="100%"/>

---

### ▶️ Live Ativa (Vendas em Tempo Real)
<img src="TelasImagens/TELA LIVE-ATIVA.png" alt="Live Ativa" width="100%"/>

---

### ➕ Iniciar Nova Live
<img src="TelasImagens/TELA INICIAR -NOVA-LIVE.png" alt="Nova Live" width="100%"/>

---

### 🛒 Nova Venda
<img src="TelasImagens/TELA NOVA-VENDA.png" alt="Nova Venda" width="100%"/>

---

### 👥 Clientes
<img src="TelasImagens/TELA CLIENTES.png" alt="Clientes" width="100%"/>

---

### ✏️ Editar Cliente
<img src="TelasImagens/TELA EDITAR-CLIENTE.png" alt="Editar Cliente" width="100%"/>

---

### 📦 Produtos
<img src="TelasImagens/TELA PRODUTOS.png" alt="Produtos" width="100%"/>

---

### ➕ Novo Produto
<img src="TelasImagens/TELA NOVO-PRODUTO.png" alt="Novo Produto" width="100%"/>

---

### 💸 Gastos Mensais
<img src="TelasImagens/TELA GASTOS.png" alt="Gastos" width="100%"/>

---

### ➕ Novo Gasto
<img src="TelasImagens/TELA NOVO-GASTO.png" alt="Novo Gasto" width="100%"/>

---

### 🗑️ Excluir Gasto
<img src="TelasImagens/TELA EXCLUIR-GASTO.png" alt="Excluir Gasto" width="100%"/>

---

## 🏗️ Arquitetura do Projeto

```
LiveStore/
├── Controllers/          # MVC Controllers (Live, Venda, Cliente, Produto, Gasto, Login)
├── Data/
│   ├── ApplicationDbContext.cs   # Contexto do Entity Framework
│   └── DbInitializer.cs          # Seed automático de dados iniciais
├── Migrations/           # Migrations do Entity Framework Core
├── Models/               # Entidades do banco de dados
├── Repositories/         # Padrão Repository (acesso ao banco)
├── Services/             # Lógica de negócio (PDF, WhatsApp, etc.)
│   └── Interfaces/       # Contratos dos serviços
├── Views/                # Razor Views (frontend)
│   ├── Live/
│   ├── Venda/
│   ├── Cliente/
│   ├── Produto/
│   ├── Gasto/
│   └── Shared/           # Layout principal e partials
└── wwwroot/              # Arquivos estáticos (CSS, JS, imagens)
```

---

## 🚀 Como Rodar Localmente

### Pré-requisitos

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- [SQL Server](https://www.microsoft.com/pt-br/sql-server/sql-server-downloads) (ou SQL Server Express)
- [Git](https://git-scm.com/)

---

### 1. Clonar o Repositório

```bash
git clone https://github.com/j0tinhaa/Pim-Vendas-Live.git
cd Pim-Vendas-Live
git checkout pjota
```

---

### 2. Configurar a Connection String

Abra o arquivo `appsettings.json` e altere a connection string para o seu banco de dados local:

```json
"ConnectionStrings": {
  "DefaultConnection": "Server=SEU_SERVIDOR; Database=LiveStoreDB; Trusted_Connection=True; TrustServerCertificate=True;"
}
```

> 💡 Se estiver usando SQL Server local com Windows Authentication, basta substituir `SEU_SERVIDOR` pelo nome da sua instância (ex: `localhost` ou `DESKTOP-XXXX`).

---

### 3. Restaurar Dependências

```bash
dotnet restore
```

---

### 4. Rodar o Projeto

```bash
dotnet run
```

> ✅ Na primeira execução, o sistema cria automaticamente as tabelas no banco e popula com dados de exemplo (clientes, produtos, lives e vendas realistas). Não precisa rodar nenhum script SQL manualmente!

Acesse: **http://localhost:5091**

---

### 5. Login Padrão

```
Usuário: admin
Senha:   admin123
```

> ⚠️ Recomendamos alterar as credenciais antes de usar em produção.

---

## 📦 Dependências Principais

| Pacote | Versão | Uso |
|--------|--------|-----|
| `Microsoft.EntityFrameworkCore.SqlServer` | 8.x | ORM para SQL Server |
| `Microsoft.EntityFrameworkCore.Tools` | 8.x | Migrations e CLI |
| `QuestPDF` | Latest | Geração de relatórios PDF |
| `Bootstrap` | 5 | Interface responsiva |
| `Bootstrap Icons` | Latest | Ícones da interface |
| `DataTables` | Latest | Tabelas interativas |

---

## 📄 Envio de Relatórios por WhatsApp

O sistema gera um **PDF personalizado** com as compras de cada cliente e oferece integração com WhatsApp:

- **Modo Mock (padrão):** Salva o PDF localmente e gera um link `wa.me` para envio manual — ideal para testes.
- **Modo Produção:** Basta trocar o `MockWhatsAppService` por uma implementação real (Twilio ou Meta Cloud API) no `Program.cs`. O guia está nos comentários do `WhatsAppService.cs`.

---

## ☁️ Deploy (Hospedagem)

Para hospedar em produção:

```bash
dotnet publish -c Release -o ./publish
```

Compacte a pasta `publish/` e faça o upload no seu servidor (ex: [Somee.com](https://somee.com), Azure App Service, etc.).

---

## 👨‍💻 Desenvolvido por

<div align="center">

**Pjotah - João Pedro Vinhal**  

</div>

---

<div align="center">

⭐ Se este projeto te ajudou, deixe uma estrela no repositório!

</div>
