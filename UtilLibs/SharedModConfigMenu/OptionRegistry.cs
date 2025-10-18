//using PeterHan.PLib.Core;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//namespace UtilLibs.SharedModConfigMenu
//{
//	internal class OptionRegistry
//	{
//		const string GettersKey = "SMCM_Option_Getters";
//		const string SettersKey = "SMCM_Option_Setters";
//		const string TypesKey = "SMCM_Option_Types";

//		public static void RegisterOption<T>(string optionID, Func<T> getValue, Action<T> onSetValue, bool namespacedConfig = true)
//		{
//			if (namespacedConfig)
//				optionID = IO_Utils.ModID + "_" + optionID;

//			var getters = PRegistry.GetData<Dictionary<string, Func<object>>>(GettersKey) ?? new Dictionary<string, Func<object>>();
//			if(getters.ContainsKey(optionID))
//			{
//				SgtLogger.warning($"Option with ID {optionID} is already registered! Overwriting getter.");
//				getters[optionID] = () => getValue();
//			}
//			else
//			{
//				getters.Add(optionID, () => getValue());
//			}
//			PRegistry.PutData(GettersKey, getters);

//			var setters = PRegistry.GetData<Dictionary<string, Action<object>>>(SettersKey) ?? new Dictionary<string, Action<object>>();
//			if (setters.ContainsKey(optionID))
//			{
//				SgtLogger.warning($"Option with ID {optionID} is already registered! Overwriting setter.");
//				setters[optionID] = (obj) => onSetValue((T)obj);
//			}
//			else
//			{
//				setters.Add(optionID, (obj) => onSetValue((T)obj));
//			}

//			PRegistry.PutData(SettersKey, setters);
//			var types = PRegistry.GetData<Dictionary<string, Type>>(TypesKey) ?? new Dictionary<string, Type>();
//			if (types.ContainsKey(optionID))
//			{
//				SgtLogger.warning($"Option with ID {optionID} is already registered! Overwriting type.");
//				types[optionID] = typeof(T);
//			}
//			else
//			{
//				types.Add(optionID, typeof(T));
//			}
//		}
//	}
//}
