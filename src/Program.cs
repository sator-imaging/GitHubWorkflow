using System.CommandLine;
using GitHubWorkflow;
using System.Linq;
using System;
using System.CommandLine.Parsing;

var workflowArgument = new Argument<string>("workflow-file")
{
    Description = "GitHub workflow file name (.yml/.yaml or base name).",
    CustomParser = result =>
    {
        var value = result.Tokens.SingleOrDefault()?.Value ?? string.Empty;
        if (string.IsNullOrWhiteSpace(value))
        {
            result.AddError("workflow file name is required.");
            return string.Empty;
        }

        if (value.Contains('/') || value.Contains('\\'))
        {
            result.AddError("workflow file must be a file name without any path separators.");
            return string.Empty;
        }

        var resolvedPath = WorkflowUtilities.ResolveWorkflowPath(value);
        if (resolvedPath == null)
        {
            result.AddError($"workflow file '{value}' not found under .github/workflows with .yml or .yaml extension.");
            return string.Empty;
        }

        return resolvedPath;
    }
};

var cmdFlag = new Option<bool>("--cmd")
{
    Description = "Emit commands formatted for Windows cmd.exe (on by default on Windows; prepend @echo off, replace trailing backslash with ^, etc).",
    DefaultValueFactory = _ => OperatingSystem.IsWindows(),
};

var wslFlag = new Option<bool>("--wsl")
{
    Description = "Force bash-compatible output (disables --cmd even on Windows).",
    Validators =
            {
                v =>{},
            },
};

var onceFlag = new Option<bool>("--once", "-1")
{
    Description = "Use only the first matrix combination per job.",
};


var sharedValidator = (CommandResult cmdResult) =>
{
    var useCmd = cmdResult.GetValue(cmdFlag);
    var useWsl = cmdResult.GetValue(wslFlag);

    if (useCmd && useWsl)
    {
        cmdResult.AddError("Options --cmd and --wsl cannot be used together.");
    }
};


var dryCommand = new Command("dry", "Prints run steps for a workflow. [Default]")
{
    cmdFlag,
    wslFlag,
    onceFlag,
    workflowArgument,
};

var runCommand = new Command("run", "Generates a runnable script for a workflow and prints the command to execute it.")
{
    cmdFlag,
    wslFlag,
    onceFlag,
    workflowArgument,
};

var rootCommand = new RootCommand("GitHub Actions workflow tool.")
{
    // works as 'run' by default
    cmdFlag,
    wslFlag,
    onceFlag,
    workflowArgument,

    // sub commands
    dryCommand,
    runCommand,
};

var dryAction = (ParseResult args) =>
{
    var resolvedPath = args.GetRequiredValue(workflowArgument);
    var useCmd = args.GetValue(cmdFlag);
    var useWsl = args.GetValue(wslFlag);
    var onceOnly = args.GetValue(onceFlag);

    var useCmdFormatting = useCmd && !useWsl;

    return ConsoleHelpers.RunWithErrorHandling(() =>
    {
        DryCommand.Run(resolvedPath, useCmdFormatting, onceOnly);
        return 0;
    });
};

var runAction = (ParseResult args) =>
{
    var resolvedPath = args.GetRequiredValue(workflowArgument);
    var useCmd = args.GetValue(cmdFlag);
    var useWsl = args.GetValue(wslFlag);
    var onceOnly = args.GetValue(onceFlag);

    var useCmdFormatting = useCmd && !useWsl;

    return ConsoleHelpers.RunWithErrorHandling(() =>
    {
        return RunCommand.Run(resolvedPath, useCmdFormatting, useWsl, onceOnly);
    });
};

dryCommand.SetAction(dryAction);
runCommand.SetAction(runAction);
rootCommand.SetAction(runAction);

dryCommand.Validators.Add(sharedValidator);
runCommand.Validators.Add(sharedValidator);
rootCommand.Validators.Add(sharedValidator);

return await rootCommand.Parse(args).InvokeAsync();
