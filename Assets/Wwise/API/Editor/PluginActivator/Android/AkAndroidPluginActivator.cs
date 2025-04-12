/*******************************************************************************
The content of this file includes portions of the proprietary AUDIOKINETIC Wwise
Technology released in source code form as part of the game integration package.
The content of this file may not be used without valid licenses to the
AUDIOKINETIC Wwise Technology.
Note that the use of the game engine is subject to the Unity(R) Terms of
Service at https://unity3d.com/legal/terms-of-service
 
License Usage
 
Licensees holding valid licenses to the AUDIOKINETIC Wwise Technology may use
this file in accordance with the end user license agreement provided with the
software or, alternatively, in accordance with the terms contained
in a written agreement between you and Audiokinetic Inc.
Copyright (c) 2025 Audiokinetic Inc.
*******************************************************************************/

#if UNITY_EDITOR
using UnityEditor;

[UnityEditor.InitializeOnLoad]
public class AkAndroidPluginActivator : AkPlatformPluginActivator
{
	public override string WwisePlatformName => "Android";
	public override string PluginDirectoryName => "Android";

	static AkAndroidPluginActivator()
	{
		if (UnityEditor.AssetDatabase.IsAssetImportWorkerProcess())
		{
			return;
		}

		AkPluginActivator.RegisterPlatformPluginActivator(BuildTarget.Android, new AkAndroidPluginActivator());
	}
	
	private const int ARCH_INDEX = 1;
	private const int CONFIG_INDEX = 2;
	public override AkPluginActivator.PluginImporterInformation GetPluginImporterInformation(PluginImporter pluginImporter)
	{
		var parts = GetPluginPathParts(pluginImporter.assetPath);
		return new AkPluginActivator.PluginImporterInformation
		{
			PluginConfig = parts[CONFIG_INDEX],
			PluginArch = parts[ARCH_INDEX]
		};
	}

	internal override bool ConfigurePlugin(PluginImporter pluginImporter, AkPluginActivator.PluginImporterInformation pluginImporterInformation)
	{
		if (pluginImporterInformation.PluginArch == "armeabi-v7a")
		{
			pluginImporter.SetPlatformData(BuildTarget.Android, "CPU", "ARMv7");
		}
		else if (pluginImporterInformation.PluginArch == "arm64-v8a")
		{
			pluginImporter.SetPlatformData(BuildTarget.Android, "CPU", "ARM64");
		}
		else if (pluginImporterInformation.PluginArch == "x86")
		{
			pluginImporter.SetPlatformData(BuildTarget.Android, "CPU", "x86");
		}
		else
		{
			UnityEngine.Debug.Log("WwiseUnity: Architecture not found: " + pluginImporterInformation.PluginArch);
		}
		return true;
	}
}
#endif