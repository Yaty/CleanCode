using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;

namespace LightSaberFactory
{
    class Program
    {
        static void Main(string[] args)
        {
            //check that data input is valid if command is CHECK
			if (args [0] == "check") {
				var lines = File.ReadAllLines (args [1]);
				//first line is the header
				var header = lines [0].Split (":".ToCharArray ());
				//if it is data for template of type "register-confirmation"
				if (header [0] == "template" && header [1] == "register-confirmation") {
					var body = lines.Skip (1);
					//the list of the mandatory keys
					Dictionary<string,bool> dic1 = new Dictionary<string, bool> { 
						{ "name",false },
						{ "title", false },
						{ "mail",false }, 
						{ "code",false }
					};
					//then try to find if the keys are present in the data
					foreach (var line in body) {
						var pair = line.Split (":".ToCharArray ());
						var key = pair [0].Trim ();
						//if key is present then it's ok, 
						if (dic1.ContainsKey (key)) {
							//mark it true
							dic1 [key] = true;
						}
					}
					//if there is at least one false value remaining then error
					if (dic1.Any (x => !x.Value))
						Console.WriteLine ("-5"); //error code 5 represents bad data input 
					else
						Console.WriteLine ("data is valid"); //0 means no error found
				} else {
					//it's not a valid data file
					Console.WriteLine ("it's not a valid template");
				}
			} else if (args [0] == "fill") {
				var lines = File.ReadAllLines (args [1]);
				//first line is the header
				var header = lines [0].Split (":".ToCharArray ());
				//if it is data for template of type "register-confirmation"
				if (header [0] == "template" && header [1] == "register-confirmation") {
					var content = File.ReadAllText (args [2]);
					var body = lines.Skip (1);
					Dictionary<string,string> dic2 = new Dictionary<string, string> ();
					Dictionary<string,string> dic3 = new Dictionary<string, string> {
						{"datetime",DateTime.Now.ToString()},
						{"website","http://thelightsabersguild.com"}
					};
					//then add all the keys found to a dictionary of values
					foreach (var line in body) {
						var pair = line.Split (":".ToCharArray ());
						var key = pair [0].Trim ();
						var value = pair [1];
						//if key is present then it's ok, 
						if (dic2.ContainsKey (key)) {
							//mark it true
							dic2[key] = value;
						} else {
							dic2.Add (key, value);
						}
					}

					//then replace the values
					foreach (var keyvalue in dic2) {
						content = content.Replace ("{{" + keyvalue.Key + "}}", keyvalue.Value);
					}
					//and replace the special function keys
					foreach (var funcvalue in dic3) {
						content = content.Replace ("{{=" + funcvalue.Key + "}}", funcvalue.Value);
					}

					Console.WriteLine (content);
				} else {
					//it's not a valid data template
					Console.WriteLine("can't fill a template with invalid data");
				}
			} else {
				Console.WriteLine ("valid commands are [check],[fill]");
			}

			Console.Read ();
        }
    }
}