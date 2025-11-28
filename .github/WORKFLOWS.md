# GitHub Workflows Guide

This repository includes two different GitHub Actions workflows for building, testing, and publishing NuGet packages with auto-incrementing version numbers.

## Workflows

### 1. Simple Build & Publish (`build-test-publish.yml`)

**Best for**: Simple projects that don't need complex branching strategies.

**Versioning Strategy**:
- Uses commit count for auto-incrementing patch version
- Format: `{major}.{minor}.{commitCount}`
- Example: `1.0.247` (247 commits)
- For releases: Uses the Git tag version (e.g., `v1.2.0`)

**Triggers**:
- Push to `master`, `main`, or `develop` branches
- Pull requests to `master` or `main`
- Published releases
- Manual workflow dispatch

**Publishing**:
- **NuGet.org**: Publishes on release or push to master/main
- **GitHub Packages**: Publishes on push to master/main

---

### 2. GitVersion Build & Publish (`build-test-publish-gitversion.yml`)

**Best for**: Projects using GitFlow or similar branching strategies.

**Versioning Strategy**:
- Uses [GitVersion](https://gitversion.net/) for semantic versioning
- Automatically calculates version based on branch, tags, and commits
- Supports semantic versioning with pre-release tags

**Examples**:
- `main` branch: `1.2.3`
- `develop` branch: `1.3.0-alpha.5`
- `release/1.2` branch: `1.2.0-beta.1`
- `feature/new-feature` branch: `1.3.0-alpha.new-feature.7`

**Triggers**:
- Push to `master`, `main`, `develop`, `feature/**`, `release/**`, `hotfix/**`
- Pull requests to `master`, `main`, or `develop`
- Published releases
- Manual workflow dispatch

**Publishing**:
- **NuGet.org (Stable)**: Publishes on release or push to master/main
- **NuGet.org (Pre-release)**: Publishes pre-release versions from develop branch

---

## Setup Instructions

### Prerequisites

1. **NuGet API Key**:
   - Get your API key from [NuGet.org](https://www.nuget.org/account/apikeys)
   - Add it to your GitHub repository secrets as `NUGET_API_KEY`
   - Go to: Repository ? Settings ? Secrets and variables ? Actions ? New repository secret

2. **GitHub Packages** (optional):
   - No setup needed - uses `GITHUB_TOKEN` automatically
   - Package will be published to `https://nuget.pkg.github.com/OWNER/index.json`

### Choose Your Workflow

#### Option A: Simple Versioning (Recommended for most projects)

Use **`build-test-publish.yml`** if you want:
- Simple, straightforward versioning
- Version based on commit count
- Minimal configuration

**Setup**:
1. The workflow is already configured and ready to use
2. Add your `NUGET_API_KEY` secret
3. Commit and push to trigger the workflow

**Version Control**:
- Edit `<Version>` in `src/Valdiated.Primatives/Validated.Primitives.csproj` to change major.minor
- Patch version auto-increments with each commit
- For releases, create a Git tag: `git tag v1.2.0 && git push --tags`

---

#### Option B: GitVersion (Advanced)

Use **`build-test-publish-gitversion.yml`** if you want:
- Semantic versioning with branch-based strategies
- Pre-release versions (alpha, beta)
- GitFlow or GitHub Flow branching model

**Setup**:
1. `GitVersion.yml` is already configured
2. Add your `NUGET_API_KEY` secret
3. Commit and push to trigger the workflow

**Version Control**:
Edit `GitVersion.yml` to customize:
- Branch naming patterns
- Version increment strategies
- Pre-release tags

**Semantic Version Bumping**:
Use commit message flags to control version bumps:
```bash
git commit -m "feat: add new feature +semver:minor"
git commit -m "fix: bug fix +semver:patch"
git commit -m "BREAKING CHANGE: major update +semver:major"
git commit -m "docs: update readme +semver:none"
```

---

## Workflow Features

Both workflows include:

### Build & Test
- ? Restore NuGet dependencies
- ? Build in Release configuration
- ? Run all unit tests
- ? Generate test reports
- ? Code coverage reports (GitVersion workflow)

### Package Creation
- ? Create NuGet package (.nupkg)
- ? Create symbol package (.snupkg) for debugging
- ? Include README.md in package
- ? Source Link support for debugging
- ? Upload artifacts for download

### Publishing
- ? Publish to NuGet.org
- ? Publish symbols to NuGet.org
- ? Optional: Publish to GitHub Packages
- ? Skip duplicate versions automatically

---

## Usage Examples

### Manual Workflow Trigger

Run workflows manually from GitHub UI:
1. Go to Actions tab
2. Select the workflow
3. Click "Run workflow"
4. Choose the branch

### Creating a Release

1. **Commit your changes**:
   ```bash
   git add .
   git commit -m "feat: add new validated primitive"
   git push
   ```

2. **Create a tag** (for simple workflow):
   ```bash
   git tag v1.2.0
   git push --tags
   ```

3. **Create a GitHub Release**:
   - Go to repository ? Releases ? Draft a new release
   - Choose the tag (or create new tag `v1.2.0`)
   - Add release notes
   - Click "Publish release"
   - Workflow will automatically publish to NuGet.org

### Pre-release Versions (GitVersion only)

Push to the `develop` branch to create pre-release versions:
```bash
git checkout develop
git merge feature/my-feature
git push
```
This will publish a version like `1.3.0-alpha.5` to NuGet.org

---

## Consuming Packages

### From NuGet.org
```bash
dotnet add package Validated.Primitives
```

### From GitHub Packages
```bash
# Add GitHub Packages source
dotnet nuget add source "https://nuget.pkg.github.com/OWNER/index.json" \
  --name github \
  --username YOUR_GITHUB_USERNAME \
  --password YOUR_GITHUB_PAT

# Install package
dotnet add package Validated.Primitives
```

---

## Troubleshooting

### Build Fails
- Check the Actions tab for detailed error logs
- Ensure all tests pass locally: `dotnet test`
- Verify .NET SDK version matches workflow

### Publishing Fails
- Verify `NUGET_API_KEY` is correctly set in repository secrets
- Check NuGet.org for package name conflicts
- Ensure version number hasn't been published before

### Version Not Incrementing
- **Simple workflow**: Check commit count in repository
- **GitVersion**: Review branch configuration in `GitVersion.yml`
- Use `git log --oneline` to verify commit history

### GitHub Packages 403 Error
- Verify repository has "packages: write" permission
- Check that `GITHUB_TOKEN` has proper scopes
- Ensure package name matches repository name pattern

---

## Customization

### Change Version Format

**Simple workflow**: Edit the version calculation in `build-test-publish.yml`:
```yaml
VERSION="$MAJOR_MINOR.$COMMIT_COUNT"
```

**GitVersion**: Modify `GitVersion.yml` configuration:
```yaml
mode: ContinuousDelivery  # or ContinuousDeployment
```

### Add Additional Build Steps

Add steps in the `build-and-test` job:
```yaml
- name: Custom Build Step
  run: |
    # Your custom commands here
```

### Publish to Private NuGet Feed

Add a new job or modify the publish job:
```yaml
- name: Publish to Private Feed
  run: |
    dotnet nuget push ./artifacts/*.nupkg \
      --api-key ${{ secrets.PRIVATE_FEED_KEY }} \
      --source https://your-private-feed.com/nuget
```

---

## Best Practices

1. **Use Releases for Stable Versions**: Create GitHub releases for production-ready versions
2. **Test Before Publishing**: Always run tests locally before pushing
3. **Semantic Commit Messages**: Use conventional commits for better version control
4. **Tag Major Releases**: Use Git tags for major version milestones
5. **Monitor Builds**: Subscribe to workflow notifications in repository settings

---

## Resources

- [GitHub Actions Documentation](https://docs.github.com/en/actions)
- [GitVersion Documentation](https://gitversion.net/docs/)
- [NuGet Package Publishing](https://learn.microsoft.com/en-us/nuget/nuget-org/publish-a-package)
- [Semantic Versioning](https://semver.org/)
