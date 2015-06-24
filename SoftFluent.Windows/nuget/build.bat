mkdir package\lib
mkdir package\lib
mkdir package\lib\net40

copy "..\SoftFluent.Windows\bin\Release" "package\lib\net40"
copy "SoftFluent.Windows.csproj.nuspec" "package\"
nuget pack "package\SoftFluent.Windows.csproj.nuspec"
rmdir /S /Q package