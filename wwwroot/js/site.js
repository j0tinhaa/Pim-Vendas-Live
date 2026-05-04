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

// ── Autocomplete (Clientes e Produtos) ──────────────────────────────────────
function setupAutocomplete(inputSelector, dataUrl, itemRenderer, onSelect) {
    const $input = $(inputSelector);
    if (!$input.length) return;

    $input.wrap('<div class="autocomplete-wrapper" style="position: relative;"></div>');
    const $list = $('<div class="autocomplete-list" style="display:none;"></div>').appendTo($input.parent());

    let timeout = null;

    $input.on('input', function() {
        clearTimeout(timeout);
        const term = $(this).val();
        if (term.length < 1) {
            $list.hide().empty();
            return;
        }

        timeout = setTimeout(() => {
            $.get(dataUrl, { term: term }, function(data) {
                $list.empty();
                if (data.length === 0) {
                    $list.hide();
                    return;
                }

                data.forEach(item => {
                    const $item = $(itemRenderer(item)).addClass('autocomplete-item');
                    $item.on('mousedown', function(e) {
                        e.preventDefault(); // Evita blur do input antes de clicar
                        onSelect(item, $input);
                        $list.hide();
                    });
                    $list.append($item);
                });
                $list.show();
            });
        }, 200); // 200ms debounce
    });

    $input.on('blur', () => setTimeout(() => $list.hide(), 150));
    $input.on('focus', () => { if ($list.children().length > 0) $list.show(); });
}

$(function() {
    setupAutocomplete('#ClienteInstagram', '/Venda/BuscarClientes',
        (c) => `<div><strong>${c.instagram}</strong> <span class="text-muted">${c.nome ? '- ' + c.nome : ''}</span></div>`,
        (c, $input) => $input.val(c.instagram)
    );
});

// ── Modal Global de Confirmação ─────────────────────────────────────────────
function confirmarExclusao(url, texto, inputsExtras) {
    $('#modalConfirmarExclusaoTexto').text(texto || 'Tem certeza que deseja excluir este item?');
    $('#formConfirmarExclusao').attr('action', url);
    
    let htmlInputs = '';
    if (inputsExtras) {
        for(let key in inputsExtras) {
            htmlInputs += `<input type="hidden" name="${key}" value="${inputsExtras[key]}" />`;
        }
    }
    $('#modalConfirmarExclusaoInputs').html(htmlInputs);
    
    var myModal = new bootstrap.Modal(document.getElementById('modalConfirmarExclusao'));
    myModal.show();
}
