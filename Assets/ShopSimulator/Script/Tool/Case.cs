using UnityEngine;

public class Case : MonoBehaviour
{
    [SerializeField] private ToolShop tool;
    [SerializeField] private Transform dropPoint;

    public void InteractCase(PlayerController player, ToolShop newTool = null)
    {
        if (tool != null && player != null)
        {
            AudioManager.Instance.PlaySFX("Click");
            GrabTool(player);
        }
        else if (tool == null && newTool != null)
        {
            AudioManager.Instance.PlaySFX("Click");
            InsertTool(newTool);
            player.RemoveTool();
        }
    }

    void InsertTool(ToolShop newTool)
    {
        tool = newTool;
        newTool.InsertToCase(dropPoint);
    }

    void GrabTool(PlayerController player)
    {
        player.GrabToolFromCase(tool);
        tool = null;
    }
}