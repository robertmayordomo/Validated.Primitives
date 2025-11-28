# ?? GitHub Actions CI/CD Setup Complete!

Your repository is now configured with automated build, test, and NuGet publishing workflows.

## ?? Files Created

### Workflows
- ? `.github/workflows/build-test-publish.yml` - Simple commit-based versioning
- ? `.github/workflows/build-test-publish-gitversion.yml` - Advanced GitVersion workflow
- ? `GitVersion.yml` - GitVersion configuration

### Documentation
- ? `.github/QUICKSTART.md` - Quick 3-step setup guide
- ? `.github/WORKFLOWS.md` - Comprehensive workflow documentation
- ? `.github/BADGES.md` - Status badges for README

### Build Scripts
- ? `build-local.ps1` - Local build/test script (Windows)
- ? `build-local.sh` - Local build/test script (Linux/Mac)

---

## ?? Next Steps

### 1. Add Your NuGet API Key (Required)

1. Go to https://www.nuget.org/account/apikeys
2. Create a new API key with "Push" permission
3. In GitHub: Settings ? Secrets ? Actions ? New secret
4. Name: `NUGET_API_KEY`
5. Paste your API key
6. Save

### 2. Choose Your Workflow

**Recommended: Simple Workflow**
- Uses `build-test-publish.yml`
- Version format: `1.0.{commitCount}`
- Perfect for most projects
- No additional configuration needed

**Advanced: GitVersion Workflow**
- Uses `build-test-publish-gitversion.yml`
- Semantic versioning
- Pre-release support (alpha, beta, rc)
- Best for complex branching strategies

### 3. Test Locally (Optional)

**Windows:**
```powershell
.\build-local.ps1
```

**Linux/Mac:**
```bash
chmod +x build-local.sh
./build-local.sh
```

### 4. Push Your Code

```bash
git add .
git commit -m "Add GitHub Actions CI/CD workflow"
git push
```

? Your workflow will run automatically!

---

## ?? What Happens Now

### On Every Push to `master`/`main`:
1. ? Restores NuGet packages
2. ? Builds project in Release mode
3. ? Runs all unit tests
4. ? Generates test reports
5. ? Creates NuGet package (`.nupkg`)
6. ? Creates symbol package (`.snupkg`)
7. ? Publishes to NuGet.org
8. ? Publishes to GitHub Packages

### On Pull Requests:
1. ? Builds and tests
2. ? Shows test results
3. ? Does NOT publish packages

### On Releases:
1. ? Uses release tag as version (e.g., `v1.2.0` ? `1.2.0`)
2. ? Publishes stable package to NuGet.org

---

## ?? Creating Your First Release

### Method 1: Using GitHub UI

1. **Push your changes:**
   ```bash
   git add .
   git commit -m "feat: ready for first release"
   git push
   ```

2. **Create a release:**
   - Go to your repository on GitHub
   - Click **Releases** (right sidebar)
   - Click **Draft a new release**
   - Click **Choose a tag**
   - Type: `v1.0.0` (create new tag)
   - Target: `master` or `main`
   - Release title: `v1.0.0 - Initial Release`
   - Add description of changes
   - Click **Publish release**

3. **Watch it publish:**
   - Go to **Actions** tab
   - Watch your package being published! ??

### Method 2: Using Git Tags

```bash
# Create and push a tag
git tag v1.0.0
git push origin v1.0.0

# Then create the release in GitHub UI
```

---

## ?? Version Management

### Simple Workflow Versioning

Edit `src/Valdiated.Primatives/Validated.Primitives.csproj`:

```xml
<Version>1.0.0</Version>  <!-- Change to 1.1.0 for minor, 2.0.0 for major -->
```

Versions will be: `{major}.{minor}.{commitCount}`

Examples:
- Commit 1: `1.0.1`
- Commit 50: `1.0.50`
- Commit 100: `1.0.100`

### GitVersion Workflow

Versions are calculated automatically based on:
- Branch name
- Commit messages
- Git tags

