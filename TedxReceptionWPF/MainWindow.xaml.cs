using FastReport;
using System.Windows;
using System.Windows.Controls;

namespace ReceptionTagPrinterWPF
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            Loaded += MainWindow_Loaded;
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            LoadPrinters();
        }

        private void BtnPrint_Click(object sender, RoutedEventArgs e)
        {
            Print(TxtName.Text);
        }

        private void Print(string name)
        {
            try
            {
                Report report = new Report();
                report.Load("Badge.frx");
                report.SetParameterValue("name", name);
                report.SetParameterValue("id", 101); // You can modify this logic
                report.Show();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}");
            }
        }

        private void BtnCrap_Click(object sender, RoutedEventArgs e)
        {
            LoadFromClipboardToDataGrid(DGResult);
        }

        private void LoadFromClipboardToDataGrid(DataGrid dgv)
        {
            try
            {
                if (!Clipboard.ContainsText())
                {
                    MessageBox.Show("Clipboard is empty or does not contain text.");
                    return;
                }

                dgv.Items.Clear();
                string clipboardText = Clipboard.GetText();
                string[] lines = clipboardText.Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);

                foreach (string line in lines)
                {
                    string[] parts = line.Split('\t');
                    if (parts.Length >= 2)
                    {
                        dgv.Items.Add(new PersonModel
                        {
                            ID = parts[0].Trim(),
                            Name = parts[1].Trim()
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}");
            }
        }

        private void BtnPrintDGV_Click(object sender, RoutedEventArgs e)
        {
            foreach (var item in DGResult.SelectedItems.Count > 0 ? DGResult.SelectedItems : DGResult.Items)
            {
                if (item is PersonModel person)
                {
                    PrintSilently(person.ID, person.Name, CBPrinters.SelectedItem?.ToString());
                }
            }
        }

        private void LoadPrinters()
        {
            CBPrinters.Items.Clear();
            foreach (string printer in System.Drawing.Printing.PrinterSettings.InstalledPrinters)
            {
                CBPrinters.Items.Add(printer);
            }

            if (CBPrinters.Items.Count > 0)
            {
                CBPrinters.SelectedIndex = 0;
            }
        }

        private void BtnRefreshPrinter_Click(object sender, RoutedEventArgs e)
        {
            LoadPrinters();
        }

        private void PrintSilently(string id, string name, string printerName)
        {
            try
            {
                Report report = new Report();
                report.Load("Badge.frx");
                report.SetParameterValue("id", id);
                report.SetParameterValue("name", name);
                report.PrintSettings.Printer = printerName;  

                report.Print();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Printing failed: {ex.Message}");
            }
        }

        private void BtnEditDesign_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Report report = new Report();
                report.Load("Badge.frx");
                report.Design();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to open designer: {ex.Message}");
            }
        }
    }

    public class PersonModel
    {
        public string ID { get; set; }
        public string Name { get; set; }
    }
}
