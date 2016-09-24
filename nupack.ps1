$version = $env:APPVEYOR_BUILD_VERSION

Write-Host "Setting nuget version to $version"

$content = Get-Content NetHierarchy.nuspec
$content = $content -replace '\$version\$', $version

$content | Out-File NetHierarchy.compiled.nuspec

& nuget pack NetHierarchy.compiled.nuspec