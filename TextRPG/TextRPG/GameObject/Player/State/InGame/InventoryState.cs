using TextRPG.Managers;
using TextRPG.UI;
using TextRPG.Utils;
public class InventoryState : IPlayerState<InventoryState, InventoryState.SubState>
{
    public static InventoryState Instance { get; } = new InventoryState();
    public StateStack<SubState> StateStack { get; set; } = new StateStack<SubState>();
    public string InputStr { get; set; }
    public int InputInt { get; set; }
    PortalInfo NextPortal { get; set; } = null;
    public ConsoleKey InputKey { get; set; }
    Item SelectedItem { get; set; }

    public void Enter()
    {
        StateStack.Push(SubState.AskShowInventoryText);
    }
    public void Exit()
    {
        StateStack.Clear();
    }
    public enum SubState
    {
        AskShowInventoryText,
        SelectItemKindText,
        SelectPotionText,
        SelectHelmetsText,
        SelectArmorsText,
        SelectShoesText,
        SelectSwordsText,
        FailedOutOfRangeText,
        ShowSelectedItemText,
        FailedCellItemText,
    }

    public void Update()
    {
        SubState State = StateStack.Pop();

        switch (State)
        {
            case SubState.AskShowInventoryText:
                Manager.UI.Inventory.AskShowInventoryText.DisplayMessage();
                InputKey = Manager.UI.GetInputAsKeyInfo();
                StateStack.Push(State);
                switch (InputKey)
                {
                    case ConsoleKey.D0:
                    case ConsoleKey.Escape:
                        Player.ChangeState<IdleState, IdleState.SubState>();
                        break;
                    case ConsoleKey.D1:
                        StateStack.Push(SubState.SelectItemKindText);
                        break;
                    default:
                        StateStack.Push(SubState.FailedOutOfRangeText);
                        break;
                }
                break;
            case SubState.SelectItemKindText:
                Manager.UI.Inventory.SelectItemKindText.DisplayMessage();
                InputKey = Manager.UI.GetInputAsKeyInfo();
                StateStack.Push(State);
                switch (InputKey)
                {
                    case ConsoleKey.D0:
                    case ConsoleKey.Escape:
                        StateStack.Pop();
                        break;
                    case ConsoleKey.D1:
                        StateStack.Push(SubState.SelectPotionText);
                        break;
                    case ConsoleKey.D2:
                        StateStack.Push(SubState.SelectHelmetsText);
                        break;
                    case ConsoleKey.D3:
                        StateStack.Push(SubState.SelectArmorsText);
                        break;
                    case ConsoleKey.D4:
                        StateStack.Push(SubState.SelectShoesText);
                        break;
                    case ConsoleKey.D5:
                        StateStack.Push(SubState.SelectSwordsText);
                        break;
                    default:
                        StateStack.Push(SubState.FailedOutOfRangeText);
                        break;
                }
                break;
            case SubState.SelectPotionText:
                var potions = Player.Potions;
                Manager.UI.Inventory.SelectPotionText.FormatMessage(originalMsg =>
                {
                    string f = "";
                    for (int i = 0; i < potions.Count; i++)
                    {
                        f += string.Format("{0}. {1}\n", i + 1, potions[i].Name);
                    }
                    if (f.Length > 0 && f[f.Length - 1] == '\n')
                        f = f.Substring(0, f.Length - 1);
                    return string.Format(originalMsg, f);
                });
                Manager.UI.Inventory.SelectPotionText.DisplayMessage();
                InputKey = Manager.UI.GetInputAsKeyInfo();
                StateStack.Push(State);
                switch (InputKey)
                {
                    case ConsoleKey.D0:
                    case ConsoleKey.Escape:
                        StateStack.Pop();
                        break;
                    default:
                        InputInt = InputKey - ConsoleKey.D0;
                        if (0 < InputInt && InputInt <= potions.Count)
                        {
                            SelectedItem = potions[InputInt - 1];
                            StateStack.Push(SubState.ShowSelectedItemText);
                        }
                        else
                        {
                            StateStack.Push(SubState.FailedOutOfRangeText);
                        }
                        break;
                }
                break;
            case SubState.SelectHelmetsText:
                var helmets = Player.Helmets;
                Manager.UI.Inventory.SelectHelmetsText.FormatMessage(originalMsg =>
                {
                    string f = "";
                    for (int i = 0; i < helmets.Count; i++)
                    {
                        f += string.Format("{0}. {1}\n", i + 1, helmets[i].Name);
                    }
                    if (f.Length > 0 && f[f.Length - 1] == '\n')
                        f = f.Substring(0, f.Length - 1);
                    return string.Format(originalMsg, f);
                });
                Manager.UI.Inventory.SelectHelmetsText.DisplayMessage();
                InputKey = Manager.UI.GetInputAsKeyInfo();
                StateStack.Push(State);
                switch (InputKey)
                {
                    case ConsoleKey.D0:
                    case ConsoleKey.Escape:
                        StateStack.Pop();
                        break;
                    default:
                        InputInt = InputKey - ConsoleKey.D0;
                        if (0 < InputInt && InputInt <= helmets.Count)
                        {
                            SelectedItem = helmets[InputInt - 1];
                            StateStack.Push(SubState.ShowSelectedItemText);
                        }
                        else
                        {
                            StateStack.Push(SubState.FailedOutOfRangeText);
                        }
                        break;
                }
                break;
            case SubState.SelectArmorsText:
                var armors = Player.Armors;
                Manager.UI.Inventory.SelectArmorsText.FormatMessage(originalMsg =>
                {
                    string f = "";
                    for (int i = 0; i < armors.Count; i++)
                    {
                        f += string.Format("{0}. {1}\n", i + 1, armors[i].Name);
                    }
                    if (f.Length > 0 && f[f.Length - 1] == '\n')
                        f = f.Substring(0, f.Length - 1);
                    return string.Format(originalMsg, f);
                });
                Manager.UI.Inventory.SelectArmorsText.DisplayMessage();
                InputKey = Manager.UI.GetInputAsKeyInfo();
                StateStack.Push(State);
                switch (InputKey)
                {
                    case ConsoleKey.D0:
                    case ConsoleKey.Escape:
                        StateStack.Pop();
                        break;
                    default:
                        InputInt = InputKey - ConsoleKey.D0;
                        if (0 < InputInt && InputInt <= armors.Count)
                        {
                            SelectedItem = armors[InputInt - 1];
                            StateStack.Push(SubState.ShowSelectedItemText);
                        }
                        else
                        {
                            StateStack.Push(SubState.FailedOutOfRangeText);
                        }
                        break;
                }
                break;
            case SubState.SelectShoesText:
                var shoes = Player.Shoes;
                Manager.UI.Inventory.SelectShoesText.FormatMessage(originalMsg =>
                {
                    string f = "";
                    for (int i = 0; i < shoes.Count; i++)
                    {
                        f += string.Format("{0}. {1}\n", i + 1, shoes[i].Name);
                    }
                    if (f.Length > 0 && f[f.Length - 1] == '\n')
                        f = f.Substring(0, f.Length - 1);
                    return string.Format(originalMsg, f);
                });
                Manager.UI.Inventory.SelectShoesText.DisplayMessage();
                InputKey = Manager.UI.GetInputAsKeyInfo();
                StateStack.Push(State);
                switch (InputKey)
                {
                    case ConsoleKey.D0:
                    case ConsoleKey.Escape:
                        StateStack.Pop();
                        break;
                    default:
                        InputInt = InputKey - ConsoleKey.D0;
                        if (0 < InputInt && InputInt <= shoes.Count)
                        {
                            SelectedItem = shoes[InputInt - 1];
                            StateStack.Push(SubState.ShowSelectedItemText);
                        }
                        else
                        {
                            StateStack.Push(SubState.FailedOutOfRangeText);
                        }
                        break;
                }
                break;
            case SubState.SelectSwordsText:
                var swords = Player.Swords;
                Manager.UI.Inventory.SelectSwordsText.FormatMessage(originalMsg =>
                {
                    string f = "";
                    for (int i = 0; i < swords.Count; i++)
                    {
                        f += string.Format("{0}. {1}\n", i + 1, swords[i].Name);
                    }
                    if (f.Length > 0 && f[f.Length - 1] == '\n')
                        f = f.Substring(0, f.Length - 1);
                    return string.Format(originalMsg, f);
                });
                Manager.UI.Inventory.SelectSwordsText.DisplayMessage();
                InputKey = Manager.UI.GetInputAsKeyInfo();
                StateStack.Push(State);
                switch (InputKey)
                {
                    case ConsoleKey.D0:
                    case ConsoleKey.Escape:
                        StateStack.Pop();
                        break;
                    default:
                        InputInt = InputKey - ConsoleKey.D0;
                        if (0 < InputInt && InputInt <= swords.Count)
                        {
                            SelectedItem = swords[InputInt - 1];
                            StateStack.Push(SubState.ShowSelectedItemText);
                        }
                        else
                        {
                            StateStack.Push(SubState.FailedOutOfRangeText);
                        }
                        break;
                }
                break;
            case SubState.ShowSelectedItemText:
                Manager.UI.Inventory.ShowSelectedItemText.FormatMessage(originalMsg =>
                {
                    string power = SelectedItem.Type == ItemType.Sword ? "공격력" : SelectedItem.Type == ItemType.Potion ? "체력 회복" : "방어력";
                    return string.Format(originalMsg,
                        SelectedItem.Name,
                        SelectedItem.Summary,
                        $"{SelectedItem.Power.ToString()}{power}",
                        (SelectedItem.Gold * .8f ).ToString());
                });
                Manager.UI.Inventory.ShowSelectedItemText.DisplayMessage();
                InputKey = Manager.UI.GetInputAsKeyInfo();
                switch (InputKey)
                {
                    case ConsoleKey.D0:
                    case ConsoleKey.Escape:
                        StateStack.Pop();
                        break;
                    case ConsoleKey.D1:
                        // 플레이어 장착
                        switch (SelectedItem.Type)
                        {
                            case ItemType.Potion:
                                Player.UsePotion(SelectedItem);
                                break;
                            default:
                                Player.EquipItem(SelectedItem);
                                break;
                        }
                        break;
                    case ConsoleKey.D2:
                        Player.SellItem(SelectedItem);
                        break;
                    default:
                        StateStack.Push(State);
                        StateStack.Push(SubState.FailedOutOfRangeText);
                        break;
                }
                break;
            case SubState.FailedOutOfRangeText:
                Manager.UI.ETC.FailedOutOfRangeText.DisplayMessage();
                break;
            case SubState.FailedCellItemText:
                Manager.UI.Inventory.FailedCellItemText.DisplayMessage();
                break;
        }
    }
}
