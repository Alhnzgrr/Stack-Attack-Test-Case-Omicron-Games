using StackAttack.Core;
using UnityEditor;
using UnityEngine;

namespace StackAttack.Player
{
    public static class SkillBuilder
    {
        private const string SpriteFolder = "Assets/_Project/Art/Sprites/";
        private const string PrefabFolder = "Assets/_Project/Prefabs/Player/";
        private const string TrailMaterial = "Assets/_Project/Art/Materials/ProjectileTrail.mat";
        private const float PixelsPerUnit = 256f;

        [MenuItem("StackAttack/Build Skill Prefabs")]
        private static void Build()
        {
            bool confirmed = EditorUtility.DisplayDialog(
                "Build Skill Prefabs",
                "This imports the boomerang and rocket sprites and writes their prefabs.",
                "Build",
                "Cancel");

            if (!confirmed)
                return;

            Sprite boomerangSprite = ImportSprite("Boomerang");
            Sprite rocketSprite = ImportSprite("Rocket");
            Material trail = AssetDatabase.LoadAssetAtPath<Material>(TrailMaterial);

            BuildBoomerang(boomerangSprite, trail);
            BuildRocket(rocketSprite, trail);

            AssetDatabase.SaveAssets();
            Debug.Log("Skill prefabs built.");
        }

        private static void BuildBoomerang(Sprite sprite, Material trailMaterial)
        {
            GameObject go = new GameObject("Boomerang");

            go.transform.localScale = new Vector3(0.8f, 0.8f, 1f);

            Paint(go, sprite, new Color(1f, 0.82f, 0.24f, 1f));
            Move(go, 0.35f);
            Streak(go, trailMaterial, 0.25f, 0.22f, new Color(1f, 0.82f, 0.24f, 0.9f));

            go.AddComponent<Boomerang>();

            Save(go, "Boomerang");
        }

        private static void BuildRocket(Sprite sprite, Material trailMaterial)
        {
            GameObject go = new GameObject("Rocket");

            go.transform.localScale = new Vector3(0.7f, 0.7f, 1f);

            Paint(go, sprite, new Color(1f, 0.45f, 0.2f, 1f));
            Move(go, 0.3f);
            Streak(go, trailMaterial, 0.3f, 0.24f, new Color(1f, 0.55f, 0.2f, 0.9f));

            go.AddComponent<Rocket>();

            Save(go, "Rocket");
        }

        private static void Paint(GameObject go, Sprite sprite, Color color)
        {
            SpriteRenderer renderer = go.AddComponent<SpriteRenderer>();

            renderer.sprite = sprite;
            renderer.color = color;
            renderer.sortingOrder = 25;
        }

        // Dynamic with no gravity, matching the projectile that already works: a body
        // is what makes the trigger callbacks fire against the stacks, which carry
        // colliders and nothing else.
        private static void Move(GameObject go, float radius)
        {
            Rigidbody2D body = go.AddComponent<Rigidbody2D>();

            body.bodyType = RigidbodyType2D.Dynamic;
            body.gravityScale = 0f;
            body.sleepMode = RigidbodySleepMode2D.NeverSleep;

            CircleCollider2D collider = go.AddComponent<CircleCollider2D>();

            collider.isTrigger = true;
            collider.radius = radius;
        }

        private static void Streak(GameObject go, Material material, float time, float width, Color color)
        {
            TrailRenderer trail = go.AddComponent<TrailRenderer>();

            trail.sharedMaterial = material;
            trail.time = time;
            trail.startWidth = width;
            trail.endWidth = 0f;
            trail.minVertexDistance = 0.05f;
            trail.autodestruct = false;
            trail.startColor = color;
            trail.endColor = new Color(color.r, color.g, color.b, 0f);
            trail.sortingOrder = 24;
        }

        private static void Save(GameObject go, string name)
        {
            PrefabUtility.SaveAsPrefabAsset(go, PrefabFolder + name + ".prefab");
            Object.DestroyImmediate(go);
        }

        private static Sprite ImportSprite(string name)
        {
            string path = SpriteFolder + name + ".png";
            TextureImporter importer = (TextureImporter)AssetImporter.GetAtPath(path);

            importer.textureType = TextureImporterType.Sprite;
            importer.spriteImportMode = SpriteImportMode.Single;
            importer.spritePixelsPerUnit = PixelsPerUnit;
            importer.mipmapEnabled = false;
            importer.alphaIsTransparency = true;
            importer.filterMode = FilterMode.Bilinear;

            // FullRect only reachable through the settings struct, and a tight mesh
            // would trim the transparent margin the shapes are drawn inside.
            TextureImporterSettings settings = new TextureImporterSettings();

            importer.ReadTextureSettings(settings);
            settings.spriteMeshType = SpriteMeshType.FullRect;
            importer.SetTextureSettings(settings);

            importer.SaveAndReimport();

            return AssetDatabase.LoadAssetAtPath<Sprite>(path);
        }
    }
}
