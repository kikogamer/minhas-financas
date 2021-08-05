using AutoFixture;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using minhas_financas.api.Extensions;
using minhas_financas.api.V1.Controllers;
using minhas_financas.api.V1.ViewModels;
using minhas_financas.business.Interfaces;
using minhas_financas.business.Notificacoes;
using Moq;
using System;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace minhas_financas.api.ut.V1.Controllers
{
    public class UsuariosControllerTest
    {
        private readonly Fixture _fixture;
        private readonly Mock<INotificador> _mockNotificador;
        private readonly Mock<UserManager<IdentityUser>> _mockUserManager;
        private readonly Mock<SignInManager<IdentityUser>> _mockSignInManager;
        private readonly Mock<JwtGeradorToken> _mockJwtGeradorToken;
        private readonly UsuariosController _controller;

        public UsuariosControllerTest()
        {
            _fixture = new Fixture();
            _mockNotificador = new Mock<INotificador>();
            _mockUserManager = new Mock<UserManager<IdentityUser>>(Mock.Of<IUserStore<IdentityUser>>(),
                                                                   null,
                                                                   null,
                                                                   null,
                                                                   null,
                                                                   null,
                                                                   null,
                                                                   null,
                                                                   null);
            _mockSignInManager = new Mock<SignInManager<IdentityUser>>(_mockUserManager.Object,
                                                                       Mock.Of<IHttpContextAccessor>(),
                                                                       Mock.Of<IUserClaimsPrincipalFactory<IdentityUser>>(),
                                                                       null,
                                                                       null,
                                                                       null,
                                                                       null);
            _mockJwtGeradorToken = new Mock<JwtGeradorToken>();
            _controller = new UsuariosController(_mockNotificador.Object,
                                                 _mockSignInManager.Object,
                                                 _mockUserManager.Object,
                                                 _mockJwtGeradorToken.Object);
        }

        [Fact]
        public async Task Adicionar_RetornaBadRequest_QuandoModelEhInvalido()
        {
            var notificacoes = _fixture.CreateMany<Notificacao>().ToList();
            _mockNotificador.Setup(notificador => notificador.TemNotificacao()).Returns(true);
            _mockNotificador.Setup(notificador => notificador.ObterNotificacoes()).Returns(notificacoes);
            
            foreach (var notificacao in notificacoes)
            {
                _controller.ModelState.AddModelError(_fixture.Create<string>(), notificacao.Mensagem);
            }

            var result = await _controller.Adicionar(createUser: null);
            var returnValue = Assert.IsType<BadRequestObjectResult>(result);
            var badRequestResponse = Assert.IsType<ApiBadRequestResponse>(returnValue.Value);
            Assert.False(badRequestResponse.Success);
            Assert.Equal(badRequestResponse.Erros, notificacoes.Select(n => n.Mensagem).ToList());
        }

        [Fact]
        public async Task Adicionar_RetornaCreated_QuandoModelEhValido()
        {
            _mockUserManager.Setup(userManager => 
                userManager.CreateAsync(It.IsAny<IdentityUser>(), It.IsAny<string>()))
                    .ReturnsAsync(IdentityResult.Success);

            _mockJwtGeradorToken.Setup(geradorToken => geradorToken.GerarToken(It.IsAny<string>()))
                .ReturnsAsync(_fixture.Create<LoginResponseViewModel>());

            var createUser = _fixture.Create<UserViewModel>();
            var result = await _controller.Adicionar(createUser);
            var returnValue = Assert.IsType<CreatedAtActionResult>(result);
            var createdResponse = Assert.IsType<ApiCreatedResponse>(returnValue.Value);
            Assert.True(createdResponse.Success);
            Assert.IsType<LoginResponseViewModel>(createdResponse.Data);
        }

        [Fact]
        public async Task Adicionar_RetornaBadRequest_QuandoNaoConsegueCriarUsuario()
        {
            var notificacoes = _fixture.CreateMany<Notificacao>().ToList();
            _mockNotificador.Setup(notificador => notificador.TemNotificacao()).Returns(true);
            _mockNotificador.Setup(notificador => notificador.ObterNotificacoes()).Returns(notificacoes);

            _mockUserManager.Setup(userManager => 
                userManager.CreateAsync(It.IsAny<IdentityUser>(), It.IsAny<string>()))
                    .ReturnsAsync(IdentityResult.Failed(_fixture.CreateMany<IdentityError>().ToArray()));

            _mockJwtGeradorToken.Setup(geradorToken => geradorToken.GerarToken(It.IsAny<string>()))
                .ReturnsAsync(_fixture.Create<LoginResponseViewModel>());

            var createUser = _fixture.Create<UserViewModel>();
            var result = await _controller.Adicionar(createUser);
            var returnValue = Assert.IsType<BadRequestObjectResult>(result);
            var badRequestResponse = Assert.IsType<ApiBadRequestResponse>(returnValue.Value);
            Assert.False(badRequestResponse.Success);
            Assert.Equal(badRequestResponse.Erros, notificacoes.Select(n => n.Mensagem).ToList());
        }

        [Fact]
        public async Task Adicionar_RetornaBadRequest_QuandoCriaUsuarioMasTemNotificacao()
        {
            var notificacoes = _fixture.CreateMany<Notificacao>().ToList();
            _mockNotificador.Setup(notificador => notificador.TemNotificacao()).Returns(true);
            _mockNotificador.Setup(notificador => notificador.ObterNotificacoes()).Returns(notificacoes);

            _mockUserManager.Setup(userManager => 
                userManager.CreateAsync(It.IsAny<IdentityUser>(), It.IsAny<string>()))
                    .ReturnsAsync(IdentityResult.Success);

            _mockJwtGeradorToken.Setup(geradorToken => geradorToken.GerarToken(It.IsAny<string>()))
                .ReturnsAsync(_fixture.Create<LoginResponseViewModel>());

            var createUser = _fixture.Create<UserViewModel>();
            var result = await _controller.Adicionar(createUser);
            var returnValue = Assert.IsType<BadRequestObjectResult>(result);
            var badRequestResponse = Assert.IsType<ApiBadRequestResponse>(returnValue.Value);
            Assert.False(badRequestResponse.Success);
            Assert.Equal(badRequestResponse.Erros, notificacoes.Select(n => n.Mensagem).ToList());
        }

        [Fact]
        public void Obter_RetornaOk()
        {
            var result = _controller.Obter(Guid.NewGuid());
            var returnValue = Assert.IsType<OkObjectResult>(result);
            var okResponse = Assert.IsType<ApiOkResponse>(returnValue.Value);
            Assert.True(okResponse.Success);
        }
    }
}
