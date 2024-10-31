using System;
using System.Collections.Generic;
using System.Reflection;
using Unity.Netcode;

namespace ExtraInformation.Utils
{
	internal enum __RpcExecStage
	{
		None,
		Server,
		Client,
	}

	internal static class NetCodeUtil
	{
		private const string fieldName = "__rpc_exec_stage";
		
		private static readonly Dictionary<__RpcExecStage, object> cachedEnumValues = new Dictionary<__RpcExecStage, object>();
		private static bool isSetupDone = false;

		private static void Setup(FieldInfo field)
		{
			// Get the type of the protected enum
			Type enumType = field.FieldType;

			// Cache the parsed enum values
			foreach (__RpcExecStage rpcExecStage in Enum.GetValues(typeof(__RpcExecStage)))
			{
				cachedEnumValues[rpcExecStage] = Enum.Parse(enumType, rpcExecStage.ToString());
			}

			isSetupDone = true;
		}

		internal static bool IsRpcExecState(NetworkBehaviour networkBehaviour, __RpcExecStage rpcExecStage)
		{
			FieldInfo field = networkBehaviour.GetType().GetField(fieldName, BindingFlags.Instance | BindingFlags.NonPublic);

			if (!isSetupDone)
			{
				Setup(field);
			}

			object boxedEnumValue = field!.GetValue(networkBehaviour);

			object enumValue = cachedEnumValues[rpcExecStage];

			return boxedEnumValue == enumValue;
		}
	}
}