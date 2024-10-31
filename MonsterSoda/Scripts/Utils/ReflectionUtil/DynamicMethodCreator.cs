using System.Reflection;
using System.Reflection.Emit;

namespace MonsterSoda.Utils.ReflectionUtil
{
	public static class DynamicMethodCreator
	{
		public static DynamicMethod GetDynamicMethod(MethodInfo methodInfo, string functionName)
		{
			//// Get MethodInfo of the base method
			//MethodInfo methodInfo = typeof(GrabbableObject).GetMethod("EquipItem", BindingFlags.Instance | BindingFlags.Public);
			
			// Create DynamicMethod based on the methodInfo
			DynamicMethod dynamicMethod = new DynamicMethod(functionName, methodInfo.ReturnType, new[] { methodInfo.DeclaringType }, methodInfo.DeclaringType);

			// Create ILGenerator for the dynamic method
			ILGenerator il = dynamicMethod.GetILGenerator();

			// Emit argument with index 0 - DeclaringType
			il.Emit(OpCodes.Ldarg_0);

			// Emit method call
			il.EmitCall(OpCodes.Call, methodInfo, null);

			// Emit method return value
			il.Emit(OpCodes.Ret);

			return dynamicMethod;
		}
	}
}