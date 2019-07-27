using System;
using System.Collections.Generic;
using UnityEngine;

namespace Mkey
{
    public class BombCombiner : MonoBehaviour
    {
        [SerializeField]
        private CombinedBombAndBomb bombAndBombPrefab;
        [SerializeField]
        private CombinedBombAndRocket bombAndRocketPrefab;
        [SerializeField]
        private CombinedRocketAndRocket rocketAndRocketPrefab;
        [SerializeField]
        private CombinedColorBombAndRocket colorBombAndRocketPrefab;
        [SerializeField]
        private CombinedColorBombAndBomb colorBombAndBombPrefab;
        [SerializeField]
        private CombinedColorBombAndColorBomb colorBombAndColorBombPrefab;

        [SerializeField]
        private Sprite glow;

        private MatchSoundMaster MSound { get { return MatchSoundMaster.Instance; } }
        private MatchPlayer MPlayer { get { return MatchPlayer.Instance; } }
        private MatchBoard MBoard { get { return MatchBoard.Instance; } }
        private GameObjectsSet MatchSet { get { return MPlayer.MatchSet; } }
        private ParallelTween collectTween;
        private DynamicClickBombObjectData sourceColorBombData; // need only for color bomb

        public void CombineAndExplode(GridCell gCell, DynamicClickBombObject bomb, Action completeCallBack)
        {
            if(!gCell || ! bomb)
            {
                completeCallBack?.Invoke();
                return;
            }

            NeighBors nG = gCell.Neighbors;
            BombDir bd1 = bomb.GetBombDir();
            BombCombine bC = BombCombine.None;
            List<DynamicClickBombObject> nBs = GetNeighBombs(gCell);

            if(bomb.GetBombDir() == BombDir.Color)
            {
                sourceColorBombData = bomb.OData;
            }

            foreach (var item in nBs) // search color bomb
            {
                BombDir bd2 = item.GetBombDir();
                if (bd2 == BombDir.Color)
                {
                    if(sourceColorBombData==null) sourceColorBombData = item.OData;
                    bC = GetCombineType(bd1, bd2);
                    break;
                }
            }

            if (bC == BombCombine.None) // search radial bomb
            {
                foreach (var item in nBs)
                {
                    BombDir bd2 = item.GetBombDir();
                    if (bd2 == BombDir.Radial)
                    {
                        bC = GetCombineType(bd1, bd2);
                        break;
                    }
                }
            }

            if (bC == BombCombine.None) // search hor or vert bomb
            {
                foreach (var item in nBs)
                {
                    BombDir bd2 = item.GetBombDir();
                    if (bd2 == BombDir.Horizontal || bd2 == BombDir.Vertical)
                    {
                        bC = GetCombineType(bd1, bd2);
                        break;
                    }
                }
            }

            switch (bC)
            {
                case BombCombine.ColorBombAndColorBomb:     // clean full board
                    collectTween = new ParallelTween();
                    nBs.Add(bomb);
                    foreach (var item in nBs)
                    {
                        item.transform.parent = null;
                        item.SetToFront(true);
                        Creator.CreateSprite(item.transform, glow, item.transform.position, SortingOrder.BombCreator - 1);
                        collectTween.Add((callBack) =>
                        {
                            item.MoveToBomb(gCell, 0, () => { Destroy(item.gameObject); callBack(); });
                        });
                    }
                    collectTween.Start(() =>
                    {
                        MSound.SoundPlayMakeBomb(0.05f, null);
                        CombinedColorBombAndColorBomb bigBomb = Instantiate(colorBombAndColorBombPrefab);
                        bigBomb.transform.localScale = gCell.transform.lossyScale;
                        bigBomb.transform.position = gCell.transform.position;
                        bigBomb.ApplyToGrid(gCell, 0.2f, completeCallBack);
                    });

                    break;

                case BombCombine.BombAndBomb:               // big bomb explode
                    collectTween = new ParallelTween();
                    nBs.Add(bomb);
                    foreach ( var item in nBs)
                    {
                        item.transform.parent = null;
                        item.SetToFront(true);
                        Creator.CreateSprite(item.transform, glow, item.transform.position, SortingOrder.BombCreator - 1);
                        collectTween.Add((callBack) => 
                        {
                            item.MoveToBomb(gCell, 0, ()=> { Destroy(item.gameObject); callBack(); });
                        });
                    }
                    collectTween.Start(() =>
                    {
                        MSound.SoundPlayMakeBomb(0.05f, null);
                        CombinedBombAndBomb bigBomb =  Instantiate(bombAndBombPrefab);
                        bigBomb.transform.localScale = gCell.transform.lossyScale; 
                        bigBomb.transform.position = gCell.transform.position;
                        bigBomb.ApplyToGrid(gCell, 0.2f, completeCallBack);
                    });
                    break;
                case BombCombine.RocketAndRocket:           // 2 rows or 2 columns
                    collectTween = new ParallelTween();
                    nBs.Add(bomb);
                    foreach (var item in nBs)
                    {
                        item.transform.parent = null;
                        item.SetToFront(true);
                        Creator.CreateSprite(item.transform, glow, item.transform.position, SortingOrder.BombCreator - 1);
                        collectTween.Add((callBack) =>
                        {
                            item.MoveToBomb(gCell, 0, () => { Destroy(item.gameObject, 0.2f); callBack(); });
                        });
                    }
                    collectTween.Start(() =>
                    {
                        MSound.SoundPlayMakeBomb(0.05f, null);
                        CombinedRocketAndRocket bigBomb = Instantiate(rocketAndRocketPrefab);
                        bigBomb.transform.localScale = gCell.transform.lossyScale;
                        bigBomb.transform.position = gCell.transform.position;
                        bigBomb.ApplyToGrid(gCell, 0.2f, completeCallBack);
                    });
                    break;
                case BombCombine.ColorBombAndBomb:          // replace color match with bomb
                    collectTween = new ParallelTween();
                    nBs.Add(bomb);

                    foreach (var item in nBs)
                    {
                        item.transform.parent = null;
                        item.SetToFront(true);
                        Creator.CreateSprite(item.transform, glow, item.transform.position, SortingOrder.BombCreator - 1);
                        collectTween.Add((callBack) =>
                        {
                            item.MoveToBomb(gCell, 0, () => { Destroy(item.gameObject); callBack(); });
                        });
                    }
                    collectTween.Start(() =>
                    {
                        MSound.SoundPlayMakeBomb(0.05f, null);
                        CombinedColorBombAndBomb colorBombAndBomb = Instantiate(colorBombAndBombPrefab);
                        colorBombAndBomb.transform.localScale = gCell.transform.lossyScale;
                        colorBombAndBomb.transform.position = gCell.transform.position;
                        colorBombAndBomb.OData = sourceColorBombData;
                        colorBombAndBomb.ApplyToGrid(gCell, 0.2f, completeCallBack);
                        colorBombAndBomb.GetComponent<SpriteRenderer>().sprite = sourceColorBombData.ObjectImage;
                    });

                    break;

                case BombCombine.BombAndRocket:             // 3 rows and 3 columns
                    collectTween = new ParallelTween();
                    nBs.Add(bomb);
                    foreach (var item in nBs)
                    {
                        item.transform.parent = null;
                        item.SetToFront(true);
                        Creator.CreateSprite(item.transform, glow, item.transform.position, SortingOrder.BombCreator - 1);
                        collectTween.Add((callBack) =>
                        {
                            item.MoveToBomb(gCell, 0, () => { Destroy(item.gameObject); callBack(); });
                        });
                    }
                    collectTween.Start(() =>
                    {
                        MSound.SoundPlayMakeBomb(0.05f, null);
                        CombinedBombAndRocket bombAndRocket = Instantiate(bombAndRocketPrefab);
                        bombAndRocket.transform.localScale = gCell.transform.lossyScale;
                        bombAndRocket.transform.position = gCell.transform.position;
                        bombAndRocket.ApplyToGrid(gCell, 0.2f, completeCallBack);
                    });
                    break;
                case BombCombine.ColorBombAndRocket:        // replace color bomb with rockets
                    collectTween = new ParallelTween();
                    nBs.Add(bomb);
                   
                    foreach (var item in nBs)
                    {
                        item.transform.parent = null;
                        item.SetToFront(true);
                        Creator.CreateSprite(item.transform, glow, item.transform.position, SortingOrder.BombCreator - 1);
                        collectTween.Add((callBack) =>
                        {
                            item.MoveToBomb(gCell, 0, () => { Destroy(item.gameObject); callBack(); });
                        });
                    }
                    collectTween.Start(() =>
                    {
                        MSound.SoundPlayMakeBomb(0.05f, null);
                        CombinedColorBombAndRocket colorBombAndRocket = Instantiate(colorBombAndRocketPrefab);
                        colorBombAndRocket.transform.localScale = gCell.transform.lossyScale;
                        colorBombAndRocket.transform.position = gCell.transform.position;
                        colorBombAndRocket.OData = sourceColorBombData;
                        colorBombAndRocket.ApplyToGrid(gCell, 0.2f, completeCallBack);
                        colorBombAndRocket.GetComponent<SpriteRenderer>().sprite = sourceColorBombData.ObjectImage;
                    });
                    break;

                case BombCombine.None:                      // simple explode
                    gCell.ExplodeBomb(0.0f, true, true, bd1 == BombDir.Color, false, () =>
                    {
                        completeCallBack?.Invoke();
                    });
                    break;
                default:
                    completeCallBack?.Invoke();
                    break;
            }
        }

