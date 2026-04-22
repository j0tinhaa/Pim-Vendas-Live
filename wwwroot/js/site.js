$(function () {

    // ── DataTable da listagem de vendas ──────────────────────────────────────
    $('#tabelaVendas').DataTable({
        order: [[4, 'desc']], // ordena por Data da Venda (coluna 4) decrescente
        language: {
            decimal:        "",
            emptyTable:     "Nenhuma venda registrada",
            info:           "Mostrando _START_ até _END_ de _TOTAL_ registros",
            infoEmpty:      "Nenhum registro encontrado",
            infoFiltered:   "(filtrado de _MAX_ registros)",
            thousands:      ".",
            lengthMenu:     "Mostrar _MENU_ registros",
            loadingRecords: "Carregando...",
            processing:     "Processando...",
            search:         "Buscar:",
            zeroRecords:    "Nenhuma venda encontrada",
            paginate: {
                first:    "Primeiro",
                last:     "Último",
                next:     "Próximo",
                previous: "Anterior"
            },
            aria: {
                orderable:        "Ordenar por esta coluna",
                orderableReverse: "Inverter ordenação"
            }
        }
    });

    // ── Auto-fechar alertas após 5 segundos ──────────────────────────────────
    setTimeout(function () {
        $(".alert").fadeOut("slow", function () {
            $(this).remove();
        });
    }, 5000);

});

// ── Sidebar mobile ───────────────────────────────────────────────────────────
function toggleSidebar() {
    document.getElementById("sidebar").classList.toggle("active");
}

// Fecha sidebar ao redimensionar para desktop
window.addEventListener("resize", function () {
    if (window.innerWidth >= 768) {
        document.getElementById("sidebar").classList.remove("active");
    }
});

// Fecha sidebar ao clicar fora (usa closest para robustez)
document.addEventListener("click", function (event) {
    if (window.innerWidth >= 768) return;

    const sidebar = document.getElementById("sidebar");
    const btnHamburguer = document.querySelector(".btn-hamburguer");

    if (!sidebar || !btnHamburguer) return;
    if (sidebar.contains(event.target) || btnHamburguer.contains(event.target)) return;

    sidebar.classList.remove("active");
});
