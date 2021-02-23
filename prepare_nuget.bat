dotnet pack -o ../nuget/ -p:NuspecFile=RePacker.nuspec RePacker/RePacker.csproj -c Release

dotnet pack -o ../nuget/ -p:NuspecFile=RePacker.Unsafe.nuspec RePacker.Unsafe/RePacker.Unsafe.csproj -c Release

