$ver = $(dn gitversion | ConvertFrom-Json).LegacySemVer
Write-Host "Push tag '$ver' to origin"
git push tag $ver