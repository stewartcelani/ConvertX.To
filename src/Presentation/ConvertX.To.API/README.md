This project requires an ``appsettings.json`` key in the following format.

I have this sitting in an ``appsettings.secret.json`` that isn't committed to the repo.

```json
{
  "MsGraphSettings": {
    "TenantId": "14983a8c-812c-4d78-b2bc-39660ef7b186",
    "ClientId": "89df1baf-3f91-426c-8f4c-f1ba2e4629cc",
    "ClientSecret": "t0t8Q~Pc-!@818H38yQH84gnzoMvOKF62OIR11a-O",
    "Scope": "https://graph.microsoft.com/.default",
    "AuthenticationEndpoint": "https://login.microsoftonline.com",
    "GraphEndpoint": "https://graph.microsoft.com/beta/sites/a534da4a-43cb-4aa7-b648-5f893dc68005/drive/items"
  }
}
```