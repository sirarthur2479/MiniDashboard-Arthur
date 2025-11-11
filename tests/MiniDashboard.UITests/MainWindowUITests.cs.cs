using FlaUI.Core;
using FlaUI.UIA3;
using FlaUI.Core.Tools;
using FlaUI.Core.AutomationElements;
using FlaUI.Core.Conditions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Diagnostics;
using System.Linq;
using System.IO;

[TestClass]
public class MiniDashboardUITests {
    private Application? _app;
    private UIA3Automation? _automation;

    [TestInitialize]
    public void Setup() {
        // Launch your WPF app
        // Get the path to the WPF app relative to the test project
        var currentDir = Directory.GetCurrentDirectory(); // test bin folder
        var appPath = Path.Combine(currentDir, "..", "..", "..", "..", "..", "src", "MiniDashboard.App", "bin", "Debug", "net9.0-windows", "MiniDashboard.App.exe");

        if (!File.Exists(appPath))
        {
            throw new FileNotFoundException("Cannot find MiniDashboard.App.exe. Build the WPF app first.", appPath);
        }

        _app = Application.Launch(Path.GetFullPath(appPath));
        _automation = new UIA3Automation();
    }

    public TestContext TestContext { get; set; }

    [TestCleanup]
    public void Cleanup() {
        // Only close if no test failed
        if (this.TestContext.CurrentTestOutcome == UnitTestOutcome.Passed) {
            _automation?.Dispose();
            _app?.Close();
        } else {
            Console.WriteLine("Test failed — app left open for inspection.");
        }
    }

    [TestMethod]
    public void AddItem_ShowsInDataGrid() {
        if (_app == null || _automation == null)
            throw new InvalidOperationException("Application not initialized");
        var window = _app?.GetMainWindow(_automation)
            ?? throw new InvalidOperationException("Main window not found"); ;

        var nameBox = window.FindFirstDescendant(cf => cf.ByAutomationId("NewItemName"))?.AsTextBox()
            ?? throw new InvalidOperationException("Name textbox not found"); ;
        var descBox = window.FindFirstDescendant(cf => cf.ByAutomationId("NewItemDescription"))?.AsTextBox()
            ?? throw new InvalidOperationException("Description textbox not found"); ;
        var priceBox = window.FindFirstDescendant(cf => cf.ByAutomationId("NewItemPrice"))?.AsTextBox()
            ?? throw new InvalidOperationException("Price textbox not found"); ;
        var addButton = window.FindFirstDescendant(cf => cf.ByAutomationId("AddButton"))?.AsButton()
            ?? throw new InvalidOperationException("Add button not found"); ;

        Assert.IsNotNull(nameBox);
        Assert.IsNotNull(addButton);

        // Fill in item data
        nameBox.Enter("Test Item");
        descBox.Enter("Test Description");
        priceBox.Enter("9.99");

        // Click Add
        addButton.Invoke();

        // Check that DataGrid contains new item
        var dataGrid = window.FindFirstDescendant(cf => cf.ByControlType(FlaUI.Core.Definitions.ControlType.DataGrid)).AsDataGridView();

        // Wait up to 2 seconds for row to appear
        var retry = Retry.WhileFalse(() => {
            if (dataGrid == null || dataGrid.Rows == null)
                return false;

            return dataGrid.Rows.Any(row => {
                if (row?.Cells == null || row.Cells.Length <= 1)
                    return false;

                var cell = row.Cells[1];
                var textElement = cell?.FindFirstChild()?.AsLabel();
                var text = textElement?.Text;

                return text != null && text == "Test Item";
            });
        }, TimeSpan.FromSeconds(2));

        Assert.IsTrue(retry.Result, "The 'Test Item' row did not appear in the DataGrid within 2 seconds.");

    }
}
