using LiveStore.Services.Interfaces;

namespace LiveStore.Services
{
    /// <summary>
    /// Implementação MOCK do serviço de WhatsApp.
    ///
    /// O que faz:
    ///   1. Salva o PDF em wwwroot/relatorios-temp/ com um nome único
    ///   2. Retorna a URL local do PDF para download imediato no browser
    ///   3. Retorna um link wa.me com mensagem pronta para o WhatsApp Web
    ///
    /// Para ativar envio REAL (produção), substitua esta implementação por:
    ///   - TwilioWhatsAppService (ver comentários abaixo)
    ///   - WhatsAppCloudApiService (Meta Business API)
    ///
    /// Basta registrar a nova implementação em Program.cs:
    ///   builder.Services.AddScoped<IWhatsAppService, TwilioWhatsAppService>();
    /// </summary>
    public class MockWhatsAppService : IWhatsAppService
    {
        private readonly IWebHostEnvironment _env;
        private readonly ILogger<MockWhatsAppService> _logger;

        public MockWhatsAppService(IWebHostEnvironment env, ILogger<MockWhatsAppService> logger)
        {
            _env    = env;
            _logger = logger;
        }

        public Task<ResultadoEnvioRelatorio> EnviarRelatorioAsync(
            string telefone, string nomeCliente, byte[] pdfBytes, string nomeArquivoPdf)
        {
            try
            {
                // 1) Salva o PDF na pasta pública wwwroot/relatorios-temp/
                var tempDir = Path.Combine(_env.WebRootPath, "relatorios-temp");
                Directory.CreateDirectory(tempDir);

                var fileName = $"{Guid.NewGuid():N}_{nomeArquivoPdf}";
                var fullPath = Path.Combine(tempDir, fileName);
                File.WriteAllBytes(fullPath, pdfBytes);

                var urlPdf = $"/relatorios-temp/{fileName}";

                // 2) Monta link wa.me com mensagem pré-preenchida
                var telLimpo = telefone.Replace(" ", "").Replace("-", "").Replace("(", "").Replace(")", "");
                if (telLimpo.Length > 0 && !telLimpo.StartsWith("+"))
                    telLimpo = "55" + telLimpo;

                var mensagem = Uri.EscapeDataString(
                    $"Olá, {nomeCliente}! 🛍️\n\n" +
                    "Segue o relatório das suas compras da nossa live.\n" +
                    "Qualquer dúvida é só chamar!\n\n" +
                    "Obrigada pela preferência! 💖\n— LiveStore"
                );
                var urlWhatsApp = $"https://wa.me/{telLimpo}?text={mensagem}";

                _logger.LogInformation("PDF gerado para {Cliente} → {UrlPdf}", nomeCliente, urlPdf);

                return Task.FromResult(new ResultadoEnvioRelatorio
                {
                    Sucesso     = true,
                    Mensagem    = $"PDF gerado. Abra o WhatsApp e envie manualmente para {telefone}.",
                    UrlPdf      = urlPdf,
                    UrlWhatsApp = urlWhatsApp
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao gerar PDF para {Cliente}", nomeCliente);
                return Task.FromResult(new ResultadoEnvioRelatorio
                {
                    Sucesso  = false,
                    Mensagem = $"Erro ao gerar relatório: {ex.Message}"
                });
            }
        }
    }

    /*
    ════════════════════════════════════════════════════════════════════════
    COMO ATIVAR ENVIO REAL VIA TWILIO WHATSAPP API
    ════════════════════════════════════════════════════════════════════════

    1. Crie conta em twilio.com → habilite WhatsApp Sandbox ou número aprovado
    2. Instale o pacote: dotnet add package Twilio
    3. Adicione no appsettings.json:
       "Twilio": {
         "AccountSid": "ACxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx",
         "AuthToken":  "xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx",
         "FromNumber": "whatsapp:+14155238886"
       }

    4. Crie a classe TwilioWhatsAppService:

    public class TwilioWhatsAppService : IWhatsAppService
    {
        private readonly IConfiguration _config;
        private readonly IWebHostEnvironment _env;

        public TwilioWhatsAppService(IConfiguration config, IWebHostEnvironment env)
        {
            _config = config;
            _env    = env;
            TwilioClient.Init(config["Twilio:AccountSid"], config["Twilio:AuthToken"]);
        }

        public async Task<ResultadoEnvioRelatorio> EnviarRelatorioAsync(
            string telefone, string nomeCliente, byte[] pdfBytes, string nomeArquivoPdf)
        {
            // Salva PDF temporariamente em pasta pública
            var tempDir  = Path.Combine(_env.WebRootPath, "relatorios-temp");
            Directory.CreateDirectory(tempDir);
            var fileName = $"{Guid.NewGuid():N}_{nomeArquivoPdf}";
            File.WriteAllBytes(Path.Combine(tempDir, fileName), pdfBytes);

            // URL PÚBLICA do PDF — precisa ser acessível pela internet!
            // Em produção, substitua pelo domínio real: https://seudominio.com/relatorios-temp/...
            var pdfUrl = $"https://SEU_DOMINIO/relatorios-temp/{fileName}";

            var telFormatado = $"whatsapp:+55{telefone.Replace(" ","").Replace("-","")}";
            var msg = await MessageResource.CreateAsync(
                body:      $"Olá {nomeCliente}! Seu relatório de compras segue em anexo. 💖",
                mediaUrl:  new List<Uri> { new Uri(pdfUrl) },
                from:      new PhoneNumber(_config["Twilio:FromNumber"]),
                to:        new PhoneNumber(telFormatado)
            );

            return new ResultadoEnvioRelatorio
            {
                Sucesso  = msg.Status != MessageResource.StatusEnum.Failed,
                Mensagem = msg.Status.ToString(),
                UrlPdf   = pdfUrl
            };
        }
    }

    5. Em Program.cs, troque:
       builder.Services.AddScoped<IWhatsAppService, MockWhatsAppService>();
       por:
       builder.Services.AddScoped<IWhatsAppService, TwilioWhatsAppService>();
    ════════════════════════════════════════════════════════════════════════
    */
}
