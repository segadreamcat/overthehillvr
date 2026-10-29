Add-Type -Path "D:\Projects\overthehillvr-fan\MelonLoader\net6\Mono.Cecil.dll"
Add-Type -Path "D:\Projects\overthehillvr-fan\MelonLoader\net6\Mono.Cecil.Rocks.dll"

$dllPath = "D:\Projects\overthehillvr-fan\Mods\OverTheHillVR.dll"

# Always restore clean base from git commit b76920b to ensure pristine IL
Write-Output "Restoring clean base OverTheHillVR.dll from commit b76920b..."
git -C "D:\Projects\overthehillvr-fan" checkout b76920b -- Mods/OverTheHillVR.dll

$rawBytes = [System.IO.File]::ReadAllBytes($dllPath)
$memStream = New-Object System.IO.MemoryStream($rawBytes, $true)
$asm = [Mono.Cecil.AssemblyDefinition]::ReadAssembly($memStream)
$mod = $asm.MainModule
$driverType = $mod.GetType("OverTheHillVR.VrDriver")
$vmathType = $mod.GetType("OverTheHillVR.VMath")

Write-Output "Patching OverTheHillVR.VrDriver for Fanatec Direct Drive safeties and Guardian recenter..."

# 1. Add fields: isDirectDrive (bool), ffbLastTick (float), prevRawPos (Vector3), prevRawYaw (float), prevRawValid (bool)
$f_isDD = New-Object Mono.Cecil.FieldDefinition("isDirectDrive", [Mono.Cecil.FieldAttributes]::Private, $mod.TypeSystem.Boolean)
$driverType.Fields.Add($f_isDD)

$f_lastTick = New-Object Mono.Cecil.FieldDefinition("ffbLastTick", [Mono.Cecil.FieldAttributes]::Private, $mod.TypeSystem.Single)
$driverType.Fields.Add($f_lastTick)

$vec3Type = ($driverType.Fields | Where-Object { $_.Name -eq "hmdPos" }).FieldType
$f_prevPos = New-Object Mono.Cecil.FieldDefinition("prevRawPos", [Mono.Cecil.FieldAttributes]::Private, $vec3Type)
$driverType.Fields.Add($f_prevPos)

$f_prevYaw = New-Object Mono.Cecil.FieldDefinition("prevRawYaw", [Mono.Cecil.FieldAttributes]::Private, $mod.TypeSystem.Single)
$driverType.Fields.Add($f_prevYaw)

$f_prevValid = New-Object Mono.Cecil.FieldDefinition("prevRawValid", [Mono.Cecil.FieldAttributes]::Private, $mod.TypeSystem.Boolean)
$driverType.Fields.Add($f_prevValid)
Write-Output "Added fields: isDirectDrive, ffbLastTick, prevRawPos, prevRawYaw, prevRawValid"

# 2. Add SafeFfb static method
$attrs = [Mono.Cecil.MethodAttributes]([int][Mono.Cecil.MethodAttributes]::Public -bor [int][Mono.Cecil.MethodAttributes]::Static)
$m_safeFfb = New-Object Mono.Cecil.MethodDefinition("SafeFfb", $attrs, $mod.TypeSystem.Int32)
$m_safeFfb.Parameters.Add((New-Object Mono.Cecil.ParameterDefinition("targetConst", [Mono.Cecil.ParameterAttributes]::None, $mod.TypeSystem.Int32)))
$m_safeFfb.Parameters.Add((New-Object Mono.Cecil.ParameterDefinition("lastConst", [Mono.Cecil.ParameterAttributes]::None, $mod.TypeSystem.Int32)))
$m_safeFfb.Parameters.Add((New-Object Mono.Cecil.ParameterDefinition("dt", [Mono.Cecil.ParameterAttributes]::None, $mod.TypeSystem.Single)))
$m_safeFfb.Parameters.Add((New-Object Mono.Cecil.ParameterDefinition("driving", [Mono.Cecil.ParameterAttributes]::None, $mod.TypeSystem.Boolean)))
$m_safeFfb.Parameters.Add((New-Object Mono.Cecil.ParameterDefinition("focused", [Mono.Cecil.ParameterAttributes]::None, $mod.TypeSystem.Boolean)))
$m_safeFfb.Parameters.Add((New-Object Mono.Cecil.ParameterDefinition("isDD", [Mono.Cecil.ParameterAttributes]::None, $mod.TypeSystem.Boolean)))

