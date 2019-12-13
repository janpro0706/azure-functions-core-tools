﻿using Azure.Functions.Cli.Kubernetes.Models.Kubernetes;
<<<<<<< HEAD
using Colors.Net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
=======
using System;
using System.Collections.Generic;
using System.Linq;
>>>>>>> Function keys in Kubernetes
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Azure.Functions.Cli.Kubernetes.FuncKeys
{
    public class FuncAppKeysHelper
    {
        private const string FuncAppKeysVolumeName = "functions-keys-volume";
        private const string KubernetesSecretsMountPath = "/run/secrets/functions-keys";
        private const string AzureWebJobsSecretStorageTypeEnvVariableName = "AzureWebJobsSecretStorageType";
        private const string AzureWebJobsKubernetesSecretNameEnvVariableName = "AzureWebJobsKubernetesSecretName";
        private const string MasterKey = "host.master";
        private const string HostFunctionKey = "host.function.default";
        private const string HostSystemKey = "host.systemKey.default";
        private const string FunctionKeyPrefix = "functions.";
        private const string FunctionDefaultKeyName = "default";
        /// <summary>
        /// Implementation of this method creates the Host and Function Keys
        /// </summary>
        /// <param name="functionNames">The <see cref="IEnumerable{string}"></see> of function names </param>
        /// <returns>The <see cref="IDictionary{string, string}"/> of function app's host and function keys</returns>
        public static IDictionary<string, string> CreateKeys(IEnumerable<string> functionNames)
        {
            var funcAppKeys = new Dictionary<string, string>
            {
                { MasterKey, GenerateKey() },
                { HostFunctionKey, GenerateKey() },
                { HostSystemKey, GenerateKey() }
            };

            if (functionNames?.Any() == true)
            {
                foreach (var funcName in functionNames)
                {
                    funcAppKeys[$"{FunctionKeyPrefix}{funcName}.{FunctionDefaultKeyName}"] = GenerateKey();
                }
            }

            return funcAppKeys;
        }

<<<<<<< HEAD
        public static IDictionary<string, string> FuncKeysKubernetesEnvironVariables(string keysSecretCollectionName, bool mountKeysAsContainerVolume)
        {
            var funcKeysKubernetesEnvironVariables = new Dictionary<string, string>
            {
                { AzureWebJobsSecretStorageTypeEnvVariableName, "kubernetes" }
            };

            //if keys needs are not to be mounted as container volume then add "AzureWebJobsKubernetesSecretName" enviornment varibale to the container 
            if (!mountKeysAsContainerVolume)
            {
                funcKeysKubernetesEnvironVariables.Add(AzureWebJobsKubernetesSecretNameEnvVariableName, $"secrets/{keysSecretCollectionName}");
            }

            return funcKeysKubernetesEnvironVariables;
        }

        public static void CreateFuncAppKeysVolumeMountDeploymentResource(IEnumerable<DeploymentV1Apps> deployments, string funcAppKeysSecretsCollectionName)
=======
        public static void AddAppKeysEnvironVariableNames(IDictionary<string, string> envVariables,
            string funcAppKeysSecretsCollectionName,
            string funcAppKeysConfigMapName,
            bool mountFuncKeysAsContainerVolume)
        {
            if (envVariables == null)
            {
                envVariables = new Dictionary<string, string>();
            }

            //if funcAppKeysSecretsCollectionName, funcAppKeysConfigMapName or mountFuncKeysAsContainerVolume has been assigned then that means the func app keys needs to be managed as kubernetes secret/configMap
            if ((!string.IsNullOrWhiteSpace(funcAppKeysSecretsCollectionName) || !string.IsNullOrWhiteSpace(funcAppKeysConfigMapName) || mountFuncKeysAsContainerVolume)
                && !envVariables.ContainsKey(AzureWebJobsSecretStorageTypeEnvVariableName))
            {
                envVariables.Add(AzureWebJobsSecretStorageTypeEnvVariableName, "kubernetes");
            }

            if (!envVariables.ContainsKey(AzureWebJobsKubernetesSecretNameEnvVariableName)
                && !mountFuncKeysAsContainerVolume)
            {
                if (!string.IsNullOrWhiteSpace(funcAppKeysSecretsCollectionName))
                {
                    envVariables.Add(AzureWebJobsKubernetesSecretNameEnvVariableName, $"secrets/{funcAppKeysSecretsCollectionName}");
                }
                else if (!string.IsNullOrWhiteSpace(funcAppKeysConfigMapName))
                {
                    envVariables.Add(AzureWebJobsKubernetesSecretNameEnvVariableName, $"configmaps/{funcAppKeysConfigMapName}");
                }
            }
        }

        public static void CreateFuncAppKeysVolumeMountDeploymentResource(IEnumerable<DeploymentV1Apps> deployments,
            string funcAppKeysSecretsCollectionName,
            string funcAppKeysConfigMapName)
>>>>>>> Function keys in Kubernetes
        {
            if (deployments?.Any() == false)
            {
                return;
            }

            var volume = new VolumeV1
            {
                Name = FuncAppKeysVolumeName
            };

            if (!string.IsNullOrWhiteSpace(funcAppKeysSecretsCollectionName))
            {
                volume.VolumeSecret = new VolumeSecretV1 { SecretName = funcAppKeysSecretsCollectionName };
            }
<<<<<<< HEAD

            //Mount the app keys as volume mount to the container at the path "/run/secrets/functions-keys"
=======
            else if (!string.IsNullOrWhiteSpace(funcAppKeysConfigMapName))
            {
                volume.VolumeConfigMap = new VolumeConfigMapV1 { Name = funcAppKeysSecretsCollectionName };
            }

>>>>>>> Function keys in Kubernetes
            foreach (var deployment in deployments)
            {
                deployment.Spec.Template.Spec.Volumes = new VolumeV1[] { volume };
                deployment.Spec.Template.Spec.Containers.First().VolumeMounts = new ContainerVolumeMountV1[]
                {
                        new ContainerVolumeMountV1
                        {
                            Name = FuncAppKeysVolumeName,
                            MountPath = KubernetesSecretsMountPath
                        }
                };
            }
        }

<<<<<<< HEAD
        public static void FunKeysMessage(SecretsV1 existingKeysSecret, SecretsV1 newKeysSecret)
        {
            if (existingKeysSecret?.Data?.Any() == true || newKeysSecret?.Data?.Any() == true)
            {
                ColoredConsole.WriteLine("Http Functions:");
            }

            if (existingKeysSecret?.Data?.Any() == true)
            {
                var existingFunctionKeys = existingKeysSecret.Data.Where(item => item.Key.StartsWith("functions"));
                PrintKeyOutputMessage(existingFunctionKeys, " # this didn't change");
            }

            if (newKeysSecret?.Data?.Any() == true)
            {
                var newFunctionKeys = newKeysSecret.Data.Where(item => item.Key.StartsWith("functions"));
                PrintKeyOutputMessage(newFunctionKeys, " # this was added");
            }
        }

        private static void PrintKeyOutputMessage(IEnumerable<KeyValuePair<string, string>> functionKeys, string keyMsg)
        {
            var keyOutputMsgTemplate = "http://[ip]/api/{0}?code={1}";
            foreach (var funcKey in functionKeys)
            {
                var functionName = funcKey.Key.Split('.')[1];
                var functionKey = Encoding.UTF8.GetString(Convert.FromBase64String(funcKey.Value));
                ColoredConsole.WriteLine(string.Concat("\t", string.Format(keyOutputMsgTemplate, functionName, functionKey), keyMsg));
            }
        }

=======
>>>>>>> Function keys in Kubernetes
        private static string GenerateKey()
        {
            using (var rng = RandomNumberGenerator.Create())
            {
                byte[] data = new byte[40];
                rng.GetBytes(data);
                string secret = Convert.ToBase64String(data);

                // Replace pluses as they are problematic as URL values
                return secret.Replace('+', 'a');
            }
        }
    }
}
