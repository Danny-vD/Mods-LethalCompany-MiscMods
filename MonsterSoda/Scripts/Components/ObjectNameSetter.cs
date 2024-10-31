using UnityEngine;

namespace MonsterSoda.Components
{
	public class ObjectNameSetter : MonoBehaviour
	{
		private ScanNodeProperties scanNodeProperties;
		private GrabbableObject grabbableObject;

		private void Awake()
		{
			scanNodeProperties = GetComponentInChildren<ScanNodeProperties>();
			grabbableObject    = GetComponentInChildren<GrabbableObject>();
		}

		public void SetName(string newName)
		{
			if (scanNodeProperties == null)
			{
				Awake();
			}

			if (scanNodeProperties)
			{
				scanNodeProperties.headerText = newName;
			}

			grabbableObject.itemProperties.itemName = newName;
		}
	}
}