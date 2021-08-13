using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using minhas_financas.api.V1.ViewModels;
using minhas_financas.business.Interfaces;
using minhas_financas.business.Notificacoes;
using System;
using System.Linq;

namespace minhas_financas.api.Controllers
{
    [ApiController]
    public abstract class MainController : ControllerBase
    {
        private readonly INotificador _notificador;
        public readonly IUser AppUser;
        
        protected MainController(INotificador notificador, IUser appUser)
        {
            _notificador = notificador;
            AppUser = appUser;
        }

        protected Guid GetUsuarioId()
        {
            return AppUser.GetUserId();
        }
                
        protected ActionResult CustomCreatedResponse(string uri, string id, object result = null)
        {
            if (OperacaoValida())
                return CreatedAtAction(uri, new { id = id }, new ApiCreatedResponse(true, result));
            
            var erros = _notificador.ObterNotificacoes().Select(n => n.Mensagem).ToList();
            return BadRequest(new ApiBadRequestResponse(false, erros));
        }
                
        protected ActionResult CustomResponse(object result = null)
        {
            if (OperacaoValida()) return Ok(new ApiOkResponse(true, result));
            
            var erros = _notificador.ObterNotificacoes().Select(n => n.Mensagem).ToList();
            return BadRequest(new ApiBadRequestResponse(false, erros));
        }

        protected ActionResult CustomResponse(ModelStateDictionary modelState)
        {
            if (!modelState.IsValid) NotificarErroModelInvalida(modelState);
            return CustomResponse();
        }

        protected void NotificarErroModelInvalida(ModelStateDictionary modelState)
        {
            var erros = modelState.Values.SelectMany(e => e.Errors);
            foreach (var erro in erros)
            {
                var errorMsg = erro.Exception == null ? erro.ErrorMessage : erro.Exception.Message;
                NotificarErro(errorMsg);
            }
        }

        protected void NotificarErro(string mensagem)
        {
            _notificador.Handle(new Notificacao(mensagem));
        }

        protected bool OperacaoValida()
        {
            return !_notificador.TemNotificacao();
        }

        protected bool UsuarioAutenticado()
        {
            return AppUser.IsAuthenticated();
        }

    }
}
