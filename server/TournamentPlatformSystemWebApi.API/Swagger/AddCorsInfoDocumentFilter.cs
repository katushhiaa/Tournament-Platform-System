using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace TournamentPlatformSystemWebApi.API.Swagger
{
    public class AddCorsInfoDocumentFilter : IDocumentFilter
    {
        private readonly string _message;

        public AddCorsInfoDocumentFilter(string message)
        {
            _message = message ?? string.Empty;
        }

        public void Apply(OpenApiDocument swaggerDoc, DocumentFilterContext context)
        {
            if (swaggerDoc.Info == null)
            {
                swaggerDoc.Info = new OpenApiInfo { Title = "API", Version = "v1" };
            }

            if (string.IsNullOrWhiteSpace(swaggerDoc.Info.Description))
            {
                swaggerDoc.Info.Description = _message;
            }
            else
            {
                swaggerDoc.Info.Description += "\n\n" + _message;
            }
        }
    }
}
