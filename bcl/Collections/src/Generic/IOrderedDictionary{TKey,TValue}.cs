namespace Hyprx.Collections.Generic;

#if HYPRX_PUBLIC
public
#else
internal
#endif
interface IOrderedDictionary<TKey, TValue> : IDictionary<TKey, TValue>
{
    void Insert(int index, TKey key, TValue value);
}