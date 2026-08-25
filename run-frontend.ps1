Push-Location $PSScriptRoot

try {
    Start-Process powershell -ArgumentList @(
        "-NoExit",
        "-Command",
        "Set-Location './KorpERP-angular-front'; npm start"
    )
}
finally {
    Pop-Location
}