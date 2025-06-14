using System;

namespace Hyprx.Crypto;

[CLSCompliant(false)]
public interface IUnsignedCsrng
{
    ushort NextUInt16();

    uint NextUInt32();

    long NextUInt64();
}