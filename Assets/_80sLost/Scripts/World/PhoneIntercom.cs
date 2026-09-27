using UnityEngine;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

namespace Lost80s
{
    /// <summary>
    /// Building intercom: interact to open, type the apartment code on the
    /// keyboard, Enter to call. Display is a simple text overlay (HintUI) rather
    /// than a hand-modelled keypad UI - upgrade to real button art later.
    /// </summary>
    public class PhoneIntercom : MonoBehaviour, IInteractable
    {
        [SerializeField] private string code = "888";
        [SerializeField] private int maxDigits = 3;
        [SerializeField] private InteractableDoor doorToUnlock;
        [SerializeField] private AudioClip keyClip;
        [SerializeField] private AudioClip successClip;
        [SerializeField] private AudioClip errorClip;

        public string InteractPrompt => _isOpen ? "Закрыть домофон" : "Домофон";

        private bool _isOpen;
        private bool _unlocked;
        private string _buffer = "";

#if ENABLE_INPUT_SYSTEM
        // NOT contiguous with Key.Digit0 in the Input System's Key enum (Digit1..Digit9
        // come first, then Digit0, then modifier keys) - list them explicitly instead
        // of doing Key.Digit0 + i, which used to land on Shift/Ctrl/Alt/Win.
        private static readonly Key[] DigitKeys =
        {
            Key.Digit0, Key.Digit1, Key.Digit2, Key.Digit3, Key.Digit4,
            Key.Digit5, Key.Digit6, Key.Digit7, Key.Digit8, Key.Digit9,
        };
#endif

        public void Interact(GameObject interactor)
        {
            _isOpen = !_isOpen;
            _buffer = "";
            HintUI.Show(_isOpen ? BuildDisplay() : "", 999f);
        }

        private void Update()
        {
            if (!_isOpen) return;
#if ENABLE_INPUT_SYSTEM
            var kb = Keyboard.current;
            if (kb == null) return;

            for (int i = 0; i < DigitKeys.Length; i++)
            {
                if (kb[DigitKeys[i]].wasPressedThisFrame) AppendDigit(i);
            }

            if (kb.enterKey.wasPressedThisFrame || kb.numpadEnterKey.wasPressedThisFrame) Submit();
            if (kb.escapeKey.wasPressedThisFrame) Close();
#endif
        }

        private void AppendDigit(int digit)
        {
            if (_buffer.Length >= maxDigits) return;
            _buffer += digit.ToString();
            PlayClip(keyClip);
            HintUI.Show(BuildDisplay(), 999f);
        }

        private void Submit()
        {
            if (!_unlocked && _buffer == code)
            {
                _unlocked = true;
                PlayClip(successClip);
                if (doorToUnlock != null) doorToUnlock.Unlock();
                Close();
            }
            else
            {
                PlayClip(errorClip);
                _buffer = "";
                HintUI.Show(BuildDisplay(), 999f);
            }
        }

        private void Close()
        {
            _isOpen = false;
            HintUI.Show("", 0.01f);
        }

        private string BuildDisplay()
        {
            return $"ДОМОФОН\nКвартира: {_buffer.PadRight(maxDigits, '_')}\n[0-9] набор  [Enter] вызов  [Esc] выход";
        }

        private void PlayClip(AudioClip clip)
        {
            PlayerAudio.Play(clip);
        }
    }
}
