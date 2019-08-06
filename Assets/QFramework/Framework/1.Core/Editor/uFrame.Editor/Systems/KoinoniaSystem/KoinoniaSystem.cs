using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using QF.GraphDesigner;
//using ICSharpCode.SharpZipLib.Zip;
using QF.GraphDesigner.Unity.KoinoniaSystem.Drawers;
using QF.GraphDesigner.Unity.KoinoniaSystem.Extensions;
using QF.GraphDesigner.Unity.WindowsPlugin;
using Invert.Data;
using QF.GraphDesigner.Unity.KoinoniaSystem.Classes;
using QF.GraphDesigner.Unity.KoinoniaSystem.Commands;
using QF.GraphDesigner.Unity.KoinoniaSystem.Data;
using QF.GraphDesigner.Unity.KoinoniaSystem.Events;
using QF.GraphDesigner.Unity.KoinoniaSystem.Services;
using QF.GraphDesigner.Unity.KoinoniaSystem.ViewModels;
using QF;
using UnityEditor;
using UnityEngine;
using MessageType = QF.GraphDesigner.Unity.WindowsPlugin.MessageType;

namespace QF.GraphDesigner.Unity.KoinoniaSystem
{

    public class KoinoniaSystem : DiagramPlugin, 
        IExecuteCommand<LoginCommand>, 
        IExecuteCommand<QueueRevisionForInstallCommand>,
        IExecuteCommand<QueueRevisionForUninstallCommand>,
        IExecuteCommand<PingServerCommand>,
        IExecuteCommand<RefreshFrontPagePackagesCommand>,
        IExecuteCommand<RunQueuedOperationsCommand>,
        IExecuteCommand<SelectPackageCommand>
    {

        #region Fields
        private List<UFramePackagePreviewDescriptor> _previews;
        private List<UFramePackageDescriptor> _packages;
        private Dictionary<string, List<UFramePackageRevisionDescriptor>> _revisions;
        private IRepository _settings;
        private KoinoniaSettings _koinoniaSettings;
        private AuthorizationState _authorizationState;
        private static List<UFramePackage> _installedPackages;
        private IDesctiptorsService _descriptorsService;
        private List<UFramePackageDescriptor> _frontPagePackages;
        private List<UFramePackageDescriptor> _installedPackagesDescriptors;
        private List<UFramePackageRevisionDescriptor> _selectedPackageRevisions;
        public string ApplicationPath { get; set; }

        #endregion

        #region Properties

        public override bool Enabled
        {
            get { return false; }
        }

        public override decimal LoadPriority
        {
            get { return 9999; }
        }

        public UFramePackageDescriptor SelectedPackage { get; set; }

        public List<UFramePackageRevisionDescriptor> SelectedPackageRevisions
        {
            get { return _selectedPackageRevisions ?? (_selectedPackageRevisions = new List<UFramePackageRevisionDescriptor>()); }
            set { _selectedPackageRevisions = value; }
        }

        public IRepository Settings
        {
            get { return _settings ?? (_settings = InvertApplication.Container.Resolve<IRepository>("Settings")); }
            set { _settings = value; }
        }

        public KoinoniaSettings KoinoniaSettings
        {
            get { return _koinoniaSettings ?? (_koinoniaSettings = Settings.All<KoinoniaSettings>().FirstOrDefault() ?? Settings.Create<KoinoniaSettings>()); }
            set { _koinoniaSettings = value; }
        }       
        
        public IDesctiptorsService DescriptorsService
        {
            get { return _descriptorsService ?? (_descriptorsService = InvertApplication.Container.Resolve<IDesctiptorsService>()); }
            set { _descriptorsService = value; }
        }

        public bool IsRemoteServerAvailable { get; set; }

        public AuthorizationState AuthorizationState
        {
            get
            {
                if (string.IsNullOrEmpty(KoinoniaSettings.AccessToken) ||
                    (!string.IsNullOrEmpty(KoinoniaSettings.AccessToken) &&
                     DateTime.Now > KoinoniaSettings.AccessTokenExpirationDate))
                {
                    return AuthorizationState.Unauthorized;
                }
                else
                {
                    return AuthorizationState.LoggedIn;
                }

            }
            set { }
        }

        public string GlobalProgressMessage { get; set; }

        public List<UFramePackageDescriptor> FrontPagePackages
        {
            get { return _frontPagePackages; }
            set { _frontPagePackages = value; }
        }

        public List<UFramePackageDescriptor> InstalledPackagesDescriptors
        {
            get { return _installedPackagesDescriptors ?? (_installedPackagesDescriptors = new List<UFramePackageDescriptor>()); }
            set { _installedPackagesDescriptors = value; }
        }

        #endregion

        public override void Initialize(QFrameworkContainer container)
        {
            base.Initialize(container);
            var typeDatabase = new TypeDatabase(new JsonRepositoryFactory(Path.Combine(Application.dataPath, "../uFrame")));
            container.RegisterInstance<IRepository>(typeDatabase, "Settings");
            container.RegisterInstance<IDesctiptorsService>(new DescriptorsService(typeDatabase));
        }
        public override void Loaded(QFrameworkContainer container)
        {
            base.Loaded(container);
            UpdateAvailability();
            ApplicationPath = Application.dataPath;
        }

        public void UpdateAvailability()
        {
            InvertApplication.ExecuteInBackground(new PingServerCommand()
            {
            //    Server = "http://invertgamestudios.com"
                Server = "http://google.com"
            });

        }
        public void DownloadAndExtractPackage(UFramePackageDescriptor package, UFramePackageRevisionDescriptor revision)
        {
            GlobalProgressMessage = string.Format("Downloading {0} {1}...", package.Title, revision.VersionTag);

            WWW req = null;
            string dataPath = null;
            byte[] bytes = null;

            var d = ThreadingUtils.WaitOnMainThread(() =>
            {

                //Pre
                if (req == null)
                {
                    dataPath = Application.dataPath;
                    //var snapshotUri = command.RevisionDescriptor.SnapshotUri;
                    req = new WWW(revision.SnapshotUri);
                }

                if (req.isDone)
                {
                    //Post
                    bytes = req.bytes;
                    //GlobalProgressMessage = string.Format("Extracting {0} {1}...", command.PackageDescriptor.Title, command.RevisionDescriptor.VersionTag);
                    return true;
                }

                return false;
            });

            while (!d.Done) { Thread.Sleep(100); }

            GlobalProgressMessage = string.Format("Extracting {0} {1}...", package.Title, revision.VersionTag);

            var stream = new MemoryStream(bytes);
            var packagesPath = Path.Combine(dataPath, "uFramePackages");

            EnsurePath(packagesPath);

            var packagePath = Path.Combine(packagesPath, package.Id);
            
            EnsurePath(packagePath);
            ExtractZipFile(stream, packagePath);

        }
        public void DeletePackageFiles(UFramePackage package)
        {
            var packagePath = Path.Combine(Path.Combine(ApplicationPath, "uFramePackages"), package.Id);
            ThreadingUtils.DispatchOnMainThread(() =>
            {
                FileUtil.DeleteFileOrDirectory(packagePath);
            });
        }

        public void Execute(LoginCommand command)
        {
            AuthorizationState = AuthorizationState.InProgress;
            GlobalProgressMessage = "Logging in...";

            Thread.Sleep(2000);

            KoinoniaSettings.AccessToken = "12345";
            KoinoniaSettings.AccessTokenExpirationDate = DateTime.Now.AddHours(2);

            Settings.Commit();

            GlobalProgressMessage = null;

            InvertApplication.SignalEvent<ILoggedInEvent>(_ => _.LoggedIn());
        }
        public void Execute(QueueRevisionForInstallCommand command)
        {
            KoinoniaSettings.PackagesToInstall.Add(command.RevisionDescriptor.Id);
            Settings.Commit();
        }
        public void Execute(QueueRevisionForUninstallCommand command)
        {
            KoinoniaSettings.PackagesToUninstall.Add(command.Package.Id);
            Settings.Commit();
        }
        public void Execute(PingServerCommand command)
        {

            WWW ping = null;
            DateTime timeOut = DateTime.Now.AddSeconds(15);
            GlobalProgressMessage = "Contacting Koinonia Servers....";
            
            var d = ThreadingUtils.WaitOnMainThread(() =>
            {
                ping = ping ?? new WWW(command.Server);
                return ping.isDone;
            });

            while (!d.Done)
            {
                if (DateTime.Now > timeOut) //time outs
                {
                    ThreadingUtils.DispatchOnMainThread(() =>
                    {
                        IsRemoteServerAvailable = string.IsNullOrEmpty(ping.error);
                        GlobalProgressMessage = null;
                        ping.Dispose();
                    });
                    return;
                }
                else
                {
                    Thread.Sleep(100);
                }
            }

            ThreadingUtils.DispatchOnMainThread(()=>
            {
                IsRemoteServerAvailable = string.IsNullOrEmpty(ping.error);
                GlobalProgressMessage = null;
                ping.Dispose();
            });

        }
      
        public void Execute(RunQueuedOperationsCommand command)
        {

            if (AuthorizationState != AuthorizationState.LoggedIn)
            {
                return;
            }

            if (KoinoniaSettings.PackagesToUninstall.Any())
            {
                ServePackagesToUninstall();
                return;
            }

            if (KoinoniaSettings.PackagesToInstall.Any())
            {
                ServePackagesToInstall();
                return;
            }

        }
        private void ServePackagesToInstall()
        {
            GlobalProgressMessage = string.Format("Installing packages...");

            var revisionsToInstall = KoinoniaSettings.PackagesToInstall;

            foreach (var id in revisionsToInstall)
            {
                var revision = DescriptorsService.GetRevisionById(id);
                var package = DescriptorsService.GetPackageDescriptorByRevision(revision);
                DownloadAndExtractPackage(package, revision);
            }


        }
        private void ServePackagesToUninstall()
        {
            GlobalProgressMessage = string.Format("Removing packages...");

            var packagesToUnisntall = KoinoniaSettings.PackagesToUninstall
                .Where(id => InstalledPackages.FirstOrDefault(p => p.Id == id) != null)
                .Select(id => InstalledPackages.FirstOrDefault(p => p.Id == id)).ToArray();

            foreach (var package in packagesToUnisntall)
            {
                package.Uninstall();
            }

            KoinoniaSettings.PackagesToUninstall.Clear();
            Settings.Commit();

            foreach (var package in packagesToUnisntall)
            {
                DeletePackageFiles(package);
            }

            ThreadingUtils.DispatchOnMainThread(AssetDatabase.Refresh);
        }
        
        public void Execute(RefreshFrontPagePackagesCommand command)
        {
            GlobalProgressMessage = "Refreshing...";
            FrontPagePackages = DescriptorsService.GetLatest().ToList();
            GlobalProgressMessage = null;
        }

        #region Static Shit

        public static void ExtractZipFile(Stream inputStream, string outFolder)
        {
            //ZipInputStream zf = null;
            //try
            //{
            //    using (ZipInputStream s = new ZipInputStream(inputStream))
            //    {

            //        ZipEntry theEntry;
            //        while ((theEntry = s.GetNextEntry()) != null)
            //        {

            //            //Console.WriteLine(theEntry.Name);
            //            string directoryName = Path.GetDirectoryName(theEntry.Name);
            //            string fileName = Path.GetFileName(theEntry.Name);

            //            // create directory
            //            if (directoryName.Length > 0)
            //            {
            //                Directory.CreateDirectory(Path.Combine(outFolder, directoryName));
            //            }

            //            if (fileName != String.Empty)
            //            {
            //                using (FileStream streamWriter = File.Create(Path.Combine(outFolder, theEntry.Name)))
            //                {

            //                    int size = 2048;
            //                    byte[] data = new byte[2048];
            //                    while (true)
            //                    {
            //                        size = s.Read(data, 0, data.Length);
            //                        if (size > 0)
            //                        {
            //                            streamWriter.Write(data, 0, size);
            //                        }
            //                        else
            //                        {
            //                            break;
            //                        }
            //                    }
            //                }
            //            }
            //        }
            //    }

            //}
            //finally
            //{
            //    if (zf != null)
            //    {
            //        zf.IsStreamOwner = true; // Makes close also shut the underlying stream
            //        zf.Close(); // Ensure we release resources
            //    }
            //}
        }

        public static List<UFramePackage> InstalledPackages
        {
            get { return _installedPackages ?? (_installedPackages = GetInstalledPackages().ToList()); }
            set { _installedPackages = value; }
        }



        public static void EnsurePath(string path)
        {
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }

        }

