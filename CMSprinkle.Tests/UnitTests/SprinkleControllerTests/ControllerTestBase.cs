using CMSprinkle.Auth;
using CMSprinkle.Controllers;
using CMSprinkle.Data;
using FakeItEasy;
using NUnit.Framework;
using System.ComponentModel.DataAnnotations;

namespace CMSprinkle.Tests.UnitTests.SprinkleControllerTests;

public abstract class ControllerTestBase
{
    protected ICMSprinkleAuth _mockAuth;
    protected ICMSprinkleDataService _mockDataService;
    protected SprinkleController _controller;

    [SetUp]
    public async Task Setup()
    {
        _mockAuth = A.Fake<ICMSprinkleAuth>();
        _mockDataService = A.Fake<ICMSprinkleDataService>();
        _controller = new SprinkleController(_mockAuth, _mockDataService);
    }

    // for verifying validation attributes
    protected IList<ValidationResult> ValidateModel(object model)
    {
        var validationResults = new List<ValidationResult>();
        var ctx = new ValidationContext(model);
        Validator.TryValidateObject(model, ctx, validationResults, true);
        return validationResults;
    }
}