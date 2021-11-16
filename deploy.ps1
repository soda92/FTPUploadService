Set-Location $PSScriptRoot
Set-Location FtpService\bin\Debug\netcoreapp3.1
if (Test-Path -Path linux-arm64.zip) {
    Remove-Item linux-arm64.zip
}
Set-Location $PSScriptRoot/FtpService
dotnet build --runtime linux-arm64
Set-Location $PSScriptRoot\FtpService\bin\Debug\netcoreapp3.1
7z a linux-arm64.zip .\linux-arm64\
scp linux-arm64.zip board:/home/toybrick/publish/
Set-Location $PSScriptRoot
