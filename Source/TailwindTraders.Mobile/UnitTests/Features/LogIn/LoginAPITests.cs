using System.Threading.Tasks;
using NUnit.Framework;
using UnitTests.Framework;

namespace UnitTests.Features.LogIn
{
#if !DEBUG
    [Ignore(Constants.IgnoreReason)]
#endif
    public class LoginAPITests : BaseAPITest
    {
        [Test]
        public async Task GetAsync()
        {
            await PreauthenticateAsync(() => Task.CompletedTask);
        }
    }
}
