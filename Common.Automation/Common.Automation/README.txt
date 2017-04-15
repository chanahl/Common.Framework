AssemblyInfo.cs has been stripped down to bare essentials.
This template assumes you will be using some sort of shared assembly info file so include that.
Add a description (non-empty string) to AssemblyInfo.cs: [assembly: AssemblyDescription("")].

References are empty.
For those artifacts you wish to package add them here as project references.
Optional: set property Copy Local to False.

Configuration Manager at the solution level will now contain a new configuration (CI).
Change all project configurations to Debug.

For this project go to properties : build events : post-build event command line.
Edit the output variable under :Pack to point to where the .nupkg will be created.