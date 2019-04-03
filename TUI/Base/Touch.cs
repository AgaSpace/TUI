﻿using System;
using System.Collections.Generic;

namespace TUI
{
    public enum TouchState
    {
        Begin,
        Moving,
        End
    }

    public class Touch : IVisual<Touch>, ICloneable
    {
        #region Data

        public int AbsoluteX { get; private set; }
        public int AbsoluteY { get; private set; }
        public TouchState State { get; private set; }
        public byte Prefix { get; private set; }
        public UIUserSession Session { get; internal set; }
        public bool Undo { get; set; }
        public byte StateByte { get; private set; }

        public bool Red      => (StateByte & 1) > 0;
        public bool Green    => (StateByte & 2) > 0;
        public bool Blue     => (StateByte & 4) > 0;
        public bool Yellow   => (StateByte & 8) > 0;
        public bool Actuator => (StateByte & 16) > 0;
        public bool Cutter   => (StateByte & 32) > 0;

        #endregion

        #region IVisual

            #region Data

            public int X { get; set; }
            public int Y { get; set; }
            public int Width { get; set; }
            public int Height { get; set; }

            public IEnumerable<(int, int)> Points { get { yield return (X, Y); } }
            public (int X, int Y, int Width, int Height) Padding(PaddingConfig paddingData) =>
                UI.Padding(X, Y, Width, Height, paddingData);

            #endregion

            #region Initialize

            public void InitializeVisual(int x, int y, int width = 1, int height = 1)
            {
                X = x;
                Y = y;
                Width = width;
                Height = height;
            }

            #endregion
            #region XYWH

            public (int X, int Y, int Width, int Height) XYWH(int dx = 0, int dy = 0)
            {
                return (X + dx, Y + dy, Width, Height);
            }

            public Touch SetXYWH(int x, int y, int width = -1, int height = -1)
            {
                X = x;
                Y = y;
                Width = width >= 0 ? width : Width;
                Height = height >= 0 ? height : Height;
                return this;
            }

            #endregion
            #region Move

            public Touch Move(int dx, int dy)
            {
                X = X + dx;
                Y = Y + dy;
                return this;
            }

            public Touch MoveBack(int dx, int dy)
            {
                X = X - dx;
                Y = Y - dy;
                return this;
            }

            #endregion
            #region Contains, Intersects

            public bool Contains(int x, int y) =>
                x >= X && y >= Y && x < X + Width && y < Y + Height;
            public bool Intersecting(int x, int y, int width, int height) =>
                x < X + Width && X < x + width && y < Y + Height && Y < y + height;
            public bool Intersecting(Touch o) => false;

            #endregion

        #endregion

        #region Initialize

        public Touch(int x, int y, TouchState state, byte prefix = 0, byte stateByte = 0)
        {
            InitializeVisual(x, y);
            AbsoluteX = X;
            AbsoluteY = Y;
            State = state;
            Prefix = prefix;
            StateByte = stateByte;
        }

        #endregion
        #region Clone

        public object Clone() => MemberwiseClone();

        #endregion
        #region SimulatedEndTouch

        public Touch SimulatedEndTouch()
        {
            Touch touch = (Touch)Clone();
            touch.State = TouchState.End;
            return touch;
        }

        #endregion
    }
}