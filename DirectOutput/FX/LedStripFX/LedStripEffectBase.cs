﻿using DirectOutput.Cab.Toys.Layer;

namespace DirectOutput.FX.LedStripFX
{
    /// <summary>
    /// Base class for effects targeting Ledstrip toys
    /// </summary>
    public abstract class LedStripEffectBase : EffectBase
    {
        #region Config properties

        private string _ToyName;

        /// <summary>
        /// Gets or sets the name of the toy which is to be controlled by the effect.
        /// </summary>
        /// <value>
        /// The name of the toy which is controlled by the effect.
        /// </value>
        public string ToyName
        {
            get { return _ToyName; }
            set
            {
                if (_ToyName != value)
                {
                    _ToyName = value;
                    LedStrip = null;
                }
            }
        }


        /// <summary>
        /// Gets the LedStrip object which is referenced by the ToyName property.
        /// This property is initialized by the Init method.
        /// </summary>
        /// <value>
        /// The led LedStrip object which is referenced by the ToyName property.
        /// </value>
        protected LedStrip LedStrip { get; private set; }




        private float _Width = 100;

        /// <summary>
        /// Gets or sets the width of target area of the ledstrip which is controlled by the effect.
        /// </summary>
        /// <value>
        /// The width of the target area for the effect (0-100).
        /// </value>
        public float Width
        {
            get { return _Width; }
            set { _Width = value.Limit(0, 100); }
        }

        private float _Height = 100;

        /// <summary>
        /// Gets or sets the height of target area of the ledstrip which is controlled by the effect.
        /// </summary>
        /// <value>
        /// The height of the target area for the effect (0-100).
        /// </value>
        public float Height
        {
            get { return _Height; }
            set { _Height = value.Limit(0, 100); }
        }

        private float _Left = 0;

        /// <summary>
        /// Gets or sets the left resp. X positon of the upper left corner of the target area of the ledstrip which is controlled by the effect.
        /// </summary>
        /// <value>
        /// The left resp. X position of the upper left corner of the target area for the effect (0-100).
        /// </value>
        public float Left
        {
            get { return _Left; }
            set { _Left = value.Limit(0, 100); }
        }

        private float _Top = 0;

        /// <summary>
        /// Gets or sets the top resp. Y positon of the upper left corner of the target area of the ledstrip which is controlled by the effect.
        /// </summary>
        /// <value>
        /// The top resp. Y position of the upper left corner of the target area for the effect (0-100).
        /// </value>
        public float Top
        {
            get { return _Top; }
            set { _Top = value.Limit(0, 100); }
        }




        private int _LayerNr = 0;

        /// <summary>
        /// Gets or sets the number of the layer which is targeted by the effect.
        /// </summary>
        /// <value>
        /// The number of the target layer for the effect.
        /// </value>
        public int LayerNr
        {
            get { return _LayerNr; }
            set { _LayerNr = value; }
        }
        #endregion


        protected int AreaLeft=0;
        protected int AreaTop=0;
        protected int AreaRight=0;
        protected int AreaBottom=0;

        /// <summary>
        /// Gets the table object which was specified during initialisation of the effect.
        /// </summary>
        /// <value>
        /// The table object which was specified during initialisation of the effect..
        /// </value>
        protected Table.Table Table { get; private set; }


        /// <summary>
        /// The led strip layer array of a led strip as specified by the ToyName and the LayerNr.
        /// This reference is initialized by the Init method.
        /// </summary>
        /// <value>
        /// The led strip layer array.
        /// </value>
        protected RGBAData[,] LedStripLayer;


        /// <summary>
        /// Initializes the effect.
        /// Resolves object references.
        /// </summary>
        /// <param name="Table">Table object containing the effect.</param>
        public override void Init(Table.Table Table)
        {
            if (!ToyName.IsNullOrWhiteSpace() && Table.Pinball.Cabinet.Toys.Contains(ToyName) && Table.Pinball.Cabinet.Toys[ToyName] is LedStrip)
            {
                LedStrip = (LedStrip)Table.Pinball.Cabinet.Toys[ToyName];
                LedStripLayer = LedStrip.GetLayer(LayerNr);

                AreaLeft = (LedStrip.Width / 100 * Left).RoundToInt();
                AreaTop = (LedStrip.Height / 100 * Top).RoundToInt();
                AreaRight = (LedStrip.Width / 100 * (Left + Width).Limit(0, 100)).RoundToInt();
                AreaBottom = (LedStrip.Height / 100 * (Top + Height).Limit(0, 100)).RoundToInt();

                int Tmp;
                if (AreaLeft > AreaRight) { Tmp = AreaRight; AreaRight = AreaLeft; AreaLeft = AreaRight; }
                if (AreaTop > AreaBottom) { Tmp = AreaBottom; AreaBottom = AreaTop; AreaTop = Tmp; }


            }

            this.Table = Table;
        }



        /// <summary>
        /// Finishes the effect and releases object references
        /// </summary>
        public override void Finish()
        {
            LedStripLayer = null;
            LedStrip = null;
            Table = null;
            base.Finish();
        }
    }
}
