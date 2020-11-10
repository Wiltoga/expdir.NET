if ($args[0] -eq "--help")
{
	echo "Usage :"
	echo ""
	echo "    expdir [<directory>]"
	echo ""
	echo "<directory> : folder to start exploring. By default, the current folder."
}
else
{
	cd $PSScriptRoot
	if ($args[0] -eq $null)
	{
		$location = Get-Location
	}
	else
	{
		$location = $args[0]
	}
	./expdirapp.exe $location
	if(Test-Path script.ps1)
	{
		$curr = Get-Location
		./script.ps1
		rm "$curr/script.ps1"
	}
}
