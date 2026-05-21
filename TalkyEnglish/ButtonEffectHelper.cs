using System.Collections.Generic;
using System.Windows.Forms;
using Guna.UI2.WinForms;

namespace TalkyEnglish.GUI
{
    public static class ButtonEffectHelper
    {
        /// <summary>
        /// Xóa hiệu ứng xám mặc định khi hover và click trên tất cả Guna2Button trong parent.
        /// </summary>
        public static void RemoveGrayEffect(Control parent)
        {
            foreach (var btn in GetAllButtons(parent))
            {
                btn.HoverState.FillColor = btn.FillColor;
                btn.PressedColor = btn.FillColor;
                btn.PressedDepth = 0;
            }
        }

        private static IEnumerable<Guna2Button> GetAllButtons(Control parent)
        {
            foreach (Control c in parent.Controls)
            {
                if (c is Guna2Button btn) yield return btn;
                foreach (var child in GetAllButtons(c))
                    yield return child;
            }
        }
    }
}
