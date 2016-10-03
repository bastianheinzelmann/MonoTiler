using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonoTiler
{
    public struct Tuple
    {
        public int TileIndex;
        public int TileSheetIndex;

        public Tuple(int tileIndex, int tileSheetIndex)
        {
            TileIndex = tileIndex;
            TileSheetIndex = tileIndex;
        }
    }
}
