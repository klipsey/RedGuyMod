using RoR2;
using UnityEngine;

internal enum RavagerCameraParams
{
    DEFAULT,
    AIM,
    DRAG,
    SLAM,
    CLING,
    EMOTE
}

namespace RedGuyMod.Modules
{
    internal static class CameraParams
    {
        internal static CharacterCameraParamsData defaultCameraParams;
        internal static CharacterCameraParamsData aimCameraParams;
        internal static CharacterCameraParamsData slamCameraParams;
        internal static CharacterCameraParamsData dragCameraParams;
        internal static CharacterCameraParamsData clingCameraParams;
        internal static CharacterCameraParamsData emoteCameraParams;

        internal static void InitializeParams()
        {
            defaultCameraParams = NewCameraParams("ccpRobRavager", 70f, 1.37f, new Vector3(0f, 0f, -8.1f));
            aimCameraParams = NewCameraParams("ccpRobRavagerAim", 70f, 1.37f, new Vector3(0.15f, -0.1f, -15f));
            slamCameraParams = NewCameraParams("ccpRobRavagerSlam", 70f, 1.37f, new Vector3(0f, -2.5f, -8.3f));
            dragCameraParams = NewCameraParams("ccpRobRavagerDrag", 70f, 1.37f, new Vector3(-0.3f, -0.5f, -6.5f));
            clingCameraParams = NewCameraParams("ccpRobRavagerCling", 70f, 1.1f, new Vector3(0f, 0f, -7f));
            emoteCameraParams = NewCameraParams("ccpRobRavagerEmote", 70f, 0.4f, new Vector3(0f, 0f, -6f));
        }

        private static CharacterCameraParamsData NewCameraParams(string name, float pitch, float pivotVerticalOffset, Vector3 standardPosition)
        {
            return NewCameraParams(name, pitch, pivotVerticalOffset, standardPosition, 0.1f);
        }

        private static CharacterCameraParamsData NewCameraParams(string name, float pitch, float pivotVerticalOffset, Vector3 idealPosition, float wallCushion)
        {
            CharacterCameraParamsData newParams = new CharacterCameraParamsData();

            newParams.maxPitch = pitch;
            newParams.minPitch = -pitch;
            newParams.pivotVerticalOffset = pivotVerticalOffset;
            newParams.idealLocalCameraPos = idealPosition;
            newParams.wallCushion = wallCushion;

            return newParams;
        }

        internal static CameraTargetParams.CameraParamsOverrideHandle OverrideCameraParams(CameraTargetParams camParams, RavagerCameraParams camera, float transitionDuration = 0.5f)
        {
            CharacterCameraParamsData paramsData = GetNewParams(camera);

            CameraTargetParams.CameraParamsOverrideRequest request = new CameraTargetParams.CameraParamsOverrideRequest
            {
                cameraParamsData = paramsData,
                priority = 0,
            };

            return camParams.AddParamsOverride(request, transitionDuration);
        }

        internal static CharacterCameraParams CreateCameraParamsWithData(RavagerCameraParams camera)
        {

            CharacterCameraParams newPaladinCameraParams = ScriptableObject.CreateInstance<CharacterCameraParams>();

            newPaladinCameraParams.name = camera.ToString().ToLower() + "Params";

            newPaladinCameraParams.data = GetNewParams(camera);

            return newPaladinCameraParams;
        }

        internal static CharacterCameraParamsData GetNewParams(RavagerCameraParams camera)
        {
            CharacterCameraParamsData paramsData = defaultCameraParams;

            switch (camera)
            {

                default:
                case RavagerCameraParams.DEFAULT:
                    paramsData = defaultCameraParams;
                    break;
                case RavagerCameraParams.AIM:
                    paramsData = aimCameraParams;
                    break;
                case RavagerCameraParams.DRAG:
                    paramsData = dragCameraParams;
                    break;
                case RavagerCameraParams.SLAM:
                    paramsData = slamCameraParams;
                    break;
                case RavagerCameraParams.CLING:
                    paramsData = clingCameraParams;
                    break;
                case RavagerCameraParams.EMOTE:
                    paramsData = emoteCameraParams;
                    break;
            }

            return paramsData;
        }
    }
}