using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Invert.Common;
using QF.GraphDesigner.Unity.KoinoniaSystem.Classes;
using QF.GraphDesigner.Unity.KoinoniaSystem.Commands;
using QF.GraphDesigner.Unity.KoinoniaSystem.Data;
using QF.GraphDesigner.Unity.KoinoniaSystem.ViewModels;
using QF.GraphDesigner;
using UnityEditor;
using UnityEngine;

namespace QF.GraphDesigner.Unity.KoinoniaSystem
{

    public class PackageManagerUISystem : DiagramPlugin, IDrawPackageManager, IDrawPackagePage, IDrawPackageControlPanel
    {
        private IPlatformDrawer _platformDrawer;
        private KoinoniaSystem _koinoniaSystem;

        [InspectorProperty]
        public string Username { get; set; }

        [InspectorProperty]
        public string Password { get; set; }

        public IPlatformDrawer PlatformDrawer
        {
            get { return _platformDrawer ?? (_platformDrawer = InvertApplication.Container.Resolve<IPlatformDrawer>()); }
            set { _platformDrawer = value; }
        }

        public KoinoniaSystem KoinoniaSystem
        {
            get { return _koinoniaSystem ?? (_koinoniaSystem = InvertApplication.Container.Resolve<KoinoniaSystem>()); }
            set { _koinoniaSystem = value; }
        }


        public GUIStyle ProjectPageDescriptionStyle
        {
            get
            {
                return _projectPageDescriptionStyle ?? (_projectPageDescriptionStyle = new GUIStyle()
                {
                    wordWrap = true,
                    alignment = TextAnchor.UpperLeft
                }.WithAllStates(Color.white).WithFont("Verdana", 14));
            }
            set { _projectPageDescriptionStyle = value; }
        }


      //  [MenuItem("uFrame Dev/Package Manager")]
        public static void OpenPackageManagerWindow()
        {
            var c = InvertApplication.Container;
            var packageManagerWindow = EditorWindow.GetWindow<PackageManagerWindow>();
            packageManagerWindow.minSize = packageManagerWindow.maxSize = new Vector2(800, 600);
            packageManagerWindow.Show();
            packageManagerWindow.Repaint();
            packageManagerWindow.Focus();
        }    
        
        public static void OpenPackageControlPanelWindow(Rect position, UFramePackageDescriptor package)
        {
            var packageControlPanel = EditorWindow.CreateInstance<PackageControlPanel>();
            packageControlPanel.minSize = packageControlPanel.maxSize = new Vector2(200, 200);
            packageControlPanel.Package = package;
            Debug.Log("Showing at position "+position.x+" , "+position.y);
            packageControlPanel.ShowAsDropDown(position ,new Vector2(200, 200));
            packageControlPanel.Repaint();
            packageControlPanel.Focus();
        }

        public static Dictionary<string, Texture> ImageCache
        {
            get { return _imageCache ?? (_imageCache = new Dictionary<string, Texture>()); }
            set { _imageCache = value; }
        }

        public static Dictionary<string, string> ContentCache
        {
            get { return _contentCache ?? (_contentCache = new Dictionary<string, string>()); }
            set { _contentCache = value; }
        }

        public Texture GetImage(string url)
        {
            Texture texture;
            if (ImageCache.TryGetValue(url, out texture))
            {
                return texture ?? ElementDesignerStyles.GetSkinTexture("LoadingImage");
            }
            ImageCache.Add(url, null);
            DownloadImage(url);
            if (ImageCache.TryGetValue(url, out texture))
            {
                return texture ?? ElementDesignerStyles.GetSkinTexture("LoadingImage");
            }
            else
            {
                return ElementDesignerStyles.GetSkinTexture("LoadingImage");
            }

            return ElementDesignerStyles.ArrowDownTexture;
        }

        public string GetContent(string url)
        {
            string texture;
            if (ContentCache.TryGetValue(url, out texture))
            {
                return texture;
            }
            ContentCache.Add(url, null);
            DownloadString(url);
            if (ContentCache.TryGetValue(url, out texture))
            {
                return texture;
            }
            return "Loading...";
        }
        public void DownloadImage(string url)
        {
            var ww = new WWW(url);

            EditorApplication.CallbackFunction callbackFunction = null;
            callbackFunction = () =>
            {
                if (ww.isDone)
                {
                    EditorApplication.update -= callbackFunction;
                    ImageCache[url] = ww.texture;
                }
            };
            EditorApplication.update += callbackFunction;

        }
        public void DownloadString(string url)
        {
            var ww = new WWW(url);
            while (!ww.isDone)
            {

            }
            ContentCache[url] = ww.text;
        }

