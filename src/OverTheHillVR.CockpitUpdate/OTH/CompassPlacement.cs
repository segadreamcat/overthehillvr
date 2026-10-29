using System.Reflection;
using Il2CppFusion;

namespace OTH;

internal sealed class CompassPlacement
{
	private bool raised;

	private float originalDown;

	private FieldInfo down;

	private FieldInfo place;

	internal void Update()
	{
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		bool flag = CockpitUpdate.Read<bool>("vrOn") && CockpitUpdate.Read<bool>("inCockpit") && CockpitUpdate.IsDriving && NetworkBool.op_Implicit(CockpitUpdate.NativeInput.inputIsEnabled);
		SetRaised(flag);
	}

	internal void SetRaised(bool value)
	{
		if (raised != value)
		{
			if ((object)down == null)
			{
				down = CockpitUpdate.DriverType.GetField("sHudDown", BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
			}
			if ((object)place == null)
			{
				place = CockpitUpdate.DriverType.GetField("sPlaceHud", BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
			}
			if (value)
			{
				originalDown = (float)down.GetValue(null);
			}
			down.SetValue(null, value ? (originalDown - 0.22f) : originalDown);
			place.SetValue(null, true);
			raised = value;
			if (value)
			{
				CockpitUpdate.Self.Log("Driving HUD raised 22 cm to clear the compass above the rearview mirror.");
			}
		}
	}

	internal void Restore()
	{
		SetRaised(value: false);
	}
}
