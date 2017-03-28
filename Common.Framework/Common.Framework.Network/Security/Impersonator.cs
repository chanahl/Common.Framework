// <copyright file="Impersonator.cs">
//     Copyright (c) 2017 All rights reserved.
// </copyright>
// <clrversion>4.0.30319.42000</clrversion>
// <author>Alex H.-L. Chan</author>

using System;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Security.Principal;

namespace Common.Framework.Network.Security
{
    public class Impersonator : IDisposable
    {
        private WindowsImpersonationContext _wic;

        public Impersonator()
        {
        }

        public Impersonator(
            string userName,
            string domainName,
            string password)
        {
            Impersonate(
                userName,
                domainName,
                password,
                LogonType.Logon32LogonInteractive,
                LogonProvider.Logon32ProviderDefault);
        }

        public Impersonator(
            string userName,
            string domainName,
            string password,
            LogonType logonType,
            LogonProvider logonProvider)
        {
            Impersonate(
                userName,
                domainName,
                password,
                logonType,
                logonProvider);
        }

        public void Impersonate(
            string userName,
            string domainName,
            string password)
        {
            Impersonate(
                userName,
                domainName,
                password,
                LogonType.Logon32LogonInteractive,
                LogonProvider.Logon32ProviderDefault);
        }

        public void Impersonate(
            string userName,
            string domainName,
            string password,
            LogonType logonType,
            LogonProvider logonProvider)
        {
            UndoImpersonation();

            var logonToken = IntPtr.Zero;
            var logonTokenDuplicate = IntPtr.Zero;
            try
            {
                // revert to the application pool identity, saving the identity of the current requestor
                _wic = WindowsIdentity.Impersonate(IntPtr.Zero);

                if (Win32NativeMethods.LogonUser(
                    userName,
                    domainName,
                    password,
                    (int)logonType,
                    (int)logonProvider,
                    ref logonToken) != 0)
                {
                    if (Win32NativeMethods.DuplicateToken(
                        logonToken, (int)ImpersonationLevel.SecurityImpersonation, ref logonTokenDuplicate) != 0)
                    {
                        var wi = new WindowsIdentity(logonTokenDuplicate);
                        wi.Impersonate();

                        // discard the returned identity context (which is the context of the application pool)
                    }
                    else
                    {
                        throw new Win32Exception(Marshal.GetLastWin32Error());
                    }
                }
                else
                {
                    throw new Win32Exception(Marshal.GetLastWin32Error());
                }
            }
            finally
            {
                if (logonToken != IntPtr.Zero)
                {
                    Win32NativeMethods.CloseHandle(logonToken);
                }

                if (logonTokenDuplicate != IntPtr.Zero)
                {
                    Win32NativeMethods.CloseHandle(logonTokenDuplicate);
                }
            }
        }

        public void Dispose()
        {
            UndoImpersonation();
        }

        private void UndoImpersonation()
        {
            if (_wic != null)
            {
                _wic.Undo();
            }

            _wic = null;
        }
    }
}