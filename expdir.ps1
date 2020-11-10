Invoke-Expression "$PSScriptRoot/expdirapp.exe $args"
if(Test-Path location)
{
	$location = "$(cat $PSScriptRoot/location)"
	cd $location
	rm "$PSScriptRoot/location"
}