$m_safeFfb.Body.InitLocals = $true
$m_safeFfb.Body.Variables.Add((New-Object Mono.Cecil.Cil.VariableDefinition($mod.TypeSystem.Int32))) # 0: baseline
$m_safeFfb.Body.Variables.Add((New-Object Mono.Cecil.Cil.VariableDefinition($mod.TypeSystem.Int32))) # 1: maxDelta
$m_safeFfb.Body.Variables.Add((New-Object Mono.Cecil.Cil.VariableDefinition($mod.TypeSystem.Int32))) # 2: delta

$il = $m_safeFfb.Body.GetILProcessor()
$lbl_retZero = $il.Create([Mono.Cecil.Cil.OpCodes]::Ldc_I4_0)
$lbl_skipDD = $il.Create([Mono.Cecil.Cil.OpCodes]::Ldarg_1)
$lbl_notInit = $il.Create([Mono.Cecil.Cil.OpCodes]::Ldarg_1)
$lbl_storeBase = $il.Create([Mono.Cecil.Cil.OpCodes]::Stloc_0)
$lbl_skipMin = $il.Create([Mono.Cecil.Cil.OpCodes]::Conv_I4)
$lbl_checkNeg = $il.Create([Mono.Cecil.Cil.OpCodes]::Ldloc_2)
$lbl_clampBounds = $il.Create([Mono.Cecil.Cil.OpCodes]::Ldarg_0)
$lbl_checkMin = $il.Create([Mono.Cecil.Cil.OpCodes]::Ldarg_0)
$lbl_retTarget = $il.Create([Mono.Cecil.Cil.OpCodes]::Ldarg_0)

# If (!driving) return 0;
$il.Emit([Mono.Cecil.Cil.OpCodes]::Ldarg_3)
$il.Emit([Mono.Cecil.Cil.OpCodes]::Brfalse, $lbl_retZero)
# If (!focused) return 0;
$il.Emit([Mono.Cecil.Cil.OpCodes]::Ldarg, $m_safeFfb.Parameters[4])
$il.Emit([Mono.Cecil.Cil.OpCodes]::Brfalse, $lbl_retZero)

# If (isDD) targetConst = (int)(targetConst * 0.65f);
$il.Emit([Mono.Cecil.Cil.OpCodes]::Ldarg, $m_safeFfb.Parameters[5])
$il.Emit([Mono.Cecil.Cil.OpCodes]::Brfalse, $lbl_skipDD)
$il.Emit([Mono.Cecil.Cil.OpCodes]::Ldarg_0)
$il.Emit([Mono.Cecil.Cil.OpCodes]::Conv_R4)
$il.Emit([Mono.Cecil.Cil.OpCodes]::Ldc_R4, [float]0.65)
$il.Emit([Mono.Cecil.Cil.OpCodes]::Mul)
$il.Emit([Mono.Cecil.Cil.OpCodes]::Conv_I4)
$il.Emit([Mono.Cecil.Cil.OpCodes]::Starg, $m_safeFfb.Parameters[0])

# Baseline calc
$il.Append($lbl_skipDD)
$il.Emit([Mono.Cecil.Cil.OpCodes]::Ldc_I4, [int]99999)
$il.Emit([Mono.Cecil.Cil.OpCodes]::Bne_Un, $lbl_notInit)
$il.Emit([Mono.Cecil.Cil.OpCodes]::Ldc_I4_0)
$il.Emit([Mono.Cecil.Cil.OpCodes]::Br, $lbl_storeBase)
$il.Append($lbl_notInit)
$il.Append($lbl_storeBase)

