az login

New-AzResourceGroupDeployment -ResourceGroupName dev-sample-wus-rsg -TemplateFile result.json

az deployment group create --resource-group dev-sample-wus-rsg --template-file result.json