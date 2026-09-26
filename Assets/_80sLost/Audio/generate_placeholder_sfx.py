"""
One-off generator for placeholder SFX (Assets/_80sLost/Audio/*.wav).
Pure stdlib, no dependencies. Re-run any time to regenerate/tweak.
These are deliberately simple synthesized stand-ins - swap the .wav
files for real audio later and Unity will pick them up (same names,
so existing AudioClip references in the scene keep working).
"""
import math
import random
import struct
import wave

SR = 44100


def _write(path, samples):
    samples = [max(-1.0, min(1.0, s)) for s in samples]
    frames = b"".join(struct.pack("<h", int(s * 32767)) for s in samples)
    with wave.open(path, "wb") as w:
        w.setnchannels(1)
        w.setsampwidth(2)
        w.setframerate(SR)
        w.writeframes(frames)


def _n(seconds):
    return int(SR * seconds)


def envelope(i, n, attack, release):
    a = _n(attack)
    r = _n(release)
    if i < a:
        return i / max(a, 1)
    if i > n - r:
        return max(0.0, (n - i) / max(r, 1))
    return 1.0


def tone(freq, dur, attack=0.01, release=0.05, wave_shape="sine", vol=0.6, freq_end=None):
    n = _n(dur)
    out = []
    for i in range(n):
        t = i / SR
        f = freq if freq_end is None else freq + (freq_end - freq) * (i / n)
        phase = 2 * math.pi * f * t
        if wave_shape == "sine":
            s = math.sin(phase)
        elif wave_shape == "square":
            s = 1.0 if math.sin(phase) >= 0 else -1.0
        elif wave_shape == "triangle":
            s = 2 * abs(2 * ((f * t) % 1) - 1) - 1
        else:
            s = math.sin(phase)
        out.append(s * vol * envelope(i, n, attack, release))
    return out


def noise(dur, attack=0.01, release=0.05, vol=0.5, seed=0):
    n = _n(dur)
    rnd = random.Random(seed)
    return [(rnd.uniform(-1, 1)) * vol * envelope(i, n, attack, release) for i in range(n)]


def mix(*tracks):
    n = max(len(t) for t in tracks)
    out = [0.0] * n
    for t in tracks:
        for i, s in enumerate(t):
            out[i] += s
    return out


def concat(*tracks):
    out = []
    for t in tracks:
        out.extend(t)
    return out


def silence(dur):
    return [0.0] * _n(dur)


def lowpass(samples, alpha=0.2):
    out = []
    prev = 0.0
    for s in samples:
        prev = prev + alpha * (s - prev)
        out.append(prev)
    return out


def two_note(f1, f2, dur1=0.12, dur2=0.16, gap=0.02, vol=0.6):
    return concat(
        tone(f1, dur1, attack=0.005, release=0.03, vol=vol),
        silence(gap),
        tone(f2, dur2, attack=0.005, release=0.05, vol=vol),
    )


SFX = {
    # Doors
    "door_open": lambda: tone(220, 0.35, attack=0.02, release=0.15, wave_shape="triangle", vol=0.35, freq_end=260),
    "door_close": lambda: mix(tone(90, 0.18, attack=0.005, release=0.12, wave_shape="sine", vol=0.6),
                               noise(0.06, attack=0.001, release=0.05, vol=0.25)),
    "door_locked": lambda: concat(tone(180, 0.08, attack=0.002, release=0.03, wave_shape="square", vol=0.3),
                                   silence(0.03),
                                   tone(140, 0.12, attack=0.002, release=0.05, wave_shape="square", vol=0.3)),

    # Pickups
    "pickup": lambda: tone(660, 0.15, attack=0.005, release=0.1, wave_shape="sine", vol=0.4, freq_end=880),
    "tape_pickup": lambda: two_note(523, 784, vol=0.4),

    # Escape / ending
    "window_escape": lambda: lowpass(noise(0.6, attack=0.05, release=0.4, vol=0.5), alpha=0.15),
    "ending_win": lambda: concat(
        tone(523, 0.18, attack=0.01, release=0.08, vol=0.45),
        tone(659, 0.18, attack=0.01, release=0.08, vol=0.45),
        tone(784, 0.35, attack=0.01, release=0.2, vol=0.5),
    ),
    "ending_lose": lambda: mix(
        tone(110, 1.4, attack=0.1, release=0.8, wave_shape="sine", vol=0.4, freq_end=70),
        noise(1.4, attack=0.2, release=0.8, vol=0.15),
    ),

    # Entity
    "entity_fixated": lambda: mix(
        tone(80, 0.8, attack=0.02, release=0.5, wave_shape="triangle", vol=0.45, freq_end=55),
        tone(83, 0.8, attack=0.02, release=0.5, wave_shape="triangle", vol=0.3, freq_end=57),
    ),
    "entity_lost_fixation": lambda: tone(300, 0.4, attack=0.01, release=0.3, wave_shape="sine", vol=0.35, freq_end=150),

    # Camcorder
    "camcorder_raise": lambda: concat(
        noise(0.03, attack=0.001, release=0.02, vol=0.3),
        tone(500, 0.08, attack=0.005, release=0.05, wave_shape="square", vol=0.25, freq_end=700),
    ),
    "camcorder_lower": lambda: concat(
        tone(500, 0.06, attack=0.002, release=0.04, wave_shape="square", vol=0.25, freq_end=350),
        noise(0.03, attack=0.001, release=0.02, vol=0.25),
    ),
    "battery_depleted": lambda: tone(400, 0.5, attack=0.01, release=0.35, wave_shape="triangle", vol=0.4, freq_end=100),

    # Intercom
    "intercom_key": lambda: tone(1200, 0.06, attack=0.002, release=0.03, wave_shape="square", vol=0.2),
    "intercom_success": lambda: two_note(659, 988, dur1=0.1, dur2=0.22, vol=0.45),
    "intercom_error": lambda: concat(
        tone(180, 0.1, attack=0.002, release=0.04, wave_shape="square", vol=0.3),
        silence(0.02),
        tone(160, 0.16, attack=0.002, release=0.06, wave_shape="square", vol=0.3),
    ),

    # Misc UI / world
    "hint_ping": lambda: tone(880, 0.14, attack=0.005, release=0.1, wave_shape="sine", vol=0.3),
    "seal_alarm": lambda: concat(*[
        tone(440 if i % 2 == 0 else 370, 0.18, attack=0.01, release=0.05, wave_shape="square", vol=0.35)
        for i in range(6)
    ]),
    "zone_enter": lambda: lowpass(mix(
        tone(120, 0.7, attack=0.1, release=0.5, wave_shape="sine", vol=0.25, freq_end=90),
        noise(0.7, attack=0.15, release=0.4, vol=0.1),
    ), alpha=0.25),
}


def main():
    for name, gen in SFX.items():
        samples = gen()
        _write(f"{name}.wav", samples)
        print(f"wrote {name}.wav ({len(samples) / SR:.2f}s)")


if __name__ == "__main__":
    main()
