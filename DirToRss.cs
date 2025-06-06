using System;
using System.IO;
using System.Xml;

public class DirToRss
{
    public static void Main(string[] args)
    {
        // 1. Validate command line arguments
        if (args.Length != 3)
        {
            Console.WriteLine("Usage: DirToRss <sourceDir> <outputFile> <urlBase>");
            return;
        }

        string sourceDir = args[0];
        string outputFile = args[1];
        string urlBase = args[2];

        if (!Directory.Exists(sourceDir))
        {
            Console.WriteLine($"Error: Source directory '{sourceDir}' does not exist.");
            return;
        }

        // 2. Create the RSS feed
        try
        {
            int count = 0;
            using (XmlWriter writer = XmlWriter.Create(outputFile, new XmlWriterSettings { Indent = true }))
            {
                writer.WriteStartDocument();
                writer.WriteStartElement("rss");
                writer.WriteAttributeString("version", "2.0");

                writer.WriteStartElement("channel");
                writer.WriteElementString("title", "Directory RSS Feed");
                writer.WriteElementString("link", urlBase); // Link to the base URL of the feed
                writer.WriteElementString("description", $"RSS feed of HTML files in {sourceDir}");
                writer.WriteElementString("lastBuildDate", DateTime.Now.ToString("R")); // RFC-822 date format

                // 3. Get all HTML files in the source directory
                string[] htmlFiles = Directory.GetFiles(sourceDir, "*.html");

                // 4. Add each HTML file as an item to the RSS feed
                foreach (string filePath in htmlFiles)
                {
                    string fileName = Path.GetFileName(filePath);
                    string title = Path.GetFileNameWithoutExtension(filePath).Replace('_', ' ');
                    string itemUrl = urlBase + fileName; // Concatenate base URL with filename

                     // Get the creation time of the file and format it to RFC-822
                    DateTime creationTime = File.GetCreationTimeUtc(filePath);
                    string pubDate = creationTime.ToString("R"); // "R" for RFC-822 format


                    writer.WriteStartElement("item");
                    writer.WriteElementString("title", title);
                    writer.WriteElementString("link", itemUrl);
                    writer.WriteElementString("guid", itemUrl); // GUID is often the same as the link
                    writer.WriteElementString("pubDate", pubDate);
                    writer.WriteEndElement(); // item

                    count++;
                }

                writer.WriteEndElement(); // channel
                writer.WriteEndElement(); // rss
                writer.WriteEndDocument();
            }

            Console.WriteLine($"RSS feed with {count} items generated at '{outputFile}'");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"An error occurred: {ex.Message}");
        }
    }
}