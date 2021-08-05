using System.Collections.Generic;

namespace minhas_financas.api.V1.ViewModels
{
    public abstract class ApiResponse
    {
        public readonly bool Success;

        public ApiResponse(bool success)
        {
            Success = success;
        }
    }

    public class ApiOkResponse : ApiResponse
    {
        public readonly object Data;

        public ApiOkResponse(bool success, object data) : base(success)
        {
            Data = data;
        }
    }

    public class ApiCreatedResponse : ApiOkResponse
    {
        public ApiCreatedResponse(bool success, object data) : base(success, data) {}
    }

    public class ApiBadRequestResponse : ApiResponse
    {
        public readonly List<string> Erros;

        public ApiBadRequestResponse(bool success, List<string> erros) : base(success)
        {
            Erros = erros;
        }
    }
}
