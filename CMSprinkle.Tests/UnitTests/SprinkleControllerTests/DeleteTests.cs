using FakeItEasy;
using Microsoft.AspNetCore.Mvc;
using NUnit.Framework;

namespace CMSprinkle.Tests.UnitTests.SprinkleControllerTests;

[TestFixture]
public class DeleteTests : ControllerTestBase
{
    [Test]
    public async Task Unauthorized_users_only_get_401()
    {
        // arrange
        A.CallTo(() => _mockAuth.IsAllowed()).Returns(false);

        // act
        var result = await _controller.Delete("something");

        // assert
        Assert.That(result, Is.InstanceOf<UnauthorizedResult>());
    }

    [Test]
    public async Task Problem_with_deleting_show_error_at_home()
    {
        // arrange
        var contentKey = "key-" + Path.GetRandomFileName();
        var expectedErrorMessage = $"There was an error deleting '{contentKey}'";
        A.CallTo(() => _mockAuth.IsAllowed()).Returns(true);
        A.CallTo(() => _mockDataService.Delete(contentKey))
            .Throws(new Exception("data service problem"));

        // act
        var result = await _controller.Delete(contentKey);

        // assert
        Assert.That(_controller.TempData["Error"], Is.EqualTo(expectedErrorMessage));
        Assert.That(result, Is.InstanceOf<RedirectToActionResult>());
        var redirectResult = result as RedirectToActionResult;
        Assert.That(redirectResult.ControllerName, Is.EqualTo("Sprinkle").Or.EqualTo(null));
        Assert.That(redirectResult.ActionName, Is.EqualTo("Home"));
    }

    [Test]
    public async Task Success_should_have_no_error_message_and_return_to_home()
    {
        // arrange
        var contentKey = "key-" + Path.GetRandomFileName();
        A.CallTo(() => _mockAuth.IsAllowed()).Returns(true);

        // act
        var result = await _controller.Delete(contentKey);

        // assert
        Assert.That(result, Is.InstanceOf<RedirectToActionResult>());
        Assert.That(_controller.TempData["Error"], Is.EqualTo(null));
        Assert.That(result, Is.InstanceOf<RedirectToActionResult>());
        var redirectResult = result as RedirectToActionResult;
        Assert.That(redirectResult.ControllerName, Is.EqualTo("Sprinkle").Or.EqualTo(null));
        Assert.That(redirectResult.ActionName, Is.EqualTo("Home"));
    }
}