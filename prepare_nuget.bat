rm -r RePacker/bin/
rm -r RePacker/obj/
dotnet restore RePacker/RePacker.csproj
dotnet pack RePacker/RePacker.csproj -c Release -o ../nuget/

rm -r RePacker.Unsafe/bin/
rm -r RePacker.Unsafe/obj/
dotnet restore RePacker.Unsafe/RePacker.Unsafe.csproj
dotnet pack RePacker.Unsafe/RePacker.Unsafe.csproj -c Release -o ../nuget/ -p:License=MIT

