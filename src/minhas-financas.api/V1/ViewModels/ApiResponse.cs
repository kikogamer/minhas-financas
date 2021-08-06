using System.Collections.Generic;

namespace minhas_financas.api.V1.ViewModels
{
    public abstract class ApiResponse
    {
        public bool Success { get; }

        protected ApiResponse(bool success)
        {
            Success = success;
        }
    }

    public class ApiOkResponse : ApiResponse
    {
        public object Data { get; }

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
        public List<string> Erros { get; }

        public ApiBadRequestResponse(bool success, List<string> erros) : base(success)
        {
            Erros = erros;
        }
    }
}
