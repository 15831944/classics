$artifactsDir = "$PSScriptRoot\artifacts"
$glossDir = "$PSScriptRoot\gloss"
$downloadDir = "$artifactsDir\downloads"
if (-not (Test-Path $downloadDir)) {
    New-Item -Path $downloadDir -ItemType Directory
}

$maxBook = 1
$outputPath = "$PSScriptRoot\Documents\Aeneid"
$commonWordCount = 250

# download whitaker's words
if (-not (Test-Path "$downloadDir\DICTPAGE.RAW")) {
    Invoke-WebRequest -Uri "http://archives.nd.edu/whitaker/dictpage.zip" -OutFile "$downloadDir\dictpage.zip"
    Expand-Archive -LiteralPath "$downloadDir\dictpage.zip" -DestinationPath "$downloadDir"
}

# download aeneid
if (-not (Test-Path "$downloadDir\aeneid1.xml")) {
    $urlBase = "http://www.perseus.tufts.edu/hopper/xmlchunk?doc=Perseus%3Atext%3A1999.02.0055%3Abook%3D"
    for ($i = 1; $i -le $maxBook; $i++) {
        Invoke-WebRequest -Uri "$urlBase$i" -OutFile "$downloadDir\aeneid$i.xml"
    }
}

# combine dictionaries
Get-Content "$downloadDir\DICTPAGE.RAW","$glossDir\latin-dictionary-suppliment.txt" | Set-Content "$outputPath\LatinDictionary.txt"

# build
dotnet restore
dotnet build
$textFiles = @()
$glossFiles = @()
for ($i = 1; $i -le $maxBook; $i++) {
    $textFiles += "$downloadDir\aeneid$i.xml"
    $glossFiles += "$glossDir\aeneid$i.xml"
}

$textFilesJoined = $textFiles -Join ","
$glossFilesJoined = $glossFiles -Join ","
dotnet run --no-restore --no-build --project "$PSScriptRoot\src\IxMilia.Classics.ProcessFile\IxMilia.Classics.ProcessFile.csproj" $textFilesJoined $glossFilesJoined $outputPath $commonWordCount "$outputPath\LatinDictionary.txt"
& "$PSScriptRoot\Documents\Aeneid\make-pdf.bat"
