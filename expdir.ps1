$dir = Get-Location
$curr = "$PSScriptRoot"
$args = """$dir""" + $args
cd $curr
Invoke-Expression "./expdirapp.exe $args"
if(Test-Path location)
{
	$location = "$(cat "./location")"
	rm "./location"
	cd $location
}
else
{
    cd $dir
}
