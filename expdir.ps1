./expdirapp.exe
if(Test-Path script.ps1)
{
	$curr = Get-Location
	./script.ps1
	rm "$($curr)/script.ps1"
}