using AutoFixture;
using minhas_financas.business.Interfaces;
using minhas_financas.business.Notificacoes;
using System;
using Xunit;

namespace minhas_financas.business.ut.Notificacoes
{
    public class NotificadorTest
    {
        private readonly INotificador _notificador;
        private readonly Fixture _fixture;

        public NotificadorTest()
        {
            _notificador = new Notificador();
            _fixture = new Fixture();
        }

        [Fact]
        public void Handle_AdicionaNotificacaoQuandoEhValida()
        {
            var notificacao = _fixture.Create<Notificacao>();
            _notificador.Handle(notificacao);
            Assert.True(_notificador.TemNotificacao());
            Assert.Single(_notificador.ObterNotificacoes());
            Assert.Collection(_notificador.ObterNotificacoes(), item => item.Equals(notificacao));
        }

        [Fact]
        public void Handle_NaoAdicionaNotificacaoQuandoEhNula()
        {
            Assert.Throws<ArgumentException>(() => _notificador.Handle(null));
            Assert.Empty(_notificador.ObterNotificacoes());
        }
    }
}
