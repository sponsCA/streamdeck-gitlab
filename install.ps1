Param(
    [string]$target = "Debug",
    [string]$distributionTool = "C:/Perso/streamdeck-gitlab/DistributionTool.exe",
    [string]$outputDir = "C:/Perso/streamdeck-gitlab/temp"
)

Push-Location -Path "bin/$target/"

$uuid = "dev.spons.gitlab"

Stop-Process -Name "StreamDeck" -Force -ErrorAction SilentlyContinue
Stop-Process -Name "$uuid" -Force -ErrorAction SilentlyContinue

Start-Sleep -Seconds 2

Remove-Item -Path "$outputDir/$uuid.streamDeckPlugin" -Recurse -Force -ErrorAction SilentlyContinue
Start-Process "$distributionTool" -ArgumentList "--build --input $uuid.sdPlugin --output $outputDir" -NoNewWindow -Wait
Remove-Item -Path "$env:APPDATA/Elgato/StreamDeck/Plugins/$uuid.sdPlugin" -Recurse -Force -ErrorAction SilentlyContinue

Invoke-Item -Path "$outputDir/$uuid.streamDeckPlugin"

Pop-Location
