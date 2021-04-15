using System.Diagnostics;
using UnityEngine.UI;

namespace Core.UI
{
    public class SwitchNode: Node
    {

        private bool _on;
        private string _name_base;
        private string _name_on;
        private string _name_off;
        
        public bool ON
        {
            get { return _on; }
            set { _on = value; Text = getName(); }
        }
        private string getName()
        {
            return ON ? _name_on : _name_off;
        }

        public SwitchNode(string __name_base, int mode, bool on = true):this(__name_base + "\nON", __name_base + "\nOFF", mode, on)
        {
        }
        
        public SwitchNode(string __name_on, string __name_off, int mode, bool on = true):base(on?__name_on:__name_off, mode)
        {
            _name_on = __name_on;
            _name_off = __name_off;
            this.AddCallback(() =>
            {
                this.ON = !this.ON;
            });
        }
        
        
    }
}