# Max slew delta: (int)max(50f, 12000f * dt)
$il.Emit([Mono.Cecil.Cil.OpCodes]::Ldc_R4, [float]12000.0)
$il.Emit([Mono.Cecil.Cil.OpCodes]::Ldarg_2)
$il.Emit([Mono.Cecil.Cil.OpCodes]::Mul)
$il.Emit([Mono.Cecil.Cil.OpCodes]::Dup)
$il.Emit([Mono.Cecil.Cil.OpCodes]::Ldc_R4, [float]50.0)
$il.Emit([Mono.Cecil.Cil.OpCodes]::Bge_Un, $lbl_skipMin)
$il.Emit([Mono.Cecil.Cil.OpCodes]::Pop)
$il.Emit([Mono.Cecil.Cil.OpCodes]::Ldc_R4, [float]50.0)
$il.Append($lbl_skipMin)
$il.Emit([Mono.Cecil.Cil.OpCodes]::Stloc_1)

# Delta: targetConst - baseline
$il.Emit([Mono.Cecil.Cil.OpCodes]::Ldarg_0)
$il.Emit([Mono.Cecil.Cil.OpCodes]::Ldloc_0)
$il.Emit([Mono.Cecil.Cil.OpCodes]::Sub)
$il.Emit([Mono.Cecil.Cil.OpCodes]::Stloc_2)

# Slew limit clamp
$il.Emit([Mono.Cecil.Cil.OpCodes]::Ldloc_2)
$il.Emit([Mono.Cecil.Cil.OpCodes]::Ldloc_1)
$il.Emit([Mono.Cecil.Cil.OpCodes]::Ble, $lbl_checkNeg)
$il.Emit([Mono.Cecil.Cil.OpCodes]::Ldloc_0)
$il.Emit([Mono.Cecil.Cil.OpCodes]::Ldloc_1)
$il.Emit([Mono.Cecil.Cil.OpCodes]::Add)
$il.Emit([Mono.Cecil.Cil.OpCodes]::Starg, $m_safeFfb.Parameters[0])
$il.Emit([Mono.Cecil.Cil.OpCodes]::Br, $lbl_clampBounds)

$il.Append($lbl_checkNeg)
$il.Emit([Mono.Cecil.Cil.OpCodes]::Ldloc_1)
$il.Emit([Mono.Cecil.Cil.OpCodes]::Neg)
$il.Emit([Mono.Cecil.Cil.OpCodes]::Bge, $lbl_clampBounds)
$il.Emit([Mono.Cecil.Cil.OpCodes]::Ldloc_0)
$il.Emit([Mono.Cecil.Cil.OpCodes]::Ldloc_1)
$il.Emit([Mono.Cecil.Cil.OpCodes]::Sub)
$il.Emit([Mono.Cecil.Cil.OpCodes]::Starg, $m_safeFfb.Parameters[0])

# Hardware boundary clamp [-8500, 8500]
$il.Append($lbl_clampBounds)
$il.Emit([Mono.Cecil.Cil.OpCodes]::Ldc_I4, [int]8500)
$il.Emit([Mono.Cecil.Cil.OpCodes]::Ble, $lbl_checkMin)
$il.Emit([Mono.Cecil.Cil.OpCodes]::Ldc_I4, [int]8500)
$il.Emit([Mono.Cecil.Cil.OpCodes]::Starg, $m_safeFfb.Parameters[0])

$il.Append($lbl_checkMin)
$il.Emit([Mono.Cecil.Cil.OpCodes]::Ldc_I4, [int]-8500)
$il.Emit([Mono.Cecil.Cil.OpCodes]::Bge, $lbl_retTarget)
$il.Emit([Mono.Cecil.Cil.OpCodes]::Ldc_I4, [int]-8500)
$il.Emit([Mono.Cecil.Cil.OpCodes]::Starg, $m_safeFfb.Parameters[0])

$il.Append($lbl_retTarget)
$il.Emit([Mono.Cecil.Cil.OpCodes]::Ret)

$il.Append($lbl_retZero)
$il.Emit([Mono.Cecil.Cil.OpCodes]::Ret)

$m_safeFfb.Body.MaxStackSize = 16
$driverType.Methods.Add($m_safeFfb)
Write-Output "Added method: SafeFfb"

