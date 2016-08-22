using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration.Install;
using System.ServiceProcess;

namespace AutoBackupServices
{
    [RunInstaller(true)]
    public partial class ProjectInstaller : System.Configuration.Install.Installer
    {
        public ProjectInstaller()
        {
            InitializeComponent();

            serviceInstaller1.StartType = ServiceStartMode.Automatic;
            serviceInstaller1.AfterInstall += new InstallEventHandler(AfterInstallEventHandler);
            serviceInstaller1.BeforeUninstall += new InstallEventHandler(BeforeUninstallEventHandler);
        }

        private void AfterInstallEventHandler(Object src, InstallEventArgs args)
        {
            ServiceController service = new ServiceController(SERVICE_NAME);
            if (service.Status != ServiceControllerStatus.Running &&
                service.Status != ServiceControllerStatus.StartPending)
            {
                service.Start();
            }
        }

        private void BeforeUninstallEventHandler(Object src, InstallEventArgs args)
        {
            ServiceController service = new ServiceController(SERVICE_NAME);
            if (service.Status != ServiceControllerStatus.Stopped &&
                service.Status != ServiceControllerStatus.StopPending)
            {
                service.Stop();
            }
        }
    }
}
