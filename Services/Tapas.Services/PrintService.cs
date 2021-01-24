using System;
using System.Drawing;
using System.Drawing.Printing;
using System.IO;

public class PrintService
{
    private static string filePath;
    private Font printFont;
    private StreamReader streamToPrint;

    public PrintService()
    {
        this.Printing();
    }

    // This is the main entry point for the application.
    public static void Main(string[] args)
    {
        string sampleName = Environment.GetCommandLineArgs()[0];
        if (args.Length != 1)
        {
            Console.WriteLine("Usage: " + sampleName + " <file path>");
            return;
        }

        filePath = args[0];
        new PrintService();
    }

    // Print the file.
    public void Printing()
    {
        try
        {
            this.streamToPrint = new StreamReader(filePath);
            try
            {
                this.printFont = new Font("Arial", 10);
                PrintDocument pd = new PrintDocument();
                pd.PrintPage += new PrintPageEventHandler(this.Pd_PrintPage);

                // Print the document.
                pd.Print();
            }
            finally
            {
                this.streamToPrint.Close();
            }
        }
        catch (Exception)
        {
        }
    }

    // The PrintPage event is raised for each page to be printed.
    private void Pd_PrintPage(object sender, PrintPageEventArgs ev)
    {
        float linesPerPage;
        float yPos;
        int count = 0;
        float leftMargin = ev.MarginBounds.Left;
        float topMargin = ev.MarginBounds.Top;
        string line = null;

        // Calculate the number of lines per page.
        linesPerPage = ev.MarginBounds.Height /
           this.printFont.GetHeight(ev.Graphics);

        // Iterate over the file, printing each line.
        while (count < linesPerPage &&
           ((line = this.streamToPrint.ReadLine()) != null))
        {
            yPos = topMargin + (count * this.printFont.GetHeight(ev.Graphics));
            ev.Graphics.DrawString(line, this.printFont, Brushes.Black, leftMargin, yPos, new StringFormat());
            count++;
        }

        // If more lines exist, print another page.
        ev.HasMorePages = line != null;
    }
}
