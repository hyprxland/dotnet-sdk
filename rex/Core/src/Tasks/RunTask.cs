using Hyprx.Results;
using Hyprx.Rex.Collections;

namespace Hyprx.Rex.Tasks;

public delegate Task<Result<Outputs>> RunTaskAsync(TaskContext context, CancellationToken cancellationToken = default);