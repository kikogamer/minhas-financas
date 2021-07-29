using minhas_financas.business.Notificacoes;
using System.Collections.Generic;

namespace minhas_financas.business.Interfaces
{
    public interface INotificador
    {
        bool TemNotificacao();
        List<Notificacao> ObterNotificacoes();
        void Handle(Notificacao notificacao);
    }
}
