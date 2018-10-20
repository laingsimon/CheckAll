# CheckAll
__Utility for managing the staging of changed files__  

If you use `git add -p` it will only stage changed files; ___CheckAll___  will include these; amongst other features.

## Features

1. Stage untracked files
2. Stage parts of files
3. Revert changes to files
4. Unstage files previously staged
5. Diff each file independently (visual or textual)

This tool must be run from the windows command prompt; it cannot accept input from the bash (mingw) console.

## Installation / usage
This tool requires Microsoft .Net v4 or later to be installed.

1. Download the latest release and extract
2. Run the following command `git config --global alias.check "!'/path/to/CheckAll/CheckAll.exe'"`

replace `/path/to/CheckAll` with the path to the extracted files, use `/` instead of `\`, for example use `/c/CheckAll/CheckAll.exe` if the release is extracted to `c:\CheckAll\CheckAll.exe`

Once the above steps have been completed, you can run the following command from within a repository directory

`git check`

Folllow the prompts on screen.

Press `Ctrl+C` anytime to exit the tool.