# 3. Add SafeDamper static method
$m_safeDamper = New-Object Mono.Cecil.MethodDefinition("SafeDamper", $attrs, $mod.TypeSystem.Int32)
$m_safeDamper.Parameters.Add((New-Object Mono.Cecil.ParameterDefinition("targetDamper", [Mono.Cecil.ParameterAttributes]::None, $mod.TypeSystem.Int32)))
$m_safeDamper.Parameters.Add((New-Object Mono.Cecil.ParameterDefinition("isDD", [Mono.Cecil.ParameterAttributes]::None, $mod.TypeSystem.Boolean)))
$m_safeDamper.Parameters.Add((New-Object Mono.Cecil.ParameterDefinition("driving", [Mono.Cecil.ParameterAttributes]::None, $mod.TypeSystem.Boolean)))

$ilD = $m_safeDamper.Body.GetILProcessor()
$lbl_dampCheckDD = $ilD.Create([Mono.Cecil.Cil.OpCodes]::Ldarg_1)
$lbl_dampClampMax = $ilD.Create([Mono.Cecil.Cil.OpCodes]::Ldarg_0)
$lbl_dampRet = $ilD.Create([Mono.Cecil.Cil.OpCodes]::Ldarg_0)

# if (!driving) return 0;
$ilD.Emit([Mono.Cecil.Cil.OpCodes]::Ldarg_2)
$ilD.Emit([Mono.Cecil.Cil.OpCodes]::Brtrue, $lbl_dampCheckDD)
$ilD.Emit([Mono.Cecil.Cil.OpCodes]::Ldc_I4_0)
$ilD.Emit([Mono.Cecil.Cil.OpCodes]::Ret)

# if (isDD)
$ilD.Append($lbl_dampCheckDD)
$ilD.Emit([Mono.Cecil.Cil.OpCodes]::Brfalse, $lbl_dampClampMax)
# if (targetDamper < 1200) targetDamper = 1200;
$ilD.Emit([Mono.Cecil.Cil.OpCodes]::Ldarg_0)
$ilD.Emit([Mono.Cecil.Cil.OpCodes]::Ldc_I4, [int]1200)
$ilD.Emit([Mono.Cecil.Cil.OpCodes]::Bge, $lbl_dampClampMax)
$ilD.Emit([Mono.Cecil.Cil.OpCodes]::Ldc_I4, [int]1200)
$ilD.Emit([Mono.Cecil.Cil.OpCodes]::Starg, $m_safeDamper.Parameters[0])

# if (targetDamper > 10000) targetDamper = 10000;
$ilD.Append($lbl_dampClampMax)
$ilD.Emit([Mono.Cecil.Cil.OpCodes]::Ldc_I4, [int]10000)
$ilD.Emit([Mono.Cecil.Cil.OpCodes]::Ble, $lbl_dampRet)
$ilD.Emit([Mono.Cecil.Cil.OpCodes]::Ldc_I4, [int]10000)
$ilD.Emit([Mono.Cecil.Cil.OpCodes]::Starg, $m_safeDamper.Parameters[0])

$ilD.Append($lbl_dampRet)
$ilD.Emit([Mono.Cecil.Cil.OpCodes]::Ret)

$m_safeDamper.Body.MaxStackSize = 8
$driverType.Methods.Add($m_safeDamper)
Write-Output "Added method: SafeDamper"

# 4. Extract existing references from FfbTick
$m_ffbTick = $driverType.Methods | Where-Object { $_.Name -eq "FfbTick" }
$refConst = ($m_ffbTick.Body.Instructions | Where-Object { $_.Operand -ne $null -and $_.Operand.ToString() -like "*UpdateConstantForceSimple*" } | Select-Object -First 1).Operand
$refTime = ($m_ffbTick.Body.Instructions | Where-Object { $_.Operand -ne $null -and $_.Operand.ToString() -like "*get_unscaledTime*" } | Select-Object -First 1).Operand
$refDelta = ($m_ffbTick.Body.Instructions | Where-Object { $_.Operand -ne $null -and $_.Operand.ToString() -like "*get_unscaledDeltaTime*" } | Select-Object -First 1).Operand

