using LiveStore.Models;
using Microsoft.EntityFrameworkCore;

namespace LiveStore.Data
{
    public static class DbInitializer
    {
        public static void Initialize(IServiceProvider serviceProvider)
        {
            using var context = new ApplicationDbContext(
                serviceProvider.GetRequiredService<DbContextOptions<ApplicationDbContext>>());

            // Garante que o banco seja deletado e recriado com os esquemas atuais
            // Cria as tabelas do banco automaticamente (sem tentar apagar o banco antes)
            context.Database.Migrate();

            // Verifica se já existem clientes (caso Migrate persista dados, o que não deveria ocorrer com EnsureDeleted)
            if (context.Clientes.Any())
                return; // O BD já tem dados

            // 1. Criar Clientes (Reais)
            var clientes = new ClienteModel[]
            {
                new ClienteModel { InstagramUser = "@carla_modas", Nome = "Carla Dias", Telefone = "11999991111" },
                new ClienteModel { InstagramUser = "@julia.store", Nome = "Júlia Mendes", Telefone = "21988882222" },
                new ClienteModel { InstagramUser = "@marcos_silva", Nome = "Marcos Silva", Telefone = "31977773333" },
                new ClienteModel { InstagramUser = "@ana_paula12", Nome = "Ana Paula", Telefone = "41966664444" },
                new ClienteModel { InstagramUser = "@luiza_fashion", Nome = "Luiza Costa", Telefone = "51955555555" },
                new ClienteModel { InstagramUser = "@patricia", Nome = "Patrícia Alves", Telefone = null }, // Sem telefone
                new ClienteModel { InstagramUser = "@roberta", Nome = "Roberta Oliveira", Telefone = "61944446666" },
                new ClienteModel { InstagramUser = "@fernanda.rj", Nome = "Fernanda", Telefone = "71933337777" },
                new ClienteModel { InstagramUser = "@camila_99", Nome = "Camila Souza", Telefone = null }, // Sem telefone
                new ClienteModel { InstagramUser = "@rafael_compras", Nome = "Rafael", Telefone = "81922228888" }
            };
            context.Clientes.AddRange(clientes);
            context.SaveChanges();

            // 2. Criar Produtos (Vestuário, Acessórios)
            var produtos = new ProdutoModel[]
            {
                new ProdutoModel { Codigo = "BLUSA-001", Tipo = "Blusa", Cor = "Preta", Nome = "Blusa Canelada Preta", Preco = 49.90m, Tamanho = "M" },
                new ProdutoModel { Codigo = "BLUSA-002", Tipo = "Blusa", Cor = "Branca", Nome = "T-Shirt Básica", Preco = 39.90m, Tamanho = "P" },
                new ProdutoModel { Codigo = "CALCA-001", Tipo = "Calça", Cor = "Jeans", Nome = "Mom Jeans", Preco = 119.90m, Tamanho = "38" },
                new ProdutoModel { Codigo = "CALCA-002", Tipo = "Calça", Cor = "Preta", Nome = "Alfaiataria Slim", Preco = 139.90m, Tamanho = "40" },
                new ProdutoModel { Codigo = "VEST-001", Tipo = "Vestido", Cor = "Vermelho", Nome = "Vestido Midi Vermelho", Preco = 159.90m, Tamanho = "M" },
                new ProdutoModel { Codigo = "ACES-001", Tipo = "Acessório", Cor = "Dourado", Nome = "Colar Dourado", Preco = 29.90m, Tamanho = "Único" },
                new ProdutoModel { Codigo = "ACES-002", Tipo = "Acessório", Cor = "Prata", Nome = "Brinco Argola Prata", Preco = 19.90m, Tamanho = "Único" }
            };
            context.Produtos.AddRange(produtos);
            context.SaveChanges();

            // 3. Criar Lives (1 Ativa, 1 Finalizada)
            var liveFinalizada = new LiveModel
            {
                Nome = "Live Queima de Estoque",
                Descricao = "Descontos de até 50%",
                IniciadaEm = DateTime.Now.AddDays(-2).Date.AddHours(19),
                EncerradaEm = DateTime.Now.AddDays(-2).Date.AddHours(22),
                Status = StatusLive.Encerrada
            };
            
            var liveAtiva = new LiveModel
            {
                Nome = "Lançamento Coleção Inverno",
                Descricao = "Novidades exclusivas!",
                IniciadaEm = DateTime.Now.AddHours(-1),
                Status = StatusLive.Ativa
            };
            
            context.Lives.AddRange(liveFinalizada, liveAtiva);
            context.SaveChanges();

            // 4. Criar Vendas
            var vendas = new VendaModel[]
            {
                // Vendas Live Finalizada
                new VendaModel { LiveId = liveFinalizada.Id, ClienteInstagram = "@carla_modas", CodigoProduto = "BLUSA-001", NomeProduto = "Blusa Canelada Preta", ProdutoId = produtos[0].Id, Valor = 49.90m, DataVenda = liveFinalizada.IniciadaEm.AddMinutes(15), Status = StatusVenda.Entregue },
                new VendaModel { LiveId = liveFinalizada.Id, ClienteInstagram = "@carla_modas", CodigoProduto = "ACES-001", NomeProduto = "Colar Dourado", ProdutoId = produtos[5].Id, Valor = 29.90m, DataVenda = liveFinalizada.IniciadaEm.AddMinutes(20), Status = StatusVenda.Entregue },
                
                new VendaModel { LiveId = liveFinalizada.Id, ClienteInstagram = "@julia.store", CodigoProduto = "CALCA-001", NomeProduto = "Mom Jeans", ProdutoId = produtos[2].Id, Valor = 119.90m, DataVenda = liveFinalizada.IniciadaEm.AddMinutes(45), Status = StatusVenda.Pago },
                new VendaModel { LiveId = liveFinalizada.Id, ClienteInstagram = "@julia.store", CodigoProduto = "VEST-001", NomeProduto = "Vestido Midi Vermelho", ProdutoId = produtos[4].Id, Valor = 159.90m, DataVenda = liveFinalizada.IniciadaEm.AddMinutes(50), Status = StatusVenda.Pago },
                
                new VendaModel { LiveId = liveFinalizada.Id, ClienteInstagram = "@patricia", CodigoProduto = "BLUSA-002", NomeProduto = "T-Shirt Básica", ProdutoId = produtos[1].Id, Valor = 39.90m, DataVenda = liveFinalizada.IniciadaEm.AddMinutes(70), Status = StatusVenda.Reservado }, // patricia não tem telefone
                
                new VendaModel { LiveId = liveFinalizada.Id, ClienteInstagram = "@ana_paula12", CodigoProduto = "CALCA-002", NomeProduto = "Alfaiataria Slim", ProdutoId = produtos[3].Id, Valor = 139.90m, DataVenda = liveFinalizada.IniciadaEm.AddMinutes(85), Status = StatusVenda.Cancelado }, // cancelada

                // Vendas Live Ativa
                new VendaModel { LiveId = liveAtiva.Id, ClienteInstagram = "@luiza_fashion", CodigoProduto = "VEST-001", NomeProduto = "Vestido Midi Vermelho", ProdutoId = produtos[4].Id, Valor = 159.90m, DataVenda = liveAtiva.IniciadaEm.AddMinutes(10), Status = StatusVenda.Reservado },
                new VendaModel { LiveId = liveAtiva.Id, ClienteInstagram = "@rafael_compras", CodigoProduto = "BLUSA-001", NomeProduto = "Blusa Canelada Preta", ProdutoId = produtos[0].Id, Valor = 49.90m, DataVenda = liveAtiva.IniciadaEm.AddMinutes(25), Status = StatusVenda.Reservado },
                new VendaModel { LiveId = liveAtiva.Id, ClienteInstagram = "@rafael_compras", CodigoProduto = "ACES-002", NomeProduto = "Brinco Argola Prata", ProdutoId = produtos[6].Id, Valor = 19.90m, DataVenda = liveAtiva.IniciadaEm.AddMinutes(30), Status = StatusVenda.Pago },
            };
            context.Vendas.AddRange(vendas);
            context.SaveChanges();

            // 5. Criar Registro de Envio de Relatório para a live finalizada
            // Simulando que "Júlia" e "Carla" já receberam o relatório.
            var envios = new ClienteLiveRelatorioModel[]
            {
                new ClienteLiveRelatorioModel { LiveId = liveFinalizada.Id, ClienteInstagram = "@julia.store", Enviado = true, DataEnvio = DateTime.Now.AddDays(-1) },
                new ClienteLiveRelatorioModel { LiveId = liveFinalizada.Id, ClienteInstagram = "@carla_modas", Enviado = true, DataEnvio = DateTime.Now.AddDays(-1).AddHours(2) }
            };
            context.RelatoriosEnviados.AddRange(envios);
            context.SaveChanges();
        }
    }
}
