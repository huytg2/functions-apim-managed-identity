# Your tenant ID (in the Azure portal, under Azure Active Directory > Overview).
$tenantID = 'ca4b9986-c729-4ee0-a5be-39c116865241'

# The name of your web app, which has a managed identity that should be assigned to the server app's app role.
$webAppName = 'func-app-poc-35'
$resourceGroupName = 'rg-test-iac'

# The name of the server app that exposes the app role.
$serverApplicationName = 'poc-apim' # For example, MyApi

# The name of the app role that the managed identity should be assigned to.
$appRoleName = 'Reader' # For example, MyApi.Read.All

# Look up the web app's managed identity's object ID.
$managedIdentityObjectId = (Get-AzWebApp -ResourceGroupName $resourceGroupName -Name $webAppName).identity.principalid

Connect-MgGraph -TenantId $tenantId -Scopes 'Application.Read.All','Application.ReadWrite.All','AppRoleAssignment.ReadWrite.All','Directory.AccessAsUser.All','Directory.Read.All','Directory.ReadWrite.All'

# Look up the details about the server app's service principal and app role.
$serverServicePrincipal = (Get-MgServicePrincipal -Filter "DisplayName eq '$serverApplicationName'")
$serverServicePrincipalObjectId = $serverServicePrincipal.Id
$appRoleId = ($serverServicePrincipal.AppRoles | Where-Object {$_.Value -eq $appRoleName }).Id

# Assign the managed identity access to the app role.
New-MgServicePrincipalAppRoleAssignment -ServicePrincipalId $serverServicePrincipalObjectId -PrincipalId '2e7f9031-1356-4606-9762-ce3fe321eed3' -ResourceId $serverServicePrincipalObjectId -AppRoleId $appRoleId