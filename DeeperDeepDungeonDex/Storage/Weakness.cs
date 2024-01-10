using System;

namespace DeeperDeepDungeonDex.Storage;

public enum Weakness {
    None = 0x00,
    Stun = 0x01,
    Heavy = 0x02,
    Slow = 0x04,
    Sleep = 0x08,
    Bind = 0x10,
    Undead = 0x20, // currently only for mobs in PotD floors 51-200
    StunUnknown = 0x40 | Stun,
    HeavyUnknown = 0x80 | Heavy,
    SlowUnknown = 0x100 | Slow,
    SleepUnknown = 0x200 | Sleep,
    BindUnknown = 0x400 | Bind,
    UndeadUnknown = 0x800 | Undead,
    All = Stun | Heavy | Slow | Sleep | Bind,
    AllUnknown = StunUnknown | HeavyUnknown | SlowUnknown | SleepUnknown | BindUnknown
}
