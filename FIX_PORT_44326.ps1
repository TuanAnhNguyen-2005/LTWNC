# Script để sửa lỗi port 44326 conflict

Write-Host "=== Fixing Port 44326 Conflict ===" -ForegroundColor Cyan
Write-Host ""

# Step 1: Stop all IIS Express processes
Write-Host "Step 1: Stopping IIS Express processes..." -ForegroundColor Yellow
Get-Process | Where-Object {$_.ProcessName -like "*iis*"} | Stop-Process -Force -ErrorAction SilentlyContinue
Start-Sleep -Seconds 2
Write-Host "   ✓ IIS Express processes stopped" -ForegroundColor Green

# Step 2: Check and remove URL reservations
Write-Host ""
Write-Host "Step 2: Checking URL reservations..." -ForegroundColor Yellow
$reservations = netsh http show urlacl | Select-String -Pattern "44326"
if ($reservations) {
    Write-Host "   Found reservations for port 44326:" -ForegroundColor Yellow
    $reservations | ForEach-Object { Write-Host "   $_" -ForegroundColor Gray }
    
    Write-Host ""
    Write-Host "   Attempting to remove reservations..." -ForegroundColor Yellow
    netsh http delete urlacl url=https://localhost:44326/ 2>$null
    netsh http delete urlacl url=https://+:44326/ 2>$null
    Write-Host "   ✓ Reservations removed (if they existed)" -ForegroundColor Green
} else {
    Write-Host "   ✓ No URL reservations found for port 44326" -ForegroundColor Green
}

# Step 3: Check if port is in use
Write-Host ""
Write-Host "Step 3: Checking if port 44326 is in use..." -ForegroundColor Yellow
$connection = Get-NetTCPConnection -LocalPort 44326 -ErrorAction SilentlyContinue
if ($connection) {
    Write-Host "   ⚠ Port 44326 is in use by process:" -ForegroundColor Yellow
    $connection | ForEach-Object {
        $proc = Get-Process -Id $_.OwningProcess -ErrorAction SilentlyContinue
        Write-Host "   Process: $($proc.ProcessName) (PID: $($_.OwningProcess))" -ForegroundColor Gray
    }
} else {
    Write-Host "   ✓ Port 44326 is not in use" -ForegroundColor Green
}

# Step 4: Summary
Write-Host ""
Write-Host "=== Summary ===" -ForegroundColor Cyan
Write-Host "1. ✓ Stopped IIS Express processes" -ForegroundColor Green
Write-Host "2. ✓ Checked/removed URL reservations" -ForegroundColor Green
Write-Host "3. ✓ Checked port usage" -ForegroundColor Green
Write-Host ""
Write-Host "Next steps:" -ForegroundColor Yellow
Write-Host "1. Close Visual Studio completely" -ForegroundColor White
Write-Host "2. Reopen Visual Studio" -ForegroundColor White
Write-Host "3. Open solution and set MVC_STUDENT as Startup Project" -ForegroundColor White
Write-Host "4. Press F5 to run" -ForegroundColor White
Write-Host ""



