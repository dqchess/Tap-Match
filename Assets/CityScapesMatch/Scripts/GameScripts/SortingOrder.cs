using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Mkey
{
    public static class SortingOrder
    {
        #region sorting orders
        public static int Base
        {
            get
            {
                return 2;
            }
        }

        public static int Main
        {
            get
            {
                return Base + 9;
            }
        }

        public static int Blocked
        {
            get
            {
                return Main;
            }
        }

        public static int Under
        {
            get
            {
                return Main - 1;
            }
        }

        public static int UnderToFront
        {
            get
            {
                return MainToFront - 1;
            }
        }

        public static int Over
        {
            get
            {
                return Main + 2;
            }
        }

        public static int MainToFront
        {
            get
            {
                return Base + 30;
            }
        }

        public static int OverToFront
        {
            get
            {
                return MainToFront + 1;
            }
        }


        public static int MainIddle
        {
            get
            {
                return MainToFront + 1;
            }
        }

        public static int MainExplode
        {
            get
            {
                return MainToFront +10;
            }
        }

        public static int Booster
        {
            get { return Base + 16; }
        }

        public static int BoosterToFront
        {
            get { return GuiOverlay; }
        }

        public static int BombCreator
        {
            get { return MainToFront-1; }
        }

        public static int GuiOverlay {get { return 120;}}
        #endregion sorting orders
    }
}