// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;

#pragma warning disable IDE1006, SA1025, SA1310, CS8981, SA1300

internal static partial class FFI
{
    /// <summary>Common Unix errno error codes.</summary>
    // These values were defined in src/Native/System.Native/fxerrno.h
    //
    // They compare against values obtained via Interop.Sys.GetLastError() not Marshal.GetLastPInvokeError()
    // which obtains the raw errno that varies between unixes. The strong typing as an enum is meant to
    // prevent confusing the two. Casting to or from int is suspect. Use GetLastErrorInfo() if you need to
    // correlate these to the underlying platform values or obtain the corresponding error message.
    [SuppressMessage("StyleCop.CSharp.SpacingRules", "SA1025:Code should not contain multiple whitespace in a row")]
    internal enum Error
    {
        SUCCESS = 0,

        E2BIG = 0x10001,           // Argument list too long.
        EACCES = 0x10002,           // Permission denied.
        EADDRINUSE = 0x10003,           // Address in use.
        EADDRNOTAVAIL = 0x10004,           // Address not available.
        EAFNOSUPPORT = 0x10005,           // Address family not supported.
        EAGAIN = 0x10006,           // Resource unavailable, try again (same value as EWOULDBLOCK),
        EALREADY = 0x10007,           // Connection already in progress.
        EBADF = 0x10008,           // Bad file descriptor.
        EBADMSG = 0x10009,           // Bad message.
        EBUSY = 0x1000A,           // Device or resource busy.
        ECANCELED = 0x1000B,           // Operation canceled.
        ECHILD = 0x1000C,           // No child processes.
        ECONNABORTED = 0x1000D,           // Connection aborted.
        ECONNREFUSED = 0x1000E,           // Connection refused.
        ECONNRESET = 0x1000F,           // Connection reset.
        EDEADLK = 0x10010,           // Resource deadlock would occur.
        EDESTADDRREQ = 0x10011,           // Destination address required.
        EDOM = 0x10012,           // Mathematics argument out of domain of function.
        EDQUOT = 0x10013,           // Reserved.
        EEXIST = 0x10014,           // File exists.
        EFAULT = 0x10015,           // Bad address.
        EFBIG = 0x10016,           // File too large.
        EHOSTUNREACH = 0x10017,           // Host is unreachable.
        EIDRM = 0x10018,           // Identifier removed.
        EILSEQ = 0x10019,           // Illegal byte sequence.
        EINPROGRESS = 0x1001A,           // Operation in progress.
        EINTR = 0x1001B,           // Interrupted function.
        EINVAL = 0x1001C,           // Invalid argument.
        EIO = 0x1001D,           // I/O error.
        EISCONN = 0x1001E,           // Socket is connected.
        EISDIR = 0x1001F,           // Is a directory.
        ELOOP = 0x10020,           // Too many levels of symbolic links.
        EMFILE = 0x10021,           // File descriptor value too large.
        EMLINK = 0x10022,           // Too many links.
        EMSGSIZE = 0x10023,           // Message too large.
        EMULTIHOP = 0x10024,           // Reserved.
        ENAMETOOLONG = 0x10025,           // Filename too long.
        ENETDOWN = 0x10026,           // Network is down.
        ENETRESET = 0x10027,           // Connection aborted by network.
        ENETUNREACH = 0x10028,           // Network unreachable.
        ENFILE = 0x10029,           // Too many files open in system.
        ENOBUFS = 0x1002A,           // No buffer space available.
        ENODEV = 0x1002C,           // No such device.
        ENOENT = 0x1002D,           // No such file or directory.
        ENOEXEC = 0x1002E,           // Executable file format error.
        ENOLCK = 0x1002F,           // No locks available.
        ENOLINK = 0x10030,           // Reserved.
        ENOMEM = 0x10031,           // Not enough space.
        ENOMSG = 0x10032,           // No message of the desired type.
        ENOPROTOOPT = 0x10033,           // Protocol not available.
        ENOSPC = 0x10034,           // No space left on device.
        ENOSYS = 0x10037,           // Function not supported.
        ENOTCONN = 0x10038,           // The socket is not connected.
        ENOTDIR = 0x10039,           // Not a directory or a symbolic link to a directory.
        ENOTEMPTY = 0x1003A,           // Directory not empty.
        ENOTRECOVERABLE = 0x1003B,           // State not recoverable.
        ENOTSOCK = 0x1003C,           // Not a socket.
        ENOTSUP = 0x1003D,           // Not supported (same value as EOPNOTSUP).
        ENOTTY = 0x1003E,           // Inappropriate I/O control operation.
        ENXIO = 0x1003F,           // No such device or address.
        EOVERFLOW = 0x10040,           // Value too large to be stored in data type.
        EOWNERDEAD = 0x10041,           // Previous owner died.
        EPERM = 0x10042,           // Operation not permitted.
        EPIPE = 0x10043,           // Broken pipe.
        EPROTO = 0x10044,           // Protocol error.
        EPROTONOSUPPORT = 0x10045,           // Protocol not supported.
        EPROTOTYPE = 0x10046,           // Protocol wrong type for socket.
        ERANGE = 0x10047,           // Result too large.
        EROFS = 0x10048,           // Read-only file system.
        ESPIPE = 0x10049,           // Invalid seek.
        ESRCH = 0x1004A,           // No such process.
        ESTALE = 0x1004B,           // Reserved.
        ETIMEDOUT = 0x1004D,           // Connection timed out.
        ETXTBSY = 0x1004E,           // Text file busy.
        EXDEV = 0x1004F,           // Cross-device link.
        ESOCKTNOSUPPORT = 0x1005E,           // Socket type not supported.
        EPFNOSUPPORT = 0x10060,           // Protocol family not supported.
        ESHUTDOWN = 0x1006C,           // Socket shutdown.
        EHOSTDOWN = 0x10070,           // Host is down.
        ENODATA = 0x10071,           // No data available.

