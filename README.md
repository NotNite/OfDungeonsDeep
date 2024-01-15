# DeeperDeepDungeonDex

Rewrite of DeepDungeonDex.

This plugin uses a modified version of [the Deep Dungeon Compendium website](https://github.com/djcooke/compendium) as its data source, with permission.

## Updating data

To update mob/floor information:

```sh
dotnet run --project DeeperDeepDungeonDex.ExcelExporter -- "C:\Program Files (x86)\Steam\steamapps\common\FINAL FANTASY XIV Online\game\sqpack"
node processor.js
```

## TODO

- [x] Parse the already existing Deep Dungeon spreadsheets instead of the YAML submodule bullshit
  - This information is wrong and buggy - best to use the existing data the community's made
- [x] Show basic mob information
- [ ] Show basic floor information
- [ ] Show The Strat:tm:
