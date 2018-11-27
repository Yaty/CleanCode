using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;

namespace LightSaberFactory {

    public class TemplateConfig {
        
        private TemplateLineConfig Header;
        public List<TemplateLineConfig> Body;

        private readonly Dictionary<string, bool> validationDict;
        public TemplateConfig(TemplateLineConfig Header, List<TemplateLineConfig> Body) {
            this.Header = Header;
            this.Body = Body;

            this.validationDict = new Dictionary<string, bool> { 
                { "name",false },
                { "title", false },
                { "mail",false }, 
                { "code",false }
            };
        }

        public bool IsValid() {
            if (Header.Key == "template" && Header.Value == "register-confirmation") {
                //then try to find if the keys are present in the data
                foreach (TemplateLineConfig line in Body) {
                    //if key is present then it's ok, 
                    if (this.validationDict.ContainsKey (line.Key)) {
                        //mark it true
                        this.validationDict[line.Key] = true;
                    }
                }

                return this.validationDict.Any(x => x.Value);
            }

            return false;
        }
    }

    public class TemplateLineConfig {
        public string Key {get; set;}
        public string Value {get; set;}

        public TemplateLineConfig(string Key, string Value) {
            this.Key = Key;
            this.Value = Value;
        }
    }

    public class TemplateParser {
        public TemplateConfig ParseConfig(string[] lines) {
            // first line is the header
            TemplateLineConfig header = this.ParseLine(lines[0]);
            List<TemplateLineConfig> body = new List<TemplateLineConfig>();

            var remainingLines = lines.Skip(1);

            foreach (string line in remainingLines) {
                body.Add(this.ParseLine(line));
            }

            return new TemplateConfig(header, body);
        }

        public string ParseContent(string content, TemplateConfig config) {
            Dictionary<string,string> dic3 = new Dictionary<string, string> {
                {"datetime",DateTime.Now.ToString()},
                {"website","http://thelightsabersguild.com"}
            };

            foreach (TemplateLineConfig line in config.Body) {
                content = content.Replace ("{{" + line.Key + "}}", line.Value);
            }

            //and replace the special function keys
            foreach (var funcvalue in dic3) {
                content = content.Replace ("{{=" + funcvalue.Key + "}}", funcvalue.Value);
            }

            return content;
        }

        private TemplateLineConfig ParseLine(string line) {
            string[] splittedLine = line.Split (":".ToCharArray());
            string key = splittedLine[0].Trim();
            string value = splittedLine[1].Trim();
            return new TemplateLineConfig(key, value);
        }
    }

    public class FileReader {
        public static string[] getLines(string filePath) {
            return File.ReadAllLines (filePath);
        }

        public static string getContent(string filePath) {
            return File.ReadAllText(filePath);
        }
    }

    enum ExitCode : int {
        Success = 0,
        InvalidTemplate = -5,
        InvalidCommand = -10,
    }

    class Program {
        static int Main(string[] args) {
            string command = args[0];
            string templateConfigFilePath = args[1];
            string[] templateConfigLines = FileReader.getLines(templateConfigFilePath);

            TemplateParser parser = new TemplateParser();
            TemplateConfig templateConfig = parser.ParseConfig(templateConfigLines);

            switch (command) {
                case "check":
                    if (!templateConfig.IsValid()) {
                        Console.WriteLine("Invalid config file.");
                        return (int) ExitCode.InvalidTemplate; 
                    }

                    Console.WriteLine("Valid config file.");
                    return (int) ExitCode.Success;
                case "fill":
                    if (!templateConfig.IsValid()) {
                        Console.WriteLine("Invalid config file.");
                        return (int) ExitCode.InvalidTemplate; 
                    }

                    string templateFilePath = args[2];
                    string templateFileContent = FileReader.getContent(templateFilePath);
                    string content = parser.ParseContent(templateFileContent, templateConfig);

                    Console.WriteLine(content);
                    return (int) ExitCode.Success;
                default:
                    Console.WriteLine ("valid commands are [check],[fill]");
                    return (int) ExitCode.InvalidCommand;
            }
        }
    }
}