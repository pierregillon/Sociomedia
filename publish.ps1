$runtimes = @("linux-x64", "win10-x64")

mkdir ./dist

echo $($runtimes)

foreach($runtime in $runtimes) {

    dotnet publish --configuration Release --runtime $($runtime) ./src/Sociomedia.sln

    Compress-Archive -Path "src/Sociomedia.Front/bin/Release/*/$($runtime)/publish/*" -DestinationPath "./dist/Sociomedia.Front-$($runtime).zip"

    Compress-Archive -Path "src/Sociomedia.FeedAggregator/bin/Release/*/$($runtime)/publish/*" -DestinationPath "./dist/Sociomedia.FeedAggregator-$($runtime).zip"
    
    Compress-Archive -Path "src/Sociomedia.ProjectionSynchronizer/bin/Release/*/$($runtime)/publish/*" -DestinationPath "./dist/Sociomedia.ProjectionSynchronizer-$($runtime).zip"
}

