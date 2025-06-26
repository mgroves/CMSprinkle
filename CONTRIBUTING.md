# Contributing to CMSprinkle

First, thank you for contributing! 

All types of contributions are encouraged and valued. See the [Table of Contents](#table-of-contents) for different ways to help and details about how this project handles them. Please make sure to read the relevant section before making your contribution. It will make it a lot easier for maintainers and smooth out the experience for all involved.

> If you like the project, but just don't have time to contribute code or docs, there are other easy ways to support the project and show your appreciation:
> - Star the project
> - Tweet about it
> - Mention this project in your project's readme
> - Mention the project at local meetups
> - Tell your friends/colleagues

## Table of Contents

- [Code of Conduct](#code-of-conduct)
- [I Have a Question](#i-have-a-question)
- [I Want To Contribute](#i-want-to-contribute)
- [Reporting Bugs](#reporting-bugs)
- [Suggesting Enhancements](#suggesting-enhancements)
- [Your First Code Contribution](#your-first-code-contribution)
- [Improving The Documentation](#improving-the-documentation)
- [Styleguides](#styleguides)
- [Commit Messages](#commit-messages)
- [Join The Project Team](#join-the-project-team)


## Code of Conduct

This project and everyone participating in it is governed by the
[CONTRIBUTING.md Code of Conduct](CODE_OF_CONDUCT.md).
By participating, you are expected to uphold this code. Please report unacceptable behavior to [@mgroves](https://github.com/mgroves).

## I Have a Question

> Before you ask a question, make sure you've read the [README](README.md) and any other available documentation.

Also, search for existing [Issues](https://github.com/mgroves/CMSprinkle/issues) that might help you. In case you have found a suitable issue and still need clarification, write your question in that issue. If it's an old issue, or only partially relevant to your question, then start a new issue.

- Open an [Issue](https://github.com/mgroves/CMSprinkle/issues/new).
- Provide as much context as you can about what you're running into.
- If you've found a bug, please include a summary, reproduction steps, your expected behavior, the actual behavior, relevant code (minimal examples appreciated), stack trace, and any other notes, screenshots, videos that could help.
- Provide project and platform versions (e.g. database version, .NET version, operating system, etc), depending on what seems relevant.

I'll get to issues as soon as I can, but I can't guarantee any response time.

## I Want To Contribute

> ### Legal Notice 
> When contributing to this project, you must agree that you have authored 100% of the content, that you have the necessary rights to the content and that the content you contribute may be provided under the project license.

If you are contributing a bug fix, just go on ahead and submit a PR and I'll review it as soon as I can. Please submit at least one automated test to demonstrate the bug as well.

If you want to contribute a new feature, formatting, code refactoring, or any other non-trivial changes, please first create an issue and discuss it.

If you want to contribute documentation, bless you. Right now it's just a README file, but any documentation contributions are welcome. If it's a small change/recommendation, just go ahead and submit a PR. If it's a larger or more significant change, please open an issue first to discuss it.

## Styleguides

Generally, use default styles from Visual Studio and/or Visual Studio Code. I'm not going to nitpick over styles, but if anything stands out to me as weird, I might ask for a change.

Also, when in doubt, favor verbosity over terseness, especially for readability sake.

**Example**:

I'd prefer:
```
var isApproved = <condition 1> || <condition 2>;
if(isApproved) {
    // whatever
}
```
over:
```
if(<condition 1> || <condition 2>) {
    // whatever
}
```
The latter is terse, but the former is more readable and more self-documenting.

In general, stick to [Framework design guidelines](https://learn.microsoft.com/en-us/dotnet/standard/design-guidelines/) as much as you can, but deviation is okay, especially if you make a thoughtful case.

### Commit Messages

Write a decent commit message that would help you better understand the commit as if you were reading it 6 months from now. Use generative AI if you have to. If your commit is related to an issue, make sure to use the issue number in the commit message. 

Good Examples:
- `#14 fixing gonzo bug in foo.cs`
- `I refactored using the flargle algorithm #18`

Bad Examples:
- `fixed bug`
- `committing gabbagoo.cs and rudytoot.cs`

## Attribution
This guide is based on the **contributing.md**. [Make your own](https://contributing.md/)!
