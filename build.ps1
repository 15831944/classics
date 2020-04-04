$artifactsDir = "$PSScriptRoot\artifacts"
$glossDir = "$PSScriptRoot\gloss"
$downloadDir = "$artifactsDir\downloads"
if (-not (Test-Path $downloadDir)) {
    New-Item -Path $downloadDir -ItemType Directory
}

#
# Prepare dictionary
#

# download whitaker's words
if (-not (Test-Path "$downloadDir\DICTPAGE.RAW")) {
    Invoke-WebRequest -Uri "http://archives.nd.edu/whitaker/dictpage.zip" -OutFile "$downloadDir\dictpage.zip"
    Expand-Archive -LiteralPath "$downloadDir\dictpage.zip" -DestinationPath "$downloadDir"
}

# build tool
dotnet restore
dotnet build

function Build-Glossary([string] $mode, [string[]] $textFiles, [string[]] $glossFiles, [string] $outputPath, [int] $commonWordCount) {
    $textFilesJoined = $textFiles -Join ","
    $glossFilesJoined = $glossFiles -Join ","
    dotnet run --no-restore --no-build --project "$PSScriptRoot\src\IxMilia.Classics.ProcessFile\IxMilia.Classics.ProcessFile.csproj" $mode $textFilesJoined $glossFilesJoined $outputPath $commonWordCount "$outputPath\LatinDictionary.txt"
}

function Build-Aeneid() {
    $maxBook = 1
    $outputPath = "$PSScriptRoot\Documents\Aeneid"
    $commonWordCount = 250

    $aeneidGlossDir = "$glossDir\aeneid"

    # combine dictionaries
    Get-Content "$downloadDir\DICTPAGE.RAW","$aeneidGlossDir\dictionary-suppliment.txt" | Set-Content "$outputPath\LatinDictionary.txt"

    # download aeneid
    if (-not (Test-Path "$downloadDir\aeneid1.xml")) {
        $urlBase = "http://www.perseus.tufts.edu/hopper/xmlchunk?doc=Perseus%3Atext%3A1999.02.0055%3Abook%3D"
        for ($i = 1; $i -le $maxBook; $i++) {
            Invoke-WebRequest -Uri "$urlBase$i" -OutFile "$downloadDir\aeneid$i.xml"
        }
    }

    # build Aeneid
    $textFiles = @()
    $glossFiles = @()
    for ($i = 1; $i -le $maxBook; $i++) {
        $textFiles += "$downloadDir\aeneid$i.xml"
        $glossFiles += "$aeneidGlossDir\aeneid$i.xml"
    }

    Build-Glossary -mode poetry -textFiles $textFiles -glossFiles $glossFiles -outputPath $outputPath -commonWordCount $commonWordCount
    & "$PSScriptRoot\Documents\Aeneid\make-pdf.bat"
}

function Build-DeBelloCivili() {
    $maxBoox = 1
    $outputPath = "$PSScriptRoot\Documents\DeBelloCivili"

    $deBelloGlossDir = "$glossDir\deBelloCivili"

    # combine dictionaries
    Get-Content "$downloadDir\DICTPAGE.RAW","$deBelloGlossDir\dictionary-suppliment.txt" | Set-Content "$outputPath\LatinDictionary.txt"

    # download content
    if (-not (Test-Path "$downloadDir\deBelloCivili1.xml")) {
        $urlBase = "http://www.perseus.tufts.edu/hopper/xmlchunk?doc=Perseus%3Atext%3A1999.02.0075%3Abook%3D"
        for ($i = 1; $i -le $maxBoox; $i++) {
            Invoke-WebRequest -Uri "$urlBase$i" -OutFile "$downloadDir\deBelloCivili$i.xml"
        }
    }

    # build de bello civili
    $textFiles = @()
    $glossFiles = @()
    for ($i = 1; $i -le $maxBook; $i++) {
        $textFiles += "$downloadDir\deBelloCivili$i.xml"
        $glossFiles += "$deBelloGlossDir\deBelloCivili$i.xml"
    }

    Build-Glossary -mode prose -textFiles $textFiles -glossFiles $glossFiles -outputPath $outputPath -commonWordCount $commonWordCount
    & "$PSScriptRoot\Documents\DeBelloCivili\make-pdf.bat"
}

Build-Aeneid
Build-DeBelloCivili
