#addin "Cake.Putty"
#tool nuget:?package=NUnit.ConsoleRunner&version=3.4.0
//////////////////////////////////////////////////////////////////////
// ARGUMENTS
//////////////////////////////////////////////////////////////////////

var target = Argument("target", "Default");
var configuration = Argument("configuration", "Release");
var framework = Argument("framework", "netcoreapp2.0");
var runtime = Argument("runtime", "linux-arm");
var destinationIp = Argument("destinationPi", "192.168.1.1");
var publishDirectory = Argument("publishDirectory", @"./publish");
var destinationDirectory = Argument("destinationDirectory", @"/home/pi/apps/Appliance");
var username = Argument("username", "pi");
var password = Argument("password", "raspberry");
var executableName = Argument("executableName", "HomeSecurity");

//////////////////////////////////////////////////////////////////////
// PREPARATION
//////////////////////////////////////////////////////////////////////

// Define directories.
var binaryDir = Directory("./bin");
var objectDir = Directory("./obj");
var publishDir = Directory("./publish");
var projectFile = "./" + executableName + "/" + executableName + ".csproj";

//////////////////////////////////////////////////////////////////////
// TASKS
//////////////////////////////////////////////////////////////////////

Task("Clean")
    .Does(() =>
    {
        CleanDirectory(binaryDir);
        CleanDirectory(objectDir);
        CleanDirectory(publishDir);
    });

Task("Restore-NuGet-Packages")
    .IsDependentOn("Clean")
    .Does(() =>
{
    NuGetRestore(projectFile);
});

Task("Build")
    .IsDependentOn("Restore-NuGet-Packages")
    .Does(() =>
    {
        var settings = new DotNetCoreBuildSettings
        {
            Framework = framework,
            Configuration = configuration,
            OutputDirectory = "./bin/"
        };

        DotNetCoreBuild(projectFile, settings);
    });

Task("Publish")
    .IsDependentOn("Build")
    .Does(() =>
    {
        var settings = new DotNetCorePublishSettings
        {
            Framework = framework,
            Configuration = configuration,
            OutputDirectory = "./publish/",
            Runtime = runtime
        };
                    
        DotNetCorePublish(projectFile, settings);
    });

Task("Deploy")
    .IsDependentOn("Publish")
    .Does(() =>
    {
        var destination = destinationIp + ":" + destinationDirectory;

        if(IsRunningOnWindows())
        {
            var files = GetFiles("./publish/*");

            if(runtime.StartsWith("win")) 
            {
                destination = @"\\" + destinationIp + @"\" + destinationDirectory;
                CopyFiles(files, destination, true);
            }
            else
            {
                var fileArray = files.Select(m => @"""" + m.ToString() + @"""").ToArray();
                Pscp(fileArray, destination, new PscpSettings
                                                    { 
                                                        SshVersion = SshVersion.V2, 
                                                        User = username,
                                                        Password = password
                                                    }
                );

                var plinkCommand = "chmod u+x,o+x " + destinationDirectory + "/" + executableName;
                Plink(username + "@" + destination, plinkCommand);
            }
        }
        else
        {
            using(var process = StartAndReturnProcess("scp", new ProcessSettings{ Arguments = "-r " + publishDirectory + "/. " + username + "@" + destination }))
            {
                process.WaitForExit();
            }
        }
    });

Task("Default")
    .IsDependentOn("Deploy");

RunTarget(target);