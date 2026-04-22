$(function () {

    const dtLang = {
        emptyTable:     "Nenhum registro encontrado",
        info:           "Mostrando _START_ até _END_ de _TOTAL_ registros",
        infoEmpty:      "Nenhum registro encontrado",
        infoFiltered:   "(filtrado de _MAX_ registros)",
        thousands:      ".",
        lengthMenu:     "Mostrar _MENU_ registros",
        loadingRecords: "Carregando...",
        processing:     "Processando...",
        search:         "Buscar:",
        zeroRecords:    "Nenhum resultado encontrado",
        paginate:       { first: "Primeiro", last: "Último", next: "Próximo", previous: "Anterior" }
    };

    // Tabela de vendas (live ativa e detalhes)
    if ($('#tabelaVendas').length) {
        $('#tabelaVendas').DataTable({ language: dtLang, order: [[4, 'desc']], pageLength: 25 });
    }

    // Tabela de produtos
    if ($('#tabelaProdutos').length) {
        $('#tabelaProdutos').DataTable({ language: dtLang, order: [[0, 'asc']] });
    }

    // Tabela de clientes
    if ($('#tabelaClientes').length) {
        $('#tabelaClientes').DataTable({ language: dtLang, order: [[3, 'desc']] });
    }

    // Auto-fechar alertas após 6s
    setTimeout(function () {
        $(".alert").fadeOut("slow", function () { $(this).remove(); });
    }, 6000);
});

// ── Sidebar mobile ───────────────────────────────────────────────────────────
function toggleSidebar() {
    document.getElementById("sidebar").classList.toggle("active");
}

window.addEventListener("resize", function () {
    if (window.innerWidth >= 768)
        document.getElementById("sidebar").classList.remove("active");
});

document.addEventListener("click", function (e) {
    if (window.innerWidth >= 768) return;
    const sidebar = document.getElementById("sidebar");
    const btn     = document.querySelector(".btn-hamburguer");
    if (!sidebar || !btn) return;
    if (!sidebar.contains(e.target) && !btn.contains(e.target))
        sidebar.classList.remove("active");
});
