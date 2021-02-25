# Copy from URL Test

Test harness to test the performance of copying from a RA-GRS Secondary to a primary

## Script to start the container

``` bash
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
