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
	$dir = Get-Location
	cd $PSScriptRoot
	if ($args[0] -eq $null)
	{
		./expdirapp.exe $dir
	}
	else
	{
		./expdirapp.exe $args[0]
	}
	if(Test-Path script.ps1)
	{
		$curr = Get-Location
		./script.ps1
		rm "$curr/script.ps1"
	}
	else
	{
		cd $dir
	}
}