        public void DrawPackageManager(Rect bounds)
        {

            if (!string.IsNullOrEmpty(KoinoniaSystem.GlobalProgressMessage))
            {
                var loginScreenSide = 200;
                var loadingScreenBounds = new Rect((bounds.width - loginScreenSide) / 2, (bounds.height - loginScreenSide) / 2, loginScreenSide, loginScreenSide);
                DrawLoadingScreen(loadingScreenBounds, KoinoniaSystem.GlobalProgressMessage);
                return;
            }

            if (!KoinoniaSystem.IsRemoteServerAvailable)
            {
                GUILayout.Label("Cannot reach Koinonia servers...");
                if (GUILayout.Button("Retry")) KoinoniaSystem.UpdateAvailability();
                return;
            }

            if (KoinoniaSystem.AuthorizationState == AuthorizationState.Unauthorized) DrawLoginScreen(bounds);
            else DrawPackageManagerScreen(bounds);
        }

        private void DrawPackageManagerScreen(Rect bounds)
        {

            var projectListBounds = new Rect(bounds) { width = 200 };
            var rightSizeBounds = new Rect(projectListBounds) { width = bounds.width - projectListBounds.width, x = projectListBounds.xMax };

            DrawPackageList(projectListBounds);

            if (KoinoniaSystem.SelectedPackage == null) DrawPreviewScreen(rightSizeBounds);
            else Signal<IDrawPackagePage>(_ => _.DrawPackagePage(rightSizeBounds, KoinoniaSystem.SelectedPackage));

        }

        private void DrawPackageList(Rect projectListBounds)
        {
            var buttonBounds = new Rect(projectListBounds)
            {
                height = 20,
            };

            foreach (var package in KoinoniaSystem.InstalledPackagesDescriptors)
            {
                var bounds = buttonBounds;
                PlatformDrawer.DoButton(buttonBounds, string.Format("{0}", package.Title), ElementDesignerStyles.ButtonStyle,
                    x => SelectPackage(package.Id), x => OpenPackageControlPanelWindow(new Rect(x.x,x.y,bounds.width,bounds.height), package));
                buttonBounds = new Rect(buttonBounds) { y = buttonBounds.yMax };
            }
        }


        private void DrawPreviewScreen(Rect bounds)
        {
            if (!KoinoniaSystem.FrontPagePackages.Any())
            {
                GUILayout.Label("No packages found...");
            }

            var packages = KoinoniaSystem.FrontPagePackages.Take(6).ToArray();
            var previewItemBounds = new Rect(bounds.x + 5, bounds.y + 5, 160, 200);
            for (int i = 0; i < packages.Count()/3; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    var pak = packages[i*3+j];
                    DrawPackagePreview(previewItemBounds,pak,()=>SelectPackage(pak.Id));

                    previewItemBounds = new Rect(previewItemBounds)
                    {
                        x = previewItemBounds.x + previewItemBounds.width + 10
                    };
                }
                previewItemBounds = new Rect(previewItemBounds)
                {
                    y = previewItemBounds.y + previewItemBounds.height + 10,
                    x = bounds.x + 5
                };
            }
        }

        private void SelectPackage(string id)
        {
            InvertApplication.ExecuteInBackground(new SelectPackageCommand() { Id = id });
        }

        private float rot = 0;
        private GUIStyle _messageStyle;
        private UFramePackagePreviewDescriptor _exampleDescriptor;
        private static Dictionary<string, string> _contentCache;
        private static Dictionary<string, Texture> _imageCache;
        private GUIStyle _projectPreviewVersionStyle;
        private GUIStyle _projectPreviewTitleStyle;
        private GUIStyle _projectPageDescriptionStyle;

        private void DrawLoadingScreen(Rect bounds, string message)
        {
            var size = PlatformDrawer.CalculateImageSize("Spinner");

            var imageRect = new Rect(bounds)
            {
                x = bounds.x + (bounds.width - size.x) / 2,
                y = bounds.y + (bounds.height - size.y) / 2,
                width = size.x,
                height = size.y
            };

            var textRect = new Rect(imageRect)
            {
                width = bounds.width,
                height = 15,
                y= imageRect.y + imageRect.height+ 5,
                x = bounds.x
            };

            var rotPivot = new Vector2(imageRect.x+size.x/2, imageRect.y+size.y/2);
            var mat = GUI.matrix;
            GUIUtility.RotateAroundPivot(rot, rotPivot);
            PlatformDrawer.DrawImage(imageRect, "Spinner", true);
            //Draw code
            rot += 0.4f;
            GUI.matrix = mat;
            GUI.Label(textRect, message, MessageStyle);

        }

        public GUIStyle MessageStyle
        {
            get
            {
                return _messageStyle ?? ( _messageStyle = new GUIStyle()
                {
                    alignment = TextAnchor.MiddleCenter,
                }
                .WithAllStates(Color.white)
                .WithFont("Verdana",14));
            }
            set { _messageStyle = value; }
        }