Use commit message flags:
```bash
git commit -m "feat: new feature +semver:minor"      # Bumps minor
git commit -m "fix: bug fix +semver:patch"           # Bumps patch
git commit -m "BREAKING: major change +semver:major" # Bumps major
git commit -m "docs: update readme +semver:none"     # No bump
```

---

## ?? Monitoring

### View Workflow Status

1. Go to **Actions** tab in GitHub
2. See all workflow runs
3. Click any run for detailed logs
4. Download artifacts (packages) if needed

### Add Status Badges

See `.github/BADGES.md` for badge code to add to your README.md

Example:
```markdown
[![Build & Publish](https://github.com/robertmayordomo/Validated.PrimitivesV1/actions/workflows/build-test-publish.yml/badge.svg)](https://github.com/robertmayordomo/Validated.PrimitivesV1/actions/workflows/build-test-publish.yml)
[![NuGet](https://img.shields.io/nuget/v/Validated.Primitives.svg)](https://www.nuget.org/packages/Validated.Primitives/)
```

---

## ??? Customization

### Disable GitHub Packages

Remove this job from the workflow:

```yaml
publish-github-packages:
  # ... (remove entire job)
```

### Change Publish Trigger

Edit the workflow `if` condition:

```yaml
if: |
  github.event_name == 'release'  # Only publish on releases
  # OR
  github.ref == 'refs/heads/main' # Only publish from main
```

### Add Build Steps

Add custom steps in the `build-and-test` job:

```yaml
- name: Custom Step
  run: |
    echo "Running custom build step"
    # Your commands here
```

---

## ?? Documentation

- **Quick Start**: `.github/QUICKSTART.md` - Fast setup guide
- **Detailed Guide**: `.github/WORKFLOWS.md` - Complete documentation
- **Badges**: `.github/BADGES.md` - Status badge examples

---

## ? Verification Checklist

Before your first release:

- [ ] NuGet API key added to GitHub Secrets
- [ ] Workflow file exists (`.github/workflows/build-test-publish.yml`)
- [ ] Tests pass locally (`dotnet test`)
- [ ] Build succeeds locally (`.\build-local.ps1` or `./build-local.sh`)
- [ ] Project metadata updated in `.csproj`:
  - [ ] `<Authors>` name
  - [ ] `<Company>` name
  - [ ] `<PackageProjectUrl>` URL
  - [ ] `<RepositoryUrl>` URL
  - [ ] `<Description>` text
  - [ ] `<PackageTags>` keywords
- [ ] README.md updated with installation instructions
- [ ] LICENSE file exists (MIT license specified in project)

---

## ?? Troubleshooting

### Build Fails
```bash
# Run locally to debug
.\build-local.ps1  # Windows
./build-local.sh   # Linux/Mac

# Check test results
dotnet test --verbosity detailed
```

### Publishing Fails

**"401 Unauthorized"**
- Check `NUGET_API_KEY` secret is set correctly
- Verify API key has "Push" permission
- Regenerate key on NuGet.org if needed

**"409 Conflict" or "Package already exists"**
- Version already published
- Increment version number
- Check current version on NuGet.org

**"Package validation failed"**
- Ensure package metadata is complete
- Check LICENSE file exists
- Verify README.md is included

### Workflow Not Running

- Check workflow file is in `.github/workflows/`
- Verify YAML syntax (use VS Code YAML extension)
- Check branch name matches trigger (`master` vs `main`)
- Look for errors in Actions tab

---

## ?? Learning Resources

- [GitHub Actions Documentation](https://docs.github.com/en/actions)
- [NuGet Package Publishing](https://learn.microsoft.com/en-us/nuget/nuget-org/publish-a-package)
- [Semantic Versioning](https://semver.org/)
- [GitVersion Docs](https://gitversion.net/docs/)
- [Conventional Commits](https://www.conventionalcommits.org/)

---

## ?? You're All Set!

Your project now has:
- ? Automated testing on every push
- ? Automatic NuGet package creation
- ? Auto-publishing to NuGet.org
- ? Auto-incrementing version numbers
- ? Symbol packages for debugging
- ? Test result reporting
- ? Local build scripts

**Happy coding! ??**

Need help? Check the detailed documentation in `.github/WORKFLOWS.md`
