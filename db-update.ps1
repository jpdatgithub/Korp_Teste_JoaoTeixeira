Push-Location $PSScriptRoot

try {
    Write-Host "Atualizando banco de Produtos..."
    dotnet ef database update --project ./KorpERP.Produtos.API --startup-project ./KorpERP.Produtos.API -- --environment Development

    if ($LASTEXITCODE -ne 0) {
        throw "Falha ao atualizar o banco de Produtos."
    }

    Write-Host "Atualizando banco de Notas..."
    dotnet ef database update --project ./KorpERP.Notas.API --startup-project ./KorpERP.Notas.API -- --environment Development

    if ($LASTEXITCODE -ne 0) {
        throw "Falha ao atualizar o banco de Notas."
    }

    Write-Host "Bancos atualizados com sucesso."
}
finally {
    Pop-Location
}