using System.Reflection;
using System.Reflection.Metadata;
using System.Text;

namespace ProcessTextures
{
    /// <summary>
    /// Image processing parameters
    /// </summary>
    public class Parameters
    {
        /// <summary>
        /// Directory where source files (tga) are located
        /// </summary>
        public string InputDirectory;

        /// <summary>
        /// Directory where processed files (png) will be written
        /// </summary>
        public string OutputDirectory;

        /// <summary>
        /// Generate PSD files
        /// </summary>
        public bool GeneratePSD;

        private HashSet<string> ValuelessKeys = new HashSet<string>() { "psd" };

        /// <summary>
        /// Use default parameters
        /// </summary>
        public Parameters()
        {
            InputDirectory = Directory.GetCurrentDirectory();
            OutputDirectory = InputDirectory;

            if (InputDirectory.EndsWith("\\"))
            {
                InputDirectory = InputDirectory.Remove(InputDirectory.Length - 1, 1);
            }

            if (OutputDirectory.EndsWith("\\"))
            {
                OutputDirectory = OutputDirectory.Remove(OutputDirectory.Length - 1, 1);
            }
        }

        /// <summary>
        /// Parse parameters from command line arguments
        /// </summary>
        /// <param name="args">Command line arguments</param>
        /// <exception cref="ArgumentNullException"/>
        /// <exception cref="ArgumentException"/>
        public Parameters(string[] args)
            : this()
        {
            if (args == null)
            {
                throw new ArgumentNullException("Missing parameters (got: null array)");
            }

            if (args.Length == 0)
            {
                throw new ArgumentException("Missing parameters");
            }

            // split command line arguments into pairs
            // key with null value is allowed
            var ArgumentPairs = new Dictionary<string, string>();
            int n = 0;
            while (n < args.Length)
            {
                string key;
                string value;

                if (args[n].StartsWith("-"))
                {
                    key = args[n];
                    if (n + 1 < args.Length && !args[n + 1].StartsWith("-"))
                    {
                        // key with value
                        value = args[n + 1];
                        n += 2;
                    }
                    else
                    {
                        // key without value
                        value = "";
                        n += 1;
                    }

                    if (string.IsNullOrWhiteSpace(key))
                    {
                        throw new ArgumentException("Command line arguments invalid format");
                    }

                    // remove - and -- from key string
                    if (key.StartsWith("--"))
                    {
                        key = key.Remove(0, 2);
                    }
                    else if (key.StartsWith("-"))
                    {
                        key = key.Remove(0, 1);
                    }

                    // remove " from the beginning end of string
                    if (value.StartsWith("\"") || value.StartsWith("'"))
                    {
                        value = value.Remove(0, 1);
                    }
                    if (value.EndsWith("\"") || value.EndsWith("'"))
                    {
                        value = value.Remove(value.Length - 1, 1);
                    }
                    if (value.EndsWith("\\"))
                    {
                        value = value.Remove(value.Length - 1, 1);
                    }

                    // check for missing value
                    if (string.IsNullOrWhiteSpace(value) && !ValuelessKeys.Contains(key))
                    {
                        throw new ArgumentException($"Missing value for '{key}' parameter");
                    }

                    // add pair
                    ArgumentPairs[key.ToLowerInvariant()] = value ?? "";
                }
                else
                {
                    throw new ArgumentException("Command line arguments invalid format");
                }
            }

            OutputDirectory = "";

            // parse parameters
            foreach (var pair in ArgumentPairs)
            {
                if (pair.Key == "in")
                {
                    // in - input directory
                    if (!string.IsNullOrWhiteSpace(pair.Value))
                    {
                        InputDirectory = pair.Value;
                    }
                }
                else if (pair.Key == "out")
                {
                    // out - output directory
                    if (!string.IsNullOrWhiteSpace(pair.Value))
                    {
                        OutputDirectory = pair.Value;
                    }
                }
                else if (pair.Key == "psd")
                {
                    GeneratePSD = true;
                }
            }

            // output directory is omitted, use input directory
            if (string.IsNullOrWhiteSpace(OutputDirectory))
            {
                OutputDirectory = InputDirectory;
            }
        }

        public override string ToString()
        {
            var sb = new StringBuilder();

            // properties
            PropertyInfo[] properties = GetType().GetProperties();
            foreach (PropertyInfo property in properties)
            {
                sb.AppendLine($"{property.Name}: {property.GetValue(this) ?? "null"}");
            }

            // fields
            FieldInfo[] fields = GetType().GetFields();
            foreach (FieldInfo field in fields)
            {
                sb.AppendLine($"{field.Name}: {field.GetValue(this) ?? "null"}");
            }

            return sb.ToString();
        }

        /// <summary>
        /// Create copy of other parameters
        /// </summary>
        public static Parameters Copy(Parameters Other)
        {
            var result = new Parameters();

            // fields
            FieldInfo[] fields = typeof(Parameters).GetFields();
            foreach (FieldInfo field in fields)
            {
                if (!field.IsInitOnly) // readonly?
                {
                    field.SetValue(result, field.GetValue(Other));
                }
            }

            // properties
            PropertyInfo[] properties = typeof(Parameters).GetProperties();
            foreach (PropertyInfo property in properties)
            {
                if (property.CanRead && property.CanWrite)
                {
                    property.SetValue(result, property.GetValue(Other));
                }
            }

            return result;
        }
    }
}
