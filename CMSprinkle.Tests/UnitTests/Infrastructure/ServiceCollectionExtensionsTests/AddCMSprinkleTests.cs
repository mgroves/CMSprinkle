using CMSprinkle.Auth;
using CMSprinkle.Data;
using CMSprinkle.Infrastructure;
using FakeItEasy;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;
using static CMSprinkle.Infrastructure.ServiceCollectionExtensions;

namespace CMSprinkle.Tests.UnitTests.Infrastructure.ServiceCollectionExtensionsTests;

[TestFixture]
public class AddCMSprinkleTests
{
    [Test]
    public async Task Default_route_prefix_option()
    {
        // arrange
        var fakeServiceCollection = A.Fake<IServiceCollection>();
        Action<CMSprinkleOptions> options = (x) =>
        {
            // don't set x.RoutePrefix
        };

        // act
        fakeServiceCollection.AddCMSprinkle(options);

        // assert
        Assert.That(CMSprinkleRouteAttribute.RoutePrefix, Is.EqualTo(CMSprinkleRouteAttribute.DEFAULT_ROUTE_PREFIX));
    }

    [Test]
    public async Task Custom_route_prefix_option()
    {
        // arrange
        var fakeServiceCollection = A.Fake<IServiceCollection>();
        var routePrefix = new Bogus.DataSets.Hacker().Noun().ToLower();
        Action<CMSprinkleOptions> options = (x) =>
        {
            x.RoutePrefix = routePrefix;
        };

        // act
        fakeServiceCollection.AddCMSprinkle(options);

        // assert
        Assert.That(CMSprinkleRouteAttribute.RoutePrefix, Is.EqualTo(routePrefix));

        // cleanup (cuz it's static)
        CMSprinkleRouteAttribute.RoutePrefix = CMSprinkleRouteAttribute.DEFAULT_ROUTE_PREFIX;
    }

    [Test]
    public async Task Default_content_not_found_message()
    {
        // arrange
        var fakeServiceCollection = A.Fake<IServiceCollection>();
        var routePrefix = new Bogus.DataSets.Hacker().Noun().ToLower();
        Action<CMSprinkleOptions> options = (x) =>
        {
            // don't set x.ContentNotFoundMessage
        };

        // act
        fakeServiceCollection.AddCMSprinkle(options);

        // assert
        Assert.That(GetContentResult.ContentNotFoundMessage("whatever"), Is.EqualTo("ERROR: Content Not Found (whatever)"));
    }

    [Test]
    public async Task Custom_content_not_found_message()
    {
        // arrange
        var fakeServiceCollection = A.Fake<IServiceCollection>();
        Action<CMSprinkleOptions> options = (x) =>
        {
            x.ContentNotFoundMessage = k => $"My custom message for {k}";
        };

        // act
        fakeServiceCollection.AddCMSprinkle(options);

        // assert
        Assert.That(GetContentResult.ContentNotFoundMessage("whatever"), Is.EqualTo($"My custom message for whatever"));

        // cleanup (because static)
        GetContentResult.ContentNotFoundMessage = x => $"ERROR: Content Not Found ({x})";
    }
}