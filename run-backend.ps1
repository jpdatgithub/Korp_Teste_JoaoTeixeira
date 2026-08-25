Push-Location $PSScriptRoot

try {
    Start-Process powershell -ArgumentList @(
        "-NoExit",
        "-Command",
        "dotnet run --project './KorpERP.Produtos.API' --launch-profile http"
    )

    Start-Process powershell -ArgumentList @(
        "-NoExit",
        "-Command",
        "dotnet run --project './KorpERP.Notas.API' --launch-profile http"
    )
}
finally {
    Pop-Location
}