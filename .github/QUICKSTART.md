# Quick Start: GitHub Actions Setup

## ?? Get Started in 3 Steps

### Step 1: Add NuGet API Key to GitHub Secrets

1. Get your API key from [NuGet.org](https://www.nuget.org/account/apikeys)
2. Go to your GitHub repository ? **Settings** ? **Secrets and variables** ? **Actions**
3. Click **New repository secret**
4. Name: `NUGET_API_KEY`
5. Value: Paste your NuGet API key
6. Click **Add secret**

### Step 2: Choose Your Workflow

**Option A - Simple Versioning** (Recommended)
- Use `build-test-publish.yml`
- Version format: `1.0.{commitCount}`
- No additional setup needed

**Option B - GitVersion** (Advanced)
- Use `build-test-publish-gitversion.yml`
- Semantic versioning with branch strategies
- Pre-release support (alpha, beta)
- Requires `GitVersion.yml` (already included)

### Step 3: Push Your Code

```bash
git add .
git commit -m "Add GitHub Actions workflow"
git push
```

The workflow will automatically run! ??

---

## ?? Common Tasks

### Test Locally Before Pushing

**Windows:**
```powershell
.\build-local.ps1
```

**Linux/Mac:**
```bash
chmod +x build-local.sh
./build-local.sh
```

### Create a Release

1. Push your changes:
   ```bash
   git add .
   git commit -m "feat: add new feature"
   git push
   ```

2. Create a tag:
   ```bash
   git tag v1.2.0
   git push --tags
   ```

3. Create GitHub Release:
   - Go to **Releases** ? **Draft a new release**
   - Choose tag `v1.2.0`
   - Add release notes
   - Click **Publish release**

This will automatically publish to NuGet.org! ?

### Bump Version Manually

Edit `src/Valdiated.Primatives/Validated.Primitives.csproj`:

```xml
<Version>1.1.0</Version>  <!-- Change major.minor here -->
```

The patch version auto-increments with each commit.

---

## ?? Monitor Your Builds

- Go to **Actions** tab in your repository
- Click on a workflow run to see details
- Green checkmark ? = Success
- Red X ? = Failed (click for logs)

---

## ?? Version Examples

### Simple Workflow
- Commit 1: `1.0.1`
- Commit 50: `1.0.50`
- Commit 247: `1.0.247`
- Release tag `v1.2.0`: `1.2.0`

### GitVersion Workflow
- `main` branch: `1.2.3`
- `develop` branch: `1.3.0-alpha.5`
- `feature/new-feature`: `1.3.0-alpha.new-feature.7`
- `release/1.2`: `1.2.0-beta.1`

---

## ?? Troubleshooting

**Build fails?**
- Check **Actions** tab for error logs
- Run `dotnet test` locally to verify tests pass

**Package not publishing?**
- Verify `NUGET_API_KEY` is set correctly
- Check that you're pushing to `master` or `main` branch
- Ensure version hasn't been published before

**Need help?**
- See detailed guide: `.github/WORKFLOWS.md`
- Check [GitHub Actions docs](https://docs.github.com/en/actions)

---

## ?? What Happens Automatically

Every push to `master`/`main`:
- ? Runs all tests
- ? Creates NuGet package
- ? Publishes to NuGet.org
- ? Publishes to GitHub Packages
- ? Generates test reports

Every pull request:
- ? Runs all tests
- ? Builds the package
- ? Does NOT publish

Every release:
- ? Publishes stable version to NuGet.org

---

## ?? Customization

**Change what triggers the workflow:**

Edit `.github/workflows/build-test-publish.yml`:

```yaml
on:
  push:
    branches: [ master, main ]  # Add/remove branches here
```

**Add build steps:**

```yaml
- name: My Custom Step
  run: |
    echo "Do something here"
```

**Publish to private feed:**

```yaml
- name: Publish to MyGet
  run: |
    dotnet nuget push ./artifacts/*.nupkg \
      --api-key ${{ secrets.MYGET_API_KEY }} \
      --source https://www.myget.org/F/myfeed/api/v2/package
```

---

## ?? Learn More

- Full documentation: `.github/WORKFLOWS.md`
- GitVersion: https://gitversion.net/
- NuGet Publishing: https://docs.microsoft.com/nuget/
