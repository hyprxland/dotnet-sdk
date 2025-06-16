using Hyprx.Rex.Deployments;
using Hyprx.Rex.Jobs;
using Hyprx.Rex.Tasks;

namespace Hyprx.Rex.Messaging;

public class ConsoleSink : IMessageSink
{
    public Task<bool> ReceiveAsync(IMessage message)
    {
        switch (message)
        {
            case DeployStarted deployStarted:
                Console.WriteLine($">> Deployment {deployStarted.Data.Name} started");
                return Task.FromResult(false);

            case DeployCompleted deployCompleted:
                Console.WriteLine($">> Deployment {deployCompleted.Data.Name} completed");
                return Task.FromResult(false);

            case DeployFailed deployFailed:
                Console.WriteLine($">> Deployment {deployFailed.Data.Name} failed");
                return Task.FromResult(false);

            case DeployCancelled deployCancelled:
                Console.WriteLine($">> Deployment {deployCancelled.Data.Name} cancelled");
                return Task.FromResult(false);

            case DeploySkipped deploySkipped:
                Console.WriteLine($">> Deployment {deploySkipped.Data.Name} skipped");
                return Task.FromResult(false);

            case JobStarted jobStarted:
                Console.WriteLine($">> Job {jobStarted.Data.Name} started");
                return Task.FromResult(false);

            case JobCompleted jobCompleted:
                Console.WriteLine($">> Job {jobCompleted.Data.Name} completed");
                return Task.FromResult(false);

            case JobFailed jobFailed:
                Console.WriteLine($">> Job {jobFailed.Data.Name} failed");
                return Task.FromResult(false);

            case JobCancelled jobCancelled:
                Console.WriteLine($">> Job {jobCancelled.Data.Name} cancelled");
                return Task.FromResult(false);

            case JobSkipped jobSkipped:
                Console.WriteLine($">> Job {jobSkipped.Data.Name} skipped");
                return Task.FromResult(false);

            case JobFoundMissingDependencies jobFoundMissingDependencies:
                {
                    Console.WriteLine($">>  Found missing dependencies in jobs:");
                    foreach (var (job, missing) in jobFoundMissingDependencies.Tasks)
                    {
                        Console.WriteLine($">>    {job.Name} is missing: {string.Join(", ", missing)}");
                    }

                    return Task.FromResult(false);
                }

            case JobFoundCyclycalReferences jobsFoundCyclycalReferences:
                {
                    Console.WriteLine($">>  Found cyclycal references in jobs:");
                    foreach (var job in jobsFoundCyclycalReferences.Job)
                    {
                        Console.WriteLine($">>    {job.Name}");
                    }

                    return Task.FromResult(false);
                }

            case TaskCompleted taskCompleted:
                Console.WriteLine($">>  Task {taskCompleted.Data.Name} completed");
                return Task.FromResult(false);

            case TaskSkipped taskSkipped:
                Console.WriteLine($">>  Task {taskSkipped.Data.Name} skipped");
                return Task.FromResult(false);

            case TaskCancelled taskCancelled:
                Console.WriteLine($">>  Task {taskCancelled.Data.Name} cancelled");
                return Task.FromResult(false);

            case TaskFailed taskFailed:
                Console.WriteLine($">>  Task {taskFailed.Data.Name} failed");
                return Task.FromResult(false);

            case TaskStarted taskStarted:
                Console.WriteLine($">>  Task {taskStarted.Data.Name} started");
                return Task.FromResult(false);

            case TasksFoundCyclycalReferences tasksFoundCyclycalReferences:
                {
                    Console.WriteLine($">>  Found cyclycal references in tasks:");
                    foreach (var task in tasksFoundCyclycalReferences.Tasks)
                    {
                        Console.WriteLine($">>    {task.Name}");
                    }

                    return Task.FromResult(false);
                }

            case TasksFoundMissingDependencies tasksFoundMissingDependencies:
                {
                    Console.WriteLine($">>  Found missing dependencies in tasks:");
                    foreach (var (task, missing) in tasksFoundMissingDependencies.Tasks)
                    {
                        Console.WriteLine($">>    {task.Name} is missing: {string.Join(", ", missing)}");
                    }

                    return Task.FromResult(false);
                }

            case DiagnosticMessage diagnosticMessage:
                {
                    Console.WriteLine($">>  [{diagnosticMessage.Level}] {diagnosticMessage.Message}");
                    return Task.FromResult(false);
                }

            default:
                return Task.FromResult(false);
        }
    }
}