        private static IEnumerable<UFramePackage> GetInstalledPackages()
        {
            return AppDomain.CurrentDomain.GetAllDerivedTypes(typeof(UFramePackage)).Select(x => (UFramePackage)Activator.CreateInstance(x));

        }

        [UnityEditor.Callbacks.DidReloadScripts]
        private static void OnScriptsReloaded()
        {
        //    Debug.Log(string.Format("Checking {0} packages...", InstalledPackages.Count));

      //      InstalledPackages.ForEach(l =>
    //        {
          //      l.OnLoaded();
  //          });
        }

        #endregion

        public void Execute(SelectPackageCommand command)
        {
            if (string.IsNullOrEmpty(command.Id))
            {
                SelectedPackage = null;
                SelectedPackageRevisions = null;
                return;
            }
            GlobalProgressMessage = "Fetching package...";
            SelectedPackage = DescriptorsService.GetPackageDescriptorById(command.Id);
            SelectedPackageRevisions = DescriptorsService.GetRevisionsByProject(SelectedPackage).ToList();
            GlobalProgressMessage = null;
        }
    }
}

//        [JsonProperty, InspectorProperty]
//        public bool AllowMultiple
//        {
//            get { return _allowMultiple; }
//            set
//            {
//                this.Changed("AllowMultiple", ref _allowMultiple, value);
//            }
//        }