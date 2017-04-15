// <copyright file="IconEventHandler.cs">
//     Copyright (c) 2017 All rights reserved.
// </copyright>
// <clrversion>4.0.30319.42000</clrversion>
// <author>Alex H.-L. Chan</author>

using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Common.Automation.Bots.Icon
{
    public class IconEventHandler
    {
        private static NotifyIcon _notifyIcon;
        private static IntPtr _processHandle;
        private static IntPtr _winShell;
        private static IntPtr _winDesktop;
        private static MenuItem _hideMenuItem;
        private static MenuItem _showMenuItem;

        public IconEventHandler(string text)
        {
            _notifyIcon = new NotifyIcon
            {
                Icon = new System.Drawing.Icon(@"Icon\FCBarcelona1415.ico"),
                Text = text,
                Visible = true
            };

            var menu = new ContextMenu();

            _hideMenuItem = new MenuItem("Hide", Hide_Click);
            menu.MenuItems.Add(_hideMenuItem);

            _showMenuItem = new MenuItem("Show", Show_Click);
            menu.MenuItems.Add(_showMenuItem);

            var exitMenuItem = new MenuItem("Exit", Exit_Click);
            menu.MenuItems.Add(exitMenuItem);

            _notifyIcon.ContextMenu = menu;
        }

        public IconEventHandler(
            string text,
            string icon)
        {
            _notifyIcon = new NotifyIcon
            {
                Icon = new System.Drawing.Icon(icon),
                Text = text,
                Visible = true
            };

            var menu = new ContextMenu();

            _hideMenuItem = new MenuItem("Hide", Hide_Click);
            menu.MenuItems.Add(_hideMenuItem);

            _showMenuItem = new MenuItem("Show", Show_Click);
            menu.MenuItems.Add(_showMenuItem);

            var exitMenuItem = new MenuItem("Exit", Exit_Click);
            menu.MenuItems.Add(exitMenuItem);

            _notifyIcon.ContextMenu = menu;
        }

        [DllImport("user32.dll", SetLastError = true)]
        public static extern IntPtr SetParent(IntPtr child, IntPtr parent);

        [DllImport("user32.dll")]
        public static extern IntPtr GetShellWindow();

        [DllImport("user32.dll")]
        public static extern IntPtr GetDesktopWindow();

        public void Run(Action action)
        {
            // spin off your actual work in a different thread so that the Notify Icon works correctly
            TaskFactoryStartNew(action);

            _processHandle = Process.GetCurrentProcess().MainWindowHandle;

            _winShell = GetShellWindow();

            _winDesktop = GetDesktopWindow();

            // hide the window
            ResizeWindow(false);

            // this is required for triggering WinForms activity in console application
            Application.Run();
        }

        private static void Exit_Click(
            object sender,
            EventArgs e)
        {
            _notifyIcon.Visible = false;
            Application.Exit();
            Environment.Exit(1);
        }

        private static void Hide_Click(
            object sender,
            EventArgs e)
        {
            ResizeWindow(false);
        }

        private static void Show_Click(
            object sender,
            EventArgs e)
        {
            ResizeWindow();
        }

        private static void ResizeWindow(bool restore = true)
        {
            if (restore)
            {
                _hideMenuItem.Enabled = true;
                _showMenuItem.Enabled = false;
                SetParent(_processHandle, _winDesktop);
            }
            else
            {
                _hideMenuItem.Enabled = false;
                _showMenuItem.Enabled = true;
                SetParent(_processHandle, _winShell);
            }
        }

        private static void TaskFactoryStartNew(Action action)
        {
            Task.Factory.StartNew(action);
        }
    }
}