using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using CsvHelper;

namespace Boostera
{
    public class EnvManager
    {
        private List<string> running = new List<string>();
        public bool IsRunning { get { return running.Count != 0; } }

        public async Task SetEnv(Env env)
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

        public async Task SetEnvs(List<Env> envs)
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

        public void ExportEnvsToCsv(List<Env> envs, string filePath)
        {
            using (var writer = new StreamWriter(filePath))
            {
                using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
                {
                    csv.WriteRecords(envs);
                }
            }
        }

        public List<Env> ImportEnvsFromCsv(string filePath)
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