# Find Distance ref
$refDist = $null
foreach ($m in $driverType.Methods) {
    foreach ($i in $m.Body.Instructions) {
        if ($i.Operand -ne $null -and $i.Operand.ToString() -like "*Distance(UnityEngine.Vector3*") {
            $refDist = $i.Operand
            break
        }
    }
    if ($refDist -ne $null) { break }
}

# 5. Add WatchdogCheck method to VrDriver
$attrsW = [Mono.Cecil.MethodAttributes]([int][Mono.Cecil.MethodAttributes]::Private)
$m_watchdog = New-Object Mono.Cecil.MethodDefinition("WatchdogCheck", $attrsW, $mod.TypeSystem.Void)

$f_ffbReady = $driverType.Fields | Where-Object { $_.Name -eq "ffbReady" }
$f_ffbDev = $driverType.Fields | Where-Object { $_.Name -eq "ffbDev" }
$f_ffbLastConst = $driverType.Fields | Where-Object { $_.Name -eq "ffbLastConst" }

$ilW = $m_watchdog.Body.GetILProcessor()
$lbl_retW = $ilW.Create([Mono.Cecil.Cil.OpCodes]::Ret)

# if (!this.ffbReady) return;
$ilW.Emit([Mono.Cecil.Cil.OpCodes]::Ldarg_0)
$ilW.Emit([Mono.Cecil.Cil.OpCodes]::Ldfld, $f_ffbReady)
$ilW.Emit([Mono.Cecil.Cil.OpCodes]::Brfalse, $lbl_retW)

# if (this.ffbDev == null) return;
$ilW.Emit([Mono.Cecil.Cil.OpCodes]::Ldarg_0)
$ilW.Emit([Mono.Cecil.Cil.OpCodes]::Ldfld, $f_ffbDev)
$ilW.Emit([Mono.Cecil.Cil.OpCodes]::Brfalse, $lbl_retW)

# if (Time.unscaledTime - this.ffbLastTick <= 0.20f) return;
$ilW.Emit([Mono.Cecil.Cil.OpCodes]::Call, $refTime)
$ilW.Emit([Mono.Cecil.Cil.OpCodes]::Ldarg_0)
$ilW.Emit([Mono.Cecil.Cil.OpCodes]::Ldfld, $f_lastTick)
$ilW.Emit([Mono.Cecil.Cil.OpCodes]::Sub)
$ilW.Emit([Mono.Cecil.Cil.OpCodes]::Ldc_R4, [float]0.20)
$ilW.Emit([Mono.Cecil.Cil.OpCodes]::Ble_Un, $lbl_retW)

# if (this.ffbLastConst == 0) return;
$ilW.Emit([Mono.Cecil.Cil.OpCodes]::Ldarg_0)
$ilW.Emit([Mono.Cecil.Cil.OpCodes]::Ldfld, $f_ffbLastConst)
$ilW.Emit([Mono.Cecil.Cil.OpCodes]::Brfalse, $lbl_retW)

# DIManager.UpdateConstantForceSimple(this.ffbDev, 0);
$ilW.Emit([Mono.Cecil.Cil.OpCodes]::Ldarg_0)
$ilW.Emit([Mono.Cecil.Cil.OpCodes]::Ldfld, $f_ffbDev)
$ilW.Emit([Mono.Cecil.Cil.OpCodes]::Ldc_I4_0)
$ilW.Emit([Mono.Cecil.Cil.OpCodes]::Call, $refConst)
$ilW.Emit([Mono.Cecil.Cil.OpCodes]::Pop)

# this.ffbLastConst = 0;
$ilW.Emit([Mono.Cecil.Cil.OpCodes]::Ldarg_0)
$ilW.Emit([Mono.Cecil.Cil.OpCodes]::Ldc_I4_0)
$ilW.Emit([Mono.Cecil.Cil.OpCodes]::Stfld, $f_ffbLastConst)

$ilW.Append($lbl_retW)
$m_watchdog.Body.MaxStackSize = 8
$driverType.Methods.Add($m_watchdog)
Write-Output "Added method: WatchdogCheck"

