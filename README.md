# OfDungeonsDeep
[![Download count](https://img.shields.io/endpoint?url=https://qzysathwfhebdai6xgauhz4q7m0mzmrf.lambda-url.us-east-1.on.aws/OfDungeonsDeep)](https://github.com/NotNite/OfDungeonsDeep)

Rewrite of DeepDungeonDex.

This plugin uses a modified version of [the Deep Dungeon Compendium website](https://github.com/djcooke/compendium) as its data source, with permission.

## Updating data

To update mob/floor information:

```sh
dotnet run --project OfDungeonsDeep.ExcelExporter -- "C:\Program Files (x86)\Steam\steamapps\common\FINAL FANTASY XIV Online\game\sqpack"
node processor.js
```

