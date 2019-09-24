# Walks and Benches

This repository contains code required to run the Walks And Benches website. The website is developed as a personal project aiming to learn web development concepts and experiment with Azure cloud technologies. One can upload a picture of a favourite bench or explore the exisitng pictures.

## Project Set-up

This project uses Azure Storage Emulator. To install it up run the following command:

`docker pull mcr.microsoft.com/azure-storage/azurite`

and to start it: 

`docker run -p 10000:10000 -p 10001:10001 mcr.microsoft.com/azure-storage/azurite`

The webapp can then be run and will connect to the locally emulated azure blob storage.



