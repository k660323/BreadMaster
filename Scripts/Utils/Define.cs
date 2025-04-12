using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Define
{
    public enum ObjectType
    {
        Unknown,
        Player,
        Customer
    }

    public enum State
    {
        Die,
        Moving,
        Idle,
        EnterToMoveCenter,
        ToMoveDisplayStand,
        DisplayStandHold,
        ToMoveCenter,
        ToThink,
        ToCounter,
        CounterHold,
        ToExit,
        ToRestRequest,
        ToRestRequestHold,
        ToRestSpace,
        ToRestSpaceHold,
        ToRestSpaceExit
    }

    public enum Layer
    {
        Customer = 6,
        Ground = 7,
        Block = 8,
    }
    public enum Scene
    {
        Unknown,
        Game,
    }

    public enum Sound2D
    {
        Bgm,
        Effect2D,
        MaxCount,
    }

    public enum Sound3D
    {
        Effect3D,
    }

    public enum UIEvent
    {
        Click,
        Enter,
        Down,
        Drag,
        Up,
        Exit
    }

    public enum MouseEvent
    {
        Press,
        PointerDown,
        PointerUp,
        Click,
    }

    public enum CameraMode
    {
        QuaterView,
    }

    public enum GimmickType
    {
        None,
        Center,
        BreadGenerator,
        DisplayStand,
        Counter,
        CounterWadOfMoneySP,
        RestRequest,
        RestSpace,
        Exit
    }

    public enum PanelType
    {
        None,
        RestPanel,
    }

    public enum FoodType
    {
        None,
        Bread,
        MAX
    }
}
