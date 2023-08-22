// ---------------------------------------------------------------------------------------------
//  Copyright (c) 2021-2023, Jiaqi Liu. All rights reserved.
//  See LICENSE file in the project root for license information.
// ---------------------------------------------------------------------------------------------

namespace Pal3.Scene
{
    using Core.DataLoader;
    using Core.DataReader.Cpk;
    using Core.DataReader.Pol;
    using Core.DataReader.Scn;
    using Data;
    using MetaData;
    using Rendering.Material;
    using Rendering.Renderer;
    using UnityEngine;

    public sealed class CombatScene : MonoBehaviour
    {
        private GameResourceProvider _resourceProvider;
        private bool _isLightingEnabled;
        private IMaterialFactory _materialFactory;
        private static int _lightCullingMask;
        private string _combatSceneName;

        private GameObject _parent;
        private GameObject _mesh;
        private Light _mainLight;

        private (PolFile PolFile, ITextureResourceProvider TextureProvider) _scenePolyMesh;

        public void Init(GameResourceProvider resourceProvider,
            bool isLightingEnabled)
        {
            _resourceProvider = resourceProvider;
            _isLightingEnabled = isLightingEnabled;
            _lightCullingMask = (1 << LayerMask.NameToLayer("Default")) |
                                (1 << LayerMask.NameToLayer("VFX"));
            _materialFactory = resourceProvider.GetMaterialFactory();
        }

        public void Load(GameObject parent,
            string combatSceneName)
        {
            _parent = parent;
            _combatSceneName = combatSceneName;

            var meshFileRelativeFolderPath = FileConstants.CombatSceneFolderVirtualPath + combatSceneName;

            ITextureResourceProvider sceneTextureProvider = _resourceProvider.CreateTextureResourceProvider(
                meshFileRelativeFolderPath);

            PolFile polFile = _resourceProvider.GetGameResourceFile<PolFile>(meshFileRelativeFolderPath +
                CpkConstants.DirectorySeparatorChar + combatSceneName.ToLower() + ".pol");

            _scenePolyMesh = (polFile, sceneTextureProvider);

            RenderMesh();

            if (_isLightingEnabled)
            {
                SetupEnvironmentLighting();
            }
        }

        private void RenderMesh()
        {
            // Render mesh
            _mesh = new GameObject($"Mesh_{_combatSceneName}")
            {
                isStatic = true // Combat Scene mesh is static
            };

            var polyMeshRenderer = _mesh.AddComponent<PolyModelRenderer>();
            _mesh.transform.SetParent(_parent.transform, false);

            polyMeshRenderer.Render(_scenePolyMesh.PolFile,
                _scenePolyMesh.TextureProvider,
                _materialFactory,
                isStaticObject: true, // Scene mesh is static
                Color.white);
        }

        private void SetupEnvironmentLighting()
        {
            Vector3 mainLightPosition = new Vector3(0, 20f, 0);
            Quaternion mainLightRotation = Quaternion.Euler(70f, -30f, 0f);

            var mainLightGo = new GameObject($"LightSource_Main");
            mainLightGo.transform.SetParent(_parent.transform, false);
            mainLightGo.transform.SetPositionAndRotation(mainLightPosition, mainLightRotation);

            _mainLight = mainLightGo.AddComponent<Light>();
            _mainLight.type = LightType.Directional;
            _mainLight.range = 500f;
            _mainLight.shadows = LightShadows.Soft;
            _mainLight.cullingMask = _lightCullingMask;
            RenderSettings.sun = _mainLight;

            _mainLight.color = new Color(220f / 255f, 210f / 255f, 200f / 255f);
            _mainLight.intensity = 0.9f;

            // Ambient light
            RenderSettings.ambientIntensity = 1f;
            RenderSettings.ambientLight = new Color(180f / 255f, 180f / 255f, 160f / 255f);
        }
    }
}