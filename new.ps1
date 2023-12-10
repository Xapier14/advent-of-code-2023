$Day = (Get-Date).Day
$ProjectDir = "Day${Day}"

if (Test-Path -Path $ProjectDir) {
    "Project '${ProjectDir}' already exists."
    Exit
}
New-Item -Name $ProjectDir -ItemType "directory"
Push-Location $ProjectDir
& dotnet new console --framework net7.0
& dotnet add package Xapier14.AdventOfCode