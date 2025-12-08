# Script để chạy MVC_ADMIN với IIS Express và tự động mở Chrome
param(
    [string]$Port = "64761",
    [string]$HttpsPort = "44319"
)

$projectPath = Split-Path -Parent $MyInvocation.MyCommand.Path
$mvcAdminPath = Join-Path $projectPath "MVC_ADMIN"
$projectFile = Join-Path $mvcAdminPath "MVC_ADMIN.csproj"

# Tìm IIS Express
$iisExpressPath = @(
    "${env:ProgramFiles}\IIS Express\iisexpress.exe",
    "${env:ProgramFiles(x86)}\IIS Express\iisexpress.exe",
    "${env:ProgramFiles}\Microsoft Visual Studio\2022\Community\Common7\IDE\Extensions\Microsoft\Web Tools\IIS Express\iisexpress.exe",
    "${env:ProgramFiles}\Microsoft Visual Studio\2022\Professional\Common7\IDE\Extensions\Microsoft\Web Tools\IIS Express\iisexpress.exe",
    "${env:ProgramFiles}\Microsoft Visual Studio\2022\Enterprise\Common7\IDE\Extensions\Microsoft\Web Tools\IIS Express\iisexpress.exe"
)

$iisExpress = $null
foreach ($path in $iisExpressPath) {
    if (Test-Path $path) {
        $iisExpress = $path
        break
    }
}

if (-not $iisExpress) {
    Write-Host "❌ Không tìm thấy IIS Express!" -ForegroundColor Red
    Write-Host "Vui lòng cài đặt Visual Studio hoặc IIS Express" -ForegroundColor Yellow
    exit 1
}

Write-Host "🚀 Đang khởi động MVC_ADMIN..." -ForegroundColor Cyan
Write-Host "   Project: $mvcAdminPath" -ForegroundColor Gray
Write-Host "   HTTP Port: $Port" -ForegroundColor Gray
Write-Host "   HTTPS Port: $HttpsPort" -ForegroundColor Gray
Write-Host ""

# Tìm applicationhost.config
$appHostConfig = @(
    "$projectPath\.vs\config\applicationhost.config",
    "$mvcAdminPath\.vs\config\applicationhost.config",
    "$env:USERPROFILE\Documents\IISExpress\config\applicationhost.config"
)

$configPath = $null
foreach ($config in $appHostConfig) {
    if (Test-Path $config) {
        $configPath = $config
        break
    }
}

# Nếu không có config, tạo một cái đơn giản
if (-not $configPath) {
    Write-Host "⚠️  Không tìm thấy applicationhost.config, tạo config tạm..." -ForegroundColor Yellow
    $configDir = "$projectPath\.vs\config"
    if (-not (Test-Path $configDir)) {
        New-Item -ItemType Directory -Path $configDir -Force | Out-Null
    }
    $configPath = "$configDir\applicationhost.config"
    
    # Tạo config đơn giản
    $configContent = @"
<?xml version="1.0" encoding="UTF-8"?>
<configuration>
    <system.applicationHost>
        <sites>
            <site name="MVC_ADMIN" id="1">
                <application path="/" applicationPool="Clr4IntegratedAppPool">
                    <virtualDirectory path="/" physicalPath="$($mvcAdminPath.Replace('\', '\\'))" />
                </application>
                <bindings>
                    <binding protocol="http" bindingInformation="*:$Port`:localhost" />
                    <binding protocol="https" bindingInformation="*:$HttpsPort`:localhost" />
                </bindings>
            </site>
        </sites>
    </system.applicationHost>
</configuration>
"@
    $configContent | Out-File -FilePath $configPath -Encoding UTF8
}

# Chạy IIS Express
Write-Host "📦 Đang khởi động IIS Express..." -ForegroundColor Cyan

# Kiểm tra và dừng IIS Express cũ nếu có
$oldProcess = Get-Process -Name "iisexpress" -ErrorAction SilentlyContinue
if ($oldProcess) {
    Write-Host "   Đang dừng IIS Express cũ..." -ForegroundColor Yellow
    $oldProcess | Stop-Process -Force -ErrorAction SilentlyContinue
    Start-Sleep -Seconds 2
}

