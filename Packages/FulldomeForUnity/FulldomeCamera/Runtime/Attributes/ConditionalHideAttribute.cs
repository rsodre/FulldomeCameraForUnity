using UnityEngine;
using System;
using System.Collections;

namespace Avante
{
	// From: http://www.brechtos.com/hiding-or-disabling-inspector-properties-using-propertydrawers-within-unity-5/

	[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property | AttributeTargets.Class | AttributeTargets.Struct, Inherited = true)]
	public class ConditionalHideAttribute : PropertyAttribute
	{
		//The name of the bool field that will be in control
		public string ConditionalSourceField = "";
		//TRUE = Hide in inspector / FALSE = Disable in inspector 
		public bool HideInInspector = false;

		public ConditionalHideAttribute(string conditionalSourceField)
		{
			this.ConditionalSourceField = conditionalSourceField;
			this.HideInInspector = false;
		}

		public ConditionalHideAttribute(string conditionalSourceField, bool hideInInspector)
		{
			this.ConditionalSourceField = conditionalSourceField;
			this.HideInInspector = hideInInspector;
		}
	}	
}
