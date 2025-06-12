using System.Runtime.InteropServices;

#pragma warning disable CS0649
#pragma warning disable S2344
#pragma warning disable S2346
#pragma warning disable SA1025
#pragma warning disable SA1310
#pragma warning disable S3903 // Types should be defined in named namespaces

internal static partial class FFI
{
    internal static partial class Libc
    {
        [LibraryImport("libc", StringMarshalling = StringMarshalling.Utf8, EntryPoint = "getgrgid_r", SetLastError = false)]
        internal static unsafe partial int GetGrGidR(uint uid, out Group group, byte* buf, int bufLen, out IntPtr groupPtr);

        [LibraryImport("libc", StringMarshalling = StringMarshalling.Utf8, EntryPoint = "getgrnam_r", SetLastError = false)]
        internal static unsafe partial int GetGrNamR(string name, out Group group, byte* buf, int bufLen, out IntPtr groupPtr);

        internal static unsafe uint? GetGroupId(string name)
        {
            var size = 512;
            var stackBuf = stackalloc byte[size];
            Group group;
            IntPtr groupPtr;
            int result = GetGrNamR(name, out group, stackBuf, size, out groupPtr);
            if (result == 0)
            {
                if (groupPtr == IntPtr.Zero)
                {
                    // No such group
                    return null;
                }

                return group.GroupId;
            }

            while (true)
            {
                size *= 2;
                var buf = Marshal.AllocHGlobal(size);
                try
                {
                    result = GetGrNamR(name, out group, (byte*)buf, size, out groupPtr);
                    if (result == 0)
                    {
                        if (groupPtr == IntPtr.Zero)
                        {
                            // No such group
                            return null;
                        }

                        return group.GroupId;
                    }

                    if (result == 0x10047)
                    {
                        continue;
                    }

                    return null; // EINVAL or other error
                }
                finally
                {
                    Marshal.FreeHGlobal(buf);
                }
            }
        }

        internal static unsafe string? GetGroupName(uint groupId)
        {
            var size = 512;
            var stackBuf = stackalloc byte[size];
            Group group;
            IntPtr groupPtr;
            int result = GetGrGidR(groupId, out group, stackBuf, size, out groupPtr);
            if (result == 0)
            {
                if (groupPtr == IntPtr.Zero)
                {
                    // No such group
                    return null;
                }

#if NET7_0_OR_GREATER
                return Marshal.PtrToStringUTF8((IntPtr)group.Name);
#else
                return Marshal.PtrToStringAnsi((IntPtr)group.Name);
#endif
            }

            while (true)
            {
                size *= 2;
                var buf = Marshal.AllocHGlobal(size);
                try
                {
                    result = GetGrGidR(groupId, out group, (byte*)buf, size, out groupPtr);
                    if (result == 0)
                    {
                        if (groupPtr == IntPtr.Zero)
                        {
                            // No such group
                            return null;
                        }

#if NET7_0_OR_GREATER
                        return Marshal.PtrToStringUTF8((IntPtr)group.Name);
#else
                        return Marshal.PtrToStringAnsi((IntPtr)group.Name);
#endif
                    }

                    if (result == 0x10047)
                    {
                        continue;
                    }

                    return null; // EINVAL or other error
                }
                finally
                {
                    Marshal.FreeHGlobal(buf);
                }
            }
        }

        internal unsafe struct Group
        {
            internal const int InitialBufferSize = 256;

            internal byte* Name;
            internal byte* Password;
            internal uint GroupId;

            internal IntPtr Members; // This is an array of byte pointers, representing group members
        }
    }
}