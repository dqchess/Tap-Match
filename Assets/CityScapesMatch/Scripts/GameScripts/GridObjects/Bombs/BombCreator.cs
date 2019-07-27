using UnityEngine;
using System;

namespace Mkey
{
    public class BombCreator : MonoBehaviour
    {
        [SerializeField]
        private Sprite glow;

        private GameObject createPrefab;
        private MatchSoundMaster MSound { get { return MatchSoundMaster.Instance; } }
        private MatchPlayer MPlayer { get { return MatchPlayer.Instance; } }
        private MatchBoard MBoard { get { return MatchBoard.Instance; } }
        private GameObjectsSet MatchSet { get { return MPlayer.MatchSet; } }

        private void Start()
        {
           
        }

        public void Create(MatchGroup m, Action completeCallBack)
        {
            float delay = 0;
            ParallelTween collectTween = new ParallelTween();
            MSound.SoundPlayMakeBomb(0.05f, null);

            foreach (GridCell c in m.Cells) // move and collect
            {
                Creator.CreateSprite(c.Match.transform, glow, c.Match.transform.position, SortingOrder.BombCreator - 1);
                collectTween.Add((callBack) => { c.MoveMatchAndCollect(m.lastMatchedCell, delay, false, false, true, false,  callBack); });
            }

            collectTween.Start(() =>
            {
                SetBomb(m);
                completeCallBack?.Invoke();
            });
        }

        private void SetBomb(MatchGroup m)
        {
            if (m == null) return;
            GridCell c = m.lastMatchedCell;
            DynamicClickBombObjectData b = null;
            switch (m.GetGroupType())
            {
                case MatchGroupType.HorBomb:
                    b = MatchSet.GetDynamicClickBombObject(BombDir.Horizontal, m.lastMatchedID);
                    break;
                case MatchGroupType.VertBomb:
                    b = MatchSet.GetDynamicClickBombObject(BombDir.Vertical, m.lastMatchedID);
                    break;
                case MatchGroupType.Bomb:
                    b = MatchSet.GetDynamicClickBombObject(BombDir.Radial, m.lastMatchedID);
                    break;
                case MatchGroupType.ColorBomb:
                    b = MatchSet.GetDynamicClickBombObject(BombDir.Color, m.lastMatchedID);
                    break;
            }
            if (b == null) return;
            createPrefab = b.createAnimPrefab;
            c.SetDynamicClickBomb(b);
            Creator.InstantiateAnimPrefab(createPrefab, c.DynamicObject.transform, c.DynamicObject.transform.position, SortingOrder.BombCreator + 1, false, null);
        }
    }
}
