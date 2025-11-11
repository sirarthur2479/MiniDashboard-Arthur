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
public class MiniDashboardUITests
{
    private Application? _app;
    private UIA3Automation? _automation;

    [TestInitialize]
    public void Setup() {
        // Launch your WPF app
        // Get the path to the WPF app relative to the test project
        var currentDir = Directory.GetCurrentDirectory(); // test bin folder
        var appPath = Path.Combine(currentDir, "..", "..", "..", "..", "..", "src", "MiniDashboard.App", "bin", "Debug", "net9.0-windows", "MiniDashboard.App.exe");

        if (!File.Exists(appPath)) {
            throw new FileNotFoundException("Cannot find MiniDashboard.App.exe. Build the WPF app first.", appPath);
        }

        _app = Application.Launch(Path.GetFullPath(appPath));
        _automation = new UIA3Automation();
    }

    [TestCleanup]
    public void Cleanup()
    {
        // Only close if no test failed
        if (this.TestContext.CurrentTestOutcome == UnitTestOutcome.Passed) {
            _automation?.Dispose();
            _app?.Close();
        } else {
            Console.WriteLine("Test failed — app left open for inspection.");
        }
    }

    public TestContext TestContext { get; set; }
    [TestMethod]
    public void AddItem_ShowsInDataGrid() {
        if (_app == null || _automation == null)
            throw new InvalidOperationException("Application not initialized");
        var window = _app?.GetMainWindow(_automation)
            ?? throw new InvalidOperationException("Main window not found"); ;

        var nameBox = window.FindFirstDescendant(cf => cf.ByAutomationId("NewItemName"))?.AsTextBox()
            ?? throw new InvalidOperationException("Name textbox not found");
        var descBox = window.FindFirstDescendant(cf => cf.ByAutomationId("NewItemDescription"))?.AsTextBox()
            ?? throw new InvalidOperationException("Description textbox not found");
        var priceBox = window.FindFirstDescendant(cf => cf.ByAutomationId("NewItemPrice"))?.AsTextBox()
            ?? throw new InvalidOperationException("Price textbox not found");
        var addButton = window.FindFirstDescendant(cf => cf.ByAutomationId("AddButton"))?.AsButton()
            ?? throw new InvalidOperationException("Add button not found");
        var deleteButton = window.FindFirstDescendant(cf => cf.ByAutomationId("DeleteButton"))?.AsButton()
            ?? throw new InvalidOperationException("Delete button not found");
        var dataGrid = window.FindFirstDescendant(cf => cf.ByControlType(FlaUI.Core.Definitions.ControlType.DataGrid)).AsDataGridView();

        Assert.IsNotNull(nameBox);
        Assert.IsNotNull(addButton);
        Assert.IsNotNull(deleteButton);
        Assert.IsNotNull(dataGrid);

        // Fill in item data
        nameBox.Enter("Test Item");
        descBox.Enter("Test Description");
        priceBox.Enter("9.99");

        // Click Add
        addButton.Invoke();

        // Wait for row to appear (retry up to 2s)
        var added = Retry.WhileFalse(() =>
            dataGrid.Rows.Any(r => {
                var cell = r.Cells[1]; // Name column
                var text = cell?.FindFirstChild()?.AsLabel()?.Text;
                return text == "Test Item";
            }),
            TimeSpan.FromSeconds(2)
        );

        Assert.IsTrue(added.Result, "Item was not added to DataGrid");
        // --- Delete Item ---
        var rowToDelete = dataGrid.Rows.First(r => {
            var cell = r.Cells[1];
            var text = cell?.FindFirstChild()?.AsLabel()?.Text;
            return text == "Test Item";
        });

        rowToDelete.Click();
        deleteButton.Invoke();

        // Wait for row to disappear
        var deleted = Retry.WhileTrue(() =>
            dataGrid.Rows.Any(r => {
                var cell = r.Cells[1];
                var text = cell?.FindFirstChild()?.AsLabel()?.Text;
                return text == "Test Item";
            }),
            TimeSpan.FromSeconds(2)
        );

        Assert.IsTrue(deleted.Result, "Item was not deleted from DataGrid");
    }
}
