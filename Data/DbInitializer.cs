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

            context.Database.Migrate();

            if (context.Clientes.Any())
                return;

            // 1. Criar Clientes (Reais)
            var clientes = new ClienteModel[]
            {
                new ClienteModel { InstagramUser = "@carla_modas", Nome = "Carla Dias", Telefone = "11999991111" },
                new ClienteModel { InstagramUser = "@julia.store", Nome = "Júlia Mendes", Telefone = "21988882222" },
                new ClienteModel { InstagramUser = "@marcos_silva", Nome = "Marcos Silva", Telefone = "31977773333" },
                new ClienteModel { InstagramUser = "@ana_paula12", Nome = "Ana Paula", Telefone = "41966664444" },
                new ClienteModel { InstagramUser = "@luiza_fashion", Nome = "Luiza Costa", Telefone = "51955555555" },
                new ClienteModel { InstagramUser = "@patricia", Nome = "Patrícia Alves", Telefone = null },
                new ClienteModel { InstagramUser = "@roberta", Nome = "Roberta Oliveira", Telefone = "61944446666" },
                new ClienteModel { InstagramUser = "@fernanda.rj", Nome = "Fernanda", Telefone = "71933337777" },
                new ClienteModel { InstagramUser = "@camila_99", Nome = "Camila Souza", Telefone = null },
                new ClienteModel { InstagramUser = "@rafael_compras", Nome = "Rafael", Telefone = "81922228888" }
            };
            context.Clientes.AddRange(clientes);
            context.SaveChanges();

            // 2. Criar Lives (1 Ativa, 1 Finalizada)
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

            // 3. Criar Vendas — códigos podem repetir entre lives diferentes
            var vendas = new VendaModel[]
            {
                // Vendas Live Finalizada
                new VendaModel { LiveId = liveFinalizada.Id, ClienteInstagram = "@carla_modas", Codigo = "BLUSA-001", Nome = "Blusa Canelada Preta", Descricao = "Tamanho M", Valor = 49.90m, DataVenda = liveFinalizada.IniciadaEm.AddMinutes(15), Status = StatusVenda.Entregue },
                new VendaModel { LiveId = liveFinalizada.Id, ClienteInstagram = "@carla_modas", Codigo = "ACES-001", Nome = "Colar Dourado", Valor = 29.90m, DataVenda = liveFinalizada.IniciadaEm.AddMinutes(20), Status = StatusVenda.Entregue },
                new VendaModel { LiveId = liveFinalizada.Id, ClienteInstagram = "@julia.store", Codigo = "CALCA-001", Nome = "Mom Jeans", Descricao = "Tamanho 38", Valor = 119.90m, DataVenda = liveFinalizada.IniciadaEm.AddMinutes(45), Status = StatusVenda.Pago },
                new VendaModel { LiveId = liveFinalizada.Id, ClienteInstagram = "@julia.store", Codigo = "VEST-001", Nome = "Vestido Midi Vermelho", Descricao = "Tamanho M", Valor = 159.90m, DataVenda = liveFinalizada.IniciadaEm.AddMinutes(50), Status = StatusVenda.Pago },
                new VendaModel { LiveId = liveFinalizada.Id, ClienteInstagram = "@patricia", Codigo = "BLUSA-002", Nome = "T-Shirt Básica", Descricao = "Tamanho P", Valor = 39.90m, DataVenda = liveFinalizada.IniciadaEm.AddMinutes(70), Status = StatusVenda.Reservado },
                new VendaModel { LiveId = liveFinalizada.Id, ClienteInstagram = "@ana_paula12", Codigo = "CALCA-002", Nome = "Alfaiataria Slim", Descricao = "Tamanho 40", Valor = 139.90m, DataVenda = liveFinalizada.IniciadaEm.AddMinutes(85), Status = StatusVenda.Cancelado },

                // Vendas Live Ativa — repetem códigos da live finalizada (legítimo agora)
                new VendaModel { LiveId = liveAtiva.Id, ClienteInstagram = "@luiza_fashion", Codigo = "VEST-001", Nome = "Vestido Midi Vermelho", Descricao = "Tamanho M", Valor = 159.90m, DataVenda = liveAtiva.IniciadaEm.AddMinutes(10), Status = StatusVenda.Reservado },
                new VendaModel { LiveId = liveAtiva.Id, ClienteInstagram = "@rafael_compras", Codigo = "BLUSA-001", Nome = "Blusa Canelada Preta", Descricao = "Tamanho M", Valor = 49.90m, DataVenda = liveAtiva.IniciadaEm.AddMinutes(25), Status = StatusVenda.Reservado },
                new VendaModel { LiveId = liveAtiva.Id, ClienteInstagram = "@rafael_compras", Codigo = "ACES-002", Nome = "Brinco Argola Prata", Valor = 19.90m, DataVenda = liveAtiva.IniciadaEm.AddMinutes(30), Status = StatusVenda.Pago },
            };
            context.Vendas.AddRange(vendas);
            context.SaveChanges();

            // 4. Registro de Envio de Relatório (simulando que Júlia e Carla já receberam)
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
