using LiveStore.Models;
using LiveStore.Services.Interfaces;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace LiveStore.Services
{
    public class RelatorioService : IRelatorioService
    {
        static RelatorioService()
        {
            QuestPDF.Settings.License = LicenseType.Community;
        }

        public byte[] GerarPdfVendasCliente(ClienteModel cliente, List<VendaModel> vendas, LiveModel live)
        {
            var ptBR        = new System.Globalization.CultureInfo("pt-BR");
            var totalValido = vendas.Where(v => v.Status != StatusVenda.Cancelado).Sum(v => v.Valor);
            var nomeCli     = !string.IsNullOrWhiteSpace(cliente.Nome) ? cliente.Nome : cliente.InstagramUser;

            var document = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4);
                    page.Margin(36);
                    page.DefaultTextStyle(ts => ts.FontSize(10).FontColor("#e0e0f0").FontFamily("Arial"));

                    // ── FUNDO ESCURO ──────────────────────────────────────────────
                    page.Background().Background("#0d0b22");

                    // ── HEADER ────────────────────────────────────────────────────
                    page.Header().Column(col =>
                    {
                        col.Item().Row(row =>
                        {
                            row.RelativeItem().Column(c =>
                            {
                                c.Item().Text("LiveStore").FontSize(22).Bold().FontColor("#fa6b9a");
                                c.Item().Text($"Relatório de Compras — {live.Nome}").FontSize(11).FontColor("#aaa0cc");
                            });
                            row.ConstantItem(120).AlignRight().Column(c =>
                            {
                                c.Item().Text(live.IniciadaEm.ToString("dd/MM/yyyy")).FontSize(9).FontColor("#aaa0cc");
                                c.Item().Text($"Gerado em {DateTime.Now:dd/MM HH:mm}").FontSize(8).FontColor("#666");
                            });
                        });
                        col.Item().PaddingTop(6).LineHorizontal(1.5f).LineColor("#fa6b9a");
                        col.Item().Height(8);
                    });

                    // ── CONTENT ───────────────────────────────────────────────────
                    page.Content().Column(col =>
                    {
                        // Card cliente
                        col.Item().Background("#1a1730").Border(1).BorderColor("#fa6b9a").Padding(12).Column(c =>
                        {
                            c.Item().Text($"Cliente: {nomeCli}").FontSize(13).Bold().FontColor("#fa6b9a");
                            if (!string.Equals(cliente.InstagramUser, nomeCli, StringComparison.OrdinalIgnoreCase))
                                c.Item().Text($"Instagram: {cliente.InstagramUser}").FontColor("#aaa0cc");
                            if (!string.IsNullOrWhiteSpace(cliente.Telefone))
                                c.Item().Text($"Telefone: {cliente.Telefone}").FontColor("#aaa0cc");
                        });

                        col.Item().Height(14);
                        col.Item().Text($"Compras Realizadas ({vendas.Count})").FontSize(12).Bold().FontColor("#e0e0f0");
                        col.Item().Height(6);

                        if (!vendas.Any())
                        {
                            col.Item().Background("#1a1730").Padding(12)
                                .Text("Nenhuma compra registrada.").FontColor("#888");
                        }
                        else
                        {
                            // Usa Table para layout de grade correto
                            col.Item().Table(table =>
                            {
                                table.ColumnsDefinition(cols =>
                                {
                                    cols.RelativeColumn(3); // Produto
                                    cols.RelativeColumn(1); // Código
                                    cols.RelativeColumn(1); // Valor
                                    cols.RelativeColumn(1); // Status
                                    cols.RelativeColumn(1); // Data
                                });

                                // Header
                                table.Header(header =>
                                {
                                    string[] headers = { "Produto", "Código", "Valor", "Status", "Data" };
                                    foreach (var h in headers)
                                        header.Cell().Background("#241e4a").Padding(8)
                                            .Text(h).Bold().FontSize(9).FontColor("#fa6b9a");
                                });

                                // Rows
                                bool alt = false;
                                foreach (var v in vendas)
                                {
                                    var bg = alt ? "#1a1730" : "#130f2a";
                                    var statusCor = v.Status switch
                                    {
                                        StatusVenda.Pago      => "#22c55e",
                                        StatusVenda.Entregue  => "#3b82f6",
                                        StatusVenda.Cancelado => "#ef4444",
                                        _                     => "#f59e0b"
                                    };

                                    // Coluna Produto (com sub-info)
                                    table.Cell().Background(bg).Padding(7).Column(c =>
                                    {
                                        c.Item().Text(v.NomeProduto).FontSize(10);
                                        var info = new List<string>();
                                        if (!string.IsNullOrWhiteSpace(v.Produto?.Cor))  info.Add($"Cor: {v.Produto.Cor}");
                                        if (!string.IsNullOrWhiteSpace(v.Produto?.Tipo)) info.Add($"Tipo: {v.Produto.Tipo}");
                                        if (info.Any())
                                            c.Item().Text(string.Join(" | ", info)).FontSize(8).FontColor("#aaa0cc");
                                    });

                                    table.Cell().Background(bg).Padding(7)
                                        .Text(v.CodigoProduto).FontSize(9).FontColor("#aaa0cc");

                                    table.Cell().Background(bg).Padding(7).AlignRight()
                                        .Text(v.Valor.ToString("C2", ptBR)).FontSize(10).FontColor("#22c55e");

                                    table.Cell().Background(bg).Padding(7).AlignCenter()
                                        .Text(v.Status.ToString()).FontSize(9).FontColor(statusCor);

                                    table.Cell().Background(bg).Padding(7).AlignRight()
                                        .Text(v.DataVenda.ToString("dd/MM HH:mm")).FontSize(9);

                                    alt = !alt;
                                }
                            });
                        }

                        col.Item().Height(10);

                        // Total
                        col.Item().Background("#241e4a").Padding(10).Row(row =>
                        {
                            row.RelativeItem().Text("Total (exceto cancelados):")
                                .Bold().FontColor("#e0e0f0").FontSize(11);
                            row.ConstantItem(130).AlignRight()
                                .Text(totalValido.ToString("C2", ptBR))
                                .Bold().FontSize(13).FontColor("#22c55e");
                        });

                        col.Item().Height(18);

                        col.Item().Background("#1a1730").Border(1).BorderColor("#fa6b9a").Padding(12).Column(c =>
                        {
                            c.Item().Text("Obrigada pela sua compra! 💖").FontSize(12).Bold().FontColor("#fa6b9a");
                            c.Item().Text("Em caso de dúvidas, entre em contato pelo Instagram da loja.")
                                .FontColor("#aaa0cc").FontSize(9);
                        });
                    });

                    // ── FOOTER ────────────────────────────────────────────────────
                    page.Footer().AlignCenter().Text(t =>
                    {
                        t.Span("LiveStore  •  Gerado em ").FontColor("#555").FontSize(8);
                        t.Span(DateTime.Now.ToString("dd/MM/yyyy HH:mm")).FontColor("#555").FontSize(8);
                    });
                });
            });

            return document.GeneratePdf();
        }
    }
}
