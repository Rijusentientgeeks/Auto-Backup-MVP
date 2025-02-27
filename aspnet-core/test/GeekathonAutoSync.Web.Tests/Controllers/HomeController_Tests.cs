using System.Threading.Tasks;
using GeekathonAutoSync.Models.TokenAuth;
using GeekathonAutoSync.Web.Controllers;
using Shouldly;
using Xunit;

namespace GeekathonAutoSync.Web.Tests.Controllers
{
    public class HomeController_Tests: GeekathonAutoSyncWebTestBase
    {
        [Fact]
        public async Task Index_Test()
        {
            await AuthenticateAsync(null, new AuthenticateModel
            {
                UserNameOrEmailAddress = "admin",
                Password = "123qwe"
            });

            //Act
            var response = await GetResponseAsStringAsync(
                GetUrl<HomeController>(nameof(HomeController.Index))
            );

            //Assert
            response.ShouldNotBeNullOrEmpty();
        }
    }
}