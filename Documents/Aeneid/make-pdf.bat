pushd %~dp0

del Aeneid.aux
del Aeneid.pdf
del Aeneid.ppo
del Aeneid.ppg

pdflatex Aeneid.tex
if errorlevel 1 goto error

makeindex -s Aeneid.ist -o Aeneid.gls Aeneid.glo
if errorlevel 1 goto error

makeindex -s Aeneid.ist -o Aeneid.common-gls Aeneid.common-glo
if errorlevel 1 goto error

makeindex -s Aeneid.ist -o Aeneid.uncommon-gls Aeneid.uncommon-glo
if errorlevel 1 goto error

dotnet run --project %~dp0..\..\src\IxMilia.Classics.ReorderFootnotes\IxMilia.Classics.ReorderFootnotes.csproj Aeneid.ppg Aeneid.ppo
if errorlevel 1 goto error

pdflatex Aeneid.tex
if errorlevel 1 goto error

exit /b 0

:error
popd
echo There was an error building the PDF.  See above output for details.
exit /b 1
