﻿using minhas_financas.business.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;

namespace minhas_financas.business.Notificacoes
{
    public class Notificador : INotificador
    {
        private readonly List<Notificacao> _notificacoes;

        public Notificador()
        {
            _notificacoes = new List<Notificacao>();
        }

        public void Handle(Notificacao notificacao)
        {
            if (notificacao == null)
                throw new ArgumentException("notificação inválida, adicione uma notificação válida.");

            _notificacoes.Add(notificacao);
        }

        public List<Notificacao> ObterNotificacoes()
        {
            return _notificacoes;
        }

        public bool TemNotificacao()
        {
            return _notificacoes.Any();
        }
    }
}
