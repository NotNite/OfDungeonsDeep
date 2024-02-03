# DeeperDeezNuts

Rewrite of DeepDungeonDex.

This plugin uses a modified version of [the Deep Dungeon Compendium website](https://github.com/djcooke/compendium) as its data source, with permission.

## Updating data

To update mob/floor information:

```sh
dotnet run --project OfDungeonsDeep.ExcelExporter -- "C:\Program Files (x86)\Steam\steamapps\common\FINAL FANTASY XIV Online\game\sqpack"
node processor.js
```

