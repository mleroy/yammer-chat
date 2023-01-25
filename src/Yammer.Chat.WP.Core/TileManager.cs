using Microsoft.Phone.Shell;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using Yammer.Chat.Core;

namespace Yammer.Chat.WP.Core
{
    public class TileManager : ITileManager
    {
        public void UpdateTile(int count)
        {
            IconicTileData tileData = new IconicTileData()
            {
                Count = count,
                BackgroundColor = Color.FromArgb(0xFF, 0x00, 0x72, 0xC6)
            };

            ShellTile mainTile = ShellTile.ActiveTiles.FirstOrDefault();
            if (mainTile != null)
            {
                mainTile.Update(tileData);
            }
        }

        public void ClearTile()
        {
            IconicTileData tileData = new IconicTileData()
            {
                Count = 0,
                BackgroundColor = Color.FromArgb(0xFF, 0x00, 0x72, 0xC6)
            };

            ShellTile mainTile = ShellTile.ActiveTiles.FirstOrDefault();
            if (mainTile != null)
            {
                mainTile.Update(tileData);
            }
        }
    }
}