# 6. Add CheckGuardianRecenter method to VrDriver
$m_checkRecenter = New-Object Mono.Cecil.MethodDefinition("CheckGuardianRecenter", $attrsW, $mod.TypeSystem.Void)
$f_hmdPos = $driverType.Fields | Where-Object { $_.Name -eq "hmdPos" }
$f_hmdRot = $driverType.Fields | Where-Object { $_.Name -eq "hmdRot" }
$f_hmdValid = $driverType.Fields | Where-Object { $_.Name -eq "hmdValid" }
$f_needRecenter = $driverType.Fields | Where-Object { $_.Name -eq "needRecenter" }
$f_seatValid = $driverType.Fields | Where-Object { $_.Name -eq "seatValid" }
$m_yawOf = $vmathType.Methods | Where-Object { $_.Name -eq "YawOf" }

$ilR = $m_checkRecenter.Body.GetILProcessor()
$lbl_retR = $ilR.Create([Mono.Cecil.Cil.OpCodes]::Ret)
$lbl_updatePrevR = $ilR.Create([Mono.Cecil.Cil.OpCodes]::Ldarg_0)

# if (!hmdValid) return;
$ilR.Emit([Mono.Cecil.Cil.OpCodes]::Ldarg_0)
$ilR.Emit([Mono.Cecil.Cil.OpCodes]::Ldfld, $f_hmdValid)
$ilR.Emit([Mono.Cecil.Cil.OpCodes]::Brfalse, $lbl_retR)

# if (!prevRawValid) goto updatePrev;
$ilR.Emit([Mono.Cecil.Cil.OpCodes]::Ldarg_0)
$ilR.Emit([Mono.Cecil.Cil.OpCodes]::Ldfld, $f_prevValid)
$ilR.Emit([Mono.Cecil.Cil.OpCodes]::Brfalse, $lbl_updatePrevR)

# float d = Vector3.Distance(this.hmdPos, this.prevRawPos);
$ilR.Emit([Mono.Cecil.Cil.OpCodes]::Ldarg_0)
$ilR.Emit([Mono.Cecil.Cil.OpCodes]::Ldfld, $f_hmdPos)
$ilR.Emit([Mono.Cecil.Cil.OpCodes]::Ldarg_0)
$ilR.Emit([Mono.Cecil.Cil.OpCodes]::Ldfld, $f_prevPos)
$ilR.Emit([Mono.Cecil.Cil.OpCodes]::Call, $refDist)

# if (d <= 0.25f) goto updatePrev;
$ilR.Emit([Mono.Cecil.Cil.OpCodes]::Ldc_R4, [float]0.25)
$ilR.Emit([Mono.Cecil.Cil.OpCodes]::Ble_Un, $lbl_updatePrevR)

# Trigger recenter:
$ilR.Emit([Mono.Cecil.Cil.OpCodes]::Ldarg_0)
$ilR.Emit([Mono.Cecil.Cil.OpCodes]::Ldc_I4_1)
$ilR.Emit([Mono.Cecil.Cil.OpCodes]::Stfld, $f_needRecenter)

$ilR.Emit([Mono.Cecil.Cil.OpCodes]::Ldarg_0)
$ilR.Emit([Mono.Cecil.Cil.OpCodes]::Ldc_I4_0)
$ilR.Emit([Mono.Cecil.Cil.OpCodes]::Stfld, $f_seatValid)

# updatePrev:
$ilR.Append($lbl_updatePrevR)
$ilR.Emit([Mono.Cecil.Cil.OpCodes]::Ldarg_0)
$ilR.Emit([Mono.Cecil.Cil.OpCodes]::Ldfld, $f_hmdPos)
$ilR.Emit([Mono.Cecil.Cil.OpCodes]::Stfld, $f_prevPos)

$ilR.Emit([Mono.Cecil.Cil.OpCodes]::Ldarg_0)
$ilR.Emit([Mono.Cecil.Cil.OpCodes]::Ldc_I4_1)
$ilR.Emit([Mono.Cecil.Cil.OpCodes]::Stfld, $f_prevValid)

