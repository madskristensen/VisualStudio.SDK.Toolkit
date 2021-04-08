﻿using System;
using System.Runtime.InteropServices;
using System.Threading;
using Community.VisualStudio.Toolkit;
using Microsoft;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Imaging;
using Microsoft.VisualStudio.Imaging.Interop;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using TestExtension;
using Task = System.Threading.Tasks.Task;

namespace VSSDK.TestExtension
{
    [PackageRegistration(UseManagedResourcesOnly = true, AllowsBackgroundLoading = true)]
    [Guid("05271709-8845-42fb-9d10-621cc8cffc5d")]
    [ProvideBindingPath]
    [ProvideOptionPage(typeof(OptionsProvider.GeneralOptions), nameof(TestExtension), "General", 0, 0, true)]
    [ProvideProfile(typeof(OptionsProvider.GeneralOptions), nameof(TestExtension), "General", 0, 0, true)]
    [ProvideToolWindow(typeof(RunnerWindow.Pane), Style = VsDockStyle.Float, Window = ToolWindowGuids.SolutionExplorer)]
    [ProvideToolWindowVisibility(typeof(RunnerWindow.Pane), VSConstants.UICONTEXT.NoSolution_string)]
    [ProvideToolWindow(typeof(ThemeWindow.Pane))]
    [ProvideToolWindow(typeof(MultiInstanceWindow.Pane), MultiInstances = true)]
    [ProvideMenuResource("Menus.ctmenu", 1)]
    public sealed class TestExtensionPackage : ToolkitPackage
    {
        protected override async Task InitializeAsync(CancellationToken cancellationToken, IProgress<ServiceProgressData> progress)
        {
            RunnerWindow.Initialize(this);
            ThemeWindow.Initialize(this);
            MultiInstanceWindow.Initialize(this);

            await JoinableTaskFactory.SwitchToMainThreadAsync(cancellationToken);
            await TestCommand.InitializeAsync(this);
            await RunnerWindowCommand.InitializeAsync(this);
            await ThemeWindowCommand.InitializeAsync(this);
            await MultiInstanceWindowCommand.InitializeAsync(this);

            System.Windows.Media.Imaging.BitmapSource bitmap = await KnownMonikers.Reference.ToBitmapSourceAsync(16);
            var svc = (IVsImageService2)await VS.Shell.GetImageServiceAsync();
            Microsoft.VisualStudio.ComponentModelHost.IComponentModel2 test = await VS.Shell.GetComponentModelAsync();
            Assumes.Present(bitmap);
            Assumes.Present(svc);
            Assumes.Present(test);
        }
    }
}
