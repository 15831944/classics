pushd %~dp0

set bookname=DeBelloCivili

del %bookname%.aux
del %bookname%.pdf
del %bookname%.ppo
del %bookname%.ppg

pdflatex %bookname%.tex
if errorlevel 1 goto error

makeindex -s %bookname%.ist -o %bookname%.gls %bookname%.glo
if errorlevel 1 goto error

makeindex -s %bookname%.ist -o %bookname%.common-gls %bookname%.common-glo
if errorlevel 1 goto error

makeindex -s %bookname%.ist -o %bookname%.uncommon-gls %bookname%.uncommon-glo
if errorlevel 1 goto error

dotnet run --project %~dp0..\..\src\IxMilia.Classics.ReorderFootnotes\IxMilia.Classics.ReorderFootnotes.csproj %bookname%.ppg %bookname%.ppo
if errorlevel 1 goto error

pdflatex %bookname%.tex
if errorlevel 1 goto error

exit /b 0

:error
popd
echo There was an error building the PDF.  See above output for details.
exit /b 1
