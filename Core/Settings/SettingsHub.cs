using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Hubris.Variable;

namespace Hubris
{
    /// <summary>
    /// Used to define and store settings as variables, and provide an interfact for modification
    /// that relays changes to the appropriate objects
    /// </summary>
    public class SettingsHub
    {
        #region Constants
        ///--------------------------------------------------------------------
        /// 
        /// Settings defaults as constants
        /// Defaults for variables should have one-to-one parity with the 
        /// static Variable array
        /// 
        ///--------------------------------------------------------------------

        public const int MIN_STRING_SIZE = 4;

        public const float DEF_SENS = 1.0f;
        public const bool DEF_MSMOOTH = false, DEF_USEACCEL = true, DEF_DEBUG = false;
        #endregion Constants

        #region VariableArray
        ///---------------------------------------------------------------------
        ///
        /// Array of Variables, initialized with InitVars()
        /// 
        ///--------------------------------------------------------------------

        private Variable[] varArr = InitVars();

        [ExecuteInEditMode]
        private static Variable[] InitVars()
        {
            Variable[] vars = new Variable[(int)VarType.Num_Vars]; // Use Num_Vars to ensure proper array length

            vars[(int)VarType.None] = new Variable("none", VarData.OBJECT, VarType.None, null);

            // Player settings
            vars[(int)VarType.Sens] = new Variable("sensitivity", VarData.FLOAT, VarType.Sens, DEF_SENS);
            vars[(int)VarType.MSmooth] = new Variable("msmooth", VarData.BOOL, VarType.MSmooth, DEF_MSMOOTH);

            // Dev settings
            vars[(int)VarType.Useaccel] = new Variable("useaccel", VarData.BOOL, VarType.Useaccel, DEF_USEACCEL);
            vars[(int)VarType.Debug] = new Variable("debug", VarData.BOOL, VarType.Debug, DEF_DEBUG);

            return vars;
        }
        #endregion VariableArray

        #region QuickVariables
        ///---------------------------------------------------------------------
        /// 
        /// Static Command methods for fetching commands by index, type, or 
        /// specific properties for each command
        /// 
        ///---------------------------------------------------------------------

        public Variable None
        {
            get { return varArr[(int)VarType.None]; }
        }

        public Variable Sens
        {
            get { return varArr[(int)VarType.Sens]; }
        }

        public Variable MSmooth
        {
            get { return varArr[(int)VarType.MSmooth]; }
        }

        public Variable Useaccel
        {
            get { return varArr[(int)VarType.Useaccel]; }
        }

        public Variable Debug
        {
            get { return varArr[(int)VarType.Debug]; }
        }
        #endregion QuickVariables


        public Variable GetVariable(int index)
        {
            if (index >= 0 && index < varArr.Length)
                return varArr[index];
            else
            {
                LocalConsole.Instance.LogError("HubrisSettings GetVariable(): Invalid index requested, returning Variable.None");
                return varArr[(int)VarType.None];
            }
        }

        public Variable GetVariable(VarType type)
        {
            int index = (int)type;
            if (index >= 0 && index < varArr.Length)
                return varArr[index];
            else
            {
                LocalConsole.Instance.LogError("HubrisSettings GetVariable(): Invalid index requested, returning Variable.None");
                return varArr[(int)VarType.None];
            }
        }

        /// <summary>
        /// Searches for a variable by name (string param), non-case-sensitive
        /// </summary>
        public Variable GetVarByName(string nName)
        {
            Variable varObj = varArr[(int)VarType.None];

            // Test only the first four characters in standardized lower case
            string varTest = nName.Substring(0, MIN_STRING_SIZE).ToLower();

            for (int i = 0; i < varArr.Length; i++)
            {
                if (varArr[i].Name.Substring(0, MIN_STRING_SIZE).Equals(varTest))
                {
                    varObj = varArr[i];
                    break;  // Found it, don't need to keep searching
                }
            }

            return varObj;
        }

        /// <summary>
        /// Pushes changes to a variable into the array; UpdateDirtyVar() must then be called manually
        /// </summary>
        public void PushChanges(VarType nType, object nData)
        {
            int index = (int) nType;
            if(index >= 0 && index < varArr.Length)
            {
                varArr[index].Data = nData;
            }
        }

        /// <summary>
        /// Checks if the variable specified by the VarType param is dirty, and if so, sends the new value
        /// </summary>
        public bool UpdateDirtyVar(VarType nType)
        {
            if (nType != VarType.None)  // Let's not waste our time
            {
                int index = (int)nType;
                bool success = true;

                if (varArr[index].Dirty)
                {
                    // VarType determines where the new value is sent
                    switch (nType)
                    {
                        case VarType.Sens:
                            HubrisPlayer.Instance.Sensitivity = (float)varArr[index].Data;
                            break;
                        case VarType.MSmooth:
                            HubrisPlayer.Instance.MSmooth = (bool)varArr[index].Data;
                            break;
                        case VarType.Useaccel:
                            HubrisPlayer.Instance.Movement.SetUseAccel((bool)varArr[index].Data);
                            break;
                        case VarType.Debug:
                            HubrisCore.Instance.DebugToggle();
                            break;
                        default:
                            success = false;
                            break;
                    }

                    if (success)
                    {
                        LocalConsole.Instance.Log(varArr[index].Name + " : " + varArr[index].Data);
                        varArr[index].CleanVar();
                    }
                    else
                        LocalConsole.Instance.LogWarning("HubrisSettings UpdateDirtyVar(): No corresponding behavior for dirty variable at array index " + index);
                }
                else
                {
                    success = false;
                    LocalConsole.Instance.LogWarning("HubrisSettings UpdateDirtyVar(): Variable at array index " + index + " was not dirty");
                }

                return success;
            }
            else
                return false;
        }

        /// <summary>
        /// Updates all dirty variables in the static array
        /// </summary>
        public void UpdateAllDirtyVars()
        {
            for (int i = 0; i < varArr.Length; i++)
            {
                UpdateDirtyVar((VarType)i);
            }
        }
    }
}
