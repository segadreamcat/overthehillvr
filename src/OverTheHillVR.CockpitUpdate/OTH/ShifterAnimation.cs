using System.Numerics;
using Il2Cpp;
using Il2CppFusion;
using Il2CppInterop.Runtime.InteropTypes;
using Il2CppVehiclePhysics;
using MelonLoader;
using UnityEngine;

namespace OTH;

internal sealed class ShifterAnimation
{
	private readonly ShifterMotion motion = new ShifterMotion();

	private Transform main;

	private Transform interior;

	private Transform transfer;

	private Quaternion mainRest;

	private float transferPitch;

	private static readonly Vector3 TransferBase = new Vector3(0.095f, 0.8f, -0.07f);

	internal static MelonPreferences_Entry<bool> Enabled;

	internal void Update()
	{
		//IL_0039: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_018c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0270: Unknown result type (might be due to invalid IL or missing references)
		//IL_01aa: Unknown result type (might be due to invalid IL or missing references)
		if (!CockpitUpdate.Read<bool>("inCockpit"))
		{
			return;
		}
		NetworkedVehicleStatus val = CockpitUpdate.Read<NetworkedVehicleStatus>("carStatus");
		if ((Object)(object)val == (Object)null)
		{
			return;
		}
		int num = val.GearboxGear;
		FunselektorFusionVPStandardInput nativeInput = CockpitUpdate.NativeInput;
		if ((Object)(object)nativeInput != (Object)null && NetworkBool.op_Implicit(nativeInput.inputIsEnabled) && CockpitUpdate.OwnsVehicle && nativeInput.SelectedManualGear != 0)
		{
			num = nativeInput.SelectedManualGear;
		}
		bool park = num == 0 && (Object)(object)val.vehicle != (Object)null && val.vehicle.data.Get(Channel.Input, InputData.AutomaticGear) == 1;
		GameObject val2 = CockpitUpdate.Read<GameObject>("shifterGo");
		if ((Object)(object)val2 != (Object)null && ((Object)(object)main == (Object)null || ((Il2CppObjectBase)(object)main).Pointer != ((Il2CppObjectBase)(object)val2.transform).Pointer))
		{
			main = val2.transform;
			mainRest = main.localRotation;
			motion.Reset(num, park);
			CockpitUpdate.Self.Log("Gear lever animation bound to the cockpit shifter.");
		}
		GameObject val3 = CockpitUpdate.Read<GameObject>("interiorGo");
		if ((Object)(object)val3 != (Object)null && ((Object)(object)interior == (Object)null || ((Il2CppObjectBase)(object)interior).Pointer != ((Il2CppObjectBase)(object)val3.transform).Pointer))
		{
			interior = val3.transform;
			transfer = null;
			BindTransfer(val.GearboxIsLowTransferCase);
		}
		if (!Enabled.Value)
		{
			if ((Object)(object)main != (Object)null)
			{
				main.localRotation = mainRest;
			}
			if ((Object)(object)transfer != (Object)null)
			{
				transfer.localRotation = Quaternion.identity;
			}
			return;
		}
		float deltaTime = Time.deltaTime;
		if ((Object)(object)main != (Object)null)
		{
			Vector2 vector = motion.Update(num, park, deltaTime);
			main.localRotation = Quaternion.Euler(vector.X, 0f, vector.Y) * mainRest;
		}
		if ((Object)(object)transfer != (Object)null)
		{
			float num2 = 1f - Mathf.Exp((0f - Mathf.Clamp(deltaTime, 0f, 0.1f)) / 0.07f);
			transferPitch = Mathf.Lerp(transferPitch, ShifterMotion.TransferPitch(val.GearboxIsLowTransferCase), num2);
			transfer.localRotation = Quaternion.Euler(transferPitch, 0f, 0f);
		}
	}

	private void BindTransfer(bool low)
	{
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Expected Obj, but got Unknown
		//IL_0058: Unknown result type (might be due to invalid IL or missing references)
		//IL_013b: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0102: Unknown result type (might be due to invalid IL or missing references)
		Transform val = interior.Find("part_transfer_trim");
		Transform val2 = interior.Find("part_transfer_blackleather");
		if ((Object)(object)val == (Object)null || (Object)(object)val2 == (Object)null)
		{
			return;
		}
		GameObject val3 = new GameObject("OTHVR Animated Transfer Lever");
		val3.transform.SetParent(interior, false);
		val3.transform.localPosition = TransferBase;
		transfer = val3.transform;
		(Transform, string)[] array = new (Transform, string)[2]
		{
			(val, "part_trim"),
			(val2, "part_blackleather")
		};
		for (int i = 0; i < array.Length; i++)
		{
			(Transform, string) tuple = array[i];
			Transform val4 = interior.Find(tuple.Item2);
			if ((Object)(object)val4 != (Object)null)
			{
				((Renderer)((Component)tuple.Item1).GetComponent<MeshRenderer>()).sharedMaterial = ((Renderer)((Component)val4).GetComponent<MeshRenderer>()).sharedMaterial;
			}
			tuple.Item1.SetParent(transfer, false);
			tuple.Item1.localPosition = -TransferBase;
		}
		transferPitch = ShifterMotion.TransferPitch(low);
		transfer.localRotation = Quaternion.Euler(transferPitch, 0f, 0f);
		CockpitUpdate.Self.Log("4H/4L transfer lever animation bound to the original interior parts.");
	}

	static ShifterAnimation()
	{
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
	}
}
