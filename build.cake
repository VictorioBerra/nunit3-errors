
#tool "nuget:?package=NUnit.ConsoleRunner&prerelease"

var target           = Argument("target", "Default");
var configuration   = Argument<string>("configuration", "Release");

var buildArtifacts   = Directory("./artifacts");
var testArtifacts    = Directory("./artifacts/testresults");

///////////////////////////////////////////////////////////////////////////////
// Clean
///////////////////////////////////////////////////////////////////////////////
Task("Clean")
    .Does(() =>
{
    CleanDirectories(new DirectoryPath[] { buildArtifacts });
});

///////////////////////////////////////////////////////////////////////////////
// Build
///////////////////////////////////////////////////////////////////////////////
Task("Build")
    .IsDependentOn("Clean")
    .Does(() =>
{
    var settings = new DotNetCoreBuildSettings 
    {
        Configuration = configuration
    };

    // Tests projects trigger everything to be built
    var testProjects = GetFiles("./tests/**/*.csproj");

    foreach(var project in testProjects)
	{
	    DotNetCoreBuild(project.GetDirectory().FullPath, settings);
    }
});

///////////////////////////////////////////////////////////////////////////////
// Test
///////////////////////////////////////////////////////////////////////////////
Task("Test")
    .IsDependentOn("Clean")
    .IsDependentOn("Build")
    .Does(() =>
{

    NUnit3($"./tests/**/bin/{configuration}/**/*Tests.dll");

});


Task("Default")
  .IsDependentOn("Build")
  .IsDependentOn("Test");

RunTarget(target);