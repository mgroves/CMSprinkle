using CMSprinkle.Tests.UnitTests.TestHelpers;
using CMSprinkle.ViewModels;
using FakeItEasy;
using Microsoft.AspNetCore.Mvc;
using NUnit.Framework;

namespace CMSprinkle.Tests.UnitTests.SprinkleControllerTests;

[TestFixture]
public class EditTests : ControllerTestBase
{
    [Test]
    public async Task Unauthorized_users_get_a_401()
    {
        // arrange
        A.CallTo(() => _mockAuth.IsAllowed()).Returns(false);

        // act
        var result = await _controller.Edit("whatever");

        // assert
        Assert.That(result, Is.InstanceOf<UnauthorizedResult>());
    }

    [Test]
    public async Task If_error_getting_data_redirect_to_home_and_show_a_message_to_user()
    {
        // arrange
        var troubleKey = "key-bogus";
        var expectedErrorMessage = $"There was an error opening '{troubleKey}' for editing.";
        A.CallTo(() => _mockAuth.IsAllowed()).Returns(true);
        A.CallTo(() => _mockDataService.GetAdmin(troubleKey))
            .Throws(new Exception("some exception"));

        // act
        var result = await _controller.Edit(troubleKey);

        // assert
        Assert.That(result, Is.InstanceOf<RedirectToActionResult>());
        var redirectResult = result as RedirectToActionResult;
        Assert.That(_controller.TempData["Error"], Is.EqualTo(expectedErrorMessage));
    }

    [Test]
    public async Task Existing_content_is_used_for_edit_form()
    {
        // arrange
        var content = EditViewModelHelper.Create();
        A.CallTo(() => _mockAuth.IsAllowed()).Returns(true);
        A.CallTo(() => _mockDataService.GetAdmin(content.Key)).Returns(content.Content);

        // act
        var result = await _controller.Edit(content.Key);

        // assert
        Assert.That(result, Is.InstanceOf<ViewResult>());
        var viewResult = result as ViewResult;
        Assert.That(viewResult.Model, Is.InstanceOf<EditViewModel>());
        var model = viewResult.Model as EditViewModel;
        Assert.That(model.Key, Is.EqualTo(content.Key));
        Assert.That(model.Content, Is.EqualTo(content.Content));
    }

    [Test]
    public async Task Edit_form_is_opened()
    {
        // arrange
        var content = EditViewModelHelper.Create();
        A.CallTo(() => _mockAuth.IsAllowed()).Returns(true);
        A.CallTo(() => _mockDataService.GetAdmin(content.Key)).Returns(content.Content);

        // act
        var result = await _controller.Edit(content.Key);

        // assert
        Assert.That(result, Is.InstanceOf<ViewResult>());
        var viewResult = result as ViewResult;
        Assert.That(viewResult.ViewName, Is.EqualTo(null).Or.EqualTo("Edit"));
    }
}