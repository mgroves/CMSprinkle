using CMSprinkle.Tests.UnitTests.TestHelpers;
using CMSprinkle.ViewModels;
using NUnit.Framework;
using FakeItEasy;
using Microsoft.AspNetCore.Mvc;

namespace CMSprinkle.Tests.UnitTests.SprinkleControllerTests;

[TestFixture]
public class HomeTests : ControllerTestBase
{
    [Test]
    public async Task Unauthorized_users_only_get_401()
    {
        // arrange
        A.CallTo(() => _mockAuth.IsAllowed()).Returns(false);

        // act
        var result = await _controller.Home();

        // assert
        Assert.That(result, Is.InstanceOf<UnauthorizedResult>());
    }

    [Test]
    public async Task Authorized_user_gets_all_home_data()
    {
        // arrange
        var expectedData = CMSprinkleHomeHelper.Create();

        A.CallTo(() => _mockAuth.IsAllowed()).Returns(true);
        A.CallTo(() => _mockDataService.GetAllForHome()).Returns(expectedData);

        // act
        var result = await _controller.Home();

        // assert
        Assert.That(result, Is.InstanceOf<ViewResult>());
        var viewResult = result as ViewResult;
        Assert.That(viewResult.Model, Is.InstanceOf<CMSprinkleHome>());
        var model = viewResult.Model as CMSprinkleHome;
        Assert.That(model.AllContent, Is.Not.Null);
        Assert.That(model.AllContent.Count, Is.EqualTo(expectedData.AllContent.Count));
        Assert.That(model.AllContent[0].ContentKey, Is.EqualTo(expectedData.AllContent[0].ContentKey));
    }

    [Test]
    public async Task Authorized_user_goes_to_home_view()
    {
        // arrange
        var expectedData = CMSprinkleHomeHelper.Create();

        A.CallTo(() => _mockAuth.IsAllowed()).Returns(true);
        A.CallTo(() => _mockDataService.GetAllForHome()).Returns(expectedData);

        // act
        var result = await _controller.Home();

        // assert
        Assert.That(result, Is.InstanceOf<ViewResult>());
        var viewResult = result as ViewResult;
        Assert.That(viewResult.ViewName, Is.EqualTo(null).Or.EqualTo("Home"));
    }
}