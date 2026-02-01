# Honeybadger for .Net Apps

[![Build & Test](https://github.com/honeybadger-io/honeybadger-dotnet/actions/workflows/ci.yml/badge.svg)](https://github.com/honeybadger-io/honeybadger-dotnet/actions/workflows/ci.yml)
[![NuGet version](https://badge.fury.io/nu/Honeybadger.svg)](https://www.nuget.org/packages/Honeybadger/)
[![NuGet downloads](https://img.shields.io/nuget/dt/Honeybadger.svg)](https://www.nuget.org/packages/Honeybadger/)

This is the .Net Honeybadger Notifier.

## Supported .Net versions:

All modern .Net Core applications are supported, up to .Net 9.0.

## Documentation and Support

For comprehensive documentation and support, [check out our documentation site](https://docs.honeybadger.io/lib/dotnet/).

## Changelog

Changelog is automatically generated using [Conventional Commits](https://www.conventionalcommits.org/) with [versionize](https://github.com/versionize/versionize).
Conventional Commits are enforced with a pre-commit git hook (using [husky](https://alirezanet.github.io/Husky.Net/guide/)).

## Contributing

1. Fork the repo.
2. Create a topic branch: `git checkout -b my_branch`
3. Commit your changes: `git commit -am "chore: boom"`
4. Write a test that verifies your changes
5. Push to your branch: `git push origin my_branch`
6. Send a [pull request](https://github.com/honeybadger-io/honeybadger-dotnet/pulls)
7. Make sure that CI checks are passing

## Releasing

All packages are published on nuget.org with a [Github Actions Workflow](./.github/workflows/release.yml).
The workflow does the following:

- `dotnet versionize` - bump versions and generate changelog
- `dotnet pack`
- `dotnet nuget push`

_Note: only users with write permissions can trigger this workflow (i.e. Collaborators)._

### Manual Releases

To release manually, execute the following steps:

1. Run `dotnet versionize` in the root directory. This will bump the version in all projects and commit the new version as a tag.
2. Run `dotnet pack --configuration Release -o ./artifacts`
3. Run `dotnet nuget push ./artifacts/*.nupkg --source https://api.nuget.org/v3/index.json --api-key YOUR_API_KEY`
4. Push the changes to the repository — at this point you will have pushed the git tags + the new versions of the packages to nuget.org.

## License

This Honeybadger repository and published packages are MIT licensed. See the [MIT-LICENSE](./MIT_LICENSE) file in this repository for details.
