using AutoFixture;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using minhas_financas.api.Extensions;
using minhas_financas.api.V1.ViewModels;
using Moq;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Xunit;

namespace minhas_financas.api.ut.Extensions
{
    public class JwtGeradorTokenTest
    {
        private readonly Fixture _fixture;
        private readonly Mock<UserManager<IdentityUser>> _mockUserManager;
        private readonly Mock<IOptions<AppSettings>> _mockAppSettings;

        public JwtGeradorTokenTest()
        {
            _fixture = new Fixture();
            _mockUserManager = new Mock<UserManager<IdentityUser>>(Mock.Of<IUserStore<IdentityUser>>(),
                                                                   null,
                                                                   null,
                                                                   null,
                                                                   null,
                                                                   null,
                                                                   null,
                                                                   null,
                                                                   null);
            _mockAppSettings = new Mock<IOptions<AppSettings>>();
        }

        [Fact]
        public async Task GerarToken_RetornaUmTokenValido()
        {
            var user = _fixture.Create<IdentityUser>();

            _mockUserManager.Setup(userManager => userManager.FindByEmailAsync(It.IsAny<string>()))
                .ReturnsAsync(user);
            _mockUserManager.Setup(userManager => userManager.GetClaimsAsync(user))
                .ReturnsAsync(new List<Claim>());
            _mockUserManager.Setup(userManager => userManager.GetRolesAsync(user))
                .ReturnsAsync(new List<string>() { "role test" });

            var expiracaoHoras = _fixture.Create<int>();

            _mockAppSettings.Setup(appSettings => appSettings.Value.Secret).Returns(_fixture.Create<string>());
            _mockAppSettings.Setup(appSettings => appSettings.Value.Emissor).Returns(_fixture.Create<string>());
            _mockAppSettings.Setup(appSettings => appSettings.Value.ValidoEm).Returns(_fixture.Create<string>());
            _mockAppSettings.Setup(appSettings => appSettings.Value.ExpiracaoHoras).Returns(expiracaoHoras);

            var geradorToken = new JwtGeradorToken(_mockUserManager.Object, _mockAppSettings.Object);
            var response = Assert.IsType<LoginResponseViewModel>(await geradorToken.GerarToken(user.Email));
            Assert.NotNull(response.AccessToken);
            Assert.Equal(TimeSpan.FromHours(expiracaoHoras), TimeSpan.FromSeconds(response.ExpiresIn));
            Assert.Equal(response.UserToken.Id, user.Id);
            Assert.Equal(response.UserToken.Email, user.Email);
            Assert.Single(response.UserToken.Claims, item => item.Type == "role");
        }
    }
}
