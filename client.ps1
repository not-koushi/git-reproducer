Write-Host ""
Write-Host "=== Git Reproducer Client ===" -ForegroundColor Cyan
Write-Host ""

$repo = Read-Host "Enter repository URL"

# normalize input
if ($repo -notmatch "^https?://")
{
    if ($repo -match "^[^/]+/[^/]+$")
    {
        $repo = "https://github.com/$repo"
    }
    elseif ($repo -match "^github\.com/")
    {
        $repo = "https://$repo"
    }
}

Write-Host "`nCreating job..." -ForegroundColor Cyan
$response = Invoke-RestMethod `
    -Method Post `
    -Uri "http://localhost:5000/jobs" `
    -ContentType "application/json" `
    -Body (@{ repositoryUrl = $repo } | ConvertTo-Json)

Write-Host ""
Write-Host "Job created!" -ForegroundColor Green
Write-Host "ID:" $response.id -ForegroundColor Yellow
Write-Host ""
Write-Host "Type id:<jobid> to fetch result"
Write-Host ""

while ($true)
{
    $inputText = Read-Host ">"

    if ($inputText -match "^id:(.+)$")
    {
        $jobId = $Matches[1]

        Write-Host "`nFetching job result..." -ForegroundColor Cyan

        try {
            $job = Invoke-RestMethod "http://localhost:5000/jobs/$jobId"

            # colored status
            if ($job.statusText -eq "Completed") {
                Write-Host "Status: $($job.statusText)" -ForegroundColor Green
            }
            elseif ($job.statusText -eq "Failed") {
                Write-Host "Status: $($job.statusText)" -ForegroundColor Red
            }
            else {
                Write-Host "Status: $($job.statusText)" -ForegroundColor Yellow
            }

            Write-Host ""

            # replace '.' with actual workspace path
            $workspacePath = ".\src\workspaces\$jobId"
            $logs = $job.logs -replace "Cloning into '\.'", "Cloning into `"$workspacePath`""

            Write-Host $logs -ForegroundColor Gray
        }
        catch {
            Write-Host "Job not found." -ForegroundColor Red
        }

        Write-Host ""
    }
}