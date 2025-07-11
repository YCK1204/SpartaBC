using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TextRPG.Managers;
using TextRPG.UI;
using TextRPG.Utils;

public class ShopState : IPlayerState<ShopState, ShopState.SubState>
{
    public static ShopState Instance { get; } = new ShopState();
    public StateStack<SubState> StateStack { get; set; } = new StateStack<SubState>();
    public string InputStr { get; set; }
    public int InputInt { get; set; }
    Item InputItem { get; set; }
    public ConsoleKey InputKey { get; set; }

    public enum SubState
    {
        AskShowShopText,
        SelectItemKindText,
        SelectPotionText,
        ShowHelmetsText,
        ShowArmorsText,
        ShowShoesText,
        ShowSwordsText,
        ShowItemDetailText,
        FailedOutOfRangeText,
        OnPurchaseText,
        FailedPurchaseText,
    }

    public void Enter()
    {
        StateStack.Push(SubState.AskShowShopText);
    }

    public void Exit()
    {
        StateStack.Clear();
    }

    public void Update()
    {
        SubState State = StateStack.Pop();

        switch (State)
        {
            case SubState.AskShowShopText:
                Manager.UI.Shop.AskShowShopText.DisplayMessage();
                StateStack.Push(State);
                InputKey = Manager.UI.GetInputAsKeyInfo();
                switch (InputKey)
                {
                    case ConsoleKey.I:
                        Player.ChangeState<InventoryState, InventoryState.SubState>();
                        break;
                    case ConsoleKey.D1:
                        StateStack.Push(SubState.SelectItemKindText);
                        break;
                    case ConsoleKey.D2:
                    case ConsoleKey.Escape:
                        Player.ChangeState<IdleState, IdleState.SubState>();
                        break;
                    default:
                        StateStack.Push(SubState.FailedOutOfRangeText);
                        break;
                }
                break;
            case SubState.SelectItemKindText:
                Manager.UI.Shop.SelectItemKindText.DisplayMessage();
                StateStack.Push(State);
                InputKey = Manager.UI.GetInputAsKeyInfo();
                switch (InputKey)
                {
                    case ConsoleKey.I:
                        Player.ChangeState<InventoryState, InventoryState.SubState>();
                        break;
                    case ConsoleKey.D1:
                        StateStack.Push(SubState.SelectPotionText);
                        break;
                    case ConsoleKey.D2:
                        StateStack.Push(SubState.ShowHelmetsText);
                        break;
                    case ConsoleKey.D3:
                        StateStack.Push(SubState.ShowArmorsText);
                        break;
                    case ConsoleKey.D4:
                        StateStack.Push(SubState.ShowShoesText);
                        break;
                    case ConsoleKey.D5:
                        StateStack.Push(SubState.ShowSwordsText);
                        break;
                    case ConsoleKey.Escape:
                    case ConsoleKey.D0:
                        Player.ChangeState<IdleState, IdleState.SubState>();
                        break;
                    default:
                        StateStack.Push(SubState.FailedOutOfRangeText);
                        break;
                }
                break;
            case SubState.SelectPotionText:
                Manager.UI.Shop.SelectPotionText.DisplayMessage();
                InputKey = Manager.UI.GetInputAsKeyInfo();
                StateStack.Push(State);
                switch (InputKey)
                {
                    case ConsoleKey.I:
                        Player.ChangeState<InventoryState, InventoryState.SubState>();
                        break;
                    case ConsoleKey.D0:
                    case ConsoleKey.Escape:
                        StateStack.Pop();
                        return;
                    default:
                        InputInt = InputKey - ConsoleKey.D0;
                        if (0 < InputInt && InputInt <= Manager.Data.Potions.Count)
                        {
                            InputItem = Manager.Data.Potions[InputInt - 1];
                            StateStack.Push(SubState.ShowItemDetailText);
                        }
                        else
                            StateStack.Push(SubState.FailedOutOfRangeText);
                        break;
                }
                break;
            case SubState.ShowHelmetsText:
                Manager.UI.Shop.ShowHelmetsText.DisplayMessage();
                InputKey = Manager.UI.GetInputAsKeyInfo();
                StateStack.Push(State);
                switch (InputKey)
                {
                    case ConsoleKey.I:
                        Player.ChangeState<InventoryState, InventoryState.SubState>();
                        break;
                    case ConsoleKey.D0:
                    case ConsoleKey.Escape:
                        StateStack.Pop();
                        return;
                    default:
                        InputInt = InputKey - ConsoleKey.D0;
                        if (0 < InputInt && InputInt <= Manager.Data.Helmets.Count)
                        {
                            InputItem = Manager.Data.Helmets[InputInt - 1];
                            StateStack.Push(SubState.ShowItemDetailText);
                        }
                        else
                            StateStack.Push(SubState.FailedOutOfRangeText);
                        break;
                }
                break;
            case SubState.ShowArmorsText:
                Manager.UI.Shop.ShowArmorsText.DisplayMessage();
                InputKey = Manager.UI.GetInputAsKeyInfo();
                StateStack.Push(State);
                switch (InputKey)
                {
                    case ConsoleKey.I:
                        Player.ChangeState<InventoryState, InventoryState.SubState>();
                        break;
                    case ConsoleKey.D0:
                    case ConsoleKey.Escape:
                        StateStack.Pop();
                        return;
                    default:
                        InputInt = InputKey - ConsoleKey.D0;
                        if (0 < InputInt && InputInt <= Manager.Data.Armors.Count)
                        {
                            InputItem = Manager.Data.Armors[InputInt - 1];
                            StateStack.Push(SubState.ShowItemDetailText);
                        }
                        else
                            StateStack.Push(SubState.FailedOutOfRangeText);
                        break;
                }
                break;
            case SubState.ShowShoesText:
                Manager.UI.Shop.ShowShoesText.DisplayMessage();
                InputKey = Manager.UI.GetInputAsKeyInfo();
                StateStack.Push(State);
                switch (InputKey)
                {
                    case ConsoleKey.I:
                        Player.ChangeState<InventoryState, InventoryState.SubState>();
                        break;
                    case ConsoleKey.D0:
                    case ConsoleKey.Escape:
                        StateStack.Pop();
                        return;
                    default:
                        InputInt = InputKey - ConsoleKey.D0;
                        if (0 < InputInt && InputInt <= Manager.Data.Shoes.Count)
                        {
                            InputItem = Manager.Data.Shoes[InputInt - 1];
                            StateStack.Push(SubState.ShowItemDetailText);
                        }
                        else
                            StateStack.Push(SubState.FailedOutOfRangeText);
                        break;
                }
                break;
            case SubState.ShowSwordsText:
                Manager.UI.Shop.ShowSwordsText.DisplayMessage();
                InputKey = Manager.UI.GetInputAsKeyInfo();
                StateStack.Push(State);
                switch (InputKey)
                {
                    case ConsoleKey.I:
                        Player.ChangeState<InventoryState, InventoryState.SubState>();
                        break;
                    case ConsoleKey.D0:
                    case ConsoleKey.Escape:
                        StateStack.Pop();
                        return;
                    default:
                        InputInt = InputKey - ConsoleKey.D0;
                        if (0 < InputInt && InputInt <= Manager.Data.Swords.Count)
                        {
                            InputItem = Manager.Data.Swords[InputInt - 1];
                            StateStack.Push(SubState.ShowItemDetailText);
                        }
                        else
                            StateStack.Push(SubState.FailedOutOfRangeText);
                        break;
                }
                break;
            case SubState.ShowItemDetailText:
                Manager.UI.Shop.ShowItemDetailText.FormatMessage(originalMsg =>
                {
                    string power = InputItem.Type == ItemType.Sword ? "공격력" : InputItem.Type == ItemType.Potion ? "체력 회복" : "방어력";
                    return string.Format(originalMsg,
                        InputItem.Name,
                        InputItem.Summary,
                        $"{InputItem.Power.ToString()}{power}",
                        InputItem.Gold);
                });
                Manager.UI.Shop.ShowItemDetailText.DisplayMessage();
                InputKey = Manager.UI.GetInputAsKeyInfo();
                StateStack.Push(State);
                switch (InputKey)
                {
                    case ConsoleKey.I:
                        Player.ChangeState<InventoryState, InventoryState.SubState>();
                        break;
                    case ConsoleKey.D1:
                        if (Player.Gold < InputItem.Gold)
                        {
                            StateStack.Push(SubState.FailedPurchaseText);
                            return;
                        }
                        StateStack.Push(SubState.OnPurchaseText);
                        break;
                    case ConsoleKey.D2:
                    case ConsoleKey.Escape:
                        StateStack.Pop();
                        break;
                    default:
                        StateStack.Push(SubState.FailedOutOfRangeText);
                        break;
                }
                break;
            case SubState.FailedOutOfRangeText:
                Manager.UI.ETC.FailedOutOfRangeText.DisplayMessage();
                break;
            case SubState.OnPurchaseText:
                Manager.UI.Shop.OnPurchaseText.FormatMessage(originalMsg =>
                {
                    return string.Format(originalMsg, InputItem.Name);
                });
                Player.Gold -= InputItem.Gold;
                Player.AddItemToInventory(InputItem);
                Manager.UI.Shop.OnPurchaseText.DisplayMessage();
                break;
            case SubState.FailedPurchaseText:
                Manager.UI.Shop.FailedPurchaseText.DisplayMessage();
                break;
        }
    }
}
