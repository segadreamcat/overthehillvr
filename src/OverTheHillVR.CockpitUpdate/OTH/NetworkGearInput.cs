using Il2Cpp;
using Il2CppFusion;
using UnityEngine;

namespace OTH;

internal static class NetworkGearInput
{
	internal static bool Queue(VehicleInput input, int direction)
	{
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0028: Unknown result type (might be due to invalid IL or missing references)
		if ((Object)(object)input == (Object)null || direction == 0)
		{
			return false;
		}
		NetworkButtons buttons = input.buttons;
		buttons.Set((direction > 0) ? 10 : 9, true);
		input.buttons = buttons;
		return true;
	}
}
