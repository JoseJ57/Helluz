using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace TuProyecto.Services
{
    public class ActualizadorPromocionesBgService : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<ActualizadorPromocionesBgService> _logger;

        public ActualizadorPromocionesBgService(
            IServiceProvider serviceProvider,
            ILogger<ActualizadorPromocionesBgService> logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Servicio de actualización de promociones iniciado");

            // Esperar 10 segundos antes de la primera ejecución
            await Task.Delay(TimeSpan.FromSeconds(10), stoppingToken);

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    using (var scope = _serviceProvider.CreateScope())
                    {
                        var membresiaService = scope.ServiceProvider
                            .GetRequiredService<MembresiaService>();

                        await membresiaService.ActualizarEstadosPromociones();

                        _logger.LogInformation(
                            "Estados de promociones actualizados correctamente a las {time}",
                            DateTimeOffset.Now);
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error al actualizar estados de promociones");
                }

                // Esperar 24 horas hasta la próxima ejecución
                // Para pruebas puedes usar TimeSpan.FromMinutes(1)
                await Task.Delay(TimeSpan.FromHours(24), stoppingToken);
            }
        }
    }
}