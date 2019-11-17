# Benches

This repository contains code required to run the Benches website. The website is developed as a personal project aiming to learn MVC concepts and experiment with Azure cloud technologies. One can upload a picture of a favourite bench or explore the benches uploaded by other users.

## Technologies used
- .NET Core 2.2
- .NET MVC
- Azure Storage emulator
- Azure Web Apps
- Azure Storage
- Entiy Framework

## Running the project

Clone the project from this repository. Next, you need to set up the Azure storage emulator (Azurite), so that the app runs locally.

To install it, run the following command:

`docker pull mcr.microsoft.com/azure-storage/azurite`

and to start it: 

`docker run -p 10000:10000 -p 10001:10001 mcr.microsoft.com/azure-storage/azurite`

If you don't have docker for Windows, there are alternative methods to run the emulator, which are described in this document.
https://docs.microsoft.com/en-us/azure/storage/common/storage-use-azurite?toc=%2fazure%2fstorage%2fblobs%2ftoc.json

The webapp can then be run and will connect to the locally emulated azure blob storage.

Once the app is running, it will try to create a blob container with name specified in the FileContainerName item of the appsettings. If the container does not already exist, the app with create it during StartUp. This will be the container where photos of benches are stored.

To run the app and be able to store metadata to Azure Cosmos Table storage, forllow the instructions to install the Azure Cosmos Emulator here: https://docs.microsoft.com/en-us/azure/cosmos-db/local-emulator. Next, start the emulator from command prompt as an administrator with the following command:

`CosmosDB.Emulator.exe /EnableTableEndpoint /DataPath=[LOCAL_FOLDER]`

Replace [LOCAL_FOLDER] with a directory of choice. The connection string set up in the application is the only piece needed to connect to the emulated table storage.

## Deploying project to Azure

The project can be deployed as an Azure Web App. This can be an existing Azure Web App or a brand new one. Currently, this app is deployed using deployment to local Git Repo as described here: https://docs.microsoft.com/en-gb/azure/app-service/deploy-local-git, but there are other ways to publish, such as directly from Visual Studio.

Upon deployment the AzureStorageConfig connection string needs to be updated with the one for the read Azure Storage Account. The connection string containing the Storage account name and key can be found from the Access Keys blade of the Storage Account resource in the Azure Portal. This connection string can then be added as an environmental variable to AppService, via the Configurations blade.