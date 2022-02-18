using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace UnityEditor.U2D.Animation
{
    internal class ModuleToolGroup
    {
        class ToolGroupEntry
        {
            public BaseTool tool;
            public Action activateCallback;
        }

        class ToolGroup
        {
            public int groupId;
            public List<ToolGroupEntry> tools = new List<ToolGroupEntry>();
            public int previousToolIndex;
        }
        List<ToolGroup> m_ToolGroups = new List<ToolGroup>();

        public void AddToolToGroup(int groupId, BaseTool tool, Action toolActivatedCallback)
        {
            List<ToolGroupEntry> tools = null;
            for (int i = 0; i < m_ToolGroups.Count; ++i)
            {
                if (m_ToolGroups[i].groupId == groupId)
                {
                    tools = m_ToolGroups[i].tools;
                }

                var toolIndex = m_ToolGroups[i].tools.FindIndex(x => x.tool == tool);
                if (toolIndex != -1)
                {
                    Debug.LogError(string.Format("{0} already exist in group.", tool.name));
                    return;
                }
            }

            if (tools == null)
            {
                var toolGroup = new ToolGroup()
                {
                    groupId = groupId
                };
                tools = toolGroup.tools;
                m_ToolGroups.Add(toolGroup);
            }

            tools.Add(new ToolGroupEntry()
            {
                tool = tool,
                activateCallback = toolActivatedCallback
            });
        }

        public void ActivateTool(BaseTool tool)
        {
            var toolGroupIndex = -1;
            var groupTool = m_ToolGroups.FirstOrDefault(x =>
            {
                toolGroupIndex = x.tools.FindIndex(y => y.tool == tool);
                return toolGroupIndex >= 0;
            });

            if (groupTool != null && toolGroupIndex >= 0)
            {
                var previousTool = groupTool.previousToolIndex >= 0 ? groupTool.tools[groupTool.previousToolIndex] : null;
                if (tool.isActive) // we want to deactivate the tool and switch to original
                {
                    tool.Deactivate();
                    if (previousTool != null && previousTool.tool != tool && previousTool.tool != null)
                    {
                        previousTool.tool.Activate();
                        groupTool.previousToolIndex = toolGroupIndex;
                    }
                }
                else
                {
                    for (int i = 0; i < groupTool.tools.Count; ++i)
                    {
                        var gt = groupTool.tools[i];
                        if (gt.tool.isActive)
                        {
                            groupTool.previousToolIndex = i;
                            gt.tool.Deactivate();
                        }
                    }
                    tool.Activate();
                    if (groupTool.tools[toolGroupIndex].activateCallback != null)
                        groupTool.tools[toolGroupIndex].activateCallback();
                }
            }
        }
    }
}
