using System;
using BlueprintsV2.BlueprintData;
using BlueprintsV2.ModAPI;
using UnityEngine;

namespace BlueprintsV2.Tools
{
    /// <summary>
    /// 用于将蓝图设置应用到已完工建筑，并触发生成复制设置任务
    /// </summary>
    public static class BlueprintSettingsApplier
    {
        /// <summary>
        /// 将蓝图设置应用到目标建筑（已完工），生成任务等待小人执行
        /// </summary>
        /// <param name="targetGO">目标建筑对象</param>
        /// <param name="buildingConfig">蓝图配置</param>
        /// <param name="playerId">玩家ID</param>
        /// <param name="cell">用于提示位置</param>
        /// <returns>是否成功触发复制</returns>
        public static bool ApplySettingsToCompletedBuilding(
            GameObject targetGO,
            BuildingConfig buildingConfig,
            ulong playerId,
            int cell)
        {
            if (targetGO == null || buildingConfig == null)
                return false;

            // 检查是否有设置数据
            if (!buildingConfig.HasAnyBuildingData)
                return false;

            // 1. 检查目标建筑是否有 CopyBuildingSettings，没有则添加
            var targetCopy = targetGO.GetComponent<CopyBuildingSettings>();
            if (targetCopy == null)
                targetCopy = targetGO.AddComponent<CopyBuildingSettings>();

            // 2. 获取目标建筑的 KPrefabID
            var targetPrefab = targetGO.GetComponent<KPrefabID>();
            if (targetPrefab == null)
                return false;

            // 从缓存获取模板（隐藏、非激活）
            var buildingDef = buildingConfig.BuildingDef;
            if (buildingDef == null)
                return false;

            GameObject sourceGO = BlueprintTemplateCache.GetOrCreateTemplate(buildingDef);
            if (sourceGO == null)
                return false;

            // 将蓝图数据应用到模板（完全覆盖）
            ModAPI.API_Methods.ApplyAdditionalBuildingData(sourceGO, buildingConfig, playerId);

            // 执行复制（触发事件）
            var sourceCopy = sourceGO.GetComponent<CopyBuildingSettings>();
            bool success = CopyBuildingSettings.ApplyCopy(
                targetPrefab,
                sourceGO,
                sourceGO.GetComponent<KPrefabID>(),
                sourceCopy
            );

            // 显示提示
            if (success && cell != Grid.InvalidCell)
            {
                PopFXManager.Instance.SpawnFX(
                    ModAssets.BLUEPRINTS_APPLY_SETTINGS_SPRITE,
                    "Settings copied, waiting for Duplicant",
                    null,
                    offset: Grid.CellToPos(cell),
                    Config.Instance.FXTime
                );
            }

            return success;
        }
    }
}