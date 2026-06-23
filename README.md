# GitHub Commit Diff Bug — File Deletion Hidden by Rename Detection

## Problem

A file deletion is completely invisible in GitHub's commit diff UI and REST API
when rename detection pairs it with an unrelated new file.

**Commit:** [`7f36e24`](../../commit/7f36e24)

## What Happened

The commit makes these 29 file operations (verified locally):
```
A  .../Mapping/PatientHeader/IPatientHeaderMapper.cs
A  .../Mapping/PatientHeader/PatientHeaderMapper.cs
D  .../Mapping/PatientHeader/PatientHeaderPatientInfoProfile.cs
D  .../Mapping/PatientHeader/SmallHeaderPatientInfoProfile.cs
M  (25 other modified files)
```

## What GitHub Shows

The REST API returns only **28 files**. The deletion of
`SmallHeaderPatientInfoProfile.cs` is completely absent:

```bash
$ gh api repos/matt-architect/github-rename-detection-bug/commits/7f36e24 \
    --jq '.files | length'
28

$ gh api repos/matt-architect/github-rename-detection-bug/commits/7f36e24 \
    --jq '.files[] | select(.filename | test("SmallHeader|IPatient")) | "\(.status) \(.filename)"'
added .../IPatientHeaderMapper.cs
renamed .../IPatientMapper.cs   (previous: SmallHeaderPatientInfoProfile.cs)
modified .../SmallHeaderPatientInfoQueryHandler.cs
```

**The bug:** GitHub's rename detection paired `SmallHeaderPatientInfoProfile.cs`
with `IPatientMapper.cs` (a modified file in a different folder). The result:

1. `SmallHeaderPatientInfoProfile.cs` has **no "removed" entry** — it vanishes
2. `IPatientHeaderMapper.cs` shows as **"added"** (not "renamed") — so there's
   no breadcrumb indicating where SmallHeader went
3. A user viewing this commit cannot tell that SmallHeader was deleted

## Expected Behavior

Either:
- Show all 29 operations (don't apply rename detection that swallows a deletion)
- OR if rename detection is applied, show it clearly as a rename with both
  source and destination visible (like the git CLI does with `R062`)

## Local Verification

```bash
git clone https://github.com/matt-architect/github-rename-detection-bug.git
cd github-rename-detection-bug

# Shows 29 file operations — SmallHeader clearly deleted:
git diff --no-renames --name-status HEAD~1..HEAD | wc -l
# 29

# Shows 28 with rename detection — SmallHeader consumed as rename source:
git diff -M --name-status HEAD~1..HEAD | wc -l  
# 28

git diff -M --name-status HEAD~1..HEAD | grep -E "SmallHeader|IPatient"
# R062 .../SmallHeaderPatientInfoProfile.cs → .../Shared/IPatientMapper.cs
# A    .../PatientHeader/IPatientHeaderMapper.cs
```

## Impact

- **`git log --follow`** on `SmallHeaderPatientInfoProfile.cs` shows it was deleted
- **GitHub.com** shows no trace of this deletion in the commit diff
- **GitHub REST API** omits the file entirely from the response
- Users cannot determine what happened to the file by looking at GitHub

## Notes

- The high similarity is caused by shared license/boilerplate headers
- This is a regular commit (single parent, not a merge commit)
- The commit modifies 28+ files total
- Rename detection threshold: 62% (above git's default 50%)