# Azure Receipt Read Demo

This demo proceesses pictures of receipts using Azure and Microsoft technologies, with the 
read data being saved in a SQL database to be used for any purpose. 

## Architecture
```raw
                            ┌───────────────────┐                            
                            │     Document      │                            
                            │   Intelligence    │                            
                            │                   │                            
                            └───────────────────┘                            
                                      ▲                                      
                                      │                                      
                                      │                                      
                                      ▼                                      
┌───────────────────┐       ┌───────────────────┐       ┌───────────────────┐
│                   │       │                   │       │                   │
│       SFTP        │──────▶│  Azure Function   │──────▶│    SQL Server     │
│                   │       │                   │       │                   │
└───────────────────┘       └───────────────────┘       └───────────────────┘
```

## Process

1. Images are uploaded to SFTP
2. Blob trigger on Azure Function reads in blob
3. Azure Function sends image to Document Intelligence service
4. Read data is saved to a SQL server. 
5. Data can be moved anywhere afterward. 

## Deployment

//TODO