        private BombCombine GetCombineType(BombDir bd1, BombDir bd2)
        {
            if(bd1== BombDir.Color )
            {
                if (bd2 == BombDir.Color) {return BombCombine.ColorBombAndColorBomb; }
              if (bd2 == BombDir.Radial) return BombCombine.ColorBombAndBomb;
              if (bd2 == BombDir.Horizontal || bd2 == BombDir.Vertical) return BombCombine.ColorBombAndRocket;
            }
            if (bd1 == BombDir.Radial)
            {
                if (bd2 == BombDir.Color) return BombCombine.ColorBombAndBomb;
                if (bd2 == BombDir.Radial) return BombCombine.BombAndBomb;
                if (bd2 == BombDir.Horizontal || bd2 == BombDir.Vertical) return BombCombine.BombAndRocket;
            }
            if (bd1 == BombDir.Horizontal || bd1 == BombDir.Vertical)
            {
                if (bd2 == BombDir.Color) return BombCombine.ColorBombAndRocket;
                if (bd2 == BombDir.Radial) return BombCombine.BombAndRocket;
                if (bd2 == BombDir.Horizontal || bd2 == BombDir.Vertical) return BombCombine.RocketAndRocket;
            }
            return BombCombine.RocketAndRocket;
        }

        private List <DynamicClickBombObject> GetNeighBombs(GridCell gCell)
        {
            List<DynamicClickBombObject> res = new List<DynamicClickBombObject>();
            NeighBors nG = gCell.Neighbors;
            foreach (var item in nG.Cells) // search color bomb
            {
                if (item.DynamicClickBomb)
                {
                    res.Add(item.DynamicClickBomb);
                }
            }
            return res;
        }
    }
}