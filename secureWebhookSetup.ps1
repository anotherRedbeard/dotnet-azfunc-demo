# You must install the Microsoft.Graph module to run this script.
#   Install-Module Microsoft.Graph -Scope CurrentUser -Repository PSGallery -Force

Write-Host "================================================================================================="
$scopes = "Application.ReadWrite.All"
$myTenantId = "<your tenant id>"
$myMicrosoftEntraAppRegistrationObjectId = "<your app registration object id>"
$actionGroupRoleName = "App.ActionGroupsSecureWebhook"
$azureMonitorActionGroupsAppId = "461e8683-5575-4561-ac7f-899cc907d62a" # Required. Do not change.

Connect-MgGraph -Scopes $scopes -TenantId $myTenantId

$myAppRoleId = (Get-MgApplication -ApplicationId $myMicrosoftEntraAppRegistrationObjectId).AppRoles | Where-Object { $_.Value -eq $actionGroupRoleName } | Select-Object -ExpandProperty Id

Write-Host "Checking for required service principal.."
$myActionGroupServicePrincipal = Get-MgServicePrincipal -Filter "appId eq '$azureMonitorActionGroupsAppId'"
Write-Host "myActionGroupServicePrincipal: " $myActionGroupServicePrincipal.Id

Write-Host "Getting my app registration service principal.."
$myAppReg = Get-MgApplication -Filter "id eq '$myMicrosoftEntraAppRegistrationObjectId'"
Write-Host "myAppReg: " $myAppReg.AppId
$myServicePrincipal = Get-MgServicePrincipal -Filter "appId eq '$($myAppReg.AppId)'"
Write-Host "myServicePrincipalId: " $myServicePrincipal.Id

# Create the required service principal if it does not exist
if (-not $myActionGroupServicePrincipal) {
    $myActionGroupServicePrincipal = New-MgServicePrincipal -AppId $azureMonitorActionGroupsAppId
}

# Check if the role assignment already exists
$existingRoleAssignment = Get-MgServicePrincipalAppRoleAssignment -ServicePrincipalId $myActionGroupServicePrincipal.Id | Where-Object { $_.AppRoleId -eq $myAppRoleId -and $_.PrincipalId -eq $myActionGroupServicePrincipal.Id -and $_.ResourceId -eq $myServicePrincipal.Id }

# If the role assignment does not exist, create it
if ($null -eq $existingRoleAssignment) {
    Write-Host "Doing app role assignment to the new action group Service Principal`n"
    New-MgServicePrincipalAppRoleAssignment -ServicePrincipalId $myActionGroupServicePrincipal.Id -PrincipalId $myActionGroupServicePrincipal.Id -AppRoleId $myAppRoleId -ResourceId $myServicePrincipal.Id
} 
Write-Host "================================================================================================="