$ilR.Append($lbl_retR)
$ilR.Emit([Mono.Cecil.Cil.OpCodes]::Ret)

$m_checkRecenter.Body.MaxStackSize = 8
$driverType.Methods.Add($m_checkRecenter)
Write-Output "Added method: CheckGuardianRecenter"

# 7. Patch ReadPoses(): Inject CheckGuardianRecenter() call before ret
$m_readPoses = $driverType.Methods | Where-Object { $_.Name -eq "ReadPoses" }
$lastRP = $m_readPoses.Body.Instructions[$m_readPoses.Body.Instructions.Count - 1]
$ilRP = $m_readPoses.Body.GetILProcessor()
$ilRP.InsertBefore($lastRP, ($ilRP.Create([Mono.Cecil.Cil.OpCodes]::Ldarg_0)))
$ilRP.InsertBefore($lastRP, ($ilRP.Create([Mono.Cecil.Cil.OpCodes]::Call, $m_checkRecenter)))
Write-Output "Patched ReadPoses(): Injected CheckGuardianRecenter()"

# 8. Patch Quit(): Add FfbStop() call before ret
$m_quit = $driverType.Methods | Where-Object { $_.Name -eq "Quit" }
$m_ffbStop = $driverType.Methods | Where-Object { $_.Name -eq "FfbStop" }
$lastQuit = $m_quit.Body.Instructions[$m_quit.Body.Instructions.Count - 1]
$ilQ = $m_quit.Body.GetILProcessor()
$ilQ.InsertBefore($lastQuit, ($ilQ.Create([Mono.Cecil.Cil.OpCodes]::Ldarg_0)))
$ilQ.InsertBefore($lastQuit, ($ilQ.Create([Mono.Cecil.Cil.OpCodes]::Call, $m_ffbStop)))
Write-Output "Patched Quit(): Added FfbStop() teardown"

# 9. Patch Update(): Call WatchdogCheck() at start
$m_update = $driverType.Methods | Where-Object { $_.Name -eq "Update" }
$ilU = $m_update.Body.GetILProcessor()
$firstInstU = $m_update.Body.Instructions[0]
$ilU.InsertBefore($firstInstU, ($ilU.Create([Mono.Cecil.Cil.OpCodes]::Ldarg_0)))
$ilU.InsertBefore($firstInstU, ($ilU.Create([Mono.Cecil.Cil.OpCodes]::Call, $m_watchdog)))
Write-Output "Patched Update(): Injected WatchdogCheck()"

# 10. Patch FfbTick():
$ilTick = $m_ffbTick.Body.GetILProcessor()

# Inject ffbLastTick = Time.unscaledTime at beginning of FfbTick
$firstTick = $m_ffbTick.Body.Instructions[0]
$ilTick.InsertBefore($firstTick, ($ilTick.Create([Mono.Cecil.Cil.OpCodes]::Ldarg_0)))
$ilTick.InsertBefore($firstTick, ($ilTick.Create([Mono.Cecil.Cil.OpCodes]::Call, $refTime)))
$ilTick.InsertBefore($firstTick, ($ilTick.Create([Mono.Cecil.Cil.OpCodes]::Stfld, $f_lastTick)))

$m_gameFocused = $driverType.Methods | Where-Object { $_.Name -eq "GameFocused" }