        public GUIStyle ProjectPreviewVersionStyle
        {
            get
            {
                return _projectPreviewVersionStyle ?? (_projectPreviewVersionStyle = new GUIStyle()
                {
                    alignment = TextAnchor.MiddleCenter,
                }
                .WithAllStates(Color.black)
                .WithFont("Verdana", 14));
            }
            set { _projectPreviewVersionStyle = value; }
        }

        public GUIStyle ProjectPreviewTitleStyle
        {
            get
            {
                return _projectPreviewTitleStyle ?? (_projectPreviewTitleStyle = new GUIStyle()
                {
                    alignment = TextAnchor.MiddleCenter,
                }
                .WithAllStates(Color.white)
                .WithFont("Verdana",15));
            }
            set { _projectPreviewTitleStyle = value; }
        }

        private void DrawLoginScreen(Rect bounds)
        {
            float loginScreenSide = 200;
            GUILayout.BeginArea(new Rect((bounds.width - loginScreenSide) / 2, (bounds.height - loginScreenSide) / 2, loginScreenSide, loginScreenSide));

            Username = GUILayout.TextField(Username ?? "");
            Password = GUILayout.TextField(Password ?? "");
            if (GUILayout.Button("Login"))
            {
                InvertApplication.ExecuteInBackground(new LoginCommand()
                {
                    Username = Username,
                    Password = Password
                });
            }
            GUILayout.EndArea();
        }

        private void DrawPackagePreview(Rect bounds, UFramePackageDescriptor descriptor, System.Action leftClick = null, System.Action rightClick= null)
        {
            var borderRect = new Rect(bounds);
            var previewImageRect = new Rect(borderRect)
            {
                x = borderRect.x + 5,
                y = borderRect.y + 5,
                width = bounds.width - 10,
                height = bounds.width - 10
            };

            var latestVersionRect = new Rect()
            {
                x = previewImageRect.x + previewImageRect.width/2,
                y = previewImageRect.yMax - 15,
                width = previewImageRect.width/2,
                height = 15
            };

            var titleRect = new Rect()
            {
                x = previewImageRect.x,
                y = previewImageRect.yMax,
                width = previewImageRect.width,
                height = bounds.height - 10 - previewImageRect.height
            };


            if (GUI.Button(borderRect, ""))
            {
                if (leftClick != null) leftClick();
            }


            GUI.DrawTexture(previewImageRect,GetImage(descriptor.ProjectIconUrl),ScaleMode.ScaleToFit,true );
            //GUI.Label(latestVersionRect,descriptor.LatestPublicVersionTag,ProjectPreviewVersionStyle);
            GUI.Label(titleRect,descriptor.Title,ProjectPreviewTitleStyle);


        }

        public void DrawPackagePage(Rect bounds, UFramePackageDescriptor package)
        {

            var imageBounds = new Rect(bounds.x + 5, bounds.y + 5, 150, 150);
            var titleBounds = new Rect(imageBounds)
            {
                x = imageBounds.xMax + 5,
                width = bounds.width - imageBounds.width - 10,
                height = 20
            };
            var descriptionBounds = new Rect(titleBounds)
            {
                y = titleBounds.yMax + 10,
                height = 120,
                width = 400
            };

            var backButtonRect = new Rect(descriptionBounds)
            {
                y = descriptionBounds.yMax + 20,
                height = 30,
                width = 100
            };

            GUI.DrawTexture(imageBounds, GetImage(package.ProjectIconUrl), ScaleMode.ScaleToFit, true);
            GUI.Label(titleBounds, package.Title, ProjectPreviewTitleStyle);
            GUI.Label(descriptionBounds, package.Description, ProjectPageDescriptionStyle);

            if (GUI.Button(backButtonRect, "Back"))
            {
                InvertApplication.ExecuteInBackground(new SelectPackageCommand() {Id = null});
            }

            var revButtonRect = new Rect(backButtonRect)
            {
                x = backButtonRect.xMax + 5,
                width = 200
            };

            foreach (var revision in KoinoniaSystem.SelectedPackageRevisions)
            {

                if (GUI.Button(revButtonRect, string.Format("Install {1} {0}", revision.VersionTag, package.Title)))
                {
                    InvertApplication.ExecuteInBackground(new QueueRevisionForInstallCommand()
                    {
                        PackageDescriptor = package,
                        RevisionDescriptor = revision
                    });
                    //Debug.Log("Will download revision from "+ revision.SnapshotUri);
                }

                revButtonRect = new Rect(revButtonRect)
                {
                    y = revButtonRect.yMax
                };
            }


        }

        public void DrawControlPanel(Rect bounds, UFramePackage package)
        {
            if (GUI.Button(bounds, "Uninstall"))
            {
                InvertApplication.ExecuteInBackground(new QueueRevisionForUninstallCommand()
                {
                    Package = package
                });
            }
        }
    }
}
