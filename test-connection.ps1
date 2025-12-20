# Script test ket noi database
$connectionString = "Data Source=CUATUI;Initial Catalog=NenTangHocLieu;Integrated Security=True;TrustServerCertificate=True;"

Write-Host "Testing database connection..." -ForegroundColor Yellow
Write-Host "Connection String: $connectionString" -ForegroundColor Gray

try {
    Add-Type -Path "C:\Windows\Microsoft.NET\Framework64\v4.0.30319\System.Data.dll"
    
    $connection = New-Object System.Data.SqlClient.SqlConnection($connectionString)
    $connection.Open()
    
    Write-Host "Connection successful!" -ForegroundColor Green
    Write-Host "Server Version: $($connection.ServerVersion)" -ForegroundColor Green
    Write-Host "Database: $($connection.Database)" -ForegroundColor Green
    
    $connection.Close()
    exit 0
}
catch {
    Write-Host "Connection failed!" -ForegroundColor Red
    Write-Host "Error: $($_.Exception.Message)" -ForegroundColor Red
    
    if ($_.Exception.InnerException) {
        Write-Host "Inner Exception: $($_.Exception.InnerException.Message)" -ForegroundColor Red
    }
    
    Write-Host ""
    Write-Host "Possible solutions:" -ForegroundColor Yellow
    Write-Host "1. Check if SQL Server CUATUI is running" -ForegroundColor Yellow
    Write-Host "2. Check if Windows Authentication is enabled" -ForegroundColor Yellow
    Write-Host "3. Check if database NenTangHocLieu exists on server CUATUI" -ForegroundColor Yellow
    Write-Host "4. Check if your Windows account has permission to access the database" -ForegroundColor Yellow
    
    exit 1
}
