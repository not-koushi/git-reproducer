$api = Start-Process powershell -ArgumentList "-NoExit", "-Command", "`$Host.UI.RawUI.WindowTitle='GitReproducer API'; dotnet run --no-build --project .\src\Api\" -PassThru
Start-Sleep -Seconds 2
$worker = Start-Process powershell -ArgumentList "-NoExit", "-Command", "`$Host.UI.RawUI.WindowTitle='GitReproducer Worker'; dotnet run --no-build --project .\src\Workers\" -PassThru

function Stop-Services {
    Write-Host "`nStopping API and Worker..." -ForegroundColor Yellow

    if ($api -and !$api.HasExited) {
        cmd /c "taskkill /PID $($api.Id) /T /F" | Out-Null
    }

    if ($worker -and !$worker.HasExited) {
        cmd /c "taskkill /PID $($worker.Id) /T /F" | Out-Null
    }
}

function Resolve-RepoUrl($repo)
{
    if ($repo -notmatch "^https?://")
    {
        if ($repo -match "^[^/]+/[^/]+$")
        {
            return "https://github.com/$repo"
        }
        elseif ($repo -match "^github\.com/")
        {
            return "https://$repo"
        }
    }
    return $repo
}

function Get-JobResult($jobId)
{
    Write-Host "`nFetching job result..." -ForegroundColor Cyan

    try {
        $job = Invoke-RestMethod "http://localhost:5000/jobs/$jobId"

        if ($job.statusText -eq "Completed") {
            Write-Host "Status: $($job.statusText)" -ForegroundColor Green
        }
        elseif ($job.statusText -eq "Failed") {
            Write-Host "Status: $($job.statusText)" -ForegroundColor Red
        }
        else {
            Write-Host "Status: $($job.statusText)" -ForegroundColor Yellow
        }

        $workspacePath = ".\src\workspaces\$jobId"
        $logs = $job.logs -replace "Cloning into '\.'", "Cloning into `"$workspacePath`""

        Write-Host ""
        Write-Host $logs -ForegroundColor Gray
    }
    catch {
        Write-Host "Job not found." -ForegroundColor Red
    }

    Write-Host ""
}

Write-Host "+-----------------------+" -ForegroundColor Cyan
Write-Host "| GIT REPRODUCER CLIENT |" -ForegroundColor Cyan
Write-Host "+-----------------------+" -ForegroundColor Cyan

while ($true)
{
    Write-Host ""
    $repo = Read-Host "Enter repository URL (or type exit)"

    if ($repo -eq "exit") {
        Stop-Services
        exit
    }

    $repo = Resolve-RepoUrl $repo

    Write-Host "`nCreating job..." -ForegroundColor Cyan

    $response = Invoke-RestMethod `
        -Method Post `
        -Uri "http://localhost:5000/jobs" `
        -ContentType "application/json" `
        -Body (@{ repositoryUrl = $repo } | ConvertTo-Json)

    $jobId = $response.id

    Write-Host ""
    Write-Host "Job created!" -ForegroundColor Green
    Write-Host "ID:" $jobId -ForegroundColor Yellow
    Write-Host "Commands: id:<jobid> | /url | exit"
    
    # job mode
    while ($true)
    {
        $inputText = Read-Host ">"

        if ($inputText -eq "/url") { break }

        if ($inputText -eq "exit") {
            Stop-Services
            exit
        }

        if ($inputText -match "^id:(.+)$")
        {
            Get-JobResult $Matches[1]
        }
    }
}