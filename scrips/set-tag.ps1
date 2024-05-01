$ver = $(dn gitversion | ConvertFrom-Json).LegacySemVer
Write-Host "Tagging with '$ver'"
git tag $ver