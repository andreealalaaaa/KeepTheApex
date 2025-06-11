namespace KeepTheApex;

using System.Linq;
using Microsoft.AspNetCore.Http;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

public class SwaggerFileUploadOperationFilter : IOperationFilter
{
    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        // look for actions with an IFormFile parameter
        var fileParams = context.MethodInfo.GetParameters()
            .Where(p => p.ParameterType == typeof(IFormFile))
            .ToList();
        if (!fileParams.Any()) return;

        // clear any default RequestBody and replace with multipart/form-data schema
        operation.RequestBody = new OpenApiRequestBody
        {
            Content =
            {
                ["multipart/form-data"] = new OpenApiMediaType
                {
                    Schema = new OpenApiSchema
                    {
                        Type       = "object",
                        Properties = fileParams.ToDictionary(
                            p => p.Name,
                            p => new OpenApiSchema {
                                Type   = "string",
                                Format = "binary"
                            }
                        ),
                        Required = fileParams.Select(p => p.Name).ToHashSet()
                    }
                }
            }
        };
    }
}
