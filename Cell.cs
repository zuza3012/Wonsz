using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wonsz {
    class Cell {

        private bool food;
        private bool free;
        private bool head;
        private int val;

        public bool Food {
            get { return food; }
            set { food = value; }
        }

        public bool Free {
            get { return free; }
            set { free = value; }
        }

        public bool Head {
            get { return head; }
            set { head = value; }
        }

        public int Val {
            get { return val; }
            set { val = value; }
        }
        public Cell() {
            this.food = false;
            this.free = true;
            this.head = false;
        }


        public Cell(bool food, bool free) {
            this.food = food;
            this.free = free;
            this.head = false;
        }
    }
}