        // Custom Error codes to track errors beyond kernel interface.
        EHOSTNOTFOUND = 0x20001,           // Name lookup failed
        ESOCKETERROR = 0x20002,           // Unspecified socket error

        // POSIX permits these to have the same value and we make them always equal so
        // that we do not introduce a dependency on distinguishing between them that
        // would not work on all platforms.
        EOPNOTSUPP = ENOTSUP,            // Operation not supported on socket.
        EWOULDBLOCK = EAGAIN,             // Operation would block.
    }

    // Represents a platform-agnostic Error and underlying platform-specific errno
    internal struct ErrorInfo
    {
        private readonly Error error;
        private int rawErrno;

        internal ErrorInfo(int errno)
        {
            this.error = Sys.ConvertErrorPlatformToPal(errno);
            this.rawErrno = errno;
        }

        internal ErrorInfo(Error error)
        {
            this.error = error;
            this.rawErrno = -1;
        }

        internal Error Error
        {
            get { return this.error; }
        }

        internal int RawErrno
        {
#pragma warning disable S1121
            get { return this.rawErrno == -1 ? (this.rawErrno = Sys.ConvertErrorPalToPlatform(this.error)) : this.rawErrno; }
#pragma warning restore S1121
        }

        public override string ToString()
        {
            return $"RawErrno: {this.RawErrno} Error: {this.Error} GetErrorMessage: {this.GetErrorMessage()}"; // No localization required; text is member names used for debugging purposes
        }

        internal string GetErrorMessage()
        {
            return Sys.StrError(this.RawErrno);
        }
    }

    internal static partial class Sys
    {
        internal static Error GetLastError()
        {
            return ConvertErrorPlatformToPal(Marshal.GetLastPInvokeError());
        }

        internal static ErrorInfo GetLastErrorInfo()
        {
            return new ErrorInfo(Marshal.GetLastPInvokeError());
        }

        internal static unsafe string StrError(int platformErrno)
        {
            const int MaxBufferLength = 1024; // should be long enough for most any UNIX error
            byte* buffer = stackalloc byte[MaxBufferLength];
            byte* message = StrErrorR(platformErrno, buffer, MaxBufferLength);

            if (message == null)
            {
                // This means the buffer was not large enough, but still contains
                // as much of the error message as possible and is guaranteed to
                // be null-terminated. We're not currently resizing/retrying because
                // MaxBufferLength is large enough in practice, but we could do
                // so here in the future if necessary.
                message = buffer;
            }

            return Marshal.PtrToStringUTF8((IntPtr)message) !;
        }

#if SERIAL_PORTS
        [LibraryImport(libs.IOPortsNative, EntryPoint = "SystemIoPortsNative_ConvertErrorPlatformToPal")]
        internal static partial Error ConvertErrorPlatformToPal(int platformErrno);

        [LibraryImport(libs.IOPortsNative, EntryPoint = "SystemIoPortsNative_ConvertErrorPalToPlatform")]
        internal static partial int ConvertErrorPalToPlatform(Error error);

        [LibraryImport(libs.IOPortsNative, EntryPoint = "SystemIoPortsNative_StrErrorR")]
        private static unsafe partial byte* StrErrorR(int platformErrno, byte* buffer, int bufferSize);
#else
        [LibraryImport(Libs.SystemNative, EntryPoint = "SystemNative_ConvertErrorPlatformToPal")]
        [SuppressGCTransition]
        internal static partial Error ConvertErrorPlatformToPal(int platformErrno);

        [LibraryImport(Libs.SystemNative, EntryPoint = "SystemNative_ConvertErrorPalToPlatform")]
        [SuppressGCTransition]
        internal static partial int ConvertErrorPalToPlatform(Error error);

        [LibraryImport(Libs.SystemNative, EntryPoint = "SystemNative_StrErrorR")]
        private static unsafe partial byte* StrErrorR(int platformErrno, byte* buffer, int bufferSize);
#endif
    }
}

// NOTE: extension method can't be nested inside Interop class.
[SuppressMessage("Major Bug", "S3903:Types should be defined in named namespaces")]
internal static class InteropErrorExtensions
{
    // Intended usage is e.g. Interop.Error.EFAIL.Info() for brevity
    // vs. new Interop.ErrorInfo(Interop.Error.EFAIL) for synthesizing
    // errors. Errors originated from the system should be obtained
    // via GetLastErrorInfo(), not GetLastError().Info() as that will
    // convert twice, which is not only inefficient but also lossy if
    // we ever encounter a raw errno that no equivalent in the Error
    // enum.
    public static FFI.ErrorInfo Info(this FFI.Error error)
    {
        return new FFI.ErrorInfo(error);
    }
}