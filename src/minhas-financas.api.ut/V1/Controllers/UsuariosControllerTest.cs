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
        private readonly Mock<IUser> _mockAppUser;
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
            _mockAppUser = new Mock<IUser>();
            _controller = new UsuariosController(_mockNotificador.Object,
                                                 _mockSignInManager.Object,
                                                 _mockUserManager.Object,
                                                 _mockJwtGeradorToken.Object,
                                                 _mockAppUser.Object);
        }

        [Fact]
        public async Task Adicionar_RetornaBadRequest_QuandoModelEhInvalido()
        {
            var notificacoes = _fixture.CreateMany<Notificacao>().ToList();
            _mockNotificador.Setup(notificador => notificador.TemNotificacao()).Returns(true);
            _mockNotificador.Setup(notificador => notificador.ObterNotificacoes()).Returns(notificacoes);
            
            foreach (var notificacao in notificacoes)
            {
                _controller.ModelState.TryAddModelException(_fixture.Create<string>(), 
                                                            _fixture.Create<Exception>());
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

            var createUser = _fixture.Create<CreateUserViewModel>();
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

            var createUser = _fixture.Create<CreateUserViewModel>();
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

            var createUser = _fixture.Create<CreateUserViewModel>();
            var result = await _controller.Adicionar(createUser);
            var returnValue = Assert.IsType<BadRequestObjectResult>(result);
            var badRequestResponse = Assert.IsType<ApiBadRequestResponse>(returnValue.Value);
            Assert.False(badRequestResponse.Success);
            Assert.Equal(badRequestResponse.Erros, notificacoes.Select(n => n.Mensagem).ToList());
        }

        [Fact]
        public async Task Adicionar_RetornaBadRequest_QuandoIdEhInvalido()
        {
            var notificacoes = _fixture.CreateMany<Notificacao>().ToList();
            _mockNotificador.Setup(notificador => notificador.TemNotificacao()).Returns(true);
            _mockNotificador.Setup(notificador => notificador.ObterNotificacoes()).Returns(notificacoes);

            var id = _fixture.Create<Guid>();
            _mockAppUser.Setup(user => user.IsAuthenticated()).Returns(true);
            _mockAppUser.Setup(user => user.GetUserId()).Returns(id);
                       
            var result = await _controller.Alterar(id, _fixture.Create<EditUserViewModel>());
            var returnValue = Assert.IsType<BadRequestObjectResult>(result);
            var badRequestResponse = Assert.IsType<ApiBadRequestResponse>(returnValue.Value);
            Assert.False(badRequestResponse.Success);
            Assert.Equal(badRequestResponse.Erros, notificacoes.Select(n => n.Mensagem).ToList());
        }

        [Fact]
        public async Task Adicionar_RetornaBadRequest_QuandoIdNaoEhInformado()
        {
            var notificacoes = _fixture.CreateMany<Notificacao>().ToList();
            _mockNotificador.Setup(notificador => notificador.TemNotificacao()).Returns(true);
            _mockNotificador.Setup(notificador => notificador.ObterNotificacoes()).Returns(notificacoes);

            var id = _fixture.Create<Guid>();
            _mockAppUser.Setup(user => user.IsAuthenticated()).Returns(true);
            _mockAppUser.Setup(user => user.GetUserId()).Returns(id);

            var result = await _controller.Alterar(id, editUser: null);
            var returnValue = Assert.IsType<BadRequestObjectResult>(result);
            var badRequestResponse = Assert.IsType<ApiBadRequestResponse>(returnValue.Value);
            Assert.False(badRequestResponse.Success);
            Assert.Equal(badRequestResponse.Erros, notificacoes.Select(n => n.Mensagem).ToList());
        }

        [Fact]
        public async Task Alterar_RetornaBadRequest_QuandoModelEhInvalido()
        {
            var notificacoes = _fixture.CreateMany<Notificacao>().ToList();
            _mockNotificador.Setup(notificador => notificador.TemNotificacao()).Returns(true);
            _mockNotificador.Setup(notificador => notificador.ObterNotificacoes()).Returns(notificacoes);

            var id = _fixture.Create<Guid>();
            _mockAppUser.Setup(user => user.IsAuthenticated()).Returns(true);
            _mockAppUser.Setup(user => user.GetUserId()).Returns(id);

            foreach (var notificacao in notificacoes)
            {
                _controller.ModelState.TryAddModelException(_fixture.Create<string>(), 
                                                            _fixture.Create<Exception>());
            }

            var result = await _controller.Alterar(id, editUser: null);
            var returnValue = Assert.IsType<BadRequestObjectResult>(result);
            var badRequestResponse = Assert.IsType<ApiBadRequestResponse>(returnValue.Value);
            Assert.False(badRequestResponse.Success);
            Assert.Equal(badRequestResponse.Erros, notificacoes.Select(n => n.Mensagem).ToList());
        }
                
        [Fact]
        public async Task Alterar_RetornaNotFound_QuandoUsuarioNaoEstaAutenticado()
        {
            _mockAppUser.Setup(user => user.IsAuthenticated()).Returns(false);

            var result = await _controller.Alterar(_fixture.Create<Guid>(), editUser: null);
            var returnValue = Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task Alterar_RetornaNotFound_QuandoUsuarioEhDiferenteDoAutenticado()
        {
            _mockAppUser.Setup(user => user.IsAuthenticated()).Returns(true);
            _mockAppUser.Setup(user => user.GetUserId()).Returns(_fixture.Create<Guid>());
                        
            var result = await _controller.Alterar(_fixture.Create<Guid>(), editUser: null);
            var returnValue = Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task Alterar_RetornaNotFound_QuandoUsuarioNaoExiste()
        {
            var editUser = _fixture.Create<EditUserViewModel>();
            _mockAppUser.Setup(user => user.IsAuthenticated()).Returns(true);
            _mockAppUser.Setup(user => user.GetUserId()).Returns(editUser.Id);

            var result = await _controller.Alterar(editUser.Id, editUser);
            var returnValue = Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task Alterar_RetornaOk_QuandoModelEhValido()
        {
            _mockUserManager.Setup(userManager =>
                userManager.FindByIdAsync(It.IsAny<string>()))
                    .ReturnsAsync(_fixture.Create<IdentityUser>);

            var editUser = _fixture.Create<EditUserViewModel>();
            _mockAppUser.Setup(user => user.IsAuthenticated()).Returns(true);
            _mockAppUser.Setup(user => user.GetUserId()).Returns(editUser.Id);

            var result = await _controller.Alterar(editUser.Id, editUser);
            var returnValue = Assert.IsType<OkObjectResult>(result);
            var okResponse = Assert.IsType<ApiOkResponse>(returnValue.Value);
            Assert.True(okResponse.Success);
            Assert.IsType<EditUserViewModel>(okResponse.Data);
        }

        [Fact]
        public async Task Excluir_RetornaBadRequest_QuandoModelEhInvalido()
        {
            var notificacoes = _fixture.CreateMany<Notificacao>().ToList();
            _mockNotificador.Setup(notificador => notificador.TemNotificacao()).Returns(true);
            _mockNotificador.Setup(notificador => notificador.ObterNotificacoes()).Returns(notificacoes);

            var id = _fixture.Create<Guid>();
            _mockAppUser.Setup(user => user.IsAuthenticated()).Returns(true);
            _mockAppUser.Setup(user => user.GetUserId()).Returns(id);

            foreach (var notificacao in notificacoes)
            {
                _controller.ModelState.TryAddModelException(_fixture.Create<string>(),
                                                            _fixture.Create<Exception>());
            }

            var result = await _controller.Excluir(id);
            var returnValue = Assert.IsType<BadRequestObjectResult>(result);
            var badRequestResponse = Assert.IsType<ApiBadRequestResponse>(returnValue.Value);
            Assert.False(badRequestResponse.Success);
            Assert.Equal(badRequestResponse.Erros, notificacoes.Select(n => n.Mensagem).ToList());
        }

        [Fact]
        public async Task Excluir_RetornaBadRequest_QuandoNaoConsegueExcluirUsuario()
        {
            var notificacoes = _fixture.CreateMany<Notificacao>().ToList();
            _mockNotificador.Setup(notificador => notificador.TemNotificacao()).Returns(true);
            _mockNotificador.Setup(notificador => notificador.ObterNotificacoes()).Returns(notificacoes);

            var id = _fixture.Create<Guid>();
            _mockAppUser.Setup(user => user.IsAuthenticated()).Returns(true);
            _mockAppUser.Setup(user => user.GetUserId()).Returns(id);

            _mockUserManager.Setup(userManager =>
                userManager.FindByIdAsync(It.IsAny<string>()))
                    .ReturnsAsync(_fixture.Create<IdentityUser>);
            _mockUserManager.Setup(userManager =>
                userManager.DeleteAsync(It.IsAny<IdentityUser>()))
                    .ReturnsAsync(IdentityResult.Failed(_fixture.CreateMany<IdentityError>().ToArray()));

            var result = await _controller.Excluir(id);
            var returnValue = Assert.IsType<BadRequestObjectResult>(result);
            var badRequestResponse = Assert.IsType<ApiBadRequestResponse>(returnValue.Value);
            Assert.False(badRequestResponse.Success);
            Assert.Equal(badRequestResponse.Erros, notificacoes.Select(n => n.Mensagem).ToList());
        }

        [Fact]
        public async Task Excluir_RetornaNotFound_QuandoUsuarioNaoEstaAutenticado()
        {
            _mockAppUser.Setup(user => user.IsAuthenticated()).Returns(false);
                        
            var result = await _controller.Excluir(_fixture.Create<Guid>());
            var returnValue = Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task Excluir_RetornaNotFound_QuandoUsuarioEhDiferenteDoAutenticado()
        {
            _mockAppUser.Setup(user => user.IsAuthenticated()).Returns(true);
            _mockAppUser.Setup(user => user.GetUserId()).Returns(_fixture.Create<Guid>());

            var result = await _controller.Excluir(_fixture.Create<Guid>());
            var returnValue = Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task Excluir_RetornaNotFound_QuandoUsuarioNaoExiste()
        {
            var id = _fixture.Create<Guid>();
            _mockAppUser.Setup(user => user.IsAuthenticated()).Returns(true);
            _mockAppUser.Setup(user => user.GetUserId()).Returns(id);

            var result = await _controller.Excluir(id);
            var returnValue = Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task Excluir_RetornaNoContent_QuandoModelEhValido()
        {
            _mockUserManager.Setup(userManager =>
                userManager.FindByIdAsync(It.IsAny<string>()))
                    .ReturnsAsync(_fixture.Create<IdentityUser>);

            var id = _fixture.Create<Guid>();
            _mockAppUser.Setup(user => user.IsAuthenticated()).Returns(true);
            _mockAppUser.Setup(user => user.GetUserId()).Returns(id);

            var result = await _controller.Excluir(id);
            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task Obter_RetornaNotFound_QuandoUsuarioEhDiferenteDoAutenticado()
        {
            var notificacoes = _fixture.CreateMany<Notificacao>().ToList();
            _mockAppUser.Setup(user => user.IsAuthenticated()).Returns(true);
            _mockAppUser.Setup(user => user.GetUserId()).Returns(_fixture.Create<Guid>());

            var result = await _controller.Obter(_fixture.Create<Guid>());
            var returnValue = Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task Obter_RetornaNotFound_QuandoUsuarioNaoEstaAutenticado()
        {
            var notificacoes = _fixture.CreateMany<Notificacao>().ToList();
            _mockAppUser.Setup(user => user.IsAuthenticated()).Returns(false);

            foreach (var notificacao in notificacoes)
            {
                _controller.ModelState.TryAddModelException(_fixture.Create<string>(),
                                                            _fixture.Create<Exception>());
            }

            var result = await _controller.Obter(_fixture.Create<Guid>());
            var returnValue = Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task Obter_RetornaNotFound_QuandoUsuarioNaoExiste()
        {
            var editUser = _fixture.Create<EditUserViewModel>();
            _mockAppUser.Setup(user => user.IsAuthenticated()).Returns(true);
            _mockAppUser.Setup(user => user.GetUserId()).Returns(editUser.Id);

            var result = await _controller.Obter(editUser.Id);
            var returnValue = Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task Obter_RetornaOk_QuandoModelEhValido()
        {
            _mockUserManager.Setup(userManager =>
                userManager.FindByIdAsync(It.IsAny<string>()))
                    .ReturnsAsync(_fixture.Create<IdentityUser>);

            var id = _fixture.Create<Guid>();
            _mockAppUser.Setup(user => user.IsAuthenticated()).Returns(true);
            _mockAppUser.Setup(user => user.GetUserId()).Returns(id);

            var result = await _controller.Obter(id);
            var returnValue = Assert.IsType<OkObjectResult>(result);
            var okResponse = Assert.IsType<ApiOkResponse>(returnValue.Value);
            Assert.True(okResponse.Success);
            Assert.IsType<UserViewModel>(okResponse.Data);
        }
    }
}
