#pragma warning disable SA1117, SA1500, SA1116
using Hyprx;

using static Hyprx.RexConsole;
using static Hyprx.Shell;

Task("default", () => Echo("Hello, World!"));

Task("hello", () => Echo("Hello from RexKitchen!"));

Task("env", () => Echo(Env.Expand("$MY_VALUE")));

Deployment("my-deployment",
    (ctx) =>
    {
        Echo("Starting deployment...");
        Echo("Deployment finished.");
    })
    .BeforeDeploy((before) =>
    {
        before.Task("hello", () => Echo("Preparing to say hello..."));
    });

return await RunTasksAsync(args);