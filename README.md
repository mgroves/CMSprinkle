# CMSprinkle

Coming soon: build/CI badges

## Introduction
CMSprinkle is a micro-CMS (Content Management System) designed for quick integration with ASP.NET Core applications. It allows developers to easily incorporate managed content into ASP.NET MVC Razor pages, perfect for applications requiring a _sprinkle_ of dynamic content management.

This was born out of the [C# Advent](https://csadvent.christmas), a site with only a _tiny_ bit of content that needed to be managed, but it's too much of a hassle to manage it in static files, especially from a mobile device. A full-blown CMS was also way too much for this use case.

## Features
- **Easy Integration:** Integrates with ASP.NET Core Razor pages, with a bias towards simplicity.
- **Authentication Support:** Configurable authentication for managing content: use whatever auth you want.
- **Flexible Routing:** Default /cmsprinkle base URL but you can change to whatever you want.
- **Database Support:** Compatible with Couchbase and SQL Server for content storage, whichever you're already using.
- **Customizable Error Messages:** Define custom messages for content-missing errors.
- **Minimal tracking:** Content is date/time stamped and stamped with the username who last updated it.
- **Markdown:** write your content in standard Markdown (using stackedit-js), which is rendered (and sanitized) as HTML

## What you won't get
- **CMS platform**: CMSprinkle is a _sprinkle_ of content management, not a full-blown CMS, or even a headless CMS. If you need something like that, there are a bajillion better options.
- **Database Performance focus**: Not of the box, at least. Database access is really simple. You'll have to add indexes yourself, and/or create your own implementation of ICMSprinkleDataService if you really want to squeeze out performance.
- **Binaries**: No image/video uploading, no binary file storage. Host those wherever, and put the links into your markdown.

## Getting Started

You can check out the CMSprinkle.Example project to see how CMSprinkle is used.

Follow these steps to integrate CMSprinkle into your ASP.NET Core application:

### 1. Installation
Add CMSprinkle to your project via NuGet (coming soon) or clone this repository.

### 2. Configuration in _ViewImports.cshtml
Ensure CMSprinkle is available in your Razor pages by adding it to `_ViewImports.cshtml`:

```cshtml
@addTagHelper *, CMSprinkle
```

### 3. Using CMSprinkle in a Razor Page
Incorporate CMSprinkle in a Razor page as shown below:
```cshtml
<div>
    <h2>Welcome to my page</h1>
    
    @* CMSprinkle managed content inclusion *@
    <CMSprinkle contentKey="HelloWorld" />
</div>
```

### 4. Configuration

Set up basic configuration in `Program.cs`:

```csharp
// Add authentication for CMSprinkle
builder.Services.AddTransient<ICMSprinkleAuth, YourAuthClass>();

// Configure CMSprinkle options
builder.Services.AddCMSprinkle(options =>
{
    // Route prefix for CMS pages (default: "cmsprinkle")
    options.RoutePrefix = "<your route prefix>";

    // Custom message for missing content
    options.ContentNotFoundMessage = (contentKey) => $"ERROR: Can't find {contentKey}, did you add it yet?";
});
```

__Database Connection__

Configure Couchbase for content storage:
```csharp
await builder.Services.AddCMSprinkleCouchbaseAsync("<bucket name>", "<scope name>", "<collection name>", createCollectionIfNecessary: true);
```
Alternatively, use SQLServer as the provider:
```csharp
await builder.Services.AddCMSprinkleSqlServerAsync("<sql server connection string>", "<table name>", "<schema name>",  createTableIfNecessary: true);
```

## Configuration Details
- **ICMSprinkleAuth**: Interface for configuring authentication. Create an implementation based on your security requirements. If you don't, default behavior is local access only.
- **RoutePrefix**: Determines the URL segment where CMS managed content is accessible.
- **ContentNotFoundMessage**: Function to generate custom error messages when content is not found. Helps in debugging content issues.

## Contributing
We welcome contributions! Please see the [Contributing Guide](CONTRIBUTING.md) for guidelines on how to contribute.

## License
CMSprinkle is released under the [MIT License](LICENSE).

# Powered By

* [Couchbase](https://www.couchbase.com/) ([.NET SDK](https://docs.couchbase.com/dotnet-sdk/current/hello-world/start-using-sdk.html)) - for NoSQL storage
* [Dapper](https://github.com/DapperLib/Dapper) - for SQL Server storage
* [MarkDig](https://github.com/xoofx/markdig) - for Markdown rendering
* [HtmlSanitizer](https://github.com/mganss/HtmlSanitizer) - for helping prevent XSS
* [StackEdit](https://github.com/benweet/stackedit) - for Markdown editing, with [jsdelivr](https://github.com/jsdelivr/jsdelivr) for CDN
* [NUnit](https://nunit.org/) - for automated tests
* [FakeItEasy](https://github.com/FakeItEasy/FakeItEasy) - for mocking/fakes in tests
 