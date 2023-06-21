SET ProjectPath=rxdev.Accounting.App\rxdev.Accounting.App.csproj
SET CommonParam=-p:DebugType=None -p:DebugSymbols=false 
SET PortableParam=-p:PublishSingleFile=true -p:IncludeNativeLibrariesForSelfExtract=true -p:IncludeAllContentForSelfExtract=true

del "%~dp0publish\rxdev.Accounting.exe"
dotnet publish -c Release -o "publish" -r win-x64 -p:PlatformTarget=x64 %CommonParam% %PortableParam% %ProjectPath%
ren "publish\rxdev.Accounting.App.exe" rxdev.Accounting.exe