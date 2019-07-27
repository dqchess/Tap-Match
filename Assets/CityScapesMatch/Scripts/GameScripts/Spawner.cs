using System.Collections.Generic;
using UnityEngine;

namespace Mkey
{
    public class Spawner : MonoBehaviour
    {
        public GridCell gridCell;

        private MatchSoundMaster MSound { get { return MatchSoundMaster.Instance; } }
        private MatchPlayer MPlayer { get { return MatchPlayer.Instance; } }
        private MatchBoard MBoard { get { return MatchBoard.Instance; } }
        private GameObjectsSet GOSet { get { return MBoard.GOSet; } }
        private SpawnController SpawnContr { get { return SpawnController.Instance; } }

        #region regular
        private void Start()
        {
        }
        #endregion regular

        /// <summary>
        /// spawn new MO 
        /// </summary>
        /// <param name="parent"></param>
        /// <returns></returns>
        public MatchObject Get()
        {
            MatchObjectData mOD = (SpawnContr) ? SpawnContr.GetObject() : GOSet.GetMainRandomObjects(1)[0];
            MatchObject match = MatchObject.Create(mOD, transform.position, false, false, MBoard.TargetCollectEventHandler, MBoard.MatchScoreCollectHandler);
            if (match) match.transform.localScale = transform.lossyScale;
            return match;
        }
    }
}