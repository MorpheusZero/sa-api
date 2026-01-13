namespace SoulArenasAPI.Controllers
{

    using SoulArenasAPI.Services;

    public static class HealthController
    {
        public static void MapRoutes(this WebApplication app)
        {
            app.MapGet("/health", (HealthService healthService) => 
                Results.Ok(healthService.GetHealthStatus()));
        }
    }
}