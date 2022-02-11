﻿using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Security;
using System.Text;

namespace ProcessForUWP.Core.Models
{
    public class StartInfo
    {
        public string Arguments { get; set; }
        public bool CreateNoWindow { get; set; }
        public string Domain { get; set; }
        public string FileName { get; set; }
        public bool LoadUserProfile { get; set; }
        public SecureString Password { get; set; }
        public bool RedirectStandardError { get; set; }
        public bool RedirectStandardInput { get; set; }
        public bool RedirectStandardOutput { get; set; }
        public Encoding StandardErrorEncoding { get; set; }
        public Encoding StandardOutputEncoding { get; set; }
        public string UserName { get; set; }
        public bool UseShellExecute { get; set; }
        public string WorkingDirectory { get; set; }
        public bool ErrorDialog { get; set; }
        public IntPtr ErrorDialogParentHandle { get; set; }
        public string Verb { get; set; }
        public ProcessWindowStyle WindowStyle { get; set; }
        public string PasswordInClearText { get; set; }

        public StartInfo(ProcessStartInfo info = null)
        {
            if(info!=null)
            {
                SetStartInfo(info);
            }
        }

        public void SetStartInfo(ProcessStartInfo info)
        {
            if (info != null)
            {
                Arguments = info.Arguments;
                CreateNoWindow = info.CreateNoWindow;
                Domain = info.Domain;
                FileName = info.FileName;
                LoadUserProfile = info.LoadUserProfile;
                Password = info.Password;
                RedirectStandardError = info.RedirectStandardError;
                RedirectStandardInput = info.RedirectStandardInput;
                RedirectStandardOutput = info.RedirectStandardOutput;
                StandardErrorEncoding = info.StandardErrorEncoding;
                StandardOutputEncoding = info.StandardOutputEncoding;
                UserName = info.UserName;
                UseShellExecute = info.UseShellExecute;
                WorkingDirectory = info.WorkingDirectory;
                ErrorDialog = info.ErrorDialog;
                ErrorDialogParentHandle = info.ErrorDialogParentHandle;
                Verb = info.Verb;
                WindowStyle = info.WindowStyle;
                PasswordInClearText = info.PasswordInClearText;
            }
        }

        public ProcessStartInfo GetStartInfo()
        {
            return new ProcessStartInfo
            {
                Arguments = Arguments,
                CreateNoWindow = CreateNoWindow,
                Domain = Domain,
                FileName = FileName,
                LoadUserProfile = LoadUserProfile,
                Password = Password,
                RedirectStandardError = RedirectStandardError,
                RedirectStandardInput = RedirectStandardInput,
                RedirectStandardOutput = RedirectStandardOutput,
                StandardErrorEncoding = StandardErrorEncoding,
                StandardOutputEncoding = StandardOutputEncoding,
                UserName = UserName,
                UseShellExecute = UseShellExecute,
                WorkingDirectory = WorkingDirectory,
                ErrorDialog = ErrorDialog,
                ErrorDialogParentHandle = ErrorDialogParentHandle,
                Verb = Verb,
                WindowStyle = WindowStyle,
                PasswordInClearText = PasswordInClearText
            };
        }
    }
}
