using System;
using System.Collections.Generic;

namespace MBMPlay;

public class PanelManager
{
	private Dictionary<string, List<WeakReference<PanelSkinItem>>> groupDictionary = new Dictionary<string, List<WeakReference<PanelSkinItem>>>();

	private Dictionary<string, WeakReference<PanelSkinItem>> panelDictionary = new Dictionary<string, WeakReference<PanelSkinItem>>();

	private List<string> closeRequest = new List<string>();

	private List<string> opened = new List<string>();

	public void Add(PanelSkinItem panel)
	{
		if (panel == null)
		{
			throw new ArgumentNullException("panel");
		}
		if (!panelDictionary.ContainsKey(panel.ID))
		{
			WeakReference<PanelSkinItem> weakReference = new WeakReference<PanelSkinItem>(panel);
			panelDictionary.Add(panel.ID, weakReference);
			if (!groupDictionary.ContainsKey(panel.PanelGroup))
			{
				groupDictionary.Add(panel.PanelGroup, new List<WeakReference<PanelSkinItem>>());
			}
			groupDictionary[panel.PanelGroup].Add(weakReference);
		}
	}

	public List<PanelSkinItem> GetPanelGroup(string groupName)
	{
		if (groupDictionary.ContainsKey(groupName))
		{
			List<PanelSkinItem> list = new List<PanelSkinItem>(groupDictionary[groupName].Count);
			{
				foreach (WeakReference<PanelSkinItem> item in groupDictionary[groupName])
				{
					if (item.TryGetTarget(out var target))
					{
						list.Add(target);
					}
				}
				return list;
			}
		}
		return null;
	}

	public bool TryGetPanelGroup(string groupName, out List<PanelSkinItem> panels)
	{
		if (groupDictionary.ContainsKey(groupName))
		{
			List<PanelSkinItem> list = new List<PanelSkinItem>(groupDictionary[groupName].Count);
			foreach (WeakReference<PanelSkinItem> item in groupDictionary[groupName])
			{
				if (item.TryGetTarget(out var target))
				{
					list.Add(target);
				}
			}
			panels = list;
			return true;
		}
		panels = null;
		return false;
	}

	public PanelSkinItem GetPanel(string panelID)
	{
		if (panelDictionary.ContainsKey(panelID))
		{
			if (!panelDictionary[panelID].TryGetTarget(out var target))
			{
				return null;
			}
			return target;
		}
		return null;
	}

	public bool TryGetPanel(string panelID, out PanelSkinItem panel)
	{
		if (panelDictionary.ContainsKey(panelID))
		{
			return panelDictionary[panelID].TryGetTarget(out panel);
		}
		panel = null;
		return false;
	}

	public void OpenPanel(string panelID, double time, bool closeIfOpened)
	{
		PanelSkinItem panel = GetPanel(panelID);
		if (panel == null || !groupDictionary.ContainsKey(panel.PanelGroup))
		{
			return;
		}
		foreach (WeakReference<PanelSkinItem> item in groupDictionary[panel.PanelGroup])
		{
			if (!item.TryGetTarget(out var target))
			{
				continue;
			}
			if (target.ID == panelID)
			{
				if (target.Flags.PanelOpen)
				{
					if (closeIfOpened)
					{
						target.Close(time);
					}
					continue;
				}
				target.Open(time);
				if (!opened.Contains(target.ID))
				{
					opened.Add(target.ID);
				}
				CancelCloseRequest(target.ID);
			}
			else if (target.Flags.PanelOpen)
			{
				target.Close(time);
			}
		}
	}

	public void ClosePanel(string panelID, double time)
	{
		PanelSkinItem panel = GetPanel(panelID);
		if (panel != null && panel.Flags.PanelOpen)
		{
			panel.Close(time);
		}
	}

	public void RequestClose(string panelID)
	{
		if (!opened.Contains(panelID) && !closeRequest.Contains(panelID))
		{
			closeRequest.Add(panelID);
		}
	}

	public void CancelCloseRequest(string panelID)
	{
		closeRequest.Remove(panelID);
	}

	public void CloseRequested(double time)
	{
		while (closeRequest.Count > 0)
		{
			ClosePanel(closeRequest[0], time);
			closeRequest.RemoveAt(0);
		}
		opened.Clear();
	}
}
