@echo off    
for /R %%f in (*.fng) do (
echo %%f
FEngCli.exe decompile -i "%%f" -o "%%f.json"
)