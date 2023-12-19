@echo off    
for /R %%f in (*.json) do (
echo %%f
FEngCli.exe compile -i "%%f" -o "%%f.fng"
)