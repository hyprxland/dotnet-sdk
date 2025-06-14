namespace Hyprx.Dev.Deployments;

public static class DeploymentEventNames
{
    public const string BeforeDeployment = "before:deploy";
    public const string AfterDeployment = "after:deploy";

    public const string BeforeDestroy = "before:destroy";

    public const string AfterDestroy = "after:destroy";

    public const string BeforeRollback = "before:rollback";

    public const string AfterRollback = "after:rollback";
}