# Tạo arguments string
$iisArgs = "/path:`"$mvcAdminPath`" /port:$Port /clr:v4.0"

if ($configPath) {
    $iisArgs += " /config:`"$configPath`""
}

Write-Host "   Command: $iisExpress $iisArgs" -ForegroundColor Gray

# Chạy IIS Express và giữ process
$process = Start-Process -FilePath $iisExpress -ArgumentList $iisArgs -PassThru -NoNewWindow

if (-not $process) {
    Write-Host "❌ Không thể khởi động IIS Express!" -ForegroundColor Red
    exit 1
}

Write-Host "   ✅ IIS Express đã khởi động (PID: $($process.Id))" -ForegroundColor Green

# Đợi IIS Express khởi động và kiểm tra port
Write-Host "⏳ Đợi IIS Express khởi động..." -ForegroundColor Yellow
$maxWait = 15
$waited = 0
$portReady = $false

while ($waited -lt $maxWait) {
    Start-Sleep -Seconds 1
    $waited++
    
    # Kiểm tra port có đang lắng nghe không
    $portCheck = netstat -ano | Select-String ":$Port"
    if ($portCheck) {
        $portReady = $true
        Write-Host "   ✅ Port $Port đã sẵn sàng!" -ForegroundColor Green
        break
    }
    
    Write-Host "   Đang đợi... ($waited/$maxWait giây)" -ForegroundColor Gray
}

if (-not $portReady) {
    Write-Host "⚠️  Port $Port chưa sẵn sàng sau $maxWait giây" -ForegroundColor Yellow
    Write-Host "   Nhưng vẫn thử mở Chrome..." -ForegroundColor Yellow
}

# Mở Chrome
$mvcUrl = "http://localhost:$Port"
$httpsUrl = "https://localhost:$HttpsPort"

Write-Host "🌐 Đang mở Chrome..." -ForegroundColor Cyan
Write-Host "   URL: $mvcUrl" -ForegroundColor Gray

try {
    Start-Process "chrome.exe" "$mvcUrl"
    Write-Host "✅ Đã mở Chrome với MVC Admin" -ForegroundColor Green
} catch {
    Write-Host "⚠️  Không thể mở Chrome tự động. Vui lòng mở thủ công: $mvcUrl" -ForegroundColor Yellow
}

Write-Host ""
Write-Host "✅ MVC_ADMIN đang chạy!" -ForegroundColor Green
Write-Host "   HTTP:  $mvcUrl" -ForegroundColor Cyan
Write-Host "   HTTPS: $httpsUrl" -ForegroundColor Cyan
Write-Host ""
Write-Host "💡 Nhấn Ctrl+C để dừng IIS Express" -ForegroundColor Yellow

# Giữ script chạy và theo dõi process
Write-Host ""
Write-Host "💡 Script đang chạy. Nhấn Ctrl+C để dừng IIS Express" -ForegroundColor Yellow
Write-Host ""

# Đăng ký handler để dừng IIS Express khi script dừng
$script:processId = $process.Id
Register-EngineEvent PowerShell.Exiting -Action {
    Write-Host "`n🛑 Đang dừng IIS Express..." -ForegroundColor Yellow
    Get-Process -Id $script:processId -ErrorAction SilentlyContinue | Stop-Process -Force -ErrorAction SilentlyContinue
    Get-Process -Name "iisexpress" -ErrorAction SilentlyContinue | Stop-Process -Force -ErrorAction SilentlyContinue
}

# Giữ script chạy
try {
    while ($true) {
        # Kiểm tra process còn chạy không
        $proc = Get-Process -Id $process.Id -ErrorAction SilentlyContinue
        if (-not $proc) {
            Write-Host "⚠️  IIS Express đã dừng!" -ForegroundColor Yellow
            break
        }
        Start-Sleep -Seconds 2
    }
} catch {
    Write-Host "`n🛑 Đang dừng IIS Express..." -ForegroundColor Yellow
    Get-Process -Id $process.Id -ErrorAction SilentlyContinue | Stop-Process -Force -ErrorAction SilentlyContinue
    Get-Process -Name "iisexpress" -ErrorAction SilentlyContinue | Stop-Process -Force -ErrorAction SilentlyContinue
}

