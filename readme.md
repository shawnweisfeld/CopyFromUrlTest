# Copy from URL Test

Test harness to compare the performance between SyncCopyFromUri and download/upload. 

From Blob to Blob and from Files to Blob.

Note there are two way to do server side copies in Azure Storage. Sync and Async.

Use this method to do a sync copy, this uses a high priority thread on the storage server to do the copy. **We use this in the sample.**
[BlobBaseClient.SyncCopyFromUriAsync](https://docs.microsoft.com/en-us/dotnet/api/azure.storage.blobs.specialized.blobbaseclient.synccopyfromuriasync?view=azure-dotnet)

Use this method to do an async copy, this uses a low priority thread on the storage server to do the copy.
[BlobBaseClient.StartCopyFromUriAsync](https://docs.microsoft.com/en-us/dotnet/api/azure.storage.blobs.specialized.blobbaseclient.startcopyfromuriasync?view=azure-dotnet)

## Script to start the container

``` bash
for number in {1..50}
do
az container create \
    --name "copyfromurltestb$number" \
    --resource-group "copyblobfromurl" \
    --location southcentralus \
    --cpu 2 \
    --memory 4 \
    --image "sweisfel/copyfromurltest:latest" \
    --restart-policy Never \
    --no-wait \
    --environment-variables \
        APPINSIGHTS_INSTRUMENTATIONKEY="key" \
        Config__Sources="DefaultEndpointsProtocol=https;AccountName=src01;AccountKey=key;EndpointSuffix=core.windows.net|DefaultEndpointsProtocol=https;AccountName=src02;AccountKey=key;EndpointSuffix=core.windows.net|DefaultEndpointsProtocol=https;AccountName=src03;AccountKey=key;EndpointSuffix=core.windows.net" \
        Config__Destination="DefaultEndpointsProtocol=https;AccountName=dest;AccountKey=key;EndpointSuffix=core.windows.net" \
        Config__MinFileSizeMB="1" \
        Config__MaxFileSizeMB="5" \
        Config__NumFiles="100" \
        Config__ContainerName="test3" \
        Config__Threads="2" \
        Config__UseFiles="true"
done


----
az container create \
    --name "copyfromurltest" \
    --resource-group "copyblobfromurl" \
    --location southcentralus \
    --cpu 2 \
    --memory 4 \
    --image "sweisfel/copyfromurltest:latest" \
    --restart-policy Never \
    --no-wait \
    --environment-variables \
        APPINSIGHTS_INSTRUMENTATIONKEY="key" \
        Config__Sources="DefaultEndpointsProtocol=https;AccountName=src01;AccountKey=key;EndpointSuffix=core.windows.net|DefaultEndpointsProtocol=https;AccountName=src02;AccountKey=key;EndpointSuffix=core.windows.net|DefaultEndpointsProtocol=https;AccountName=src03;AccountKey=key;EndpointSuffix=core.windows.net" \
        Config__Destination="DefaultEndpointsProtocol=https;AccountName=dest;AccountKey=key;EndpointSuffix=core.windows.net" \
        Config__MinFileSizeMB="1" \
        Config__MaxFileSizeMB="5" \
        Config__NumFiles="5000" \
        Config__ContainerName="test3" \
        Config__Threads="2" \
        Config__UseFiles="true"
```