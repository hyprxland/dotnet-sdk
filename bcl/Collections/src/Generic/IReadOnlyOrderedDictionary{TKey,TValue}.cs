namespace Hyprx.Collections.Generic;

#if HYPRX_PUBLIC
public
#else
internal
#endif
interface IReadOnlyOrderedDictionary<TKey, TValue> : IReadOnlyDictionary<TKey, TValue>
{
}