Write-Host ""
Write-Host "=== Git Reproducer Client ===" -ForegroundColor Cyan
Write-Host ""

$repo = Read-Host "Enter repository URL"

Write-Host "`nCreating job..."
$response = Invoke-RestMethod `
    -Method Post `
    -Uri "http://localhost:5000/jobs" `
    -ContentType "application/json" `
    -Body (@{ repositoryUrl = $repo } | ConvertTo-Json)

Write-Host ""
Write-Host "Job created!" -ForegroundColor Green
Write-Host "ID:" $response.id
Write-Host ""
Write-Host "Type id:<job-id> to fetch result (example: id:$($response.id))"
Write-Host ""

while ($true)
{
    $inputText = Read-Host ">"

    if ($inputText -match "^id:(.+)$")
    {
        $jobId = $Matches[1]

        Write-Host "`nFetching job result..."
        try {
            $job = Invoke-RestMethod "http://localhost:5000/jobs/$jobId"

            Write-Host ""
            Write-Host "Status: " $job.statusText -ForegroundColor Yellow
            Write-Host "----------------------------------------"
            Write-Host $job.logs
            Write-Host "----------------------------------------"
        }
        catch{
            Write-Host "Job not found." -ForegroundColor Red
        }
    }
}