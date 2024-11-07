using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using SharedModelUnloader.ViewModels;
using SharedModelUnloader.Views.Windows;

namespace SharedModelUnloader.Infrastructure.Commands
{
    [Transaction(TransactionMode.Manual)]
    public class StartCommand : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            try
            {
                var loadingWindow = new LoadingWindow
                {
                    DataContext = new LoadingWindowsViewModel(commandData.Application)
                };
                loadingWindow.ShowDialog();
                return Result.Succeeded;
            }
            catch
            { 
                return Result.Failed;
            }
        }
    }
}
