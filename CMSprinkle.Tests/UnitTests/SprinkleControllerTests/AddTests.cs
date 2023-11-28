using CMSprinkle.Tests.UnitTests.TestHelpers;
using CMSprinkle.ViewModels;
using FakeItEasy;
using Microsoft.AspNetCore.Mvc;
using NUnit.Framework;

namespace CMSprinkle.Tests.UnitTests.SprinkleControllerTests;

[TestFixture]
public class AddTests : ControllerTestBase
{
    [Test]
    public async Task Unauthorized_users_only_get_401()
    {
        // arrange
        A.CallTo(() => _mockAuth.IsAllowed()).Returns(false);

        // act
        var result = await _controller.Add();

        // assert
        Assert.That(result, Is.InstanceOf<UnauthorizedResult>());
    }

    [Test]
    public async Task Authorized_user_get_add_form()
    {
        // act
        var result = await _controller.Add();

        // assert
        Assert.That(result, Is.InstanceOf<ViewResult>());
        var viewResult = result as ViewResult;
        Assert.That(viewResult.ViewName, Is.EqualTo(null).Or.EqualTo("Add"));
    }

    [Test]
    public async Task Coming_to_add_page_first_time_has_empty_form()
    {
        // act
        var result = await _controller.Add();

        // assert
        Assert.That(result, Is.InstanceOf<ViewResult>());
        var viewResult = result as ViewResult;
        Assert.That(viewResult.Model, Is.InstanceOf<AddContentSubmitModel>());
        var model = viewResult.Model as AddContentSubmitModel;
        Assert.That(model.Key, Is.Null.Or.Empty);
        Assert.That(model.Content, Is.Null.Or.Empty);
    }

    [TestCase("", "Content key is required")]
    [TestCase(null, "Content key is required")]
    [TestCase("toolong___toolong___toolong___toolong___toolong___toolong___toolong___toolong___toolong___toolong___toolong___", "Content Key must be 90 characters or less")]
    public async Task Submitting_invalid_content_key_shows_errors_to_add_form(string? invalidContentKey, string errorMessage)
    {
        // arrange
        var submission = AddContentSubmitModelHelper
            .Create(key: invalidContentKey, forceKeyNull: invalidContentKey == null);

        // act
        var validationResults = ValidateModel(submission);

        // assert
        Assert.That(validationResults.Any(), Is.True);
        Assert.That(validationResults.Any(v => v.ErrorMessage == errorMessage));
    }

    [Test]
    public async Task Submitting_content_thats_too_long_shows_error_to_add_form()
    {
        // arrange
        var submission = AddContentSubmitModelHelper
            .Create(content: new string('x',10000001));

        // act
        var validationResults = ValidateModel(submission);

        // assert
        Assert.That(validationResults.Any(), Is.True);
        Assert.That(validationResults.Any(v => v.ErrorMessage == "Content is limited to a length of 10,000,000"));
    }

    [Test]
    public async Task Unauthorized_users_posting_only_get_401()
    {
        // arrange
        A.CallTo(() => _mockAuth.IsAllowed()).Returns(false);

        // act
        var result = await _controller.Add(A.Fake<AddContentSubmitModel>());

        // assert
        Assert.That(result, Is.InstanceOf<UnauthorizedResult>());
    }

    [Test]
    public async Task Invalid_submission_sent_back_to_form()
    {
        // arrange
        _controller.ModelState.AddModelError("error","error");

        // act
        var result = await _controller.Add(A.Fake<AddContentSubmitModel>());

        // assert
        Assert.That(result, Is.InstanceOf<ViewResult>());
        var viewResult = result as ViewResult;
        Assert.That(viewResult.ViewName, Is.EqualTo(null).Or.EqualTo("Add"));
    }

    [Test]
    public async Task Valid_forms_submitted_to_data_service()
    {
        // arrange
        var model = AddContentSubmitModelHelper.Create();

        // act
        await _controller.Add(model);

        // assert
        A.CallTo(() => _mockDataService.AddNew(model)).MustHaveHappenedOnceExactly();
    }

    [Test]
    public async Task Any_errors_from_data_service_show_user_on_form()
    {
        // arrange
        var exceptionMessage = "something went wrong in data service";
        var model = AddContentSubmitModelHelper.Create();
        A.CallTo(() => _mockDataService.AddNew(A<AddContentSubmitModel>._))
            .Throws(new Exception(exceptionMessage));

        // act
        await _controller.Add(model);

        // assert
        Assert.That(_controller.ModelState.Count, Is.GreaterThanOrEqualTo(1));
        Assert.That(_controller.ModelState.AllErrorMessages(), Contains.Item(exceptionMessage));
    }

    [Test]
    public async Task Successful_submission_sends_user_back_home()
    {
        // arrange
        var model = AddContentSubmitModelHelper.Create();

        // act
        var result = await _controller.Add(model);

        // assert
        Assert.That(result,Is.InstanceOf<RedirectToActionResult>());
        var redirectResult = result as RedirectToActionResult;
        Assert.That(redirectResult.ControllerName, Is.EqualTo("Sprinkle").Or.EqualTo(null));
        Assert.That(redirectResult.ActionName, Is.EqualTo("Home"));
    }
}