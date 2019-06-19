﻿$ErrorActionPreference = "Stop";
Remove-Item built -Force -Recurse -ErrorAction SilentlyContinue
Remove-Item doc/index.md -Force -Recurse -ErrorAction SilentlyContinue  
Remove-Item doc/_site -Force -Recurse -ErrorAction SilentlyContinue
Remove-Item doc/obj -Force -Recurse -ErrorAction SilentlyContinue    
dotnet clean 
dotnet restore

dotnet test /p:CollectCoverage=true /p:Exclude=[xunit.*]* /p:CoverletOutput='../../built/DashDashVersion.xml' /p:CoverletOutputFormat=cobertura

$gitCurrentTag = git describe --tags --abbrev=0

if($env:Build_Reason -ne "PullRequest") {

    if(Test-Path env:manualVersion) {

        Write-Host "Manually provided version detected!"
        $env:fullSemVer = $env:manualVersion
        $env:semVer = $env:manualVersion
        $env:assemblyVersion = $env:manualVersion+".0"
    }
    else {

        if($env:TF_BUILD -eq "True") {

            $temp = git-flow-version --branch $env:BUILD_SOURCEBRANCHNAME | ConvertFrom-Json
        }
        else {

            $temp = git-flow-version | ConvertFrom-Json
        }
        $env:fullSemVer = $temp.FullSemVer
        $env:semVer = $temp.SemVer
        $env:assemblyVersion = $temp.AssemblyVersion
        Write-Host "calculated version:"
        $temp | Format-List
    }


    $assemblyInfoContent = @"
// <auto-generated/>
using System.Reflection;
using System.Runtime.InteropServices;

[assembly: AssemblyVersionAttribute("$($env:assemblyVersion)")]
[assembly: AssemblyFileVersionAttribute("$($env:assemblyVersion)")]
[assembly: AssemblyInformationalVersionAttribute("$($env:fullSemVer)")]
"@

    if (-not (Test-Path "built")) {
        New-Item -ItemType Directory "built"
    }

    $assemblyInfoContent | Out-File -Encoding utf8 (Join-Path "built" "SharedAssemblyInfo.cs") -Force
    dotnet pack /p:PackageVersion=$env:fullSemVer /p:NoPackageAnalysis=true


    if ($env:TF_BUILD -eq "True" -and $env:imageName -eq "windows-latest") {
        Write-Host "Windows build detected"
        if ($gitCurrentTag -notlike $env:semVer) {
            Write-Host "Tagging build"
		    git remote set-url origin git@github.com:hightechict/DashDashVersion.git
            git tag $env:semVer
            Start-Process -Wait -ErrorAction SilentlyContinue git -ArgumentList "push", "--verbose", "origin", "$($env:semVer)"            
        }

        if ($env:Build_SourceBranch -notlike "*/feature/*") {
            Write-Host "Publishing NuGet package"
            pushd built
            dotnet nuget push *.nupkg --api-key $env:NuGet_APIKEY --no-symbols true --source https://api.nuget.org/v3/index.json 
            popd
        }

        if($env:Build_SourceBranchName -like "master"){
            Copy-Item README.md doc/index.md
            docfx ./doc/docfx.json
        }

    }
}
else{
    Copy-Item README.md doc/index.md
    docfx ./doc/docfx.json
}