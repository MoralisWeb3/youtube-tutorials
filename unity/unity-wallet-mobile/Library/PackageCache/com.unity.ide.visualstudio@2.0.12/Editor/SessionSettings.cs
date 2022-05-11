/*---------------------------------------------------------------------------------------------
 *  Copyright (c) Microsoft Corporation. All rights reserved.
 *  Licensed under the MIT License. See License.txt in the project root for license information.
 *--------------------------------------------------------------------------------------------*/

using System.Runtime.CompilerServices;
using UnityEditor;
using UnityEngine;

namespace Microsoft.Unity.VisualStudio.Editor
{
	internal static class SessionSettings
	{
		internal static string GetKey([CallerMemberName] string memberName = "")
		{
			return $"{typeof(SessionSettings).FullName}.{memberName}";
		}

		public static bool PackageVersionChecked
		{
			get
			{
				return SessionState.GetBool(GetKey(), false);
			}
			set
			{
				SessionState.SetBool(GetKey(), value);
			}
		}
	}
}
