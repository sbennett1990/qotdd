/*
 * Copyright (c) 2016 Calvin Buckley <calvin@openmailbox.org>
 *
 * Permission to use, copy, modify, and distribute this software for any
 * purpose with or without fee is hereby granted, provided that the above
 * copyright notice and this permission notice appear in all copies.
 *
 * THE SOFTWARE IS PROVIDED "AS IS" AND THE AUTHOR DISCLAIMS ALL WARRANTIES
 * WITH REGARD TO THIS SOFTWARE INCLUDING ALL IMPLIED WARRANTIES OF
 * MERCHANTABILITY AND FITNESS. IN NO EVENT SHALL THE AUTHOR BE LIABLE FOR
 * ANY SPECIAL, DIRECT, INDIRECT, OR CONSEQUENTIAL DAMAGES OR ANY DAMAGES
 * WHATSOEVER RESULTING FROM LOSS OF USE, DATA OR PROFITS, WHETHER IN AN
 * ACTION OF CONTRACT, NEGLIGENCE OR OTHER TORTIOUS ACTION, ARISING OUT OF
 * OR IN CONNECTION WITH THE USE OR PERFORMANCE OF THIS SOFTWARE.
 */

using System;
using System.Runtime.InteropServices;
using System.ComponentModel;
using Mono.Unix.Native;

namespace OpenBSD.Unistd
{
    /// <summary>
    /// Uses OpenBSD's pledge(2) syscall to restrict system operations of the
    /// process.
    /// </summary>
    /// <remarks>
    /// Consult the OpenBSD manual page:
    /// https://man.openbsd.org/pledge
    /// </remarks>
    public static class Pledge
    {
        private const int MAJOR = 6;
        private const int MINOR = 6;

        [DllImport("libc.so", SetLastError = true, CharSet = CharSet.Auto)]
        private static extern int pledge(string promises, string execpromises);

        /// <summary>
        /// Whether the process has been pledged.
        /// </summary>
        public static bool IsPledged
        {
            get; private set;
        }

        /// <summary>
        /// Current privleges the process has dropped to.
        /// </summary>
        public static string Promises
        {
            get; private set;
        }

        /// <summary>
        /// Current execpromises the process has specified.
        /// </summary>
        public static string ExecPromises
        {
            get; private set;
        }

        /// <summary>
        /// Use OpenBSD's pledge(2) syscall to force the current process into a
        /// restricted-service operating mode. Subsequent calls to Init() can
        /// reduce the abilities further, but abilities can never be regained.
        /// </summary>
        /// <param name="promises">
        /// Space-separated list of promises to drop to
        /// </param>
        /// <param name="execpromises">
        /// Space-separated list of promises that a child process will begin
        /// life with (the use of this parameter is not currently encouraged)
        /// </param>
        /// <exception cref="PlatformNotSupportedException">
        /// Thrown if the the current OS isn't OpenBSD or the version is too old.
        /// </exception>
        /// <exception cref="Win32Exception">
        /// Thrown if pledge(2) returns an error.
        /// </exception>
        public static void Init(string promises, string execpromises = null)
        {
            // check for supported version of OpenBSD
            if (!IsOpenBSD()
                || Environment.OSVersion.Version < new Version(MAJOR, MINOR))
            {
                throw new PlatformNotSupportedException
                    ($"this pledge(2) ffi only supports OpenBSD {MAJOR}.{MINOR} or later");
            }

            if (pledge(promises, execpromises) == -1)
            {
                Errno e = (Errno)Marshal.GetLastWin32Error();
                switch (e)
                {
                    case Errno.EFAULT:
                        throw new Win32Exception((int)e,
                            "promises or execpromises points outside the process's allocated address space.");
                    case Errno.EINVAL:
                        throw new Win32Exception((int)e,
                            "promises is malformed or contains invalid keywords.");
                    case Errno.EPERM:
                        throw new Win32Exception((int)e,
                            "This process is attempting to increase permissions.");
                    case Errno.ENAMETOOLONG:
                        throw new Win32Exception((int)e,
                            "promises or execpromises string is too long.");
                    default:
                        throw new Win32Exception((int)e,
                            "The system returned an unknown error.");
                }
            }
            else
            {
                IsPledged = true;
                Promises = promises;
                ExecPromises = execpromises;
            }
        }

        private static bool IsOpenBSD()
        {
            if (Environment.OSVersion.Platform != PlatformID.Unix)
            {
                return false;
            }
            Utsname uname;
            Syscall.uname(out uname);
            return uname.sysname == "OpenBSD";
        }
    }
}
