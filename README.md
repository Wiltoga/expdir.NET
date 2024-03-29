# expdir
command line tool to navigate in the directories. Basically a `ls` and `cd` tool.
Available for Windows (Powershell only) and Linux.

## Installation :

You need the .NET Core 3.1 to run this program.

First clone the project :
```
git clone https://github.com/WildGoat07/expdir.git
```
Then run :
```
dotnet build --configuration Release
```
The binaries are in the `./bin/Release/netcoreapp3.1` folder.

On Windows :
Once the project built, copy the binaries to a folder without admin rights, and add that folder to PATH. You also need the permission to run Powershell scripts.

On Linux :
Once the project built, copy the binaries somewhere with write/read rights. Then copy the [install file content](https://github.com/WildGoat07/expdir/blob/master/install) into the `.bashrc`. Do not forget to change the `dir`variable to where you put the files in the first place.

## How to use :

Use the `expdir` command to display the browser :

![](https://i.gyazo.com/823840fca845e9ae86f78ad76dfbfc06.png)

From there you can freely move using arrow keys. Press Enter to go inside a folder or its parent. `Ctrl+O` to stop the app and go to the current directory or `Ctrl+X` to cancel.

You can also use `expdir <starting directory>` to specify a folder from where to start the browser.

![](https://i.gyazo.com/957d7dc7789260a571c4ff7f5ae2c7ed.png)
