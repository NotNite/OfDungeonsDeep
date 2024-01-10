const fs = require("fs");
const path = require("path");
const yaml = require("js-yaml");

const compendium = path.join(__dirname, "compendium");
const bnpc = JSON.parse(
  fs.readFileSync(path.join(__dirname, "processor", "names.json"), "utf8")
);

const enemies = {};
const floorsets = {};

function pascalify(str) {
  return str[0].toUpperCase() + str.slice(1).toLowerCase();
}

function difficulty(str) {
  if (!str) return "Unrated";

  const allowed = ["Easy", "Medium", "Hard", "Extreme", "Unrated"];
  const first = str.split(" ")[0];
  if (allowed.includes(first)) {
    return first;
  } else {
    return "Unrated";
  }
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

function jobSpecifics(data) {
  return Object.fromEntries(
    Object.entries(data ?? {}).map(([k, v]) => [
      k,
      {
        Difficulty: difficulty(v.difficulty),
        Timing: v.timing,
        Notes: notes(v.notes)
      }
    ])
  );
}

const dirs = fs.readdirSync(compendium);
for (const dir of dirs.filter((x) => x.endsWith("_enemies"))) {
  const files = fs.readdirSync(path.join(compendium, dir));
  for (const file of files) {
    const filePath = path.join(compendium, dir, file);
    const raw = fs.readFileSync(filePath, "utf8");
    const data = yaml.load(raw.split("---")[1]);

    const id = Object.entries(bnpc).filter(
      ([, v]) => v.toLowerCase() === data.name.toLowerCase()
    );
    if (id.length !== 1) {
      console.log(`Unable to find ID for ${data.name} in ${filePath}`);
      continue;
    }

    const enemy = {
      Name: data.name,
      Nickname: data.nickname,
      Family: data.family,

      StartFloor: data.start_floor,
      EndFloor: data.end_floor,
      Hp: data.hp,

      Aggro: data.agro,
      AttackName: data.attack_name,
      AttackType: data.attack_type,

      Notes: notes(data.notes ?? []),
      Vulnerabilities: Object.fromEntries(
        Object.entries(data.vulnerabilities)
          .filter(([k, v]) => typeof v === "boolean")
          .map(([k, v]) => [pascalify(k), v])
      ),
      JobSpecifics: jobSpecifics(data.job_specifics)
    };

    const actualId = id[0][0];
    enemies[actualId] = enemy;
  }
}

for (const dir of dirs.filter((x) => x.endsWith("_floorsets"))) {
  const files = fs.readdirSync(path.join(compendium, dir));
  for (const file of files) {
    const filePath = path.join(compendium, dir, file);
    const raw = fs.readFileSync(filePath, "utf8");
    const data = yaml.load(raw.split("---")[1]);

    let type = null;
    if (dir.includes("potd")) {
      type = "PalaceOfTheDead";
    } else if (dir.includes("hoh")) {
      type = "HeavenOnHigh";
    } else if (dir.includes("eo")) {
      type = "EurekaOrthos";
    }

    if (type == null) {
      console.log(`Unable to find type for ${filePath}`);
      continue;
    }

    const floorset = {
      Boss: data.boss,
      BossAbilities: data.boss_abilities,
      BossNotes: notes(data.boss_notes),
      JobSpecifics: jobSpecifics(data.job_specifics)
    };

    if (!floorsets[type]) {
      floorsets[type] = {};
    }
    floorsets[type][data.floorset] = floorset;
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
