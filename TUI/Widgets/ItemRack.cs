﻿using System;
using TUI.Base;
using TUI.Base.Style;
using TUI.Hooks.Args;

namespace TUI.Widgets
{
    #region ItemRackStyle

    public enum ItemSize
    {
        Tiny = 0,
        Small,
        Normal,
        Large,
        Massive
    }

    public class ItemRackStyle : UIStyle
    {
        public short Type { get; set; } = 0;
        public bool Left { get; set; } = true;
        public ItemSize Size { get; set; } = ItemSize.Normal;

        public ItemRackStyle() : base() { }

        public ItemRackStyle(ItemRackStyle style)
            : base(style)
        {
            Type = style.Type;
            Left = style.Left;
        }
    }

    #endregion

    public class ItemRack : VisualObject
    {
        #region Data

        protected static readonly int[] Sizes = new int[] { 7, 9, 0, 1, 2 };

        public dynamic Sign { get; set; } = null;

        public ItemRackStyle ItemRackStyle => Style as ItemRackStyle;
        public string Text { get; protected set; } = null;

        #endregion

        #region Constructor

        public ItemRack(int x, int y, ItemRackStyle style = null,
                Func<VisualObject, Touch, bool> callback = null)
            : base(x, y, 3, 3, new UIConfiguration(), style ?? new ItemRackStyle(), callback)
        {
            ForceSection = true;
        }

        #endregion
        #region Copy

        public ItemRack(ItemRack rack)
            : this(rack.X, rack.Y, rack.ItemRackStyle, rack.Callback)
        {
        }

        #endregion
        #region ApplyThisNative

        protected override void ApplyThisNative()
        {
            base.ApplyThisNative();

            short type = ItemRackStyle.Type;
            bool left = ItemRackStyle.Left;
            int fx = left ? 54 : 0;
            bool sign = Sign != null;
            int prefix = Sizes[(int)ItemRackStyle.Size];
            for (int x = 0; x < 3; x++)
                for (int y = 0; y < 3; y++)
                {
                    dynamic tile = Tile(x, y);
                    if (tile == null)
                        continue;
                    tile.active(true);
                    tile.type = (ushort)(sign && y == 0 ? 55 : 334);
                    if (sign && y == 0)
                        tile.frameX = (short)((x == 0) ? 144 : (x == 1) ? 126 : 162);
                    else if (y == 1 && x < 2)
                        tile.frameX = (short)
                            (x == 0
                            ? (left ? 20100 : 5100) + type
                            : (left ? 25000 : 10000) + prefix);
                    else
                        tile.frameX = (short)(fx + x * 18);
                    tile.frameY = (short)(sign && y == 0 ? 0 : y * 18);
                }
        }

        #endregion
        #region SetText

        // Use this only after adding ItemRack to parent.
        public void SetText(string text)
        {
            Text = text;
        }

        #endregion
        #region CreateSign

        public void CreateSign()
        {
            if (Text == null)
                throw new NullReferenceException("CreateSign: Text is null");
            (int x, int y) = AbsoluteXY();
            CreateSignArgs args = new CreateSignArgs(x, y, Sign, this);
            TUI.Hooks.CreateSign.Invoke(args);
            if (args.Sign == null)
            {
                TUI.Hooks.Log.Invoke(new LogArgs("Can't create new sign.", LogType.Error));
                return;
            }
            Sign = args.Sign;
            Sign.text = Text;
        }

        #endregion
        #region RemoveSign

        public void RemoveSign()
        {
            if (Sign == null)
                return;
            TUI.Hooks.RemoveSign.Invoke(new RemoveSignArgs(this, Sign));
            Sign = null;
        }

        #endregion
        #region PulseThisNative

        protected override void PulseThisNative(PulseType type)
        {
            base.PulseThisNative(type);
            if (type == PulseType.PreSetXYWH)
                RemoveSign();
            else if (type == PulseType.PostSetXYWH && Text != null)
                CreateSign();
            else if (type == PulseType.Dispose)
                RemoveSign();
        }

        #endregion
        #region UpdateThisNative

        protected override void UpdateThisNative()
        {
            base.UpdateThisNative();

            (int x, int y) = AbsoluteXY();
            if (Text != null && (Sign == null || Sign.text != Text || Sign.x != x || Sign.y != y))
            {
                RemoveSign();
                CreateSign();
            }
        }

        #endregion
    }
}
