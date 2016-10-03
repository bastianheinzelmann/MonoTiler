using System;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace MonoTiler
{
    class TileSelectionCamera:Camera2D
    {

        public TileSelectionCamera(Viewport viewport) : base(viewport)
        {
        }

        protected override void Input()
        {
            _mState = Mouse.GetState();

            //Check scrolling
            if (_mState.ScrollWheelValue > _scroll)
            {
                _pos.Y += Math.Abs(_mState.ScrollWheelValue - _scroll);
                _scroll = _mState.ScrollWheelValue;
            }
            if (_mState.ScrollWheelValue < _scroll)
            {
                _pos.Y -= Math.Abs(_scroll - _mState.ScrollWheelValue);
                _scroll = _mState.ScrollWheelValue;
            }
        }
    }
}
