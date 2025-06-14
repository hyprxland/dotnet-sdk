﻿using System;

namespace Hyprx.Crypto;

/// <summary>
/// A contract for a cryptographically secure random
/// number generator.
/// </summary>
public interface ICsrng
{
    void GetBytes(byte[] bytes);

    void GetBytes(Span<byte> bytes);

    byte[] NextBytes(int length);

    short NextInt16();

    int NextInt32();

    long NextInt64();
}