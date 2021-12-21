# transaction-sorter
Website to pull transactions from YNAB API and sort using splits without showing other categories.

### Notes
- Azure Key Vault secrets must be stored in format `XXX--YYY` to match `XXX:YYY` in code
- Azure Web App settings must be stored in format `XXX__YYY` to match `XXX:YYY` in code

### To Run
- Adjust `AzureKeyVault:URL` in `appsettings.json` to local API key
- API key for YNAB should be stored as a Secret `YNAB--apikey` in Azure Key Vault
- Adjust `YNAB:Budget` key in `appsettings.json` to match your YNAB Budget Id
- Adjust YNAB Categories that are listed in `Scripts/index.ts` to list custom categories
  - `Value` will be used to pull YNAB Category Id from `YNAB:<Value>` in `appsettings.json`
- `dotnet run`

### To Run in Azure
- Export templates for current configuration are included
- Settings from `appsettings.json` which are used to run locally must be added to Azure Web App configuration
  - See formatting notes above

### Used in this Project
- .NET 5 
- F#
- Typescript
- Web API
- Vue.js
- Axios
- Webpack
- Azure App Service
- Azure Key Vault
- GitHub Actions (deployment)