# Find stloc.s V_5 where num20 is stored
for ($i = 0; $i -lt $m_ffbTick.Body.Instructions.Count; $i++) {
    $inst = $m_ffbTick.Body.Instructions[$i]
    if ($inst.OpCode.Name -eq "stloc.s" -and $inst.Operand.Index -eq 5) {
        Write-Output "Found num20 store at index $i"
        # Stack has num20. Pass (num20, ffbLastConst, dt, flag, focused, isDD) to SafeFfb
        $ilTick.InsertBefore($inst, ($ilTick.Create([Mono.Cecil.Cil.OpCodes]::Ldarg_0)))
        $ilTick.InsertBefore($inst, ($ilTick.Create([Mono.Cecil.Cil.OpCodes]::Ldfld, $f_ffbLastConst)))
        $ilTick.InsertBefore($inst, ($ilTick.Create([Mono.Cecil.Cil.OpCodes]::Call, $refDelta)))
        $ilTick.InsertBefore($inst, ($ilTick.Create([Mono.Cecil.Cil.OpCodes]::Ldloc_0))) # flag (driving)
        # CRITICAL FIX: GameFocused is STATIC -> DO NOT emit ldarg.0!
        $ilTick.InsertBefore($inst, ($ilTick.Create([Mono.Cecil.Cil.OpCodes]::Call, $m_gameFocused)))
        $ilTick.InsertBefore($inst, ($ilTick.Create([Mono.Cecil.Cil.OpCodes]::Ldarg_0)))
        $ilTick.InsertBefore($inst, ($ilTick.Create([Mono.Cecil.Cil.OpCodes]::Ldfld, $f_isDD)))
        $ilTick.InsertBefore($inst, ($ilTick.Create([Mono.Cecil.Cil.OpCodes]::Call, $m_safeFfb)))
        Write-Output "Patched FfbTick(): num20 filtered through SafeFfb with hardware limits"
        break
    }
}

# Find stloc.s V_6 where num21 (damper) is stored
for ($i = 0; $i -lt $m_ffbTick.Body.Instructions.Count; $i++) {
    $inst = $m_ffbTick.Body.Instructions[$i]
    if ($inst.OpCode.Name -eq "stloc.s" -and $inst.Operand.Index -eq 6) {
        Write-Output "Found num21 (damper) store at index $i"
        $ilTick.InsertBefore($inst, ($ilTick.Create([Mono.Cecil.Cil.OpCodes]::Ldarg_0)))
        $ilTick.InsertBefore($inst, ($ilTick.Create([Mono.Cecil.Cil.OpCodes]::Ldfld, $f_isDD)))
        $ilTick.InsertBefore($inst, ($ilTick.Create([Mono.Cecil.Cil.OpCodes]::Ldloc_0))) # flag (driving)
        $ilTick.InsertBefore($inst, ($ilTick.Create([Mono.Cecil.Cil.OpCodes]::Call, $m_safeDamper)))
        Write-Output "Patched FfbTick(): num21 filtered through SafeDamper for anti-oscillation"
        break
    }
}

$m_ffbTick.Body.MaxStackSize = 32

# 11. Patch WheelConnect(): Set isDirectDrive = true for Fanatec/DD
$m_wheelConnect = $driverType.Methods | Where-Object { $_.Name -eq "WheelConnect" }
$ilWC = $m_wheelConnect.Body.GetILProcessor()
for ($i = $m_wheelConnect.Body.Instructions.Count - 1; $i -ge 0; $i--) {
    $inst = $m_wheelConnect.Body.Instructions[$i]
    if ($inst.OpCode.Name -eq "ldc.i4.1" -and $m_wheelConnect.Body.Instructions[$i+1].OpCode.Name -eq "stloc.s") {
        Write-Output "Found successful connection return at index $i"
        $ilWC.InsertBefore($inst, ($ilWC.Create([Mono.Cecil.Cil.OpCodes]::Ldarg_0)))
        $ilWC.InsertBefore($inst, ($ilWC.Create([Mono.Cecil.Cil.OpCodes]::Ldc_I4_1)))
        $ilWC.InsertBefore($inst, ($ilWC.Create([Mono.Cecil.Cil.OpCodes]::Stfld, $f_isDD)))
        Write-Output "Patched WheelConnect(): Set isDirectDrive = true on connect"
        break
    }
}

# 12. Save modified assembly
$outPath = "D:\Projects\overthehillvr-fan\Mods\OverTheHillVR.dll.patched"
$asm.Write($outPath)
$asm.Dispose()
$memStream.Dispose()

Write-Output "Assembly written to $outPath successfully, size: $( (Get-Item $outPath).Length )"
Copy-Item $outPath $dllPath -Force
Remove-Item $outPath -Force
Write-Output "Patched OverTheHillVR.dll replaced successfully!"
