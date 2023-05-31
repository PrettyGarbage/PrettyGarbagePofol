using UnityEngine;
using System;

namespace EzNetLibrary
{
    [AttributeUsage(AttributeTargets.Field)]
	public class ReadOnlyAttribute : PropertyAttribute
	{
		public readonly bool runtimeOnly;

		public ReadOnlyAttribute(bool runtimeOnly = false)
		{
			this.runtimeOnly = runtimeOnly;
		}
	}
}