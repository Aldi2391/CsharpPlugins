﻿using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using SharedModelUnloader.ViewModels;

namespace SharedModelUnloader.Infrastructure.Commands
{
    [Transaction(TransactionMode.Manual)]
    public class StartCommand : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            UIApplication uiApp = commandData.Application;
            try
            {
                var sharedModelUnloaderWindow = new MainWindow
                {
                    DataContext = new MainWindowViewModel(uiApp)
                };
                sharedModelUnloaderWindow.ShowDialog();
                return Result.Succeeded;
            }
            catch
            { 
                return Result.Failed;
            }
        }
    }
}