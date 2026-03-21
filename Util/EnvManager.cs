using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using CsvHelper;

namespace Boostera
{
    internal class EnvManager
    {
        private List<string> running = new List<string>();
        internal bool IsRunning { get { return running.Count != 0; } }

        internal async Task SetEnv(Env env)
        {
            var id = Guid.NewGuid().ToString("N");
            running.Add(id);

            try
            {
                await Task.Run(() => Environment.SetEnvironmentVariable(env.Key, env.Value, EnvironmentVariableTarget.User));
            }
            catch
            {
            }
            finally
            {
                running.Remove(id);
            }
        }

        internal async Task SetEnvs(List<Env> envs)
        {
            var id = Guid.NewGuid().ToString("N");
            running.Add(id);

            try
            {
                foreach (var env in envs)
                {
                    try
                    {
                        await Task.Run(() => Environment.SetEnvironmentVariable(env.Key, env.Value, EnvironmentVariableTarget.User));
                    }
                    catch { }
                }
            }
            catch
            {
            }
            finally
            {
                running.Remove(id);
            }
        }

        internal void ExportEnvsToCsv(List<Env> envs, string filePath)
        {
            using (var writer = new StreamWriter(filePath))
            {
                using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
                {
                    csv.WriteRecords(envs);
                }
            }
        }

        internal List<Env> ImportEnvsFromCsv(string filePath)
        {
            using (var reader = new StreamReader(filePath))
            {
                using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
                {
                    return csv.GetRecords<Env>().ToList();
                }
            }
        }
    }
}
