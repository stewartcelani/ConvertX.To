# Variables Start
$AlertIfLessThanThisFreeSpace = 20 #Set this to the alert threshold, if free space is less than this (in GB), an email will be sent
$SMTPServer = "smtpserver"
$EmailTo = "email@email.com"
$EmailFrom = "email@email.com"
# Variables End


$scriptDirectory = split-path -parent $MyInvocation.MyCommand.Definition
$scriptName = $MyInvocation.MyCommand.Name
$logPath = "$scriptDirectory\$scriptName.log"

function Write-Log {
    param(
    [parameter(Mandatory=$true)]
    [string]$Text,
    [parameter(Mandatory=$true)]
    [ValidateSet("WARNING","ERROR","INFO")]
    [string]$Type
    )

    [string]$logMessage = [System.String]::Format("[$(Get-Date)] -"),$Type, $Text
    Add-Content -Path $logPath -Value $logMessage
}

$ComputerName = $env:COMPUTERNAME

$Alerts = @()
$Drives = Get-PSDrive -PSProvider FileSystem
ForEach ($Drive in $Drives) {
    $driveName = $Drive.Name
    $used = [math]::Round($Drive.Used / 1GB,2)    
    if ($used -eq 0) {
        # Skipping loop on drives with 0 space used = optical DVD drives       
        continue
    }
    $free = [math]::Round($Drive.Free / 1GB,2)
    $total = $used + $free
	if ($total -le 9) {
       # Accounts for DVD drives with a DVD inserted as they will show as however much data the drive has        
	   continue
    }
    $percentUsed = [math]::Round(($used / $total) * 100,2)
    $percentFree = [math]::Round(($free / $total) * 100,2) 
 
    Write-Log -Text "$driveName has $percentFree% space free ($free GB free, $used/$total GB used)" -Type INFO

    if ($free -le $AlertIfLessThanThisFreeSpace) {
        #Alert Triggered, add drive to $Alerts array so we can send an email later
        $statusText = "$driveName drive on $ComputerName only has $percentFree% free space ($free GB free, $used/$total GB used)."
        Write-Log -Text $statusText -TYPE ERROR
        Write-Log -Text "Attempting to send alert email." -TYPE WARNING
        $subject = "CRITICAL - $driveNAME DRIVE SPACE: ONLY $free GB FREE SPACE"        
        $htmlBody = "<h1 style='color:red;'>LOW DISK SPACE WARNING</h1><p>" + $driveName + " drive on " + $ComputerName + " only has " + $percentFree + "% free space (" + $free + "GB free, " + $used + "/" + $total + "GB used).</p><p>Please rectify the issue immediately.</p><p style='font-size:10;'>Script info: " + [System.String]::Format("[$(Get-Date)] ") + "$scriptDirectory\$scriptName running from $ComputerName set to alert if any drive has less than $AlertIfLessThanThisFreeSpace GB free space. Script log file location is $logPath."
        $Alerts += [PSCustomObject]@{
            DriveName = $driveName
            Used = $used
            Free = $free
            Total = $total
            StatusText = $statusText
            PercentUsed = [math]::Round(($used / $total) * 100,2)
            PercentFree = [math]::Round(($free / $total) * 100,2)
            EmailSubject = $subject
            EmailHtmlBody = $htmlBody
        }                
    }
}

# Finished looping and adding drives that are low on free space to $Alerts array, send the email
if ($Alerts.count -eq 1) {
    #Email format if only 1 drive in the alerts array is easy
    Send-MailMessage -Subject $Alerts[0].EmailSubject  -BodyAsHtml $Alerts[0].EmailHtmlBody -From "$EmailFrom" -To "$EmailTo" -SmtpServer "$SMTPServer"
} elseif ($Alerts.count -ge 2) {   
    #Email format for >=2 drives in error
    $groupSubject = "CRITICAL - MULTIPLE DRIVES <$AlertIfLessThanThisFreeSpace GB FREE SPACE"
    $groupHtmlBody = "<h1 style='color:red;'>LOW DISK SPACE WARNING</h1>"
    foreach($Alert in $Alerts) {
        #Add the specific status text for each alert/drive with less than N free space
        $status = $Alert[0].StatusText        
        $groupHtmlBody += "<p>$status</p>"
    }
    $groupHtmlBody += "<p>Please rectify these issues immediately.</p><p style='font-size:10;'>Script info: " + [System.String]::Format("[$(Get-Date)] ") + "$scriptDirectory\$scriptName running from $ComputerName set to alert if any drive has less than $AlertIfLessThanThisFreeSpace% free space. Script log file location is $logPath.</p>"
    Send-MailMessage -Subject $groupSubject  -BodyAsHtml $groupHtmlBody -From "$EmailFrom" -To "$EmailTo" -SmtpServer "$SMTPServer"
}