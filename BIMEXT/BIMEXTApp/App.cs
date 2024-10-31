using Autodesk.Revit.UI;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Windows.Media.Imaging;

namespace BIMEXTApp
{
    internal class App : IExternalApplication
    {
        public static App ThisApp = null;

        public Result OnStartup(UIControlledApplication application)
        {
            ThisApp = this;
            string tabName = "BIMEXT";
            string thisAssemblyPath = Assembly.GetExecutingAssembly().Location;
            string thisAssemblyFolder = Path.GetDirectoryName(thisAssemblyPath);
            string sharedModelUnloaderAssemblyPath = Path.Combine(thisAssemblyFolder, $"SharedModelUnloader.dll");

            AddRibbonTab(application, tabName);

            RibbonPanel panelExt = AddRibbonPanel(application, tabName, "AUTO");

            AddButton(
                application,
                sharedModelUnloaderAssemblyPath,
                panelExt,
                "Выгрузка моделей",
                "SharedModelUnloader.Infrastructure.Commands.StartCommand",
                "Выгрузка и обновления моделей в зону shared",
                "sharedModelUnloader.png"
            );

            return Result.Succeeded;
        }

        public Result OnShutdown(UIControlledApplication application)
        {
            return Result.Succeeded;
        }

        #region Добавление кнопки и панели
        public void AddRibbonTab(UIControlledApplication application, string tabName)
        {
            try
            {
                application.CreateRibbonTab(tabName);
            }
            catch {}
        }

        public RibbonPanel AddRibbonPanel(UIControlledApplication application, string tabName, string panelName)
        {

            RibbonPanel ribbonPanel = null;

            try
            {
                RibbonPanel panel = application.CreateRibbonPanel(tabName, panelName);
            }
            catch {}

            List<RibbonPanel> panels = application.GetRibbonPanels(tabName);

            foreach (RibbonPanel p in panels)
            {
                if (p.Name == panelName)
                {
                    ribbonPanel = p;
                }
            }

            return ribbonPanel;
        }

        public void AddButton(
            UIControlledApplication application, 
            string assemblyPath, 
            RibbonPanel panel, 
            string nameButton, 
            string commandButton, 
            string toolTipButton,
            string imageButton)
        {
            try
            {
                PushButton button = panel.AddItem(
                    new PushButtonData(nameButton, nameButton, assemblyPath, commandButton))
                    as PushButton;

                button.ToolTip = toolTipButton;

                BitmapImage largeImage = new BitmapImage();
                largeImage.BeginInit();
                largeImage.UriSource = new Uri("pack://application:,,,/BIMEXTApp;component/Resources/" + imageButton, UriKind.RelativeOrAbsolute);
                largeImage.EndInit();

                button.LargeImage = largeImage;
            }
            catch {}
        }

        #endregion
    }
}
