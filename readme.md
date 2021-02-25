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
(Get-AzStorageAccount -ResourceGroupName sqldwtest -Name sweisfelaegpv1 -IncludeGeoReplicationStats).GeoReplicationStats.LastSyncTime





for number in {1..50}
do
az container create \
    --name copyfromurltestsec$number\
    --resource-group "rg" \
    --location southeastasia \
    --cpu 4 \
    --memory 8 \
    --image "sweisfel/copyfromurltestsec:latest" \
    --restart-policy Never \
    --no-wait \
    --environment-variables \
        APPINSIGHTS_INSTRUMENTATIONKEY="key" \
        Config__Source="connection string" \
        Config__Destination="connection string" \
        Config__FileSizeMB="0.23" \
        Config__NumFiles="10000" \
        Config__ContainerName="test" \
        Config__Threads="10" \
        Config__LoadMode="false"
done


----
az container create \
    --name copyfromurltestseca\
    --resource-group "rg" \
    --location southeastasia \
    --cpu 2 \
    --memory 4 \
    --image "sweisfel/copyfromurltestsec:latest" \
    --restart-policy Never \
    --no-wait \
    --environment-variables \
        APPINSIGHTS_INSTRUMENTATIONKEY="key" \
        Config__Source="connection string" \
        Config__Destination="connection string" \
        Config__FileSizeMB="0.23" \
        Config__NumFiles="5" \
        Config__ContainerName="test" \
        Config__Threads="2" \
        Config__LoadMode="true"
```
