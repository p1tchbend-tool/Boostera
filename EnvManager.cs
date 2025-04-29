using System;
using System.Collections.Generic;
using System.Threading.Tasks;

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
    }
}
