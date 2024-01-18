const fs = require("fs");
const path = require("path");
const yaml = require("js-yaml");
const resx = require("resx");

const compendium = path.join(__dirname, "compendium");
const bnpc = JSON.parse(
  fs.readFileSync(path.join(__dirname, "processor", "names.json"), "utf8")
);
const actions = JSON.parse(
  fs.readFileSync(path.join(__dirname, "processor", "actions.json"), "utf8")
);

const enemies = {};
const floorsets = {};

function pascalify(str) {
  return str[0].toUpperCase() + str.slice(1).toLowerCase();
}

function notes(data) {
  if (typeof data === "string") {
    return [{ Notes: [data], Subnotes: [] }];
  } else if (Array.isArray(data)) {
    return data.map(notes).flat();
  } else if (typeof data === "object") {
    return {
      Notes: [data.note],
      Subnotes: data.subnotes != null ? notes(data.subnotes) : []
    };
  }
}

function buildNote(notes, indent = 0) {
  if (!notes) return undefined;
  let str = "";
  let indentStr = " ".repeat(indent);
  let iHateJavascript = Array.isArray(notes) ? notes : [notes];
  for (const note of iHateJavascript) {
    for (const n of note.Notes) {
      str += `${indentStr}- ${n}\n`;
    }
    for (const sub of note.Subnotes) {
      str += buildNote(sub, indent + 2);
    }
  }

  return str;
}

function deepDungeon(name) {
  if (name.includes("potd")) {
    return "PalaceOfTheDead";
  } else if (name.includes("hoh")) {
    return "HeavenOnHigh";
  } else if (name.includes("eo")) {
    return "EurekaOrthos";
  } else {
    return null;
  }
}

async function main() {
  const strings = await resx.resx2js(
    fs.readFileSync(
      path.join(__dirname, "DeeperDeepDungeonDex", "Strings.resx"),
      "utf8"
    )
  );

  function ability(prefix, data) {
    // Remove parentheses
    const name = data.name
      .replace(/\(.*\)/g, "")
      .trim()
      .toLowerCase();
    const id = Object.entries(actions).filter(
      ([, v]) => v.toLowerCase() === name
    );
    if (id.length < 1) {
      console.log(`Unable to find ID for ${data.name}`);
      return;
    }
    // Just pick the first ability for now - TODO
    const realId = parseInt(id[0][0]);

    if (data.description)
      strings[`AbilityNote_${prefix}_${realId}`] = data.description;
    if (data.warning)
      strings[`AbilityWarning_${prefix}_${realId}`] = data.warning;

    return {
      Id: realId,
      Type: data.type,
      Potency: data.potency?.toString(),
    };
  }

  const dirs = fs.readdirSync(compendium);
  for (const dir of dirs.filter((x) => x.endsWith("_enemies"))) {
    const files = fs.readdirSync(path.join(compendium, dir));

    const floor = parseInt(dir.split("_")[2]);
    const type = deepDungeon(dir);
    if (type == null) {
      console.log(`Unable to find type for ${filePath}`);
      continue;
    }

    for (const file of files) {
      const filePath = path.join(compendium, dir, file);
      const raw = fs.readFileSync(filePath, "utf8");
      const data = yaml.load(raw.split("---")[1]);

      // TODO: handle duplicate names
      const id = Object.entries(bnpc).filter(
        ([, v]) => v.toLowerCase() === data.name.toLowerCase()
      );
      if (id.length !== 1) {
        console.log(`Unable to find ID for ${data.name} in ${filePath}`);
        continue;
      }
      const realId = parseInt(id[0][0]);
      const uniqueId = `${type}_${floor}_${realId}`;

      let dmg = parseInt(data.attack_damage);
      if (isNaN(dmg)) dmg = null;

      const enemy = {
        Id: realId,
        Image: data.image.replace(".png", ".jpg"),

        DungeonType: type,
        StartFloor: data.start_floor,
        EndFloor: data.end_floor,
        Hp: data.hp,

        Aggro: data.agro,
        AttackName: data.attack_name,
        AttackType: data.attack_type,

        AttackDamage: data.attack_damage,
        Abilities: (data.abilities ?? []).map((x) => ability(uniqueId, x)),
        Vulnerabilities: Object.fromEntries(
          Object.entries(data.vulnerabilities)
            .filter(([k, v]) => typeof v === "boolean")
            .map(([k, v]) => [pascalify(k), v])
        ),
      };

      if (!enemies[type]) {
        enemies[type] = {};
      }
      if (!enemies[type][floor]) {
        enemies[type][floor] = [];
      }
      enemies[type][floor].push(enemy);

      const builtNote = buildNote(notes(data.notes));
      if (builtNote) strings[`EnemyNote_${uniqueId}`] = builtNote;
    }
  }

  for (const dir of dirs.filter((x) => x.endsWith("_floorsets"))) {
    const files = fs.readdirSync(path.join(compendium, dir));
    for (const file of files) {
      const filePath = path.join(compendium, dir, file);
      const raw = fs.readFileSync(filePath, "utf8");
      const data = yaml.load(raw.split("---")[1]);

      let type = deepDungeon(dir);
      if (type == null) {
        console.log(`Unable to find type for ${filePath}`);
        continue;
      }
      const floorsetId = parseInt(data.floorset);

      // TODO: handle duplicate names
      const id = Object.entries(bnpc).filter(
          ([, v]) => v.toLowerCase() === data.boss.toLowerCase()
      );
      if (id.length !== 1) {
          console.log(`Unable to find ID for ${data.name} in ${filePath}`);
          continue;
      }
      const realId = parseInt(id[0][0]);
      
      const floorset = {
        Id: realId,
        Image: data.boss_image.replace(".png", ".jpg"),
       
        DungeonType: type,
        Floor: floorsetId,
        Hp: parseInt(data.boss_hp),
        AttackDamage: parseInt(data.boss_attack_damage),
                  
        Boss: data.boss,
        BossAbilities: (data.boss_abilities ?? []).map((x) =>
          ability(`${type}_${floorsetId}_${realId}`, x)
        ),
      };

      if (!floorsets[type]) {
        floorsets[type] = {};
      }
      floorsets[type][floorsetId] = floorset;

      const builtNote = buildNote(notes(data.boss_notes));
      if (builtNote)
        strings[`FloorsetNote_${type}_${data.floorset}`] = builtNote;
    }
  }

  fs.writeFileSync(
    path.join(__dirname, "Data", "enemies.json"),
    JSON.stringify(enemies)
  );
  fs.writeFileSync(
    path.join(__dirname, "Data", "floorsets.json"),
    JSON.stringify(floorsets)
  );

  const resxWrite = await resx.js2resx(strings);
  fs.writeFileSync(
    path.join(__dirname, "DeeperDeepDungeonDex", "Strings.resx"),
    resxWrite
  );
}

main();
