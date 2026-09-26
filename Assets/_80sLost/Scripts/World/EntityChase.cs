using UnityEngine;

namespace Lost80s
{
    /// <summary>
    /// Minimal placeholder chase behaviour: walks straight at the player once
    /// EntityFixation reports the player has stared too long. Swap for a NavMesh
    /// agent once real level geometry exists.
    /// </summary>
    [RequireComponent(typeof(EntityFixation))]
    public class EntityChase : MonoBehaviour
    {
        [SerializeField] private float chaseSpeed = 3.5f;
        [SerializeField] private string playerTag = "Player";

        private Transform _player;
        private bool _chasing;

        private void Start()
        {
            var playerObj = GameObject.FindGameObjectWithTag(playerTag);
            _player = playerObj != null ? playerObj.transform : null;

            var fixation = GetComponent<EntityFixation>();
            fixation.onFixated.AddListener(() => _chasing = true);
            fixation.onLostFixation.AddListener(() => _chasing = false);
        }

        private void Update()
        {
            if (!_chasing || _player == null) return;

            Vector3 toPlayer = _player.position - transform.position;
            toPlayer.y = 0f;
            if (toPlayer.sqrMagnitude < 0.01f) return;

            transform.position += toPlayer.normalized * (chaseSpeed * Time.deltaTime);
            transform.rotation = Quaternion.LookRotation(toPlayer.normalized);
        }
    }
}
