# ScoreManager-Core

A [*Ballance*](https://en.wikipedia.org/wiki/Ballance) game plugin provides speedrun and statistics data exporting.

This repository contains the core functions of *ScoreManager*, including `menulevel.nmo` injection, data capture, verify and analyze.

![snapshot.png](snapshot.png)

## Tutorial

Clone, Compile and Run.

Requires .NET Framework 4.5 or later, but 4.0 should work.

See [Releases](https://github.com/BearKidsTeam/ScoreManager-Core/releases) for pre-compiled binary files.

## Hints

Put all files(including `.exe` and `.nmo`) in any folder and execute, it will automatically search Ballance location in registry and do game file injection. If it reports a failure, you need to manually input the game folder.

The program needs reading and writing permission, so if you are running it or *Ballance* in your **system partition**, please use `Run as Administrator` (The program itself was configured by `app.manifest` in compilation, but you need to configure *Ballance* `Player.exe` by yourself).

## License

MIT License (see [`LICENSE`](https://github.com/BearKidsTeam/ScoreManager-Core/blob/master/LICENSE) file).
