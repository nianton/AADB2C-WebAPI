# Docs: https://docs.microsoft.com/en-us/azure/active-directory-b2c/active-directory-b2c-devquickstarts-graph-dotnet#register-your-application-in-your-tenant

# Application/Id: (app name)/983792b8-f77b-462a-9b1a-d2ac39a0fe3e
# admin@trainingadb2c.onmicrosoft.com
# *****

Connect-MsolService

$applicationId = "[APP_ID]"
$sp = Get-MsolServicePrincipal -AppPrincipalId $applicationId
Add-MsolRoleMember -RoleObjectId fe930be7-5e62-47db-91af-98c3a49a38b1 -RoleMemberObjectId $sp.ObjectId -RoleMemberType servicePrincipal