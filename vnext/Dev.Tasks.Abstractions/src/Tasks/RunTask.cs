using Hyprx.Dev.Collections;
using Hyprx.Results;

namespace Hyprx.Dev.Tasks;

public delegate Task<Result<Outputs>> RunTaskAsync(TaskContext context, CancellationToken cancellationToken = default);