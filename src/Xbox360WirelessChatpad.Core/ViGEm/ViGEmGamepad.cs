using System;
using Nefarius.ViGEm.Client.Targets;
using Nefarius.ViGEm.Client.Targets.Xbox360;

namespace Xbox360WirelessChatpad.Core.ViGEm
{
    /// <summary>
    /// An emulated Xbox 360 (XInput) controller backed by the ViGEmBus driver.
    /// </summary>
    public class ViGEmGamepad : IVirtualGamepad
    {
        private readonly IXbox360Controller target;
        private bool connected = false;

        public event EventHandler<GamepadFeedback> FeedbackReceived;

        internal ViGEmGamepad(IXbox360Controller target)
        {
            this.target = target;

            // Reports are submitted explicitly, one atomic report per
            // receiver packet, rather than on every property change.
            this.target.AutoSubmitReport = false;

            this.target.FeedbackReceived += onTargetFeedbackReceived;
        }

        public void Connect()
        {
            if (connected)
                return;

            target.Connect();
            connected = true;
        }

        public void Disconnect()
        {
            if (!connected)
                return;

            connected = false;
            target.Disconnect();
        }

        public void SubmitState(GamepadState state)
        {
            if (!connected)
                return;

            target.SetButtonState(Xbox360Button.A, state.A);
            target.SetButtonState(Xbox360Button.B, state.B);
            target.SetButtonState(Xbox360Button.X, state.X);
            target.SetButtonState(Xbox360Button.Y, state.Y);
            target.SetButtonState(Xbox360Button.Start, state.Start);
            target.SetButtonState(Xbox360Button.Back, state.Back);
            target.SetButtonState(Xbox360Button.Guide, state.Guide);
            target.SetButtonState(Xbox360Button.LeftShoulder, state.LeftShoulder);
            target.SetButtonState(Xbox360Button.RightShoulder, state.RightShoulder);
            target.SetButtonState(Xbox360Button.LeftThumb, state.LeftThumb);
            target.SetButtonState(Xbox360Button.RightThumb, state.RightThumb);
            target.SetButtonState(Xbox360Button.Up, state.DpadUp);
            target.SetButtonState(Xbox360Button.Down, state.DpadDown);
            target.SetButtonState(Xbox360Button.Left, state.DpadLeft);
            target.SetButtonState(Xbox360Button.Right, state.DpadRight);

            target.SetAxisValue(Xbox360Axis.LeftThumbX, state.LeftStickX);
            target.SetAxisValue(Xbox360Axis.LeftThumbY, state.LeftStickY);
            target.SetAxisValue(Xbox360Axis.RightThumbX, state.RightStickX);
            target.SetAxisValue(Xbox360Axis.RightThumbY, state.RightStickY);

            target.SetSliderValue(Xbox360Slider.LeftTrigger, state.LeftTrigger);
            target.SetSliderValue(Xbox360Slider.RightTrigger, state.RightTrigger);

            target.SubmitReport();
        }

        public void Dispose()
        {
            target.FeedbackReceived -= onTargetFeedbackReceived;
            Disconnect();
        }

        private void onTargetFeedbackReceived(object sender, Xbox360FeedbackReceivedEventArgs e)
        {
            EventHandler<GamepadFeedback> handler = FeedbackReceived;
            if (handler != null)
                handler(this, new GamepadFeedback(e.LargeMotor, e.SmallMotor, e.LedNumber));
        }
    }
}
